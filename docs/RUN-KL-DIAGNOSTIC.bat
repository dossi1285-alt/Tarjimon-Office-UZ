@echo off
setlocal
cd /d "%~dp0.."
rem Windows PowerShell 5.1 can misread UTF-8 PS1 files without a BOM when -File is used.
rem Read the diagnostic explicitly as UTF-8, then execute it as a script block.
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "$script = Get-Content -LiteralPath '%~dp0DIAGNOSE-KL-OFFICE.ps1' -Raw -Encoding UTF8; & ([scriptblock]::Create($script))"
if errorlevel 1 (
  echo.
  echo Diagnostic failed. Press any key to close.
  pause >nul
)
endlocal
