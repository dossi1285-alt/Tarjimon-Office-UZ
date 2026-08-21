@echo off
setlocal EnableExtensions
cd /d "%~dp0"

echo ===============================================
echo Tarjimon Office UZ - Build repair
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

echo [1/3] GitHub'dagi to'g'ri Program.cs olinmoqda...
git fetch origin release/1.0-installer-cleanup
if errorlevel 1 (
  echo XATO: GitHub'dan yangilash muvaffaqiyatsiz.
  pause
  exit /b 1
)

git checkout FETCH_HEAD -- "TarjimonOfficeUZ.Setup.Preflight\Program.cs"
if errorlevel 1 (
  echo XATO: Program.cs yangilanmadi.
  pause
  exit /b 1
)

echo OK - Program.cs yangilandi.
echo.

echo [2/3] Eski build fayllari tozalanmoqda...
if exist "TarjimonOfficeUZ.Setup.Preflight\bin" rmdir /s /q "TarjimonOfficeUZ.Setup.Preflight\bin"
if exist "TarjimonOfficeUZ.Setup.Preflight\obj" rmdir /s /q "TarjimonOfficeUZ.Setup.Preflight\obj"
if exist "TarjimonOfficeUZ.Setup.Wix\bin" rmdir /s /q "TarjimonOfficeUZ.Setup.Wix\bin"
if exist "TarjimonOfficeUZ.Setup.Wix\obj" rmdir /s /q "TarjimonOfficeUZ.Setup.Wix\obj"

echo.
echo [3/3] Installer build qilinmoqda...
set "MSBUILD=%ProgramFiles%\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"
if not exist "%MSBUILD%" set "MSBUILD=%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
if not exist "%MSBUILD%" (
  echo XATO: MSBuild topilmadi.
  echo Visual Studio 18/2022 Community o'rnatilganligini tekshiring.
  pause
  exit /b 1
)

"%MSBUILD%" "TarjimonOfficeUZ.Setup.Wix\TarjimonOfficeUZ.Setup.Wix.wixproj" /t:Build /p:Configuration=Debug /m
set "RC=%ERRORLEVEL%"

echo.
if "%RC%"=="0" (
  echo ===============================================
  echo BUILD MUVAFFAQIYATLI YAKUNLANDI.
  echo ===============================================
) else (
  echo ===============================================
  echo BUILD XATO BILAN TUGADI. Kod: %RC%
  echo ===============================================
)
echo.
pause
exit /b %RC%
