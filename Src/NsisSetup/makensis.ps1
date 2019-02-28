Param(
	[Parameter(Mandatory=$False)]
    [string]$SourceBinFolder = "..\CNCLib\WpfClient.Start\bin\Debug\net471"
)

$OutputDir = "./Output"
if(!(Test-Path -Path $OutputDir )){
    New-Item -ItemType directory -Path $OutputDir
}

.\nsis\makensis.exe /DCopyFromFolder="$SourceBinFolder" CNCLib.WpfClient.Setup.nsi 
