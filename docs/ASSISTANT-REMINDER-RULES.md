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

Every independently confirmed positive result must be added to the project history/reminder, even when the change is small. This includes, for example:

- successful build;
- successful installer generation;
- successful detection test;
- successful UI correction;
- successful uninstall/migration test;
- successful Word test;
- successful Excel test;
- successful UndoBridge test;
- successful signing/trust verification;
- successful regression test;
- successful synchronization or release step.

The assistant must preserve cumulative history rather than replacing earlier successful results.

### 3. User notification is mandatory

Whenever the assistant updates the project's reminder/continuation memory because of a confirmed result, the assistant must explicitly tell the user that the reminder was updated.

The notification must also state what was added or changed. It should be concise but concrete, for example:

> **Eslatma yangilandi.**
> - Muvaffaqiyatli natija: ...
> - Eslatmaga qo'shildi: ...
> - Keyingi qadam: ...

Do not merely say "saved" without explaining what was recorded.

### 4. Verification before recording success

The assistant must distinguish between:

- **implemented** — source change exists;
- **built** — build completed successfully;
- **tested** — the relevant behavior was actually tested;
- **verified** — the assistant has enough evidence to confirm the expected result;
- **release-complete** — all project acceptance criteria are satisfied.

Only use the strongest status that the evidence supports. Never mark the 1.0 release complete from a partial build or isolated component test.

### 5. Completed-condition removal rule

The reminder contains active conditions/tasks that still need to be satisfied. Once the assistant independently verifies that one of those conditions has been fully satisfied according to its acceptance criteria, the assistant must:

1. Mark that condition as **COMPLETED** in the project history.
2. Remove it from the active conditions/tasks list so it is no longer presented as an outstanding task.
3. Preserve the completed item in the cumulative history so the project does not lose the record of what was accomplished.
4. Explicitly notify the user that the condition was removed from the active list and state exactly what was verified as complete.
5. Record the next remaining active condition/task.

A condition must NOT be removed merely because source code was changed or a build succeeded. It is removed only after the acceptance test for that condition is actually verified.

### 6. Next-step continuity

After recording a successful result, the reminder must preserve the next action so a future session can continue without making the user repeat the plan.

For installer work, this must remain consistent with `PROJECT_RULES.md`:

- ONE user-facing setup;
- Word + Excel together;
- Preflight migration/consent;
- no silent third-party add-in removal;
- real Word/Excel tests before release freeze.

### 7. GitHub workflow — mandatory user notification and step tracking

GitHub is part of the project's working procedure, not merely a backup.

Whenever the assistant creates or updates project files directly on GitHub, the assistant must record that change in the reminder notes and explicitly tell the user that a GitHub change was made.

The reminder must identify the required synchronization action for the user's local machine:

1. **Fetch origin** — refresh remote information in GitHub Desktop; use this when checking whether new remote commits exist.
2. **Pull origin** — download/apply the remote commits to the local repository after the assistant has made a remote change.
3. **Build/Test locally** — after Pull, rebuild the affected project(s) and run the relevant test before treating the remote change as locally verified.
4. **Commit** — when the user has made local changes that should be recorded.
5. **Push origin** — upload the user's local commit(s) to GitHub after committing.

For assistant-made GitHub changes, the default user-side sequence is:

**Fetch origin → Pull origin → Build → Test.**

If the user makes a local change after Pull, the continuation becomes:

**Commit → Push origin → Fetch/Pull as needed → Build → Test.**

The assistant must not tell the user to Push an assistant-made remote commit merely to receive it locally; the correct action is normally **Pull origin**.

Whenever a GitHub change is made, the user notification must include:

- what file/project was changed;
- the commit SHA when available;
- what the user needs to do next (`Fetch origin`, `Pull origin`, `Build`, `Test`, or `Commit/Push` as applicable);
- what result will confirm that synchronization/build/test succeeded.

### 8. Current immediate continuation

As of 2026-08-20, the Preflight detection/UI work has produced a successful partial result: duplicate handling and the own-product detection flow were improved and the user confirmed the updated dialog is working better. However, the user's Word screenshot still shows `KL Office uz`, which has not yet been confirmed as detected by the current scanner.

Next required action:

1. Extend/verify detection for `KL Office uz` and other relevant Office add-in loading mechanisms without switching to an unsafe "delete every ribbon add-in" approach.
2. Rebuild the Preflight and combined installer.
3. Test the detection list visually before pressing `Tasdiqlash` for destructive migration.
4. Only after detection passes, continue with controlled uninstall/reinstall and Word + Excel verification.

### 9. GitHub as canonical project memory

This file is part of the project's continuation memory. GitHub repository `dossi1285-alt/Tarjimon-Office-UZ`, active branch `release/1.0-installer-cleanup`, remains the canonical project source/history according to `PROJECT_RULES.md`.

When a new confirmed project result is obtained, append/update the relevant continuation record rather than relying only on the chat transcript.
