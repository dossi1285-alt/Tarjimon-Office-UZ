# Tarjimon Office UZ 1.0 — Installer Migration Requirements

Date recorded: 2026-08-19  
Branch: `release/1.0-installer-cleanup`

## Purpose

This document records the agreed installer behavior so it is not lost between work sessions.

The 1.0 product is always:

**ONE MSI → Word + Excel.**

There must not be separate Word and Excel installers.

## Current verified history

- GitHub Desktop is installed and is the normal project workflow.
- The project is maintained on GitHub repository `dossi1285-alt/Tarjimon-Office-UZ`.
- Active release branch: `release/1.0-installer-cleanup`.
- `TarjimonOfficeUZ.Word3.csproj` was a temporary mistake and was deleted. It must never be recreated.
- WiX Toolset 7 and HeatWave are installed.
- WiX SDK restore was completed.
- WiX OSMF EULA acceptance was completed locally.
- Missing VSTO/MSBuild Office targets were diagnosed and the required Visual Studio Office/VSTO tooling was installed.
- A WiX x64 Debug MSI was successfully built at `TarjimonOfficeUZ.Setup.Wix/bin/x64/Debug/TarjimonOfficeUZSetup.msi`.
- The MSI was launched and Windows showed installed-program entries, but the existing installation meant that this alone did not prove a clean new Word/Excel deployment.
- Existing Word and Excel showed the `KL Office uz` ribbon. Because this was already present before the current MSI test, its presence is not accepted as proof of the new MSI.

## Agreed installer behavior

### Phase A — Pre-install scan

When the MSI starts, it should first inspect the machine for existing Office add-ins relevant to this product.

The scan should look at Word and Excel add-in registrations and identifiable product metadata. It should not blindly enumerate or delete every ribbon add-in.

Candidates should include:

1. Existing Tarjimon Office UZ versions.
2. Other vendors' Office translator/add-in products that are plausibly conflicting with this translator.

### Phase B — User review

Before removal, the installer displays a clear list such as:

| Product | Publisher | Version | Host | Location | Action |
|---|---|---|---|---|---|
| Tarjimon Office UZ | Tarjimon Office UZ | old version | Word/Excel | detected registration | Remove/Upgrade |
| Other Translator | Vendor | version | Word | detected registration | Ask user |

The exact UI can differ, but the user must be able to understand what was detected.

### Phase C — Consent

The installer must ask for explicit consent before removing anything.

Rules:

- Our own older Tarjimon Office UZ installation: offer removal/upgrade as part of the installation flow.
- Third-party add-in: never remove silently. Require explicit user approval.
- User chooses Keep: do not remove that candidate.
- Multiple candidates: show all candidates and apply only the selected actions.
- No candidates: continue normally without unnecessary prompts.

### Phase D — Safe removal

Removal must use the product's supported uninstall mechanism whenever available.

Do not implement:

- delete every Office ribbon entry;
- delete arbitrary registry keys by pattern alone;
- delete arbitrary files merely because their name contains “translator”.

A third-party add-in may be legitimate and unrelated to Tarjimon Office UZ. Detection must therefore be conservative.

### Phase E — Install the new combined product

After the selected old/conflicting products are handled, the same MSI installs:

- `TarjimonOfficeUZ.Word`
- `TarjimonOfficeUZ.Excel`
- required Shared/Core dependencies;
- Excel UndoBridge and its required placement/registration.

No manual post-install script should be required for the normal 1.0 path.

### Phase F — Verification

After installation, the installer or test procedure must verify:

- Word add-in registration/files exist.
- Excel add-in registration/files exist.
- Word loads the add-in.
- Excel loads the add-in.
- UndoBridge works.

## Test matrix

### Test 1 — Clean machine

No previous Tarjimon installation.

Expected:

- No migration prompt.
- One MSI installs.
- Word add-in works.
- Excel add-in works.

### Test 2 — Old Tarjimon version

An older Tarjimon Office UZ is installed.

Expected:

- MSI detects it.
- User is asked whether it should be removed/upgraded.
- If approved, old version is removed safely.
- New combined MSI installs Word + Excel.

### Test 3 — Keep old Tarjimon

User chooses to keep the old installation.

Expected:

- Installer does not silently remove it.
- Installer follows the defined safe behavior for side-by-side/conflicting registration.
- Result is clearly reported.

### Test 4 — Third-party translator

A different company's Office translator/add-in is installed.

Expected:

- It is shown as a candidate only when identifiable as relevant.
- User explicitly approves or rejects removal.
- Rejected item remains untouched.
- Approved item is removed only through a supported uninstall path.

### Test 5 — Multiple candidates

Several relevant add-ins are present.

Expected:

- All candidates are listed.
- User can make individual choices.
- Only selected products are changed.

### Test 6 — Reinstall

Uninstall Tarjimon Office UZ, then run the same MSI again.

Expected:

- Clean installation.
- Word + Excel both work.

### Test 7 — Uninstall

Remove the installed product through Windows Apps/Programs and Features.

Expected:

- Tarjimon files and registrations are removed cleanly.
- Unrelated third-party add-ins remain.

## Release rule

A build-successful MSI is not enough.

The 1.0 installer is frozen only after the combined MSI passes the clean install, migration, Word, Excel, UndoBridge, uninstall and reinstall tests.

## Important project memory

If a future session starts without context, read:

1. `PROJECT_RULES.md`
2. `docs/PROJECT-AUDIT-2026-08-17.md`
3. this file

These files define the agreed architecture and installer behavior. Do not revert to separate Word/Excel installers or silently delete existing Office add-ins.
