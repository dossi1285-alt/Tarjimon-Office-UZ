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

### 8. Current immediate continuation

The previous detection test was NOT successful: the user's Word screenshot showed `KL Office uz`, while the Preflight list did not show it. The own-product row was detected, but extra `TarjimonOfficeUZ.Excel` and `TarjimonOfficeUZ.Word` rows also appeared with unknown publisher.

### 9. Latest implementation status — KL Office detection

Source implementation completed on GitHub, but NOT YET VERIFIED by a local build/test.

Commit: `b92a44c0dda7574f156b7ac9bce0321dc1cb8ef2`.

Changes in `TarjimonOfficeUZ.Setup.Preflight/Program.cs`:

- expanded translator/add-in keyword detection, including `KL Office`, `KLOffice`, `KL_Office`, `Kirill`, and related terms;
- expanded Word/Excel startup-path discovery across Office-version registry paths;
- added common Office16 startup locations under Program Files/Program Files (x86);
- expanded startup-file metadata inspection;
- added CLSID/COM registration scanning for translator-related names and server paths;
- retained duplicate grouping and the rule that third-party add-ins are not selected automatically.

Because this is only an implementation change, the active condition remains:

1. Pull the new GitHub commit.
2. Build `TarjimonOfficeUZ.Setup.Preflight`.
3. Build `TarjimonOfficeUZ.Setup.Wix`.
4. Launch the combined installer and inspect the detection list.
5. Confirm whether `KL Office uz` now appears and whether the unwanted duplicate own-product rows are gone.
6. Do NOT press `Tasdiqlash` until the detection result is accepted.

Only after that real test passes may the `KL Office detection` condition be removed from the active list.

### 10. GitHub as canonical project memory

This file is part of the project's continuation memory. GitHub repository `dossi1285-alt/Tarjimon-Office-UZ`, active branch `release/1.0-installer-cleanup`, remains the canonical project source/history.

When a new confirmed project result is obtained, append/update the relevant continuation record rather than relying only on the chat transcript.
