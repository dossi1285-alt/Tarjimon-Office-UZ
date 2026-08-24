@echo off
setlocal
chcp 65001 >nul
cd /d "%~dp0"

echo ===============================================
echo Tarjimon Office UZ - 1.1.10 Strict Display Filter
echo ===============================================

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0PATCH_DISPLAY_FILTER_1_1_10.ps1"
if errorlevel 1 (
    echo.
    echo XATO: 1.1.10 filter patch qo'llanmadi.
    pause
    exit /b 1
)

echo.
echo OK - 1.1.10 filter patch tayyor.
echo Endi asosiy build BATni ishga tushiring.
echo.
pause
exit /b 0
