////////////////////////////////////////////////////////
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
////////////////////////////////////////////////////////

#include "StepperL298N.h"

////////////////////////////////////////////////////////

 // 1010 -> 1000 -> 1001 -> 0001 -> 0101 -> 0100 -> 0110 -> 0010
static const unsigned char _L298Nhalfstep4Pin[8] PROGMEM = { 10, 8, 9, 1, 5, 4, 6, 2 };

// 1010 -> 1001 -> 0101 -> 0110
static const unsigned char _L298Nfullstep4Pin[4] PROGMEM = { 10, 9, 5, 6 };
// static unsigned char _L298Nfullstep4Pin[4] = { 1+2, 2+4, 4+8, 8+1 };

 // 1010 -> 1001 -> 0101 -> 0110
 // aAbB => a => !a=A 
static const unsigned char _L298Nfullstep2Pin[4] PROGMEM = { 3, 2, 0, 1 };

 ////////////////////////////////////////////////////////
 
 CStepperL298N::CStepperL298N()
{
}

////////////////////////////////////////////////////////

 // reference: difference between microswitch (on=LOW) and optical (on=>HIGH) 
 unsigned char CStepperL298N::_referenceOn = LOW;

pin_t CStepperL298N::_pin[NUM_AXIS][4] =
{
	{ 2, 3, 4, 5 },
	{ 6, 7, 8, 9 },
#if defined(__AVR_ATmega328P__)
	{ 14, 15, 16, 17 },			// A0-A3
#if NUM_AXIS > 3
	{ 18, 19, 11, 12 },			// A4&5,11&12
#endif
#else
	{ 54, 55, 56, 57 },			// A0-A3
#if NUM_AXIS > 3
	{ 58, 59, 60, 61 }			// A4-A5
#endif
#endif
};

pin_t CStepperL298N::_pinenable[NUM_AXIS][2] =
{
	{ 10, 0 },		// 0 ... not used
	{ 10, 0 },		// 0 ... not used
	{ 10, 0 }		// 0 ... not used
#if NUM_AXIS > 3
	,{ 10, 0 }		// 0 ... not used
#endif
//	{}
};

pin_t CStepperL298N::_pinRef[NUM_AXIS*2] =
{
	0				// 0 .. not used
};

////////////////////////////////////////////////////////

void CStepperL298N::Init(void)
{	
	super::Init();

	InitMemVar();

	register unsigned char i;

	for (i = 0; i < NUM_AXIS; i++)
	{
		if (IsActive(i))
		{
			CHAL::pinMode(_pin[i][0], OUTPUT);
			CHAL::pinMode(_pin[i][1], OUTPUT);
			if (Is4Pin(i))
			{
				CHAL::pinMode(_pin[i][2], OUTPUT);
				CHAL::pinMode(_pin[i][3], OUTPUT);
			}

			if (IsUseEN1(i))
			{
				CHAL::pinMode(_pinenable[i][0], OUTPUT);
				if (IsUseEN2(i)) CHAL::pinMode(_pinenable[i][1], OUTPUT);
			}

			if (ToReferenceId(i, true) != 0)  CHAL::pinMode(_pinenable[i][1], _referenceOn==LOW ? INPUT_PULLUP : INPUT);
			if (ToReferenceId(i, false) != 0) CHAL::pinMode(_pinenable[i][1], _referenceOn==LOW ? INPUT_PULLUP : INPUT);
		}
	}
}
////////////////////////////////////////////////////////

void CStepperL298N::InitMemVar()
{
	register unsigned char i;
	for (i = 0; i < NUM_AXIS; i++)	_stepIdx[i] = 0;
}

////////////////////////////////////////////////////////

void  CStepperL298N::Step(const unsigned char steps[NUM_AXIS], axisArray_t directionUp)
{
	for (axis_t axis=0;axis < NUM_AXIS;axis++)
	{
		if (steps[axis])
		{
			if (directionUp&1)
				_stepIdx[axis] += steps[axis];
			else
				_stepIdx[axis] -= steps[axis];
			SetPhase(axis);
		}
		directionUp /= 2;
	}
}

