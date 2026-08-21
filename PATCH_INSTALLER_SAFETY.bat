@echo off
setlocal
cd /d "%~dp0"
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0PATCH_INSTALLER_SAFETY.ps1"
if errorlevel 1 (
  echo PATCH XATO BILAN TUGADI.
  pause
  exit /b 1
)
echo PATCH MUVAFFAQIYATLI YAKUNLANDI.
pause
