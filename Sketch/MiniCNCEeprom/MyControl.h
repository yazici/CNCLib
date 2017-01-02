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

#pragma once

////////////////////////////////////////////////////////

#include <Control.h>
#include <OnOffIOControl.h>
#include <Analog8IOControl.h>
#include <ReadPinIOControl.h>
#include <PushButtonLow.h>
#include <DummyIOControl.h>
#include <ControlTemplate.h>

#include "Configuration_MiniCNC.h"

////////////////////////////////////////////////////////

class CMyControl : public CControl
{
private:

	typedef CControl super;

public:

	CMyControl()				 { }

	virtual void Kill() override;

	virtual void IOControl(uint8_t tool, unsigned short level) override;
	virtual unsigned short IOControl(uint8_t tool) override;

protected:

	virtual void Init() override;
	virtual void Initialized() override;

	virtual void TimerInterrupt() override;

	bool IsKill() override;
	virtual void Poll() override;
	virtual bool Parse(CStreamReader* reader, Stream* output) override;
	virtual void GoToReference() override;
	virtual bool GoToReference(axis_t axis, steprate_t steprate, bool toMinRef) override;

	virtual bool OnEvent(EnumAsByte(EStepperControlEvent) eventtype, uintptr_t addinfo) override;

public:

  #define EEPROM_NUM_AXIS 4
  
	enum EConfigSlot
	{
		Signature=0,
		MaxSize = 1,
		ScaleMmToMachine = MaxSize+EEPROM_NUM_AXIS,
		ReferenceType = ScaleMmToMachine+1,
		RefMove,

		MaxStepRate,
		Acc,
		Dec,
		RefMoveStepRate
	};

  struct SCNCEeprom
  {
    uint32_t  signature;
    mm1000_t  maxsize[EEPROM_NUM_AXIS];

    float     ScaleMm1000ToMachine;
  
	uint8_t	  referenceType[EEPROM_NUM_AXIS];
    uint8_t   refmove[EEPROM_NUM_AXIS];
  
    uint32_t  maxsteprate;
    uint32_t  acc;
    uint32_t  dec;
    uint32_t  refmovesteprate;
  };

private:

#ifdef SPINDEL_ENABLE_PIN
	#ifdef SPINDEL_ANALOGSPEED
		CAnalog8IOControl<SPINDEL_ENABLE_PIN> _spindel;
		inline uint8_t ConvertSpindelSpeedToIO(unsigned short level) { return (uint8_t)MulDivU32(abs(level), 255, SPINDEL_MAXSPEED)); }
#else
		COnOffIOControl<SPINDEL_ENABLE_PIN, SPINDEL_DIGITAL_ON, SPINDEL_DIGITAL_OFF> _spindel;
		inline uint8_t ConvertSpindelSpeedToIO(unsigned short level) { return (uint8_t) level; }
#endif
	#ifdef SPINDEL_DIR_PIN
		COnOffIOControl<SPINDEL_DIR_PIN, SPINDEL_DIR_CLW, SPINDEL_DIR_CCLW> _spindelDir;
	#else
		CDummyIOControl _spindelDir;
#endif
#else
	CDummyIOControl _spindel;
	CDummyIOControl _spindelDir;
	inline uint8_t ConvertSpindelSpeedToIO(unsigned short level) { return level; }
#endif  

#ifdef COOLANT_PIN
	COnOffIOControl<COOLANT_PIN, COOLANT_ON, COOLANT_OFF> _coolant;
#else
	CDummyIOControl _coolant;
#endif
#ifdef PROBE_PIN
	CReadPinIOControl<PROBE_PIN, PROBE_ON> _probe;
#else
	CDummyIOControl _probe;
#endif

#ifdef KILL_PIN
	CReadPinIOControl<KILL_PIN, KILL_PIN_ON> _kill;
#else
	CDummyIOControl _kill;
#endif

#if defined(HOLD_PIN) && defined(RESUME_PIN)
	CPushButtonLow _hold;
	CPushButtonLow _resume;
#else
	CDummyIOControl _hold;
	CDummyIOControl _resume;
#endif

#ifdef CONTROLLERFAN_FAN_PIN
	#ifdef CONTROLLERFAN_ANALOGSPEED
		#if defined(USE_RAMPSFD)
			CAnalog8InvertIOControl<CONTROLLERFAN_FAN_PIN> _controllerfan;
		#else
			CAnalog8IOControl<CONTROLLERFAN_FAN_PIN> _controllerfan;
		#endif
	#else
		COnOffIOControl<CONTROLLERFAN_FAN_PIN, CONTROLLERFAN_DIGITAL_ON, CONTROLLERFAN_DIGITAL_OFF> _controllerfan;
	#endif
	inline bool IsControllerFanTimeout() { return millis() - CStepper::GetInstance()->IdleTime() > CONTROLLERFAN_ONTIME;	}
#else
	CDummyIOControl _controllerfan;
	inline bool IsControllerFanTimeout() { return false; }
#endif

};

////////////////////////////////////////////////////////

extern CMyControl Control;
