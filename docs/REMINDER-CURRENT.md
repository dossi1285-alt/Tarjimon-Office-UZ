# Tarjimon Office UZ — Current Continuation Reminder

## 2026-08-21 — Preflight acceptance result

User ran the rebuilt Preflight and provided a screenshot.

### Confirmed
- `TransLit` is detected as a Word candidate: `60/100`, `Word`, with `Word Startup + Office startup fayli` evidence.
- `Igor Pavlov` does not appear in the list. This remains the required behavior.
- The scanner no longer shows the previous broad COM/runtime false positives in this test.
- The own product is now consolidated into ONE row: `Tarjimon Office UZ`.
- `Tarjimon Office UZ` is checked by default.
- `TransLit` is correctly unchecked by default.
- `Dastur` for the own product shows `Excel, Word`; `Windows` is not shown as an Office host.
- User-facing `Mahsulot nomi` and `Muallif / ishlab chiquvchi` columns are present.
- The own product developer/author is exactly `Dostonjon Ashurov`.
- The removed technical explanatory sentences are absent from the main window.
- The `Bekor qilish` button has visible right-side spacing from the window edge.
- Build repair completed successfully with 0 warnings and 0 errors before this acceptance test.

### Current product/UI rules
- The own product must always display as `Tarjimon Office UZ`.
- User-facing table column `Qo‘shimcha nomi` is replaced by `Mahsulot nomi`.
- User-facing `Aniqlash asosi` is replaced by `Muallif / ishlab chiquvchi`.
- For the own product, the developer/author value is exactly `Dostonjon Ashurov`; do not append words such as `ishlab chiqaruvchi`.
- `Windows` must not be displayed as a user-facing Office host in the `Dastur` column; only actual Office hosts such as `Word` and `Excel` should be shown.
- Technical detection evidence remains internal and must not be displayed as explanatory text in the main user-facing window.
- The UI must remain clean and professional; do not reintroduce the removed technical explanatory sentences.
- `Tarjimon Office UZ` branding/name is permanent and must not be changed unless the user explicitly requests it.
- `Bekor qilish` must have visible right-side padding from the window edge, comparable to the spacing between `Tasdiqlash` and `Bekor qilish`.

### Build/operation rule
- Do not give the user unnecessary manual PowerShell/CMD commands or multi-step repair work.
- Perform repository/code changes yourself whenever the available tools allow it.
- If a local action is genuinely required, provide one ready-to-run BAT/script or one clearly identified UI action and give its exact repository path/address.
- Whenever asking the user to run a BAT/script, ALWAYS provide its exact repository path, for example:
  `D:\Tarjimon-Office-UZ\FIX_BUILD_AND_BUILD.bat`
  The exact local root may differ, so also state the repository-relative path: `FIX_BUILD_AND_BUILD.bat`.
- If a Pull is required, say Pull only when appropriate; if Build is then required, explicitly say Pull → Build; if Test is then required, explicitly say Pull → Build → Test.
- After code changes, the final Git summary must be at the end of the response, not at the beginning.
- The user may be asked to do only the necessary Git operation(s) or run the prepared BAT; do not make them manually reproduce checks that can be automated.

### Failure found
The own product grouping was previously NOT ACCEPTED because `BuildProductIdentity` prioritized an MSI product code before the own-product identity.

### Fix implemented
`TarjimonOfficeUZ.Setup.Preflight/Program.cs` was changed so `IsOwnProduct` identity takes precedence over MSI product-code identity. All own-product registrations/components therefore group under:

`OWN:tarjimon-office-uz`

### Current build repair
A ready-made repair/build script exists at repository root:

`FIX_BUILD_AND_BUILD.bat`

When the user needs to run it locally, always provide the exact repository-relative path and, if known from context, the local absolute path. This BAT fetches the correct `Program.cs`, cleans the relevant `bin/obj` folders, finds MSBuild, and builds the combined WiX installer.

### Current acceptance status — PASS
The latest screenshot satisfies the current Preflight acceptance criteria. Do not change the accepted UI/detection behavior unless the user explicitly requests a new change or a later test exposes a defect.

### Next acceptance test
1. Do not press `Tasdiqlash` yet unless the next test specifically concerns the uninstall/installation action.
2. Continue with the next planned installer acceptance step without changing the accepted Preflight UI/detection behavior.

This file is a continuation checkpoint. The canonical project rules remain the permanent workflow/control document; this checkpoint records the latest operating rules and test state.
