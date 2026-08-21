@echo off
setlocal
cd /d "%~dp0.."
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0DIAGNOSE-KL-OFFICE.ps1"
if errorlevel 1 (
  echo.
  echo Diagnostic failed. Press any key to close.
  pause >nul
)
endlocal
