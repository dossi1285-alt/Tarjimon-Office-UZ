# Tarjimon Office UZ — Project Rules / 1.0 Baseline

## 1. Canonical 1.0 architecture

**Tarjimon Office UZ 1.0 uses ONE installer only.**

The final product must be installed through a single installer (one MSI/setup package). The installer installs and configures both Office add-ins:

- Microsoft Word add-in: `TarjimonOfficeUZ.Word`
- Microsoft Excel add-in: `TarjimonOfficeUZ.Excel`

After one installation, the product must be available in both Word and Excel.

### Forbidden architecture for 1.0

- Do not create a separate Word installer.
- Do not create a separate Excel installer.
- Do not split the final 1.0 installation into two independent installers.
- Do not treat a successful Word-only MSI build as a completed 1.0 installer.

## 2. Project structure

The main solution components are:

- `TarjimonOfficeUZ.Core` — core functionality.
- `TarjimonOfficeUZ.Shared` — shared UI, settings, resources and services used by Office hosts.
- `TarjimonOfficeUZ.Word` — Microsoft Word VSTO add-in.
- `TarjimonOfficeUZ.Excel` — Microsoft Excel VSTO add-in.
- `TarjimonOfficeUZ.Setup` — the single installer responsible for packaging the complete product.
- `TarjimonOfficeUZ.Tests` — tests.

## 3. Installer acceptance criterion

The 1.0 installer is considered complete only when all of the following are true:

1. One installer package is produced.
2. The installer contains the Word add-in.
3. The installer contains the Excel add-in.
4. One installation makes the add-in available in Word.
5. One installation makes the add-in available in Excel.
6. Word functionality is tested after installation.
7. Excel functionality is tested after installation.

## 4. Change discipline

When modifying project files, especially `.csproj` files:

- Prefer minimal edits over rewriting the entire file.
- Preserve existing formatting and line endings whenever possible.
- Do not introduce LF/CRLF-only full-file diffs.
- Do not commit a change that shows hundreds of unrelated line changes when only a few settings were intended to change.
- Never remove or replace signing configuration blindly.
- Before committing, inspect the Git diff and confirm that only intended changes are present.

## 5. Word and Excel are equal 1.0 targets

Word and Excel must be tested independently, but they are released together through the same installer.

A Word-only successful build means:

> Word component build is successful — NOT that the Tarjimon Office UZ 1.0 release is complete.

An Excel-only successful build means:

> Excel component build is successful — NOT that the Tarjimon Office UZ 1.0 release is complete.

## 6. Release/freeze rule

The 1.0 release can be called **100% complete / frozen** only after:

- Core/Shared build is successful.
- Word build is successful.
- Excel build is successful.
- One combined Setup/MSI build is successful.
- The combined installer installs both add-ins.
- Word real-world test is successful.
- Excel real-world test is successful.
- No known 1.0 blocker remains.

## 7. Working rule for the assistant

This file is the project's canonical architecture/rules reference. Before making consequential project changes through GitHub or while reasoning about the 1.0 release, use this file as the project baseline and do not contradict it without explicit user approval.

If a later request conflicts with these rules, explicitly point out the conflict before changing the architecture.
