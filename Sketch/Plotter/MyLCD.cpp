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

#define _CRT_SECURE_NO_WARNINGS

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <arduino.h>

#include <StepperLib.h>
#include <CNCLib.h>
#include <LiquidCrystal_I2C.h>

#include "MyLCD.h"

#ifdef __USE_LCD__

#include "PlotterControl.h"
#include "HPGLParser.h"

////////////////////////////////////////////////////////////

#define I2C_ADDR    0x27  // Define I2C Address where the PCF8574A is
#define BACKLIGHT_PIN     3
#define En_pin  2
#define Rw_pin  1
#define Rs_pin  0
#define D4_pin  4
#define D5_pin  5
#define D6_pin  6
#define D7_pin  7

////////////////////////////////////////////////////////////

LiquidCrystal_I2C	lcd(I2C_ADDR, En_pin, Rw_pin, Rs_pin, D4_pin, D5_pin, D6_pin, D7_pin);

////////////////////////////////////////////////////////////

void CMyLcd::Init()
{
	lcd.setBacklightPin(BACKLIGHT_PIN, POSITIVE);
	lcd.setBacklight(HIGH);

	lcd.backlight();			// finish with backlight on  

	lcd.begin(MYLCD_COLS, MYLCD_ROWS);			// initialize the lcd for 20 chars 4 lines, turn on backlight

	super::Init();
}

////////////////////////////////////////////////////////////

void CMyLcd::TimerInterrupt()
{
	super::TimerInterrupt();
/*
	if (READ(KILL_PIN)==0)
	{
		CStepper::GetInstance()->AbortMove();
	}	

	button.Tick(READ(BTN_EN1),READ(BTN_EN2));
*/
}

////////////////////////////////////////////////////////////

void CMyLcd::FirstDraw()
{
	lcd.clear();
//	lcd.setCursor(0, 0); lcd.print(F("123456789001234567890"));
	lcd.setCursor(0, 0); lcd.print(F("X:xxx.xx"));
	lcd.setCursor(0, 1); lcd.print(F("Y:xxx.xx"));
	lcd.setCursor(0, 2); lcd.print(F("Pen:x"));
	//	lcd.setCursor(0, 2); lcd.print(F("Herbert Aitenbichler"));
	//	lcd.setCursor(0, 3); lcd.print(F("Herbert Aitenbichler"));
}

////////////////////////////////////////////////////////////

unsigned long CMyLcd::Splash()
{
	lcd.clear();
	lcd.setCursor(6, 0); //Start at character 4 on line 0
	lcd.print(F("Hello"));

	lcd.setCursor(0, 2);
	lcd.print(F("Herbert Aitenbichler"));
	lcd.setCursor(0, 3);
	lcd.print(F(__DATE__","__TIME__));

	delay(100);

	return 5000l;
}

////////////////////////////////////////////////////////////

void CMyLcd::Draw(EDrawType /* draw */)
{
  	DrawPos(2, 0, CStepper::GetInstance()->GetCurrentPosition(X_AXIS));
	DrawPos(2, 1, CStepper::GetInstance()->GetCurrentPosition(Y_AXIS));
	DrawES(17, 0, CStepper::GetInstance()->IsReference(CStepper::GetInstance()->ToReferenceId(X_AXIS, true)));
	DrawES(18, 0, CStepper::GetInstance()->IsReference(CStepper::GetInstance()->ToReferenceId(Y_AXIS, true)));
	DrawES(19, 0, CStepper::GetInstance()->IsReference(CStepper::GetInstance()->ToReferenceId(Z_AXIS, true)));
	DrawPen(4, 2);

	lcd.setCursor(0,3);
	lcd.print(CStepper::GetInstance()->GetTotalSteps());

        unsigned char queued = CStepper::GetInstance()->QueuedMovements();
    	lcd.setCursor(18, 3);
        if (queued<10)
          lcd.print(' ');
        lcd.print((short) queued);
}

////////////////////////////////////////////////////////////

void CMyLcd::DrawPos(unsigned char col, unsigned char row, unsigned long pos)
{
	lcd.setCursor(col, row);

	char tmp[16];

	// draw in mm

	unsigned long hpglunits = RoundMulDivI32(pos, CHPGLParser::_state.HPDiv, CHPGLParser::_state.HPMul);

	unsigned short mm = (unsigned short) (hpglunits / 40);
	if (mm >= 1000)
		mm = 999;
	unsigned short mm100 = (hpglunits % 40) * 5 / 2;
	_itoa(mm, tmp, 10);
	int len = strlen(tmp);
	tmp[len++] = '.';
	if (mm100 < 10)
		tmp[len++] = '0';
	_itoa(mm100, tmp + len, 10);

	if (mm < 10)
		lcd.print(F("  "));
	else if (mm < 100)
		lcd.print(F(" "));
	lcd.print(tmp);
}

////////////////////////////////////////////////////////////

void CMyLcd::DrawES(unsigned char col, unsigned char row, bool es)
{
	lcd.setCursor(col, row);
	lcd.print(es ? F("1") : F("0"));
}

////////////////////////////////////////////////////////////

void CMyLcd::DrawPen(unsigned char col, unsigned char row)
{
	lcd.setCursor(col, row);
	lcd.print(Plotter.GetPen());
	lcd.print(Plotter.IsPenDown() ? F(" down") : F(" up  ") );
}

#endif
