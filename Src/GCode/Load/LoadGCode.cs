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

using System;
using System.IO;

using CNCLib.GCode.Commands;

using Framework.Parser;

namespace CNCLib.GCode.Load
{
    public class LoadGCode : LoadBase
    {
        readonly CommandStream _stream = new CommandStream();
        Command                _lastNoPrefixCommand;

        public override void Load()
        {
            PreLoad();

            _lastNoPrefixCommand = null;

            using (StreamReader sr = GetStreamReader())
            {
                try
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        _stream.Line = line;
                        Command();
                    }
                }
                catch (FormatException)
                {
                    Commands.Clear();
                }
            }

            PostLoad();
        }

        public void Load(string[] lines)
        {
            _lastNoPrefixCommand = null;

            try
            {
                foreach (string line in lines)
                {
                    _stream.Line = line;
                    Command();
                }
            }
            catch (FormatException)
            {
                Commands.Clear();
            }

            PostLoad();
        }

        private void Command()
        {
            int? lineNumber = null;

            if (_stream.NextCharToUpper == 'N')
            {
                _stream.Next();

                if (!_stream.IsNumber())
                {
                    throw new FormatException();
                }

                lineNumber = _stream.GetInt();
                _stream.SkipSpaces();
            }

            _stream.SkipSpaces();
            Command cmd;

            if (_stream.NextCharToUpper == 'G')
            {
                cmd = ReadGCommand();
            }
            else if ("XYZABCF".IndexOf(_stream.NextCharToUpper) >= 0)
            {
                if (_lastNoPrefixCommand == null)
                {
                    throw new FormatException();
                }

                cmd = ReadGNoPrefixCommand();
            }
            else if (_stream.NextCharToUpper == 'M')
            {
                cmd = ReadMCommand();
            }
            else if (_stream.NextCharToUpper == '#')
            {
                cmd = ReadSetParameterCommand();
            }
            else
            {
                cmd = ReadOtherCommand();
            }

            if (cmd != null)
            {
                if (lineNumber.HasValue)
                {
                    cmd.LineNumber = lineNumber;
                }

                Commands.AddCommand(cmd);
            }
        }

        private Command AddGxxMxxCommand(Command cmd, string cmdName)
        {
            cmd.SetCode(cmdName);
            cmd.ReadFrom(_stream);
            return cmd;
        }

        private Command ReadGCommand()
        {
            _stream.Next();

            string cmdName = "G" + _stream.ReadDigits();
            _stream.SkipSpaces();

            Command cmd = CommandFactory.Create(cmdName);

            if (cmd != null)
            {
                if (cmd.UseWithoutPrefix)
                {
                    _lastNoPrefixCommand = cmd;
                }

                cmd.ReadFrom(_stream);
            }
            else
            {
                cmd = AddGxxMxxCommand(CommandFactory.Create("GXX"), cmdName);
            }

            return cmd;
        }

        private Command ReadGNoPrefixCommand()
        {
            // g without prefix

            Command cmd = CommandFactory.Create(_lastNoPrefixCommand.Code);
            cmd?.ReadFrom(_stream);
            return cmd;
        }

        private Command ReadMCommand()
        {
            _stream.Next();
            string cmdName = "M" + _stream.ReadDigits();
            _stream.SkipSpaces();

            Command cmd = CommandFactory.Create(cmdName);

            if (cmd != null)
            {
                cmd.ReadFrom(_stream);
            }
            else
            {
                cmd = AddGxxMxxCommand(CommandFactory.Create("MXX"), cmdName);
            }

            return cmd;
        }

        private Command ReadOtherCommand()
        {
            Command cmd = CommandFactory.Create("GXX");
            cmd.ReadFrom(_stream);
            return cmd;
        }

        private Command ReadSetParameterCommand()
        {
            Command cmd = CommandFactory.Create("#");
            cmd.ReadFrom(_stream);
            return cmd;
        }
    }
}