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
- `TarjimonOfficeUZ.Setup` — legacy installer project retained during migration.
- `TarjimonOfficeUZ.Setup.Wix` — WiX 7 installer project for the final single-installer architecture.
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
8. A clean install/reinstall/uninstall path is tested.
9. Existing compatible Office add-ins are handled through an explicit pre-install review/consent flow described in Section 14.

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

WiX Toolset 7 is the selected final installer technology. `TarjimonOfficeUZ.Setup.Wix` is the target installer project. The legacy Visual Studio Installer Project `TarjimonOfficeUZ.Setup/TarjimonOfficeUZ.Setup.vdproj` is retained temporarily until the WiX installer is proven in build and real installation tests.

Do not silently mix installer architectures or declare the installer complete until the WiX installer satisfies the single-installer Word+Excel acceptance criteria.

## 10. Current 1.0 blocker definition

A successful legacy `TarjimonOfficeUZ.Setup.msi` build is not sufficient by itself. The final WiX Setup project must demonstrably package both `TarjimonOfficeUZ.Word` and `TarjimonOfficeUZ.Excel` outputs and the resulting single installer must install both add-ins.

If Setup currently packages only Shared/dependency output, treat that as a blocker rather than a successful 1.0 installer.

## 11. Local tool prerequisite

The local development environment now has HeatWave for Visual Studio installed for working with the WiX SDK-style installer project. This is an environment prerequisite, not a product dependency and not part of the installer package.

## 12. Full-project audit memory

The complete project audit is stored in `docs/PROJECT-AUDIT-2026-08-17.md` and must be treated as the detailed continuation of these rules.

Important permanent findings from that audit:

- `TarjimonOfficeUZ.Word3.csproj` was a temporary mistake, was deleted, and must not be recreated.
- The full uploaded working tree was inspected; the apparent 35-file local change set was line-ending-only and must not be committed as code changes.
- The final 1.0 product remains **Word + Excel + ONE MSI**.
- The WiX project is source-correct in principle but is not yet release-complete until it builds and installs both add-ins in a real test.
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

1. MSI starts and scans the machine for existing Office add-ins that are relevant to the product, especially Word/Excel translator or ribbon add-ins.
2. The installer presents a clear list of detected candidates with at least product name, publisher/vendor when available, version when available, host (Word/Excel), and installation/registration location when available.
3. The list must distinguish:
   - our own previous/older `Tarjimon Office UZ` installations;
   - other vendors' Office translator/add-in registrations that may conflict with the new product.
4. The installer must **ask for explicit user consent before removing any detected existing product**. It must never silently uninstall a third-party Office add-in.
5. The user must be able to choose to keep a detected item. Keeping it must not be treated as an installation failure.
6. For our own older versions, the installer should offer removal/upgrade as part of the same installation flow and then install the new version.
7. For a third-party candidate, removal must occur only after explicit consent and only through a supported/uninstallable mechanism; the installer must not delete arbitrary files or registry keys merely because a ribbon entry exists.
8. After the user's choices are applied, the installer installs the new combined Word + Excel add-in package.
9. The installer must then verify its own registration/files and report the result.

### Important safety rule for add-in detection

Do **not** implement this as “delete every ribbon add-in”. Office has many legitimate add-ins unrelated to translation. Detection must use identifiable registration/product metadata and a conservative candidate list. The user must see what will be removed before removal occurs.

### Required migration test matrix

At minimum test these scenarios:

- No previous Tarjimon installation → install one MSI → Word + Excel work.
- Previous Tarjimon Office UZ version exists → installer detects it → user approves removal/upgrade → new version works in Word + Excel.
- Previous Tarjimon Office UZ version exists → user chooses keep → installer follows the defined safe behavior and does not silently delete it.
- A third-party translator Office add-in exists → installer detects it as a candidate → user explicitly approves or rejects removal → only the selected item is affected.
- Multiple candidates exist → installer displays all candidates clearly and applies only the user's selections.
- No relevant candidate exists → installer proceeds without unnecessary prompts.
- Uninstall/reinstall → old registration is removed cleanly and the new combined installer can reinstall Word + Excel.

This preflight/migration flow is a **planned 1.0 requirement**, not a claim that the current MSI already implements it. It must be implemented and tested before the 1.0 release is declared frozen.

## 15. Verified work history / continuation memory — 2026-08-19

The following results were achieved during the current installer work and must be preserved as project memory:

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
- The produced MSI was launched and Windows showed `Tarjimon Office UZ` and `TarjimonOfficeUZ.Setup` entries in installed programs. However, this does **not yet prove** that the newly produced MSI was the only source of the active Word/Excel add-in, because an older installation was already present.
- Word and Excel currently show the `KL Office uz` ribbon/add-in. This existed before the current clean-install test, so its presence alone is not accepted as proof that the new MSI installed and activated the new add-ins.
- Therefore the next release test must be a controlled uninstall/clean migration test followed by installation from the newly produced single MSI.
- The installer currently must not be declared 100% complete merely because the MSI builds or because an installed-program entry appears.

### Required test evidence before release freeze

Record PASS/FAIL evidence for:

1. Combined MSI contains Word output.
2. Combined MSI contains Excel output.
3. Clean installation activates Word add-in.
4. Clean installation activates Excel add-in.
5. Existing Tarjimon version is detected and user is asked before removal.
6. Third-party translator candidates are detected conservatively and user consent is required before removal.
7. UndoBridge is installed automatically and works.
8. Word/Excel registration works for the intended Office bitness.
9. Uninstall removes the product cleanly.
10. Reinstall from the same single MSI works.

Until these are PASS, do not call the 1.0 installer “100% complete/frozen”.
