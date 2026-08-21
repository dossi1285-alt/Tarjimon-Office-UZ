# Tarjimon Office UZ — Current Continuation Reminder

## 2026-08-21 — Preflight acceptance result

User ran the rebuilt Preflight and provided a screenshot.

### Confirmed
- `TransLit` is detected as a Word candidate: `60/100`, `Word`, with `Word Startup + Office startup fayli` evidence.
- `Igor Pavlov` does not appear in the list. This remains the required behavior.
- The scanner no longer shows the previous broad COM/runtime false positives in this test.
- The own product is detected, but it is currently split into multiple rows:
  - `Tarjimon Office UZ` — Windows, checked, 90/100.
  - `TarjimonOfficeUZ.Setup` — Windows, checked, 90/100, publisher unknown.
  - `Tarjimon Office UZ` — Excel, Word, checked, 67/100.
- `TransLit` is correctly unchecked by default.

### Failure found
The own product grouping is NOT ACCEPTED yet. The current `BuildProductIdentity` prioritized an MSI product code before the own-product identity, so different own-product MSI/component registrations were split into separate rows.

### Fix implemented
`TarjimonOfficeUZ.Setup.Preflight/Program.cs` was changed so `IsOwnProduct` identity takes precedence over MSI product-code identity. All own-product registrations/components therefore group under:

`OWN:tarjimon-office-uz`

The change is intentionally minimal and does not alter the verified resizable Preflight UI.

Commit: `7b695822b7ff144fab2bfdd9c84d7ea15af967b4`
Status: **IMPLEMENTED / NOT YET TESTED**

### Next acceptance test
1. Pull the commit.
2. Build Preflight.
3. Launch Preflight.
4. Verify there is ONE `Tarjimon Office UZ` row, checked by default, with combined host information and usable uninstall information.
5. Verify `TransLit` remains present and unchecked.
6. Verify `Igor Pavlov` remains absent.
7. Verify unrelated Windows/Office components remain absent.
8. Do not press `Tasdiqlash` until this list is accepted.

This file is a continuation checkpoint. The canonical `docs/ASSISTANT-REMINDER-RULES.md` remains the permanent workflow/control document; this checkpoint records the latest test result without rewriting completed history.
