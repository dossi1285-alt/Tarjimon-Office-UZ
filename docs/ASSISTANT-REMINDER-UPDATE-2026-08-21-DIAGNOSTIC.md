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

## Diagnostic test result — corrected launcher

The corrected diagnostic was subsequently run successfully on the user's test computer.

- Report: `Desktop\KL-Office-Diagnostic.txt`
- Computer: `SERVER_IHMA`
- User: `admin_atm`
- Diagnostic timestamp: `2026-08-21 12:10:12`
- Status: **TESTED / VALID REPORT GENERATED**.
- The diagnostic completed without the previous parser error and produced the full Office runtime/registry/startup report.

## Confirmed Office loading evidence from the report

1. Word's `Office\\Word\\Addins` registry contains the own VSTO add-in, Visual Studio design-time adaptor, and Word Reader add-in. It does **not** contain a registration named `KL Office uz`.
2. Word's runtime `COMAddIns` contains the same COM/VSTO-style entries and does **not** identify `KL Office uz`.
3. Word's `AddIns` collection contains three startup templates: `Book.dot`, `mfw.dot`, and `TransLit.dot`; all are installed/autoloaded from the user's Word STARTUP directory.
4. `TransLit.dot` is the first confirmed real translator candidate because the Word runtime report shows translator-specific legacy CommandBars created while these startup add-ins are loaded:
   - `Konvertatsiya` → `Krill - Lotin` → `OnAction=Krill_Lotin.Krill_Latin`
   - `Konvertatsiya` → `Lotin - Krill` → `OnAction=Lotin_Krill.Latin_Krill`
5. `Print_Book` → `Print_Kitob` → `OnAction=Module1.PrintAsBook` is a separate non-translator function and must not be treated as translator evidence.
6. `TransLit.dot` itself has no useful Windows file-version publisher metadata, so its translator identity must come from the Office loading context plus functional behavior, not publisher/version.
7. Excel also has the own VSTO add-in and an `UndoBridge.xlam`; these are separate from the Word translator evidence.

## Important conclusion

The report **proves the existence and loading mechanism of `TransLit.dot` as a Word STARTUP/Word AddIn template and proves translator functionality through its Krill/Lotin CommandBars**. This is the first concrete example for the new detection principle: Office-specific loading source + functional evidence, rather than relying only on a product-name list.

However, the report does **NOT** yet prove that `TransLit.dot` is the same component that the user sees as the Ribbon/product named `KL Office uz`. No registry/add-in entry in the report carries the literal `KL Office uz` name. Therefore `KL Office uz` itself remains **ACTIVE / NOT YET ATTRIBUTED** and must not be silently equated with `TransLit.dot`.

## Detection-design consequence

The safe redesign must use Word `AddIns`/STARTUP as a first-class Office-specific source and use functional evidence such as translator CommandBars/macros to strengthen a candidate. It must not classify a Windows component merely from publisher/version (for example Igor Pavlov), and it must not use global machine-wide CLSID scanning as the translator source.

Before changing the Preflight detector, the next protected step is to establish whether the `KL Office uz` Ribbon is actually supplied by `TransLit.dot`, `mfw.dot`, `Book.dot`, another Office template/add-in, or a different Office loading mechanism. The detector must then use that evidence with minimal code changes.

## Active next step

1. Attribute the `KL Office uz` Ribbon to its actual Word add-in/template/loading source.
2. Then implement the minimal Office-specific detection/grouping change.
3. Preserve third-party entries unchecked by default and never select Igor Pavlov solely from publisher/version.
4. Automate build/test/verification as far as the project tooling permits.
5. Do not press `Tasdiqlash` until the detection result passes the acceptance criteria.

This update is a continuation record associated with `docs/ASSISTANT-REMINDER-RULES.md`. It must be considered when continuing this project, including in a new chat.
