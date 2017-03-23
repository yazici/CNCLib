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
#include <string.h>
#include <stdlib.h>
#include <ctype.h>
#include <arduino.h>

#include <CNCLib.h>
#include <HelpParser.h>

#include "MyControl.h"
#include "HPGLParser.h"
#include "PlotterControl.h"

////////////////////////////////////////////////////////////

enum EPenMove
{
	PD,
	PU,
	PA,
	PR
};

////////////////////////////////////////////////////////////

CHPGLParser::SState CHPGLParser::_state;

////////////////////////////////////////////////////////////

mm1000_t CHPGLParser::HPGLToMM1000X(long xx)
{
	// HPGL unit is 1/40 mm => 0.025mm
	return (xx + _state._HPOffsetX) * 25;
}

////////////////////////////////////////////////////////////

mm1000_t CHPGLParser::HPGLToMM1000Y(long yy)
{
	return (yy + _state._HPOffsetY) * 25;
}

////////////////////////////////////////////////////////////

void CHPGLParser::ReadAndSkipSemicolon()
{
	char ch = _reader->SkipSpaces();
	if (ch == ';')
		_reader->GetNextChar();
}

////////////////////////////////////////////////////////////

void CHPGLParser::Parse()
{
	_reader->IsNextChar('\r');
	if (_reader->GetChar())
	{
		_reader->SkipSpaces();
		if (IsToken(F("SP"), false, false)) { SelectPenCommand();	return; }
		if (IsToken(F("VS"), false, false)) { PenVelocityCommand();	return; }
		if (IsToken(F("VN"), false, false)) { PenVelocityNormalCommand();	return; }
		if (IsToken(F("IN"), false, false)) { InitCommand();		return; }
		if (IsToken(F("PD"), false, false)) { PenMoveCommand(PD);	return; }
		if (IsToken(F("PU"), false, false)) { PenMoveCommand(PU);	return; }
		if (IsToken(F("PA"), false, false)) { PenMoveCommand(PA);	return; }
		if (IsToken(F("PR"), false, false)) { PenMoveCommand(PR);	return; }
		if (IsToken(F("LT"), false, false)) { IgnoreCommand();		return; }
		if (IsToken(F("WU"), false, false)) { IgnoreCommand();		return; }

		Error(MESSAGE_GCODE_IllegalCommand);
	}
}

////////////////////////////////////////////////////////////

void CHPGLParser::IgnoreCommand()
{
	_reader->MoveToEnd();
}

////////////////////////////////////////////////////////////

void CHPGLParser::InitCommand()
{
	// Init HPGL Parser

	Init();

	ReadAndSkipSemicolon();
}

////////////////////////////////////////////////////////////

void CHPGLParser::PenMoveCommand(uint8_t cmdidx)
{
	Plotter.Resume();

	switch (cmdidx)
	{
		case PU:	Plotter.DelayPenUp();		_state.FeedRate = _state.FeedRateUp; break;
		case PD:	Plotter.PenDown();			_state.FeedRate = _state.FeedRateDown; break;
		case PA:	_state._HPGLIsAbsolut = true;	break;
		case PR:	_state._HPGLIsAbsolut = false;	break;
	}

	if (IsToken(F("PD"), false, false)) { PenMoveCommand(PD);	return; }
	if (IsToken(F("PU"), false, false)) { PenMoveCommand(PU);	return; }
	if (IsToken(F("PA"), false, false)) { PenMoveCommand(PA);	return; }
	if (IsToken(F("PR"), false, false)) { PenMoveCommand(PR);	return; }

	while (IsInt(_reader->GetChar()))
	{
		long xIn = GetInt32();

		//all blank or colon

		if (IsInt(_reader->SkipSpaces()))
		{
			// only blank as seperator
		}
		else if (_reader->GetChar() == ',' && IsInt(_reader->GetNextCharSkipScaces()))
		{
			// Colon
		}
		else
		{
		ERROR_MISSINGARGUMENT:
			Error(F("Missing or invalid parameter"));
			return;
		}

		long yIn = GetInt32();

		if (_reader->IsError())	goto ERROR_MISSINGARGUMENT;

		mm1000_t x = HPGLToMM1000X(xIn);
		mm1000_t y = HPGLToMM1000Y(yIn);

		if (_state._HPGLIsAbsolut)
		{
			if (x != CMotionControlBase::GetInstance()->GetPosition(X_AXIS) || y != CMotionControlBase::GetInstance()->GetPosition(Y_AXIS))
			{
				Plotter.DelayPenNow();
				CMotionControlBase::GetInstance()->MoveAbsEx(_state.FeedRate, X_AXIS, x, Y_AXIS, y, -1);
			}
		}
		else
		{
			if (x != 0 || y != 0)
			{
				Plotter.DelayPenNow();
				CMotionControlBase::GetInstance()->MoveRelEx(_state.FeedRate, X_AXIS, x, Y_AXIS, y, -1);
			}
		}
		if (_reader->SkipSpaces() != ',')
			break;

		_reader->GetNextCharSkipScaces();
	}
	ReadAndSkipSemicolon();
}

////////////////////////////////////////////////////////////

void CHPGLParser::SelectPenCommand()
{
	uint8_t newpen = GetUInt8();
	if (!Plotter.SetPen(newpen))
		Error(F("Select Pen failed"));

	ReadAndSkipSemicolon();
}

////////////////////////////////////////////////////////////

void CHPGLParser::PenVelocityNormalCommand()
{
	_state.SetFeedRates();
	ReadAndSkipSemicolon();
}

////////////////////////////////////////////////////////////

void CHPGLParser::PenVelocityCommand()
{ 
	long velocityCmPerSec = GetInt32Scale(10, 1000000, 3, 255);

	if (IsError()) return;

	// feedrate is => mm_1000 / min
	_state.FeedRateDown = CMotionControlBase::GetInstance()->GetMaxFeedRate(X_AXIS,velocityCmPerSec * 60l * 10l);

	ReadAndSkipSemicolon();
}

