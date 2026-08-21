# Assistant Reminder Update — 2026-08-21

## KL diagnostic launcher failure and correction

- User ran `docs/RUN-KL-DIAGNOSTIC.bat` after Pull.
- Actual test result: **FAILED / NOT VERIFIED**.
- Screenshot evidence: Windows PowerShell parser reported an unexpected token around the Ribbon needle list in `DIAGNOSE-KL-OFFICE.ps1`, and the launcher ended with `Diagnostic failed`.
- Root cause identified: the launcher used `powershell.exe -File` against a UTF-8 PS1 without a BOM. Windows PowerShell 5.1 can interpret such a file using the legacy code page, which is unsafe for the script's non-ASCII Ribbon strings.
- No Office/add-in uninstall or modification occurred.

## Correction

- File changed: `docs/RUN-KL-DIAGNOSTIC.bat`
- New commit: `290290dcf2004516c5c4b6309d026ad93c5b6418`
- The launcher now explicitly reads `DIAGNOSE-KL-OFFICE.ps1` as UTF-8 and executes the resulting script block, avoiding the `-File` encoding ambiguity.
- Status: **IMPLEMENTED / NOT YET TESTED**.

## Active next step

User should receive the corrected launcher and run it by double-clicking. PASS means the diagnostic completes without a parser error and creates/opens `Desktop\KL-Office-Diagnostic.txt`.

Do not press `Tasdiqlash`; do not uninstall or disable any add-in.

This update is a continuation record associated with `docs/ASSISTANT-REMINDER-RULES.md`. It must be considered when continuing this project, including in a new chat.
