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
using System.Threading.Tasks;

using CNCLib.Logic.Contract;
using CNCLib.Logic.Contract.DTO;

using Framework.Logic;

namespace CNCLib.Logic.Manager
{
    public class EepromConfigurationManager : ManagerBase, IEepromConfigurationManager
    {
        public async Task<EepromConfiguration> CalculateConfig(EepromConfigurationInput param)
        {
            var result = new EepromConfiguration();

            double acc_corr   = 1.0615;
            double jerkFactor = 25;

            result.StepsPerRotation        = param.MicroSteps * (uint)param.StepsPerRotation;
            result.DistancePerRotationInMm = param.Teeth * param.ToothSizeInMm;
            if (Math.Abs(result.DistancePerRotationInMm) > double.Epsilon)
            {
                result.StepsPerMm = result.StepsPerRotation / result.DistancePerRotationInMm;
            }

            result.EstimatedMaxStepRate     = result.StepsPerRotation * param.EstimatedRotationSpeed;
            result.EstimatedMaxSpeedInMmSec = result.DistancePerRotationInMm * param.EstimatedRotationSpeed;
            if (Math.Abs(param.TimeToAcc) > double.Epsilon)
            {
                result.EstimatedAccelerationInMmSec2 = result.EstimatedMaxSpeedInMmSec / param.TimeToAcc;
                result.EstimatedAcc                  = Math.Sqrt(result.EstimatedMaxStepRate / param.TimeToAcc) * acc_corr;
            }

            if (Math.Abs(param.TimeToDec) > double.Epsilon)
            {
                result.EstimatedDecelerationInMmSec2 = result.EstimatedMaxSpeedInMmSec / param.TimeToDec;
                result.EstimatedDec                  = Math.Sqrt(result.EstimatedMaxStepRate / param.TimeToDec) * acc_corr;
            }

            result.EstimatedJerkSpeed = result.EstimatedMaxStepRate / jerkFactor;

            result.MaxStepRate    = (uint)Math.Round(result.EstimatedMaxStepRate, 0);
            result.Acc            = (ushort)Math.Round(result.EstimatedAcc,       0);
            result.Dec            = (ushort)Math.Round(result.EstimatedDec,       0);
            result.JerkSpeed      = (uint)Math.Round(result.EstimatedJerkSpeed,   0);
            result.StepsPerMm1000 = (float)(result.StepsPerMm / 1000.0);

            return await Task.FromResult(result);
        }
    }
}