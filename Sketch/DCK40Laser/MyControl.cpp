////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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

#include <stdio.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <CNCLib.h>

#include <GCodeParser.h>
#include <ConfigEeprom.h>

#include "MyControl.h"

////////////////////////////////////////////////////////////

CMyControl Control;
CMotionControl MotionControl;
CConfigEeprom Eprom;
HardwareSerial& StepperSerial = Serial;

////////////////////////////////////////////////////////////

#ifndef MYNUM_AXIS
#error Please define MYNUM_AXIS
#endif

////////////////////////////////////////////////////////////

static const CConfigEeprom::SCNCEeprom eepromFlash PROGMEM =
{
	EPROM_SIGNATURE,
	NUM_AXIS, MYNUM_AXIS, offsetof(CConfigEeprom::SCNCEeprom,axis), sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions),
	0,0,
	0,0,0,0,
	CNC_MAXSPEED,
	CNC_ACC,
	CNC_DEC,
	STEPRATERATE_REFMOVE,
	MOVEAWAYFROMREF_MM1000,
	X_STEPSPERMM/1000.0,
	{
		{ X_MAXSIZE,     X_USEREFERENCE, REFMOVE_1_AXIS },
		{ Y_MAXSIZE,     Y_USEREFERENCE, REFMOVE_2_AXIS },
		{ Z_MAXSIZE,     Z_USEREFERENCE, REFMOVE_3_AXIS },
#if NUM_AXIS > 3
		{ A_MAXSIZE,     A_USEREFERENCE, REFMOVE_4_AXIS },
#endif
#if NUM_AXIS > 4
		{ B_MAXSIZE,     B_USEREFERENCE, REFMOVE_5_AXIS },
#endif
#if NUM_AXIS > 5
		{ C_MAXSIZE,     C_USEREFERENCE, REFMOVE_6_AXIS },
#endif
	}
};

////////////////////////////////////////////////////////////

void CMyControl::Init()
{
	CSingleton<CConfigEeprom>::GetInstance()->Init(sizeof(CConfigEeprom::SCNCEeprom), &eepromFlash, EPROM_SIGNATURE);

	CMotionControlBase::GetInstance()->InitConversionBestStepsPer(CConfigEeprom::GetConfigFloat(offsetof(CConfigEeprom::SCNCEeprom, StepsPerMm1000)));

#ifdef DISABLELEDBLINK
	DisableBlinkLed();
#endif

	StepperSerial.println(MESSAGE_MYCONTROL_Starting);

	super::Init();

#ifdef SETDIRECTION
	CStepper::GetInstance()->SetDirection(SETDIRECTION);
#endif

	for (uint8_t axis = 0; axis < NUM_AXIS; axis++)
	{
		EnumAsByte(EReverenceType) ref = (EReverenceType)CConfigEeprom::GetConfigU8(offsetof(CConfigEeprom::SCNCEeprom, axis[0].referenceType) + sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions)*axis);
		if (ref != NoReference)
			CStepper::GetInstance()->UseReference(CStepper::GetInstance()->ToReferenceId(axis, ref == EReverenceType::ReferenceToMin), true);

		CStepper::GetInstance()->SetLimitMax(axis, CMotionControlBase::GetInstance()->ToMachine(axis, CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, axis[0].size) + sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions)*axis)));
	}

	_controllerfan.Init(255);

	_laserPWM.Init();
	_laserOnOff.Init();

	_laserWater.Init();
	_laserVacuum.Init();

	_spindel.Init();
	_probe.Init();
	_kill.Init();
	_coolant.Init();

  	_hold.Init();
	_resume.Init();
	_holdresume.Init();

	CGCodeParserDefault::InitAndSetFeedRate(-STEPRATETOFEEDRATE(GO_DEFAULT_STEPRATE), G1_DEFAULT_FEEDPRATE, STEPRATETOFEEDRATE(G1_DEFAULT_MAXSTEPRATE));
	CStepper::GetInstance()->SetDefaultMaxSpeed(
		((steprate_t)CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, maxsteprate))),
		((steprate_t)CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, acc))),
		((steprate_t)CConfigEeprom::GetConfigU32(offsetof(CConfigEeprom::SCNCEeprom, dec))));

#ifdef MYUSE_LCD
	InitSD(SD_ENABLE_PIN);
#endif

}

////////////////////////////////////////////////////////////

