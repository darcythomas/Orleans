@setlocal
@echo off
@if NOT "%ECHO%"=="" @echo %ECHO%

set CMDHOME=%~dp0.
set USER=44444841

if exist "%CMDHOME%\ChirperUI\bin\Debug\Chirper2012UI.exe" (
"%CMDHOME%\ChirperClient\bin\Debug\Chirper2012Client.exe" /pub %USER%
) else (
@echo Build Chirper2012.sln and then run the program
pause
)
