﻿/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Serial.Shared;
using CNCLib.Serial.WebAPI.Hubs;
using CNCLib.Serial.WebAPI.SerialPort;

using Framework.Arduino.SerialCommunication;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CNCLib.Serial.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class SerialPortController : Controller
    {
        protected string CurrentUri => $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

        private readonly IHubContext<CNCLibHub> _hubContext;
        private static   IHubContext<CNCLibHub> _myHubContext;

        public SerialPortController(IHubContext<CNCLibHub> hubContext)
        {
            _hubContext   = hubContext;
            _myHubContext = _hubContext;
        }

        private SerialPortDefinition GetDefinition(SerialPortWrapper port)
        {
            return new SerialPortDefinition()
            {
                Id              = port.Id,
                PortName        = port.PortName,
                IsConnected     = port.IsConnected,
                IsAborted       = port.IsAborted,
                IsSingleStep    = port.IsSingleStep,
                CommandsInQueue = port.CommandsInQueue
            };
        }

        private async Task<SerialPortWrapper> GetPort(int id)
        {
            var port = SerialPortList.GetPort(id);
            if (port == null)
            {
                // rescan
                SerialPortList.Refresh();
                port = SerialPortList.GetPort(id);
            }

            return await Task.FromResult(port);
        }

        #region Query/Info

        [HttpGet]
        public async Task<IEnumerable<SerialPortDefinition>> Get()
        {
            return await Task.FromResult(SerialPortList.Ports.Select(GetDefinition));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SerialPortDefinition>> Get(int id)
        {
            var port = await GetPort(id);
            if (port == null)
            {
                return NotFound();
            }

            return GetDefinition(port);
        }

        [HttpPost("refresh")]
        public async Task<IEnumerable<SerialPortDefinition>> Refresh()
        {
            SerialPortList.Refresh();
            return await Get();
        }

        #endregion

        #region Connect/Disconnect

        [HttpPost("{id:int}/connect")]
        public async Task<ActionResult<SerialPortDefinition>> Connect(int id, int? baudRate = null, bool? dtrIsReset = true, bool? resetOnConnect = false)
        {
            bool dtrIsResetN0     = dtrIsReset ?? true;
            bool resetOnConnectN0 = resetOnConnect ?? false;
            int  baudRateN0       = baudRate ?? 250000;

            var port = await GetPort(id);
            if (port == null)
            {
                return NotFound();
            }

            if (port.IsConnected)
            {
                if (port.Serial.BaudRate == baudRateN0 && resetOnConnectN0 == false)
                {
                    return Ok(GetDefinition(port));
                }

                await port.Serial.DisconnectAsync();
            }

            port.Serial.BaudRate       = baudRateN0;
            port.Serial.DtrIsReset     = dtrIsResetN0;
            port.Serial.ResetOnConnect = resetOnConnectN0;

            await port.Serial.ConnectAsync(port.PortName);

            await _hubContext.Clients.All.SendAsync("connected", id);

            return Ok(GetDefinition(port));
        }

        [HttpPost("{id:int}/disconnect")]
        public async Task<ActionResult<SerialPortDefinition>> DisConnect(int id)
        {
            var port = await GetPort(id);
            if (port == null)
            {
                return NotFound();
            }

            await port.Serial.DisconnectAsync();
            port.Serial = null;

            await _hubContext.Clients.All.SendAsync("disconnected", id);

            return Ok();
        }

        #endregion

        #region Send/Queue/Commands

        [HttpPost("{id:int}/queue")]
        public async Task<ActionResult<IEnumerable<SerialCommand>>> QueueCommand(int id, [FromBody] SerialCommands commands)
        {
            var port = await GetPort(id);

            if (port == null || commands == null || commands.Commands == null)
            {
                return NotFound();
            }

            var ret = await port.Serial.QueueCommandsAsync(commands.Commands);
            return Ok(ret);
        }

        [HttpPost("{id:int}/send")]
        public async Task<ActionResult<IEnumerable<SerialCommand>>> SendCommand(int id, [FromBody] SerialCommands commands)
        {
            var port = await GetPort(id);

            if (port == null || commands == null || commands.Commands == null)
            {
                return NotFound();
            }

            var ret = await port.Serial.SendCommandsAsync(commands.Commands, commands.TimeOut);
            return Ok(ret);
        }

        [HttpPost("{id:int}/sendWhileOk")]
        public async Task<ActionResult<IEnumerable<SerialCommand>>> SendWhileOkCommand(int id, [FromBody] SerialCommands commands)
        {
            var port = await GetPort(id);

            if (port == null || commands == null || commands.Commands == null)
            {
                return NotFound();
            }

            var ret = new List<SerialCommand>();
            foreach (var c in commands.Commands)
            {
                var result = await port.Serial.SendCommandsAsync(new[] { c }, commands.TimeOut);
                ret.AddRange(result);
                if (result.Any() && result.LastOrDefault().ReplyType != EReplyType.ReplyOK)
                {
                    break;
                }
            }

            return Ok(ret);
        }

        [HttpPost("{id:int}/abort")]
        public async Task<ActionResult> AbortCommand(int id)
        {
            var port = await GetPort(id);
            if (port == null)
            {
                return NotFound();
            }

            port.Serial.AbortCommands();
            return Ok();
        }

        [HttpPost("{id:int}/resume")]
        public async Task<ActionResult> ResumeCommand(int id)
        {
            var port = await GetPort(id);
            if (port == null)
            {
                return NotFound();
            }

            port.Serial.ResumeAfterAbort();
            return Ok();
        }

        [HttpPost("{id:int}/enablesinglestep")]
        public async Task<ActionResult> EnableSingleStepCommand(int id)
        {
            var port = await GetPort(id);
            if (port == null)
            {
                return NotFound();
            }

            port.Serial.Pause = true;
            return Ok();
        }

        [HttpPost("{id:int}/disbablesinglestep")]
        public async Task<ActionResult> DisableSingleStepCommand(int id)
        {
            var port = await GetPort(id);
            if (port == null)
            {
                return NotFound();
            }

            port.Serial.Pause = false;
            return Ok();
        }

        [HttpPost("{id:int}/singlestep")]
        public async Task<ActionResult> SingleStepCommand(int id)
        {
            var port = await GetPort(id);
            if (port == null)
            {
                return NotFound();
            }

            port.Serial.SendNext = true;
            return Ok();
        }

        #endregion

        #region History

        [HttpPost("{id:int}/history/clear")]
        public async Task<IActionResult> ClearCommandHistory(int id)
        {
            var port = await GetPort(id);
            if (port == null)
            {
                return NotFound();
            }

            port.Serial.ClearCommandHistory();
            return Ok();
        }

        [HttpGet("{id:int}/history")]
        public async Task<ActionResult<IEnumerable<SerialCommand>>> GetCommandHistory(int id)
        {
            var port = await GetPort(id);
            if (port == null)
            {
                return NotFound();
            }

            var cmdList = port.Serial.CommandHistoryCopy;
            return Ok(cmdList);
        }

        #endregion
    }
}