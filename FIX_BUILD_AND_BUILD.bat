@echo off
setlocal EnableExtensions
cd /d "%~dp0"

echo ===============================================
echo Tarjimon Office UZ - MSI Build
 echo ===============================================
echo.

where git >nul 2>&1
if errorlevel 1 (
  echo XATO: Git topilmadi.
  pause
  exit /b 1
)

if not exist ".git" (
  echo XATO: Bu BAT Git repository ichida ishlashi kerak.
  pause
  exit /b 1
)

echo [1/3] GitHub'dagi installer kodi yangilanmoqda...
git fetch origin release/1.0-installer-cleanup
if errorlevel 1 (
  echo XATO: GitHub'dan yangilash muvaffaqiyatsiz.
  pause
  exit /b 1
)
git checkout FETCH_HEAD -- "TarjimonOfficeUZ.Setup.Wix\Package.wxs" "TarjimonOfficeUZ.Setup.Wix\TarjimonOfficeUZ.Setup.Wix.wixproj"
if errorlevel 1 (
  echo XATO: Installer fayllari yangilanmadi.
  pause
  exit /b 1
)
echo OK - Installer kodi yangilandi.
echo.

echo [2/3] Eski build fayllari tozalanmoqda...
if exist "TarjimonOfficeUZ.Setup.Wix\bin" rmdir /s /q "TarjimonOfficeUZ.Setup.Wix\bin"
if exist "TarjimonOfficeUZ.Setup.Wix\obj" rmdir /s /q "TarjimonOfficeUZ.Setup.Wix\obj"
if exist "TarjimonOfficeUZ.Word\bin" rmdir /s /q "TarjimonOfficeUZ.Word\bin"
if exist "TarjimonOfficeUZ.Word\obj" rmdir /s /q "TarjimonOfficeUZ.Word\obj"
if exist "TarjimonOfficeUZ.Excel\bin" rmdir /s /q "TarjimonOfficeUZ.Excel\bin"
if exist "TarjimonOfficeUZ.Excel\obj" rmdir /s /q "TarjimonOfficeUZ.Excel\obj"

echo.
echo [3/3] Mustaqil MSI installer build qilinmoqda...
set "MSBUILD=%ProgramFiles%\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"
if not exist "%MSBUILD%" set "MSBUILD=%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
if not exist "%MSBUILD%" (
  echo XATO: MSBuild topilmadi.
  pause
  exit /b 1
)

"%MSBUILD%" "TarjimonOfficeUZ.Setup.Wix\TarjimonOfficeUZ.Setup.Wix.wixproj" /t:Build /p:Configuration=Debug /m
set "RC=%ERRORLEVEL%"
if not "%RC%"=="0" goto BUILD_ERROR

set "MSI=TarjimonOfficeUZ.Setup.Wix\bin\Debug\TarjimonOfficeUZ.msi"
if not exist "%MSI%" (
  echo XATO: MSI build natijasi topilmadi: %MSI%
  set "RC=2"
  goto BUILD_ERROR
)

copy /y "%MSI%" "%~dp0Tarjimon Office UZ.msi" >nul
if errorlevel 1 (
  echo XATO: MSI loyiha papkasidan asosiy papkaga nusxalanmadi.
  set "RC=3"
  goto BUILD_ERROR
)

echo.
echo ===============================================
echo BUILD MUVAFFAQIYATLI YAKUNLANDI.
echo MSI tayyor:
echo D:\Tarjimon-Office-UZ\Tarjimon Office UZ.msi
echo ===============================================
pause
exit /b 0

:BUILD_ERROR
echo.
echo ===============================================
echo BUILD XATO BILAN TUGADI. Kod: %RC%
echo ===============================================
pause
exit /b %RC%
