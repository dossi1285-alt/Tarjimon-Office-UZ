@echo off
setlocal EnableExtensions
cd /d "%~dp0"

echo ===============================================
echo Tarjimon Office UZ - Preflight 1.1.7 strict filter + merge test
echo ===============================================
echo.

set "PATCH=%CD%\PATCH_PREFLIGHT_1_1_7_STRICT_FILTER_AND_MERGE.ps1"
set "SOURCE=%CD%\TarjimonOfficeUZ.Setup.Preflight\ProgramV110.cs"
set "BACKUP=%CD%\ProgramV110.cs.1.1.0.backup"
set "BUILD=%CD%\FIX_BUILD_AND_BUILD.bat"

if not exist "%PATCH%" (echo XATO: Patch topilmadi: %PATCH% & pause & exit /b 1)
if not exist "%SOURCE%" (echo XATO: Source topilmadi: %SOURCE% & pause & exit /b 2)
if not exist "%BUILD%" (echo XATO: Asosiy build BAT topilmadi: %BUILD% & pause & exit /b 3)

if not exist "%BACKUP%" copy /y "%SOURCE%" "%BACKUP%" >nul
if not exist "%BACKUP%" (echo XATO: Stabil backup yaratilmadi. & pause & exit /b 4)

echo [1/3] 1.1.7 strict filter + duplicate merge patch qo'llanmoqda...
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%PATCH%"
if errorlevel 1 (echo XATO: Patch qo'llanmadi. & pause & exit /b 5)
echo OK.
echo.

echo [2/3] Asosiy build ishga tushirilmoqda...
call "%BUILD%"
set "RC=%ERRORLEVEL%"
echo.

echo [3/3] Stabil 1.1.0 source qaytarilmoqda...
copy /y "%BACKUP%" "%SOURCE%" >nul
if errorlevel 1 (
  echo DIQQAT: Source qaytarilmadi. Backup saqlandi: %BACKUP%
  set "RC=6"
) else (
  echo OK - Stabil 1.1.0 source qaytarildi.
  del /q "%BACKUP%" >nul 2>&1
)

echo.
if "%RC%"=="0" (
 echo ===============================================
 echo 1.1.7 STRICT FILTER + MERGE TEST BUILD MUVAFFAQIYATLI.
 echo ===============================================
) else (
 echo ===============================================
 echo 1.1.7 TEST XATO BILAN TUGADI. Kod: %RC%
 echo ===============================================
)
pause
exit /b %RC%
