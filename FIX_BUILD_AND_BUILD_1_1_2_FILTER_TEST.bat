@echo off
setlocal EnableExtensions
cd /d "%~dp0"

echo ===============================================
echo Tarjimon Office UZ - Preflight 1.1.2 FILTER TEST
echo ===============================================
echo.

set "PATCH=%CD%\PATCH_PREFLIGHT_FILTER_1_1_2.ps1"
set "SOURCE=%CD%\TarjimonOfficeUZ.Setup.Preflight\ProgramV110.cs"
set "BACKUP=%CD%\ProgramV110.cs.1.1.0.backup"

if not exist "%PATCH%" (
  echo XATO: Patch fayli topilmadi:
  echo %PATCH%
  pause
  exit /b 1
)
if not exist "%SOURCE%" (
  echo XATO: ProgramV110.cs topilmadi:
  echo %SOURCE%
  pause
  exit /b 2
)

if not exist "%BACKUP%" copy /y "%SOURCE%" "%BACKUP%" >nul
if not exist "%BACKUP%" (
  echo XATO: 1.1.0 backup yaratilmagan.
  pause
  exit /b 3
)

echo [1/3] 1.1.2 filter patch qo'llanmoqda...
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%PATCH%"
if errorlevel 1 (
  echo XATO: Filter patch qo'llanmadi.
  pause
  exit /b 4
)
echo OK.
echo.

echo [2/3] Build ishga tushirilmoqda...
call "%CD%\FIX_BUILD_AND_BUILD.bat"
set "RC=%ERRORLEVEL%"
echo.

echo [3/3] 1.1.0 stabil source qaytarilmoqda...
copy /y "%BACKUP%" "%SOURCE%" >nul
if errorlevel 1 (
  echo XATO: Stabil source qaytarilmadi.
  set "RC=5"
) else (
  echo OK - 1.1.0 stabil source saqlandi.
)

echo.
echo ===============================================
if "%RC%"=="0" (
  echo 1.1.2 FILTER TEST BUILD MUVAFFAQIYATLI.
) else (
  echo 1.1.2 FILTER TEST BUILD XATO BILAN TUGADI. Kod: %RC%
)
echo ===============================================
pause
exit /b %RC%
