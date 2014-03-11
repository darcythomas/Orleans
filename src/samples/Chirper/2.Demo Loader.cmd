@setlocal
@echo off
@if NOT "%ECHO%"=="" @echo %ECHO%

set CMDHOME=%~dp0.

set DATADIR=%CMDHOME%
set DATAFILE=Network-1000nodes-27000edges.graphml

if exist "%CMDHOME%\NetworkLoader\bin\Debug\Chirper2012NetworkLoader.exe" (
"%CMDHOME%\NetworkLoader\bin\Debug\Chirper2012NetworkLoader.exe" "%DATADIR%\%DATAFILE%"
) else (
@echo Build Chirper2012.sln and then run the program
)

pause