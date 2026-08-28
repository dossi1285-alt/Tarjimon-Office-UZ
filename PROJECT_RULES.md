# Tarjimon Office UZ — Project Rules / 1.0 Baseline

## 1. Canonical 1.0 architecture

**Tarjimon Office UZ 1.0 uses ONE user-facing installer only.**

The final product must be installed through a single user-facing setup package. The setup installs and configures both Office add-ins:

- Microsoft Word add-in: `TarjimonOfficeUZ.Word`
- Microsoft Excel add-in: `TarjimonOfficeUZ.Excel`

The WiX MSI remains the combined Word + Excel payload. The recommended distributed installer is `TarjimonOfficeUZSetup.exe`, which performs the pre-install migration review and embeds/launches the combined MSI.

### Forbidden architecture for 1.0

- Do not create a separate Word installer.
- Do not create a separate Excel installer.
- Do not split the final 1.0 installation into two independent installers.
- Do not treat a successful Word-only MSI build as a completed 1.0 installer.
- Do not silently uninstall third-party Office add-ins.

## 2. Project structure

The main solution components are:

- `TarjimonOfficeUZ.Core` — core functionality.
- `TarjimonOfficeUZ.Shared` — shared UI, settings, resources and services used by Office hosts.
- `TarjimonOfficeUZ.Word` — Microsoft Word VSTO add-in.
- `TarjimonOfficeUZ.Excel` — Microsoft Excel VSTO add-in.
- `TarjimonOfficeUZ.Setup` — legacy installer project retained during migration.
- `TarjimonOfficeUZ.Setup.Wix` — WiX 7 installer project for the combined Word + Excel MSI payload.
- `TarjimonOfficeUZ.Setup.Preflight` — .NET Framework 4.8 user-facing migration/preflight launcher that embeds the generated MSI.
- `TarjimonOfficeUZ.Tests` — tests.

## 3. Installer acceptance criterion

The 1.0 installer is considered complete only when all of the following are true:

1. One user-facing installer package is produced.
2. The installer contains the Word add-in.
3. The installer contains the Excel add-in.
4. One installation makes the add-in available in Word.
5. One installation makes the add-in available in Excel.
6. Word functionality is tested after installation.
7. Excel functionality is tested after installation.
8. A clean install/reinstall/uninstall path is tested.
9. Existing relevant Office add-ins are handled through an explicit pre-install review/consent flow.
10. The preflight launcher is the recommended distribution entry point for migration scenarios.

## 4. Change discipline

When modifying project files, especially `.csproj` files:

- Prefer minimal edits over rewriting the entire file.
- Preserve existing formatting and line endings whenever possible.
- Do not introduce LF/CRLF-only full-file diffs.
- Do not commit a change that shows hundreds of unrelated line changes when only a few settings were intended to change.
- Never remove or replace signing configuration blindly.
- Before committing, inspect the Git diff and confirm that only intended changes are present.

## 5. Word and Excel are equal 1.0 targets

Word and Excel must be tested independently, but they are released together through the same user-facing installer.

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
- The preflight launcher builds successfully with the generated MSI embedded.
- The combined installer installs both add-ins.
- Word real-world test is successful.
- Excel real-world test is successful.
- Migration/consent test matrix passes.
- No known 1.0 blocker remains.

## 7. Working rule for the assistant

This file is the project's canonical architecture/rules reference. Before making consequential project changes through GitHub or while reasoning about the 1.0 release, use this file as the project baseline and do not contradict it without explicit user approval.

If a later request conflicts with these rules, explicitly point out the conflict before changing the architecture.

## 8. Repository and GitHub workflow

GitHub is the canonical remote repository for this project. The repository is `dossi1285-alt/Tarjimon-Office-UZ`.

The active release work is performed through the `release/1.0-installer-cleanup` branch unless explicitly changed by the user.

GitHub Desktop is installed and is part of the project's normal workflow. Before asking the user to repeat a local operation, first inspect the repository state and recent Git history through GitHub when available.

Required synchronization discipline:

1. Inspect the current repository/branch state before consequential changes.
2. Fetch/pull remote changes before pushing when GitHub reports that the remote is ahead.
3. Never use force-push for normal release work.
4. After a remote change, keep the user's local working tree synchronized before the next build.
5. Before commit/push, inspect the diff and reject accidental full-file rewrites, temporary files, or unrelated changes.

