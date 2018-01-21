﻿////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CNCLib.Serial.Server.SerialPort;
using CNCLib.Serial.Shared;
using Framework.Arduino.SerialCommunication;
using Microsoft.AspNetCore.Mvc;

namespace CNCLib.Serial.Server.Controllers
{
    [Route("api/[controller]")]
    public class SerialPortController : Controller
	{
	    protected string CurrentUri => $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

	    private SerialPortDefinition GetDefinition(SerialPortHelper port)
	    {
	        return new SerialPortDefinition() {Id = port.Id, PortName = port.PortName, IsConnected = port.IsConnected};
	    }

	    [HttpGet]
	    public async Task<IEnumerable<SerialPortDefinition>> Get()
        {
            return SerialPortHelper.Ports.Select((e) => GetDefinition(e));
        }

	    [HttpGet("{id:int}")]
	    public async Task<IActionResult> Get(int id)
	    {
	        var port = SerialPortHelper.GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }
	        return Ok(GetDefinition(port));
	    }

	    [HttpPost("refresh")]
	    public async Task<IEnumerable<SerialPortDefinition>> Refresh()
	    {
            SerialPortHelper.Refresh();
            return await Get();
	    }

	    [HttpPost("{id:int}/connect")]
	    public async Task<IActionResult> Connect(int id, int? baudrate=null,bool? resetOnConnect=true)
	    {
	        var port = SerialPortHelper.GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }

	        port.Serial.BaudRate = baudrate ?? 250000;
	        port.Serial.ResetOnConnect = resetOnConnect ?? true;

            port.Serial.Connect(port.PortName);

	        return Ok(GetDefinition(port));
	    }

	    [HttpPost("{id:int}/disconnect")]
	    public async Task<IActionResult> DisConnect(int id)
	    {
	        var port = SerialPortHelper.GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }

	        port.Serial.Disconnect();
	        port.Serial = null;

            return Ok();
	    }

        [HttpPost("{id:int}/queue")]
	    public async Task<IActionResult> QueueCommand(int id, [FromBody] SerialCommands commands)
	    {
	        var port = SerialPortHelper.GetPort(id);

            if (port == null || commands == null || commands.Commands == null)
            {
                return NotFound();
            }

            var ret = new List<SerialCommand>();
            foreach (var command in commands.Commands)
            {
                ret.AddRange(port.Serial.QueueCommand(command));
            }

            return Ok(ret);
	    }

	    [HttpPost("{id:int}/send")]
	    public async Task<IActionResult> SendCommand(int id, [FromBody] SerialCommands commands)
	    {
	        var port = SerialPortHelper.GetPort(id);

	        if (port == null || commands==null || commands.Commands==null)
	        {
	            return NotFound();
	        }

            var ret = new List<SerialCommand>();
            foreach (var command in commands.Commands)
            {
                ret.AddRange(await port.Serial.SendCommandAsync(command));
            }

            return Ok(ret);
        }

        [HttpPost("{id:int}/history/clear")]
	    public async Task<IActionResult> ClearCommandHistory(int id)
	    {
	        var port = SerialPortHelper.GetPort(id);
	        if (port == null)
	        {
	            return NotFound();
	        }

	        port.Serial.ClearCommandHistory();
	        return Ok();
	    }
    }
}
