 sc.exe delete CNCLib.SerialServer
 New-Service -Name "CNCLib.SerialServer" -BinaryPathName '"C:\dev\CNCLib\Src\CNCLib\CNCLib.Serial.Server\bin\Release\netcoreapp2.0\win-x64\publish\CNCLib.Serial.Server.exe"'