## 9. Installer technology decision

WiX Toolset 7 is the selected installer technology for the combined MSI payload. `TarjimonOfficeUZ.Setup.Wix` is the target MSI project. The legacy Visual Studio Installer Project `TarjimonOfficeUZ.Setup/TarjimonOfficeUZ.Setup.vdproj` is retained temporarily until the WiX installer and preflight flow are proven in build and real installation tests.

## 10. Current 1.0 blocker definition

A successful legacy `TarjimonOfficeUZ.Setup.msi` build is not sufficient by itself. The final WiX Setup project must package both `TarjimonOfficeUZ.Word` and `TarjimonOfficeUZ.Excel` outputs and the resulting user-facing setup must install both add-ins.

## 11. Local tool prerequisite

The local development environment now has HeatWave for Visual Studio installed for working with the WiX SDK-style installer project. This is an environment prerequisite, not a product dependency and not part of the installer package.

## 12. Full-project audit memory

The complete project audit is stored in `docs/PROJECT-AUDIT-2026-08-17.md` and must be treated as the detailed continuation of these rules.

Important permanent findings from that audit:

- `TarjimonOfficeUZ.Word3.csproj` was a temporary mistake, was deleted, and must not be recreated.
- The full uploaded working tree was inspected; the apparent 35-file local change set was line-ending-only and must not be committed as code changes.
- The final 1.0 product remains **Word + Excel + ONE user-facing installer**.
- Excel UndoBridge must be included and made operational by the single installer; manual post-install steps are not the 1.0 target.
- WiX 32-bit registry components must use an architecture-safe directory arrangement.
- Installer registration currently uses HKLM while the Shared startup-settings service writes HKCU; this must be deliberately reconciled and tested.
- Word/Excel VSTO signing and trust are release blockers until a proper release certificate strategy is confirmed.
- Test Explorer's current “no tests to run” message must first be checked for filtering; the project contains 10 regression tests.
- Old documentation must not override the canonical 1.0 architecture.

## 13. Mandatory assistant workflow

For this project, the assistant must work with GitHub as the canonical project source and history.

Before any consequential change:

1. Read `PROJECT_RULES.md`.
2. Read `docs/PROJECT-AUDIT-2026-08-17.md` when the task concerns build, installer, solution structure, signing, tests, or release.
3. Inspect the GitHub branch/history before asking the user to repeat local work.
4. Make the smallest justified change.
5. Inspect the resulting diff.
6. Only then ask the user to Pull origin / build / test as needed.

Never silently forget the GitHub workflow or the ONE-installer Word+Excel architecture.

## 14. Installer preflight / existing Office add-in migration requirement

The final 1.0 installer must perform an **explicit pre-install scan before changing existing Office add-ins**.

The intended user experience is:

1. User starts `TarjimonOfficeUZSetup.exe`.
2. Preflight scans the machine for existing Office add-ins that are relevant to the product, especially Word/Excel translator or ribbon add-ins.
3. The installer presents a clear list of detected candidates with at least product name, publisher/vendor when available, version when available, host (Word/Excel), and registration location when available.
4. The list distinguishes our own previous/older `Tarjimon Office UZ` installations from other vendors' relevant translator/add-in registrations.
5. The user gives explicit consent before any detected product is removed.
6. The user can keep any detected item; keeping it is not treated as permission to delete it.
7. For our own older versions, the installer offers removal/upgrade and then installs the new combined version.
8. For third-party candidates, removal happens only after explicit consent and only through a supported uninstall mechanism.
9. The installer never deletes arbitrary files or registry keys merely because a ribbon entry exists.
10. After the approved migration operations finish, the embedded combined MSI installs Word + Excel.

### Important safety rule for add-in detection

Do **not** implement this as “delete every ribbon add-in”. Office has many legitimate add-ins unrelated to translation. Detection must use identifiable registration/product metadata and a conservative candidate list. The user must see what will be removed before removal occurs.

### Required migration test matrix

