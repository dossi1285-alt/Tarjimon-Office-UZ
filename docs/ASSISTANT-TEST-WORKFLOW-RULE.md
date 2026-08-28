# Assistant Test Workflow Rule

Permanent project workflow rule — 2026-08-21

When the assistant tells the user to synchronize an assistant-made GitHub change, the final Git instruction must include every genuinely required next step in order:

**Fetch → Pull → Build → Test**

Do not stop at Pull when Build is required. Do not stop at Build when Test is required. If a test is not required, say so explicitly. If only a local acceptance test is unavoidable, state exactly what must be tested and the PASS condition.

The assistant must automate Build/Test/Verification whenever practical and must not routinely make the user run manual PowerShell commands one by one.

The Git instruction belongs at the end of the response, after the work summary and conclusions, not at the beginning.

If the assistant changed GitHub remotely, the user normally does not need Commit/Push for that change; normally Fetch → Pull is sufficient. If the user made local changes, use Commit → Push as appropriate.
