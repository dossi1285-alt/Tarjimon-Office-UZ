@echo off
setlocal
cd /d "%~dp0"

echo ===============================================
echo Tarjimon Office UZ - 1.1.9 Display Filter
echo ===============================================

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0PATCH_PREFLIGHT_DISPLAY_FILTER_1_1_9.ps1"
if errorlevel 1 (
    echo.
    echo XATO: Display Filter patch qo'llanmadi.
    pause
    exit /b 1
)

echo.
echo OK - Filter patch tayyor.
echo Endi asosiy build BATni ishga tushiring.
echo.
pause
exit /b 0
