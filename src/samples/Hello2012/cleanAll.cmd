@setlocal
@echo off
@if NOT "%ECHO%"=="" @echo %ECHO%

set CMDHOME=%~dp0

@echo Deleting build output files
For /D %%a in (%CMDHOME%*) Do (
if exist "%%a\bin" (
@echo Deleting files in %%a\bin
del /S/Q "%%a\bin"
)
if exist "%%a\obj" (
@echo Deleting files in %%a\obj
del /S/Q "%%a\obj"
)
)

@echo Deleting Deployment drop files
For /D %%a in (%CMDHOME%*Deployment) Do (
For /D %%b in (%%a\*Deployment) Do (
if exist "%%b\Debug" (
@echo Deleting deployment files in %%b
rd /S/Q "%%b\Debug"
)
)
)

pause
