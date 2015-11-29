////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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
////////////////////////////////////////////////////////

#pragma once

////////////////////////////////////////////////////////

#define MYNUM_AXIS 3

////////////////////////////////////////////////////////

#include <Steppers/StepperSMC800.h>

////////////////////////////////////////////////////////

#define CMyStepper CStepperSMC800
#define ConversionToMm1000 CMotionControlBase::ToMm1000_1_400
#define ConversionToMachine CMotionControlBase::ToMachine_1_400

////////////////////////////////////////////////////////

#define X_STEPSPERMM 400.0
#define Y_STEPSPERMM 400.0
#define Z_STEPSPERMM 400.0
#define A_STEPSPERMM 400.0

////////////////////////////////////////////////////////

#define X_MAXSIZE 200000        // in mm1000_t
#define Y_MAXSIZE 200000 
#define Z_MAXSIZE 100000 
#define A_MAXSIZE 50000 

////////////////////////////////////////////////////////

#define X_USEREFERENCE_MIN  
//#define X_USEREFERENCE_MAX

#define Y_USEREFERENCE_MIN  
//#define Y_USEREFERENCE_MAX

//#define Z_USEREFERENCE_MIN  
#define Z_USEREFERENCE_MAX

//#define A_USEREFERENCE_MIN  
//#define A_USEREFERENCE_MAX

#define REFMOVE_1_AXIS  Z_AXIS
#define REFMOVE_2_AXIS  Y_AXIS
#define REFMOVE_3_AXIS  X_AXIS
//#define REFMOVE_3_AXIS  A_AXIS

////////////////////////////////////////////////////////

#define GO_DEFAULT_STEPRATE   20000 // steps/sec
#define G1_DEFAULT_STEPRATE   10000 // steps/sec

////////////////////////////////////////////////////////

#define CNC_MAXSPEED 3000
#define CNC_ACC  150
#define CNC_DEC  180

////////////////////////////////////////////////////////

#define CONTROLLERFAN_ONTIME	10000			// switch off controllerfan if idle for 10 Sec
//#define CONTROLLERFAN_FAN_PIN	-1 //14 // 10
#undef CONTROLLERFAN_FAN_PIN

#define CONTROLLERFAN_DIGITAL_ON  HIGH
#define CONTROLLERFAN_DIGITAL_OFF LOW

////////////////////////////////////////////////////////

#define SPINDEL_ENABLE_PIN	15

#define SPINDEL_DIGITAL_ON  LOW
#define SPINDEL_DIGITAL_OFF HIGH

////////////////////////////////////////////////////////

#define PROBE_PIN	16

#define PROBE_ON  LOW
#define PROBE_OFF HIGH

////////////////////////////////////////////////////////

#undef KILL_PIN	