void CMyControl::IOControl(uint8_t tool, unsigned short level)
{
	switch (tool)
	{
		case Spindel:

			if (level != 0)
			{
				_laserPWM.On((uint8_t)level);
				_laserOnOff.On();
			}
			else
			{
				_laserOnOff.Off();
			}
			return;

		case Vacuum:  _laserVacuum.Set(level > 0); return;
			// case Coolant: _laserWater.Set(level > 0); return; do not allow water turn off

	}

	super::IOControl(tool, level);
}

////////////////////////////////////////////////////////////

unsigned short CMyControl::IOControl(uint8_t tool)
{
	switch (tool)
	{
		case Spindel: 		{ return _laserPWM.IsOn(); }
		case Probe:			{ return _probe.IsOn(); }
		case Coolant: 		{ return _laserWater.IsOn(); }
		case Vacuum: 		{ return _laserVacuum.IsOn(); }
		case ControllerFan: { return _controllerfan.GetLevel(); }
	}

	return super::IOControl(tool);
}

////////////////////////////////////////////////////////////

void CMyControl::Kill()
{
	super::Kill();

	_spindel.Off();
	_coolant.Set(false);
	_laserOnOff.Off();
}

////////////////////////////////////////////////////////////

bool CMyControl::IsKill()
{
	if (false && _kill.IsOn())
	{
#ifdef MYUSE_LCD
		Lcd.Diagnostic(F("E-Stop"));
#endif
		return true;
	}
	return false;
}

////////////////////////////////////////////////////////////

void CMyControl::TimerInterrupt()
{
	super::TimerInterrupt();

	_hold.Check();
	_resume.Check();
	_holdresume.Check();
}

////////////////////////////////////////////////////////////

void CMyControl::Initialized()
{
	super::Initialized();

	_controllerfan.SetLevel(128);
}

////////////////////////////////////////////////////////////

void CMyControl::Poll()
{
	super::Poll();

	if (IsHold())
	{
		if (_resume.IsOn() || _holdresume.IsOn())
		{
			Resume();
#ifdef MYUSE_LCD
			Lcd.ClearDiagnostic();
#endif
		}
	}
	else if (_hold.IsOn() || _holdresume.IsOn())
	{
		Hold();
#ifdef MYUSE_LCD
		Lcd.Diagnostic(F("LCD Hold"));
#endif
	}
}

////////////////////////////////////////////////////////////

void CMyControl::GoToReference()
{
	for (axis_t i = 0; i < NUM_AXIS; i++)
	{
		axis_t axis = CConfigEeprom::GetConfigU8(offsetof(CConfigEeprom::SCNCEeprom, axis[0].refmoveSequence)+sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions)*i);
		if (axis < NUM_AXIS)
		{
			EnumAsByte(EReverenceType) referenceType = (EReverenceType)CConfigEeprom::GetConfigU8(offsetof(CConfigEeprom::SCNCEeprom, axis[0].referenceType)+sizeof(CConfigEeprom::SCNCEeprom::SAxisDefinitions)*axis);
			if (referenceType != EReverenceType::NoReference)
				super::GoToReference(axis,	0,referenceType == EReverenceType::ReferenceToMin);
		}
	}
}

////////////////////////////////////////////////////////////

bool CMyControl::OnEvent(EnumAsByte(EStepperControlEvent) eventtype, uintptr_t addinfo)
{
	switch (eventtype)
	{
		case OnStartCut:
		{
			if (CGCodeParserBase::IsSpindleOn())
			{
				bool newIsCutMove = addinfo != 0;
				if (CGCodeParserBase::IsCutMove() != newIsCutMove)
				{
					CStepper::GetInstance()->IoControl(CControl::Spindel, newIsCutMove ? CGCodeParserBase::GetSpindleSpeed() : 0);
				}
			}
			break;
		}
		case OnStartEvent:
			_controllerfan.On();
			_laserWater.On();
			_laserVacuum.On();
			break;
		case OnIdleEvent:
			if (IsControllerFanTimeout())
			{
				_controllerfan.Off();
			}
			if (_laserOnOff.IsOn() == false)
			{
				if (millis() - CStepper::GetInstance()->IdleTime() > LASERWATER_ONTIME)
				{
					_laserWater.Off();
				}
				if (millis() - CStepper::GetInstance()->IdleTime() > LASERVACUUM__ONTIME)
				{
					_laserVacuum.Off();
				}
			}
			break;
	}

	return super::OnEvent(eventtype, addinfo);
}
