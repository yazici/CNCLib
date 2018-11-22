﻿////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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

using Framework.Arduino.SerialCommunication.Abstraction;
using Framework.Logging;
using Framework.Pattern;

namespace CNCLib.Wpf.Helpers
{
    public class SerialProxy
    {
        public SerialProxy()
        {
            Current = LocalCom;
        }

        public ISerial RemoteCom => Singleton<Serial.Client.SerialService>.Instance;

        private static Framework.Arduino.SerialCommunication.Serial _localSerial = new Framework.Arduino.SerialCommunication.Serial(new Logger<Framework.Arduino.SerialCommunication.Serial>());

        public ISerial LocalCom => _localSerial;

        public ISerial Current { get; private set; }

        public void SetCurrent(string portName)
        {
            Current = portName.StartsWith("com") ? LocalCom : RemoteCom;
        }
    }
}