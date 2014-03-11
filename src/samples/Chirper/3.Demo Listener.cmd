@setlocal
@echo off
@if NOT "%ECHO%"=="" @echo %ECHO%

set CMDHOME=%~dp0.

set USER=44444842

if exist "%CMDHOME%\ChirperClient\bin\Debug\Chirper2012Client.exe" (
"%CMDHOME%\ChirperClient\bin\Debug\Chirper2012Client.exe" %USER%
) else (
@echo Build Chirper2012.sln and then run the program
pause
)

