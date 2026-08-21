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

### 14. Permanent protected-project rule — added 2026-08-21

The uploaded handoff/TXT, `PROJECT_RULES.md`, `docs/PROJECT-AUDIT-2026-08-17.md`, and this reminder file are project control documents. Their completed-history, active-condition, architecture, acceptance-criteria, and explicit "do not" instructions must not be silently rewritten, removed, or treated as optional.

Before any consequential code change, the assistant must read the applicable project control documents and preserve their organization and meaning. If a section says a condition is completed, that history remains preserved. If a section says a condition is still active or that a particular action must happen first, the assistant must not bypass that sequence by implementing a different solution prematurely.

In particular, the current detection sequence is protected:

**First determine the real `KL Office uz` Word Ribbon loading source/mechanism → then design the detection/grouping change → then modify code → then GitHub commit → then Fetch → Pull → Build → Test → screenshot acceptance.**

The assistant must not broaden detection, add scoring/UI changes, or invent new candidate sources merely to make a named product appear before its actual Office loading mechanism is established.

### 15. Assistant error record — 2026-08-21

The assistant made an unverified implementation in commit `6f0c3dfcf5b9a8d559d8f99c2c7e5a67411b9eb7` (`Redesign translator detection around Office and functional evidence`) before completing the required `KL Office uz` loading-source investigation.

That commit changed the candidate model and UI by adding score/evidence fields and added a Windows Uninstall scan. It also changed the detection strategy. Although the global machine-wide CLSID scan was removed, the change was made before the required source-first investigation and therefore must be treated as **IMPLEMENTED / UNVERIFIED**, not as a verified solution.

The assistant must not claim that `Translit` detection, `Igor Pavlov` filtering, or the new scoring UI is verified until the user performs the prescribed build/test and the result passes the acceptance criteria.

This error is recorded so that it is not repeated. The next code change must return to the protected sequence and first identify how the real `KL Office uz` Ribbon is loaded.

### 16. Permanent rule for future assistant responses

After every assistant-made GitHub code change, the response to the user must immediately include:

1. The exact file/project changed.
2. The exact commit SHA.
3. The status: implemented / built / tested / verified / release-complete, using only the strongest status actually supported by evidence.
4. The exact local sequence: **Fetch origin → Pull origin → Build → Test**.
5. What the user should check in the test and what result would count as PASS.
6. A statement that the reminder was updated, plus the exact reminder entry that was added/changed.

The assistant must never make the user repeatedly remind it to update the project reminder after a confirmed result or code change.

## 17. KL Office loading-source investigation — 2026-08-21

A read-only diagnostic was added before any further detection redesign:

- File: `docs/DIAGNOSE-KL-OFFICE.ps1`
- Commit: `d3dba3f1404e5d87741989099782b80e133bbd6c`
- Status: **IMPLEMENTED / NOT YET TESTED**.
- Purpose: determine the actual Word/Excel loading source of `KL Office uz` without uninstalling, disabling, deleting, or modifying anything.
- It inspects Word `COMAddIns`, Word `AddIns`, templates, startup path, legacy CommandBars captions, Office Addins registry metadata, Excel COMAddIns/AddIns, startup folders, and Ribbon XML inside Open XML add-in/template files where available.
- It writes a read-only report to the user's Desktop as `KL-Office-Diagnostic.txt`.

This diagnostic is an investigation tool, not the detection redesign. No active detection condition is marked complete by adding it. The next decision must be based on the diagnostic evidence.

## 18. Permanent user-workflow rule — added 2026-08-21

The user explicitly does **not** want a manual, repetitive testing workflow where the assistant writes a PowerShell command/script and then makes the user run different PowerShell commands one by one to inspect or verify every change.

The assistant must take responsibility for automating as much of the build/test/verification process as the available GitHub/project tooling allows. After an assistant-made change, the user should not be burdened with manual PowerShell diagnostics unless there is a genuine machine-local fact that cannot be obtained or tested through the available tooling.

The user's normal synchronization options are:

- If the assistant changed GitHub directly: normally the user only needs **Fetch → Pull** to receive the change locally, followed by whatever build/test action is genuinely unavoidable.
- If the user made local changes: the user may **Commit → Push**, after which the assistant can work from the updated remote state.
- The assistant may explicitly ask the user to **Commit** or **Push** when the user's local state must be brought to GitHub. It should not repeatedly demand manual PowerShell testing when the task can reasonably be automated.

The assistant must prefer, in this order:

1. Automate the required build/test/verification in the project itself (for example through a test harness, build target, GitHub Actions, or another suitable project mechanism).
2. Use GitHub/project tooling to inspect results where available.
3. Ask the user for a manual local action only when it is genuinely required by the local machine/environment.

The assistant must not make the user repeatedly restate this preference in future chats. This rule is permanent project workflow memory.

### 18.1 New-chat continuity rule

If the current conversation becomes too long and work must continue in a new chat, the assistant must preserve the project's accumulated technical state and, when needed for reliable continuation, create/update a plain-text handoff file containing the current project status, completed history, active conditions, latest commits, blockers, next steps, and user workflow rules.

The handoff TXT must be generated from the actual project reminder/history and current work state, not reconstructed from guesswork. In the new chat, the assistant must use that handoff/project control information before continuing work.

