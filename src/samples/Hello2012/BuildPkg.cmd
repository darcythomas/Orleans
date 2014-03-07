@setlocal
@echo off
@if NOT "%ECHO%"=="" @echo %ECHO%

set CMDHOME=%~dp0.

if "%1"=="" (
  set OUTDIR=C:\Orleans
) else (
  set OUTDIR=%1
)
if "%2"=="" (
  set INDIR=.
) else (
  set INDIR=%2
)

set APPNAME=Hello2012

set APPDIR=%OUTDIR%\Applications
set ORLCLIENTDIR=%OUTDIR%\OrleansClient

set MYAPPDIR=%APPDIR%\%APPNAME%
set MYCLIENTDIR=%OUTDIR%\%APPNAME%Client
set MYSAMPLESRC=%OUTDIR%\Samples\%APPNAME%

@echo == Building %APPNAME% deployment package to %OUTDIR%

@echo InDir=%INDIR%
@echo OutDir=%OUTDIR%
@echo AppDir=%APPDIR%

@echo MYAPPDIR=%MYAPPDIR%
@echo MYCLIENTDIR=%MYCLIENTDIR%
@echo MYSAMPLESRC=%MYSAMPLESRC%

if not exist "%OUTDIR%" (md "%OUTDIR%")
if not exist "%APPDIR%" (md "%APPDIR%")
if not exist "%MYAPPDIR%" (md "%MYAPPDIR%")
if not exist "%MYCLIENTDIR%" (md "%MYCLIENTDIR%")
if not exist "%MYSAMPLESRC%" (md "%MYSAMPLESRC%")

@ECHO == Copy %APPNAME% server binaries
xcopy /y "%INDIR%\%APPNAME%GrainInterfaces.*" "%MYAPPDIR%\"
xcopy /y "%INDIR%\%APPNAME%Grains*" "%MYAPPDIR%\"

@ECHO == Copy %APPNAME% client binaries
xcopy /y "%INDIR%\%APPNAME%GrainInterfaces.*" "%MYCLIENTDIR%\"
xcopy /y "%INDIR%\%APPNAME%ConsoleApp.*" "%MYCLIENTDIR%\"
xcopy /y "%INDIR%\ClientConfiguration.xml" "%MYCLIENTDIR%\"
xcopy /y /s "%ORLCLIENTDIR%\*" "%MYCLIENTDIR%\"

@ECHO == Copy %APPNAME% sample sources
xcopy /y /s "%CMDHOME%\*" "%MYSAMPLESRC%\"
del "%MYSAMPLESRC%\*.cmd"
del "%MYSAMPLESRC%\*.vsprops"
del "%MYSAMPLESRC%\*.vssscc"
For /D %%a in ("%MYSAMPLESRC%\*") Do ( 
@ECHO %%a
del "%%a\*.vspscc"
if exist "%%a\obj" (rd /q/s "%%a\obj")
)