////////////////////////////////////////////////////////

void CStepperL298N::SetEnable(axis_t axis, unsigned char level, bool  force)
{
	if (IsActive(axis))
	{
		if (IsUseEN1(axis))
		{
			CHAL::digitalWrite(_pinenable[axis][0], level != LevelOff ? HIGH : LOW);
			if (IsUseEN2(axis)) CHAL::digitalWrite(_pinenable[axis][1], level != LevelOff ? HIGH : LOW);
		}
		else if (Is2Pin(axis))
		{
			// 2PIN and no enable => can't be turned off
		}
		else
		{
			if (level == LevelOff)
			{
				// 4 PIN => set all to off

				CHAL::digitalWrite(_pin[axis][0], LOW);
				CHAL::digitalWrite(_pin[axis][1], LOW);
				CHAL::digitalWrite(_pin[axis][2], LOW);
				CHAL::digitalWrite(_pin[axis][3], LOW);
			}
			else if (force)
			{
				SetPhase(axis);
			}
		}
	}
}

////////////////////////////////////////////////////////

unsigned char CStepperL298N::GetEnable(axis_t axis)
{
	if (!IsActive(axis)) return LevelOff;

	if (IsUseEN1(axis))
	{
		return ConvertLevel(CHAL::digitalRead(_pinenable[axis][0]) != LOW && 
							(IsUseEN2(axis) ? CHAL::digitalRead(_pinenable[axis][1]) != LOW : true));
	}

	if (Is2Pin(axis))	return LevelMax;		// 2PIN and no enable => can't be turned off

	// no enable PIN => with 4 PIN test if one PIN is set

	return ConvertLevel(
		CHAL::digitalRead(_pin[axis][0]) != LOW ||
		CHAL::digitalRead(_pin[axis][1]) != LOW ||
		CHAL::digitalRead(_pin[axis][2]) != LOW ||
		CHAL::digitalRead(_pin[axis][3]) != LOW);
}

////////////////////////////////////////////////////////

void CStepperL298N::SetPhase(axis_t axis)
{
	if (IsActive(axis))
	{
		register unsigned char bitmask;

		if (Is4Pin(axis))
		{
			if (_pod._stepMode[axis] == FullStep)
			{
				bitmask = pgm_read_byte(&_L298Nfullstep4Pin[_stepIdx[axis] & 0x3]);
			}
			else
			{
				bitmask = pgm_read_byte(&_L298Nhalfstep4Pin[_stepIdx[axis] & 0x7]);
			}
		}
		else
		{
			// 2 pin, only full step
			bitmask = pgm_read_byte(&_L298Nfullstep2Pin[_stepIdx[axis] & 0x3]);
		}

		CHAL::digitalWrite(_pin[axis][0], bitmask & 1);
		CHAL::digitalWrite(_pin[axis][1], bitmask & 2);
		CHAL::digitalWrite(_pin[axis][2], bitmask & 4);
		CHAL::digitalWrite(_pin[axis][3], bitmask & 8);
	}
}

////////////////////////////////////////////////////////

bool  CStepperL298N::IsReference(unsigned char referenceid)
{
	if (_pinRef[referenceid] != 0)
		return CHAL::digitalRead(_pinRef[referenceid]) == _referenceOn;

	return false;
}

////////////////////////////////////////////////////////

bool  CStepperL298N::IsAnyReference()
{
	for (axis_t axis = 0; axis < NUM_AXIS; axis++)
	{
		if ((_pod._useReference[axis * 2] && _pinRef[axis * 2] && CHAL::digitalRead(_pinRef[axis * 2]) == _referenceOn) ||
			(_pod._useReference[axis * 2 + 1] && _pinRef[axis * 2 + 1] && CHAL::digitalRead(_pinRef[axis * 2 + 1]) == _referenceOn))
			return true;
	}
	return false;
}
