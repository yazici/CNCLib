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
*/
////////////////////////////////////////////////////////

#pragma once

////////////////////////////////////////////////////////

#define MESSAGE_CONTROL_KILLED						F("Killed - command ignored!")
#define MESSAGE_CONTROL_FLUSHBUFFER					F("Flush Buffer")
#define MESSAGE_CONTROL_RESULTS						F(" => ")

#define MESSAGE_EXPR_EMPTY_EXPR						F("Empty expression")
#define MESSAGE_EXPR_FORMAT							F("Expression format error")
#define MESSAGE_EXPR_UNKNOWN_FUNCTION				F("Unknown function")
#define MESSAGE_EXPR_SYNTAX_ERROR					F("Syntax error")
#define MESSAGE_EXPR_MISSINGRPARENTHESIS			F("Missing right parenthesis")
#define MESSAGE_EXPR_ILLEGAL_OPERATOR				F("Illegal operator")
#define MESSAGE_EXPR_ILLEGAL_FUNCTION				F("Illegal function")
#define MESSAGE_EXPR_UNKNOWN_VARIABLE				F("Unknown variable")
#define MESSAGE_EXPR_FRACTORIAL						F("factorial")

#define MESSAGE_GCODE_CommentNestingError			F("Comment nesting error")
#define MESSAGE_GCODE_VaribaleMustStartWithAlpha	F("varibale must start with alpha")
#define MESSAGE_GCODE_NoValidVaribaleName			F("no valid varibale name")
#define MESSAGE_GCODE_ParameterDoesntExist			F("parameter doesnt exist")
#define MESSAGE_GCODE_NoValidVaribaleName			F("no valid varibale name")
#define MESSAGE_GCODE_ParameterNotFound				F("Parameter not found")
#define MESSAGE_GCODE_ParameterNotFound				F("Parameter not found")
#define MESSAGE_GCODE_UnspportedParameterNumber		F("unspported # parameter number")
#define MESSAGE_GCODE_ValueGreaterThanMax			F("value greater than max")
#define MESSAGE_GCODE_LinenumberExpected			F("linenumber expected")
#define MESSAGE_GCODE_CcommandExpected				F("command expected")
#define MESSAGE_GCODE_UnsupportedGCommand			F("unsupported G command")
#define MESSAGE_GCODE_MCodeExpected					F("m-code expected")
#define MESSAGE_GCODE_UnspportedMCodeIgnored		F("unspported m code ignored")
#define MESSAGE_GCODE_ParamNoExpected				F("paramNo expected")
#define MESSAGE_GCODE_EqExpected					F("= expected")
#define MESSAGE_GCODE_CommandExpected				F("command expected")
#define MESSAGE_GCODE_IllegalCommand				F("Illegal command")
#define MESSAGE_GCODE_NoValidTool					F("No valid tool")
#define MESSAGE_GCODE_SpindleSpeedExceeded			F("Spindle speed exceeded")
#define MESSAGE_GCODE_AxisNotSupported				F("axis not supported")
#define MESSAGE_GCODE_AxisAlreadySpecified			F("axis already specified")
#define MESSAGE_GCODE_ParameterSpecifiedMoreThanOnce	F("parameter specified more than once")
#define MESSAGE_GCODE_AxisOffsetMustNotBeSpecified		F("Axis offset must not be specified")
#define MESSAGE_GCODE_RalreadySpecified				F("R already specified")
#define MESSAGE_GCODE_RalreadySpecified				F("R already specified")
#define MESSAGE_GCODE_PalreadySpecified				F("P already specified")
#define MESSAGE_GCODE_LalreadySpecified				F("L already specified")
#define MESSAGE_GCODE_LmustBe1_255					F("L must be 1..255")
#define MESSAGE_GCODE_QalreadySpecified				F("Q already specified")
#define MESSAGE_GCODE_QmustBeAPositivNumber			F("Q must be a positiv number")
#define MESSAGE_GCODE_FalreadySpecified				F("F already specified")
#define MESSAGE_GCODE_FeedrateWithG0				F("F not allowed with G0")
#define MESSAGE_GCODE_IJKandRspecified				F("IJK and R specified")
#define MESSAGE_GCODE_MissingIKJorR					F("missing IKJ or R")
#define MESSAGE_GCODE_360withRandMissingAxes		F("360 with R and missing axes")
#define MESSAGE_GCODE_STATUS_ARC_RADIUS_ERROR		F("STATUS_ARC_RADIUS_ERROR")
#define MESSAGE_GCODE_LExpected						F("L expected")
#define MESSAGE_GCODE_PExpected						F("P expected")
#define MESSAGE_GCODE_UnsupportedLvalue				F("unsupported L value")
#define MESSAGE_GCODE_UnsupportedCoordinateSystemUseG54Instead		F("unsupported coordinate system, use G54 instead")
#define MESSAGE_GCODE_G41G43AreNotAllowedWithThisCommand F("G41/G42 are not allowed with this command")
#define MESSAGE_GCODE_NoAxesForProbe				F("No axes for probe")
#define MESSAGE_GCODE_ProbeOnlyForXYZ				F("Probe only for X Y Z")
#define MESSAGE_GCODE_ProbeIOmustBeOff				F("Probe-IO must be off")
#define MESSAGE_GCODE_ProbeFailed					F("Probe failed")
#define MESSAGE_GCODE_NoValidTool					F("No valid tool")
#define MESSAGE_GCODE_QmustNotBe0					F("Q must not be 0")
#define MESSAGE_GCODE_RmustBeBetweenCurrentRZ		F("R must be between current < R < Z")
#define MESSAGE_GCODE_NotImplemented				F("Not Implemented")

#define MESSAGE_PARSER_EndOfCommandExpected			F("End of command expected")
#define MESSAGE_PARSER_NotANumber_MissingScale		F("Not a number: missing scale")
#define MESSAGE_PARSER_NotANumber_MaxScaleExceeded	F("Not a number: max scale exceeded")
#define MESSAGE_PARSER_ValueLessThanMin				F("value less than min")
#define MESSAGE_PARSER_ValueGreaterThanMax			F("value greater than max")
#define MESSAGE_PARSER_NotImplemented				F("not implemented")
#define MESSAGE_PARSER_NotANumber					F("not a number")
#define MESSAGE_PARSER_OverrunOfNumber				F("overrun of number")
#define MESSAGE_PARSER_ValueLessThanMin				F("value less than min")
#define MESSAGE_PARSER_ValueGreaterThanMax			F("value greater than max")
#define MESSAGE_PARSER_ValueLessThanMin				F("value less than min")
#define MESSAGE_PARSER_ValueGreaterThanMax			F("value greater than max")

////////////////////////////////////////////////////////
