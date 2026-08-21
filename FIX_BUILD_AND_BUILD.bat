@echo off
setlocal EnableExtensions
cd /d "%~dp0"

echo ===============================================
echo Tarjimon Office UZ - MSI + Preflight Build
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

echo [1/5] GitHub'dagi installer kodi yangilanmoqda...
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

echo [2/5] Eski build fayllari tozalanmoqda...
for %%D in (
  "TarjimonOfficeUZ.Setup.Wix\bin"
  "TarjimonOfficeUZ.Setup.Wix\obj"
  "TarjimonOfficeUZ.Word\bin"
  "TarjimonOfficeUZ.Word\obj"
  "TarjimonOfficeUZ.Excel\bin"
  "TarjimonOfficeUZ.Excel\obj"
  "TarjimonOfficeUZ.Setup.Preflight\bin"
  "TarjimonOfficeUZ.Setup.Preflight\obj"
) do if exist "%%~D" rmdir /s /q "%%~D"
echo OK - eski build fayllari tozalandi.
echo.

set "MSBUILD=%ProgramFiles%\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"
if not exist "%MSBUILD%" set "MSBUILD=%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
if not exist "%MSBUILD%" (
  echo XATO: MSBuild topilmadi.
  pause
  exit /b 1
)

echo [3/5] Mustaqil MSI installer build qilinmoqda...
"%MSBUILD%" "TarjimonOfficeUZ.Setup.Wix\TarjimonOfficeUZ.Setup.Wix.wixproj" /t:Build /p:Configuration=Debug /m
set "RC=%ERRORLEVEL%"
if not "%RC%"=="0" goto BUILD_ERROR

set "MSI=%CD%\TarjimonOfficeUZ.Setup.Wix\bin\Debug\TarjimonOfficeUZ.msi"
if not exist "%MSI%" (
  echo XATO: MSI build natijasi topilmadi: %MSI%
  set "RC=2"
  goto BUILD_ERROR
)

copy /y "%MSI%" "%CD%\Tarjimon Office UZ.msi" >nul
if errorlevel 1 (
  echo XATO: MSI loyiha papkasidan asosiy papkaga nusxalanmadi.
  set "RC=3"
  goto BUILD_ERROR
)
echo OK - MSI tayyor.
echo.

echo [4/5] Preflight uchun NuGet/SDK restore qilinmoqda...
"%MSBUILD%" "TarjimonOfficeUZ.Setup.Preflight\TarjimonOfficeUZ.Setup.Preflight.csproj" /t:Restore /p:Configuration=Debug
set "RC=%ERRORLEVEL%"
if not "%RC%"=="0" goto BUILD_ERROR
echo OK - Preflight restore tugadi.
echo.

echo [4/5] Preflight launcher MSI bilan birga build qilinmoqda...
"%MSBUILD%" "TarjimonOfficeUZ.Setup.Preflight\TarjimonOfficeUZ.Setup.Preflight.csproj" /t:Build /p:Configuration=Debug /p:MsiSource="%MSI%" /m
set "RC=%ERRORLEVEL%"
if not "%RC%"=="0" goto BUILD_ERROR

set "PREFLIGHT=%CD%\TarjimonOfficeUZ.Setup.Preflight\bin\Debug\net48\TarjimonOfficeUZSetup.exe"
if not exist "%PREFLIGHT%" (
  echo XATO: Preflight launcher build natijasi topilmadi: %PREFLIGHT%
  set "RC=4"
  goto BUILD_ERROR
)

copy /y "%PREFLIGHT%" "%CD%\TarjimonOfficeUZSetup.exe" >nul
if errorlevel 1 (
  echo XATO: Preflight launcher asosiy papkaga nusxalanmadi.
  set "RC=5"
  goto BUILD_ERROR
)
echo OK - Yakuniy Setup EXE tayyor va MSI ichiga joylandi.
echo.

echo [5/5] Yakuniy fayllar tekshirilmoqda...
if not exist "%CD%\Tarjimon Office UZ.msi" (
  echo XATO: Yakuniy MSI topilmadi.
  set "RC=6"
  goto BUILD_ERROR
)
if not exist "%CD%\TarjimonOfficeUZSetup.exe" (
  echo XATO: Yakuniy Setup EXE topilmadi.
  set "RC=7"
  goto BUILD_ERROR
)

echo.
echo ===============================================
echo BUILD MUVAFFAQIYATLI YAKUNLANDI.
echo MSI:
echo D:\Tarjimon-Office-UZ\Tarjimon Office UZ.msi
echo FINAL SETUP:
echo D:\Tarjimon-Office-UZ\TarjimonOfficeUZSetup.exe
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