- No previous Tarjimon installation → launch one setup → no unnecessary prompt → Word + Excel work.
- Previous Tarjimon Office UZ version exists → detected → user approves removal/upgrade → new version works in Word + Excel.
- Previous Tarjimon Office UZ version exists → user keeps it/cancels → it is not silently removed; final behavior is documented and tested.
- A third-party translator Office add-in exists → detected as a candidate → user explicitly approves or rejects removal → only the selected item is affected.
- Multiple candidates exist → all candidates are displayed clearly and only selected items are processed.
- No relevant candidate exists → setup proceeds without unnecessary prompts.
- Candidate has no supported uninstall command → setup warns and does not forcibly delete files/registry.
- Uninstall/reinstall → old registration is removed cleanly and the same single user-facing setup can reinstall Word + Excel.

## 15. Verified work history / continuation memory — 2026-08-19

The following results were achieved during the installer work and must be preserved as project memory:

- GitHub Desktop is installed and is the normal local Git workflow for this project.
- The active release branch is `release/1.0-installer-cleanup` unless explicitly changed.
- The temporary `TarjimonOfficeUZ.Word3.csproj` mistake was removed; it must stay deleted.
- The local solution was synchronized with GitHub using Fetch/Pull/Push as required. Remote-ahead situations were reconciled rather than force-pushed.
- Visual Studio 2026 is installed and has the Microsoft 365 development workload/VSTO support.
- WiX Toolset 7 and HeatWave for Visual Studio are installed locally.
- The WiX SDK restore was successfully performed with `dotnet restore` for `TarjimonOfficeUZ.Setup.Wix/TarjimonOfficeUZ.Setup.Wix.wixproj`.
- The WiX build initially exposed the Open Source Maintenance Fee (OSMF) EULA acceptance requirement; the required local EULA acceptance was performed.
- The next build exposed missing `Microsoft.VisualStudio.Tools.Office.targets`; the Visual Studio Office/VSTO tooling was checked/installed, after which the previous MSB4019 blocker was cleared.
- A combined WiX MSI was successfully produced at the x64 Debug output: `TarjimonOfficeUZ.Setup.Wix/bin/x64/Debug/TarjimonOfficeUZSetup.msi`.
- The produced MSI was launched and Windows showed `Tarjimon Office UZ` and `TarjimonOfficeUZ.Setup` entries in installed programs. However, this did **not** prove that the newly produced MSI was the only source of the active Word/Excel add-in, because an older installation was already present.
- Word and Excel currently show the `KL Office uz` ribbon/add-in. This existed before the current clean-install test, so its presence alone is not accepted as proof that the new MSI installed and activated the new add-ins.
- Therefore the next release test must be a controlled uninstall/clean migration test followed by installation from the newly produced single user-facing setup.
- The installer must not be declared 100% complete merely because the MSI builds or because an installed-program entry appears.

## 16. Preflight launcher implementation — 2026-08-19

The preflight migration design is now implemented in source form:

- `TarjimonOfficeUZ.Setup.Preflight` targets .NET Framework 4.8 and uses WinForms for the consent UI.
- It scans Word/Excel add-in registrations under HKLM/HKCU and both 64-bit/32-bit registry views.
- It uses conservative translator-related detection keywords.
- It correlates candidates with Windows uninstall registration and executes only a supported uninstall command after explicit user selection.
- It extracts the embedded combined MSI to a temporary file and starts Windows Installer only after the migration review completes.
- `TarjimonOfficeUZ.Setup.Wix.wixproj` now builds the combined MSI first and then invokes the preflight project with the generated MSI path so the MSI can be embedded in the final `TarjimonOfficeUZSetup.exe` launcher.

This implementation is **not yet release-proven**. It must be built and tested locally before being considered complete. In particular, verify the generated EXE, embedded MSI, detection accuracy, uninstall behavior, and Word/Excel installation results before release freeze.

## 17. Important architecture clarification — ONE user-facing installer, TWO build projects

The project intentionally has **two installer-related build projects with different responsibilities**. This is not two installers for the user.

### `TarjimonOfficeUZ.Setup.Preflight`

- Contains the source code and UI logic for the user-facing `TarjimonOfficeUZSetup.exe` launcher.
- Performs the pre-install scan and explicit consent/migration review.
- Detects relevant existing Word/Excel translator/add-in registrations.
- After approved migration operations, launches the embedded combined MSI.
- This project is a **launcher/preflight implementation project**, not a second independent installer product.

### `TarjimonOfficeUZ.Setup.Wix`

- Is the WiX 7 build project for the combined MSI payload.
- Builds the Word + Excel MSI.
- Then supplies that generated MSI to the Preflight project so the MSI is embedded into the final `TarjimonOfficeUZSetup.exe`.
- It is the **installer build/orchestration project**, not a separate user-facing installer choice.

