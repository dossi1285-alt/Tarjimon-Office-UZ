# Tarjimon Office UZ — Assistant Result & Reminder Rules

## Permanent workflow rule — 2026-08-20

For this project, the assistant must maintain the project's reminder/continuation memory automatically. The user does not need to repeatedly say "save this".

### 1. Automatic recording after a successful result

When the assistant independently verifies that a requested task, test, build, fix, or other project step was successful, the assistant must record the result in the project's reminder/continuation notes.

The record must contain, when applicable:

- What was successfully completed.
- What changed compared with the previous state.
- What evidence confirmed the success.
- Any important limitation or qualification of the result.
- The next required action(s).
- Any remaining blocker or test that must still pass.

Do not record a result as successful merely because a build command completed if the project's acceptance criteria require a real-world test.

### 2. Update the reminder after every positive result

Every independently confirmed positive result must be added to the project history/reminder, even when the change is small. This includes, for example: successful build, installer generation, detection test, UI correction, uninstall/migration test, Word test, Excel test, UndoBridge test, signing/trust verification, regression test, synchronization, or release step.

The assistant must preserve cumulative history rather than replacing earlier successful results.

### 3. User notification is mandatory

Whenever the assistant updates the project's reminder/continuation memory because of a confirmed result, the assistant must explicitly tell the user that the reminder was updated and state what was added or changed.

### 4. Verification before recording success

The assistant must distinguish between implemented, built, tested, verified, and release-complete. Only use the strongest status supported by evidence. Never mark the 1.0 release complete from a partial build or isolated component test.

### 5. Completed-condition removal rule

Once the assistant independently verifies that an active condition has been fully satisfied according to its acceptance criteria, it must mark it COMPLETED in history, remove it from the active conditions/tasks list, preserve the completed history, notify the user, and record the next remaining active condition. A condition must not be removed merely because source code was changed or a build succeeded.

### 6. Next-step continuity

After each recorded result, preserve the next action. Installer rules remain: ONE user-facing setup; Word + Excel together; Preflight migration/consent; no silent third-party add-in removal; real Word/Excel tests before release freeze.

### 7. GitHub workflow — mandatory user notification and step tracking

GitHub is part of the working procedure. Whenever the assistant creates or updates project files directly on GitHub, the assistant must record that change and explicitly tell the user.

User-side synchronization for assistant-made GitHub changes:

**Fetch origin → Pull origin → Build → Test.**

If the user makes local changes:

**Commit → Push origin → Fetch/Pull as needed → Build → Test.**

The assistant must not tell the user to Push an assistant-made remote commit merely to receive it locally; normally the correct action is Pull origin.

Every GitHub-change notification must state the changed file/project, commit SHA when available, the user's next action, and the result that will confirm success.

### 8. Previous detection test result

The previous detection test was NOT fully successful: the user's Word screenshot showed `KL Office uz`, while the Preflight list did not show it. The own-product row was detected, but extra `TarjimonOfficeUZ.Excel` and `TarjimonOfficeUZ.Word` rows also appeared with unknown publisher.

### 9. Completed condition — resizable Preflight window

The requirement to keep the normal dialog size while allowing the user to enlarge the window is now **VERIFIED COMPLETE**.

Evidence from the latest user test:

- the window can be resized;
- the list expands vertically and horizontally;
- a long detection list can be viewed by enlarging the window;
- the standard starting size remains compact;
- the resize behavior works in the actual built Preflight executable.

Therefore this condition has been removed from the active conditions list. It remains in project history as completed.

### 10. Current detection/grouping problem

The second requirement is NOT complete.

The latest test showed that the scanner is finding many unrelated COM registrations such as language/runtime components. The root cause is in the current `ScanOfficeComRegistrations` strategy: it scans the entire `Software\\Classes\\CLSID` tree and then applies the broad `TranslatorWords` matcher. The matcher contains generic terms such as `language`, so unrelated COM classes can be classified as translator/add-in candidates.

This global CLSID scan is therefore too broad and is not a safe basis for the installer migration list.

The row showing publisher `Igor Pavlov` and version `25.01` should NOT be assumed to be an Office translator solely from that metadata. It may be related to another installed component (for example a 7-Zip/Igor Pavlov component), but the current scanner has not established that it is an Office add-in. It must not be selected automatically and must not be treated as a translator without an Office-specific registration/loading path.

### 11. Required detection redesign before the next code change

The next implementation should:

1. Stop treating the entire machine-wide CLSID registry as an Office add-in list.
2. Keep Office-specific registry locations as the primary detection source: `Office\\Word\\Addins`, `Office\\Excel\\Addins`, and relevant Office/VSTO/add-in registration paths.
3. Keep Word `STARTUP` and Excel `XLSTART`/configured startup paths for file-based add-ins/templates.
4. If COM/CLSID inspection is retained, use it only when there is an explicit Office add-in relationship or another reliable Office-specific registration/link; do not classify a CLSID merely because its name/path contains a generic word such as `language`.
5. Narrow the translator keyword matcher. Generic words such as `language` and `translate` alone must not be sufficient to classify a random COM class. Strong product/vendor/add-in evidence should be required.
6. Preserve third-party detection when there is reliable evidence that the component is an Office add-in, but keep it unchecked by default.
7. Group entries belonging to the same real product only after a reliable product identity has been established. Word/Excel host information should be merged into one row for the same product.
8. Do not use publisher/version alone as proof that two unrelated registrations belong to the same product.
9. Investigate `KL Office uz` specifically through the Office/Word registration and loading mechanisms that actually cause its Ribbon to appear, rather than broadening the global CLSID scan further.
10. Do not press `Tasdiqlash` until the detection list is accurate enough for safe user-controlled removal.

### 12. Current active test sequence

1. Implement the safer Office-specific detection/grouping redesign.
2. Commit the change to GitHub and record the commit SHA.
3. User: Fetch origin.
4. User: Pull origin.
5. Build `TarjimonOfficeUZ.Setup.Preflight`.
6. Build `TarjimonOfficeUZ.Setup.Wix`.
7. Launch the combined installer.
8. Verify `KL Office uz` is detected if it is actually registered as an Office add-in.
9. Verify unrelated COM/runtime components no longer appear merely because they contain generic words such as `language`.
10. Verify the same product is shown as one row with combined Word/Excel hosts.
11. Verify own product remains checked and third-party products remain unchecked.
12. Do NOT press `Tasdiqlash` until the detection result is accepted.

Only after the relevant acceptance tests pass may a condition be removed from the active list.

### 13. GitHub as canonical project memory

This file is part of the project's continuation memory. GitHub repository `dossi1285-alt/Tarjimon-Office-UZ`, active branch `release/1.0-installer-cleanup`, remains the canonical project source/history.

When a new confirmed project result is obtained, append/update the relevant continuation record rather than relying only on the chat transcript.
