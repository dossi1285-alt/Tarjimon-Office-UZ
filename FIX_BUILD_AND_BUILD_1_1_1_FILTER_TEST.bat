@echo off
setlocal EnableExtensions
cd /d "%~dp0"

echo ===============================================
echo Tarjimon Office UZ - Preflight 1.1.1 FILTER TEST
echo ===============================================
echo.

set "PATCH=%CD%\PATCH_PREFLIGHT_FILTER_1_1_1.ps1"
set "SOURCE=%CD%\TarjimonOfficeUZ.Setup.Preflight\ProgramV110.cs"
set "BACKUP=%CD%\TarjimonOfficeUZ.Setup.Preflight\obj\PreflightPatchBackup\ProgramV110.cs"

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

echo [1/3] Faqat test uchun Preflight filter patch qo'llanmoqda...
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%PATCH%"
if errorlevel 1 (
  echo XATO: Filter patch qo'llanmadi.
  pause
  exit /b 3
)
echo OK.
echo.

echo [2/3] Asosiy build ishga tushirilmoqda...
call "%CD%\FIX_BUILD_AND_BUILD.bat"
set "RC=%ERRORLEVEL%"
echo.

echo [3/3] Stabil ProgramV110.cs qaytarilmoqda...
if exist "%BACKUP%" (
  copy /y "%BACKUP%" "%SOURCE%" >nul
  if errorlevel 1 (
    echo DIQQAT: backup qaytarilmadi.
    set "RC=4"
  ) else (
    echo OK - source stabil holatga qaytarildi.
  )
) else (
  echo DIQQAT: backup topilmadi, source qaytarilmadi.
  set "RC=5"
)

echo.
echo ===============================================
if "%RC%"=="0" (
  echo 1.1.1 FILTER TEST BUILD MUVAFFAQIYATLI.
) else (
  echo 1.1.1 FILTER TEST BUILD XATO BILAN TUGADI. Kod: %RC%
)
echo ===============================================
pause
exit /b %RC%
