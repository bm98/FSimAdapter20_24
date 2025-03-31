REM  my Adapter Export script
REM  copies from Export location to my redist folder
REM 
REM assumes to start in the solution folder of the Adapter

REM copy MSFS2020 Plug and SimConnects to Redist V2020 (my FSim Redist)
xcopy .\SimConnect_ExportLoc\V2020\*.* ..\..\Redist\V2020\ /C /Y /I
REM copy MSFS2024 Plug and SimConnects to Redist V2024 (my FSim Redist)
xcopy .\SimConnect_ExportLoc\V2024\*.* ..\..\Redist\V2024\ /C /Y /I

REM copy generic DLLs to Redist  (my FSim Redist)
xcopy .\SimConnect_ExportLoc\*.dll ..\..\Redist\ /C /Y /I