### Final user-facing architecture

The user must receive/use **ONE setup entry point**:

`TarjimonOfficeUZSetup.exe`

The intended chain is:

`TarjimonOfficeUZ.Setup.Wix` build orchestration
→ generate combined `TarjimonOfficeUZSetup.msi`
→ pass/embed MSI into `TarjimonOfficeUZ.Setup.Preflight`
→ produce final `TarjimonOfficeUZSetup.exe`
→ user launches ONE EXE
→ Preflight review/consent
→ approved migration
→ embedded MSI
→ Word + Excel installed together.

The existence of both projects must **not** be interpreted as permission to distribute two independent installers.

### Legacy Setup project

`TarjimonOfficeUZ.Setup` is the old Visual Studio Installer `.vdproj` project. It is retained temporarily during migration and must not become the primary 1.0 distribution path. Once the WiX + Preflight flow is fully proven through real installation, migration, reinstall, uninstall, Word, and Excel tests, the legacy project may be removed as a separate deliberate cleanup step.

### Build/output rule

Do not delete or bypass `TarjimonOfficeUZ.Setup.Wix` merely because `TarjimonOfficeUZ.Setup.Preflight` exists. Do not delete or bypass Preflight merely because WiX builds an MSI. The two projects form one final installer pipeline and have different technical responsibilities.

The final 1.0 acceptance criterion remains **ONE user-facing setup, containing both Word and Excel, with Preflight migration/consent before MSI installation**.

## 18. Latest verified preflight changes and test baseline — 2026-08-20

The following work was completed and synchronized to GitHub before starting the next test cycle:

- The Preflight UI was corrected so the first list row is fully visible and has clean empty space above it; the compact window layout is retained.
- The old three-button flow was reduced to exactly two buttons: `Tasdiqlash` and `Bekor qilish`.
- `Tasdiqlash` means the user accepts the selected removals and continues with installation.
- `Bekor qilish` cancels the setup without performing the selected removals.
- The Windows Installer uninstall bug was fixed: registered MSI uninstall commands using `/I{GUID}` or `/I {GUID}` are converted to `/X{GUID}`/`/X {GUID}` before execution, preventing the Windows Installer help dialog that appeared during the earlier test.
- The previous test reached the genuine Windows confirmation dialog `Вы действительно хотите удалить этот продукт?`; this confirmed that the `/X` conversion was functioning. The user correctly chose not to continue deleting at that point because duplicate detection still needed correction.
- Duplicate detection is now being addressed so the same product discovered through multiple Office hosts and/or registry views is not displayed as a confusing list of repeated identical rows.
- The detection scope was expanded to recognize relevant third-party translator/Office add-ins, not only `Tarjimon Office UZ` entries. Detection remains conservative and must not become “remove every ribbon add-in”.
- Third-party candidates must be shown to the user and must remain unselected by default; they may only be removed after explicit user selection and consent, and only through a supported uninstall mechanism.
- The user observed `KL Office uz` in Word. This is an important real-world third-party add-in test case and must be considered in the detection logic and migration test matrix.
- The latest source change for duplicate/third-party detection was committed as `dd8ca36ddc65bfa2399972864675e3738eb6d5e4` with message `docs: record preflight detection and test baseline` for the documentation update; the corresponding Preflight source change is already on the same active release branch and must be pulled before local testing.
- The user has now pulled the latest changes and rebuilt both `TarjimonOfficeUZ.Setup.Preflight` and `TarjimonOfficeUZ.Setup.Wix` successfully. The next action is testing, not another architectural change.

### Immediate test objective

Start the newly built `TarjimonOfficeUZSetup.exe` and verify only these points first:

1. The list contains no duplicate representation of the same installed product.
2. The known `KL Office uz` third-party add-in is detected if its registration matches the conservative translator/add-in criteria.
3. Our own Tarjimon Office UZ entries are distinguishable from third-party entries.
4. Our own previous version is selected by default only when it is genuinely identified as our product.
5. Third-party entries are **not** selected by default.
6. `Bekor qilish` performs no uninstall.
7. Do not click `Tasdiqlash` for the destructive migration test until the detection list has been visually verified.

Only after this detection test passes should we proceed to controlled removal/reinstallation, followed by Word and Excel real-world ribbon/functionality tests.
