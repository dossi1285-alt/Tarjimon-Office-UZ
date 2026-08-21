@echo off
setlocal EnableExtensions
cd /d "%~dp0"

echo ===============================================
echo Tarjimon Office UZ - Preflight 1.1.1 FILTER TEST
echo ===============================================
echo.

set "PATCH=%CD%\PATCH_PREFLIGHT_FILTER_1_1_1.ps1"
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

echo [1/4] 1.1.0 source backup va filter patch...
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%PATCH%"
if errorlevel 1 (
  echo XATO: Filter patch qo'llanmadi.
  pause
  exit /b 3
)
echo OK.
echo.

echo [2/4] 1.1.1 test build...
call "%CD%\FIX_BUILD_AND_BUILD.bat"
set "RC=%ERRORLEVEL%"
echo.

echo [3/4] 1.1.0 source qaytarilmoqda...
if exist "%BACKUP%" (
  copy /y "%BACKUP%" "%SOURCE%" >nul
  if errorlevel 1 (
    echo DIQQAT: backup qaytarilmadi.
    set "RC=4"
  ) else (
    echo OK - 1.1.0 source qaytarildi.
  )
) else (
  echo XATO: 1.1.0 backup topilmadi: %BACKUP%
  set "RC=5"
)

echo.
echo [4/4] Vaqtinchalik backup o'chirilmoqda...
if exist "%BACKUP%" del /q "%BACKUP%"

echo.
echo ===============================================
if "%RC%"=="0" (
  echo 1.1.1 FILTER TEST BUILD MUVAFFAQIYATLI.
  echo 1.1.0 SOURCE SAQLANDI.
) else (
  echo 1.1.1 FILTER TEST BUILD XATO BILAN TUGADI. Kod: %RC%
  echo 1.1.0 SOURCE QAYTARILDI.
)
echo ===============================================
pause
exit /b %RC%