The user should not have to re-explain the project workflow or repeatedly remind the assistant of these rules.

## 19. User workflow clarification — 2026-08-21

The user clarified that they are not limited to Fetch/Pull/Push. They can also perform **Commit** locally when they have made local changes. The assistant must choose the minimum user-side Git action required by the actual situation:

- Assistant changed remote GitHub files: tell the user **Fetch → Pull**; do not ask them to Push those assistant-made changes.
- User changed files locally: tell the user **Commit → Push** when the local changes need to be delivered to GitHub.
- If a local commit already exists and only needs to reach the remote, ask for **Push** rather than another unnecessary commit.
- If no user-side Git action is required, do not ask for one.

The assistant must not give the user generic or repeated Git commands without first determining what actually changed and where.

### 19.1 Automation-first verification rule

For project work, the assistant must not routinely write manual PowerShell commands for the user to execute and then wait for screenshots/reports if the same verification can be implemented or executed automatically through the project, GitHub Actions, test harnesses, build targets, or available connected tooling.

The assistant should build the necessary automation into the project when practical, so that the user can normally receive a change with the appropriate Git action and let the project perform its own checks. Manual machine-local checks are reserved for cases where the local environment is the only source of the required evidence.

The user's requested workflow is therefore:

**Assistant implements/automates → GitHub commit/update → user performs only the necessary Git synchronization (Fetch/Pull or Commit/Push as applicable) → automated Build/Test/Verification → assistant evaluates the result.**

If the conversation becomes full, the assistant must create the requested TXT handoff from the accumulated project records before continuing in a new chat, so the workflow and technical history are preserved.

## 20. Release scope and product purpose — 2026-08-21

The immediate goal is to finish the **current release** of Tarjimon Office UZ as quickly and safely as possible. Do not deliberately defer current-release requirements to a later version when they are needed for the current installer to work correctly.

The core purpose of the current installer is:

1. Detect Office translator/add-in software already installed on the Windows computer.
2. Present the detected translator/add-in products to the user in the Preflight list so the user can decide what to keep and what to remove.
3. Mark the current Tarjimon Office UZ product as the **own product** and keep it selected by default when an older installation/version of the same product is detected, so the new version can replace/remove the old version through the supported uninstall path.
4. Do not silently remove third-party translator/add-ins. Third-party items must remain user-controlled and unchecked by default unless the project acceptance criteria explicitly say otherwise.
5. Detection must be based on reliable Office/add-in evidence and functional signals, not merely on arbitrary Windows program names or publisher names.
6. The migration list must be safe: unrelated Windows/Office components must not be presented as translators.

### 20.1 Current release versus later-version work

The assistant must prioritize only the work necessary to complete and release the current version. Work explicitly identified in project control documents as a future-version enhancement must not be pulled into the current release unless it is required to satisfy the current acceptance criteria or to prevent a release-blocking defect.

The assistant must not expand scope simply because a technically interesting improvement is possible. Finish the current installer, detection/migration, Word/Excel integration, and required acceptance tests first.

### 20.2 Protected verified/accepted work

Any project component that the project control documents or acceptance history mark as **VERIFIED**, **ACCEPTED**, **99%**, or **100%** is protected from unnecessary modification.

Such components must be treated as frozen unless:

- a confirmed regression is found;
- a current-release acceptance criterion directly requires a change; or
- the user explicitly authorizes changing that accepted component.

The assistant must preserve the existing implementation and make the smallest possible change around it rather than reopening or redesigning accepted work.

If a future requirement conflicts with a protected 99–100%/accepted component, the assistant must work around the protected component or defer the future requirement; it must not silently rewrite the accepted component.

### 20.3 Definition of done for the current release

The current release is not considered complete merely because source code compiles. Completion requires the current acceptance criteria to pass, including as applicable:

- one user-facing installer;
- Word + Excel together;
- safe Preflight detection/migration list;
- own-product handling for replacement of an older version;
- third-party items user-controlled and unchecked by default;
- no unrelated system false positives;
- required real Word/Excel acceptance tests;
- required build/test verification.

Only after those criteria pass may the current release be called release-complete.

### 20.4 Communication rule — only a few mandatory user requests

The assistant should minimize user interruptions. Apart from necessary local/environment actions, the assistant should only routinely ask the user for the following project actions:

1. **Pull** — when the assistant has changed GitHub and the user needs the remote change locally.
2. **Commit** — when the user has local changes that need to be recorded.
3. **Push** — when a local commit needs to be sent to GitHub.
4. **Build/Test or provide a real local acceptance result** — only when the required evidence genuinely cannot be obtained or automated by the assistant/project tooling.

The assistant must not repeatedly ask the user to run arbitrary diagnostic commands, copy commands into PowerShell, or perform manual registry/file checks when those checks can be automated or obtained through the project tooling.

## 21. New-chat handoff requirement

If the conversation reaches a point where continuation in a new chat is necessary, the assistant must prepare a plain-text handoff containing at minimum:

- current release goal and scope;
- protected 99–100%/verified components;
- completed history;
- active conditions;
- latest commits and their statuses;
- current blockers;
- exact next step;
- Git workflow rules;
- the four allowed routine user requests;
- the requirement to use the project control documents before making consequential changes.

The handoff must be based on the actual GitHub/project records and must be ready for use as the starting context of the new chat.
