@echo off
setlocal EnableExtensions
cd /d "%~dp0"
if not exist "TestResults" mkdir "TestResults"
set "OUT=%~dp0TestResults\WORD-ADDIN-SOURCE.txt"
> "%OUT%" echo Tarjimon Office UZ - Word add-in source diagnostic
>>"%OUT%" echo ==================================================
>>"%OUT%" echo Date: %date% %time%
>>"%OUT%" echo.

>>"%OUT%" echo [1] WINWORD PROCESS
>>"%OUT%" echo --------------------------------------------------
tasklist /FI "IMAGENAME eq WINWORD.EXE" | find /I "WINWORD.EXE" >nul
if errorlevel 1 (
  >>"%OUT%" echo WINWORD.EXE is not running.
) else (
  >>"%OUT%" echo WARNING: WINWORD.EXE is currently running. Close Word and rerun this diagnostic for a clean result.
)
>>"%OUT%" echo.

>>"%OUT%" echo [2] WORD ADD-IN REGISTRY - HKCU
>>"%OUT%" echo --------------------------------------------------
reg query "HKCU\Software\Microsoft\Office\Word\Addins\TarjimonOfficeUZ.Word" /s >>"%OUT%" 2>&1
if errorlevel 1 >>"%OUT%" echo HKCU key not found.
>>"%OUT%" echo.

>>"%OUT%" echo [3] WORD ADD-IN REGISTRY - HKLM 64-bit view
>>"%OUT%" echo --------------------------------------------------
reg query "HKLM\Software\Microsoft\Office\Word\Addins\TarjimonOfficeUZ.Word" /s >>"%OUT%" 2>&1
if errorlevel 1 >>"%OUT%" echo HKLM 64-bit-view key not found.
>>"%OUT%" echo.

>>"%OUT%" echo [4] WORD ADD-IN REGISTRY - HKLM 32-bit/WOW6432Node
>>"%OUT%" echo --------------------------------------------------
reg query "HKLM\Software\WOW6432Node\Microsoft\Office\Word\Addins\TarjimonOfficeUZ.Word" /s >>"%OUT%" 2>&1
if errorlevel 1 >>"%OUT%" echo HKLM WOW6432Node key not found.
>>"%OUT%" echo.

>>"%OUT%" echo [5] ALL WORD ADD-IN KEYS CONTAINING TARJIMON OR TRANSLIT
>>"%OUT%" echo --------------------------------------------------
reg query "HKCU\Software\Microsoft\Office\Word\Addins" /s 2>nul | findstr /I "Tarjimon TransLit Manifest FriendlyName LoadBehavior" >>"%OUT%"
reg query "HKLM\Software\Microsoft\Office\Word\Addins" /s 2>nul | findstr /I "Tarjimon TransLit Manifest FriendlyName LoadBehavior" >>"%OUT%"
reg query "HKLM\Software\WOW6432Node\Microsoft\Office\Word\Addins" /s 2>nul | findstr /I "Tarjimon TransLit Manifest FriendlyName LoadBehavior" >>"%OUT%"
>>"%OUT%" echo.

>>"%OUT%" echo [6] INSTALLED PROGRAM FILES
>>"%OUT%" echo --------------------------------------------------
if exist "%ProgramFiles%\Tarjimon Office UZ" (
  >>"%OUT%" echo FOUND: %ProgramFiles%\Tarjimon Office UZ
  dir /s /b "%ProgramFiles%\Tarjimon Office UZ\*TarjimonOfficeUZ.Word*" >>"%OUT%" 2>&1
) else >>"%OUT%" echo ProgramFiles Tarjimon Office UZ folder not found.
if defined ProgramFiles(x86) if exist "%ProgramFiles(x86)%\Tarjimon Office UZ" (
  >>"%OUT%" echo FOUND: %ProgramFiles(x86)%\Tarjimon Office UZ
  dir /s /b "%ProgramFiles(x86)%\Tarjimon Office UZ\*TarjimonOfficeUZ.Word*" >>"%OUT%" 2>&1
) else >>"%OUT%" echo ProgramFiles(x86) Tarjimon Office UZ folder not found.
>>"%OUT%" echo.

>>"%OUT%" echo [7] COMMON VSTO CLICKONCE CACHE MATCHES
>>"%OUT%" echo --------------------------------------------------
if exist "%LOCALAPPDATA%\Apps\2.0" (
  dir /s /b "%LOCALAPPDATA%\Apps\2.0\*TarjimonOfficeUZ.Word*" >>"%OUT%" 2>&1
) else >>"%OUT%" echo ClickOnce cache folder not found.
>>"%OUT%" echo.

>>"%OUT%" echo [8] CONCLUSION HINTS
>>"%OUT%" echo --------------------------------------------------
>>"%OUT%" echo - If HKCU still contains TarjimonOfficeUZ.Word after MSI removal, that user-level registration can keep the add-in available.
>>"%OUT%" echo - If a Manifest points to an existing file outside Program Files\Tarjimon Office UZ, another installation/cache is supplying the add-in.
>>"%OUT%" echo - If WINWORD.EXE was running during uninstall, restart Word before judging whether the add-in was removed.
>>"%OUT%" echo - Do not delete any registry keys manually; this report is diagnostic only.

start "" notepad.exe "%OUT%"
exit /b 0
