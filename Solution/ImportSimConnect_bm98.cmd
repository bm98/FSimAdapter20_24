REM  my SimConnect Import script
REM  copies from my redist folder into the adapters Import Location
REM 
REM assumes to start in the solution folder of the Adapter


xcopy ..\..\Redist\MS2020\*.dll .\SimConnect_ImportLoc\MS2020\   /C /Y /I
xcopy ..\..\Redist\MS2024\*.dll .\SimConnect_ImportLoc\MS2024\   /C /Y /I
