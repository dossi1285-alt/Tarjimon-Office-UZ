# Tarjimon Office UZ — UI Design Rules

## Permanent Preflight branding rule — 2026-08-21

The Preflight window is part of the Tarjimon Office UZ product and must keep a clean, professional Windows-style appearance.

### Protected product name

The product name shown in the Preflight interface is:

**Tarjimon Office UZ**

Do not replace it with a generic product name, placeholder, or another vendor name.

### Protected UI principle

The review window must not contain technical diagnostic explanations that visually clutter the normal user-facing interface. Internal detection evidence belongs in code/logging where appropriate, not as a long warning paragraph in the main window.

In particular, do not restore the previous footer sentence beginning with `Muhim: 'Igor Pavlov'...` to the normal UI. That information is an implementation/detection rule, not user-facing branding.

### Current approved visual direction

- Clear `Tarjimon Office UZ` brand header.
- Short subtitle describing Office translator detection.
- Clean white Windows-style layout.
- Simple table for detected products.
- Own-product card identifying `Tarjimon Office UZ`.
- `Tarjimon Office UZ` remains checked by default when it is detected as the own product.
- Third-party products remain user-controlled and unchecked by default.
- `Tasdiqlash` and `Bekor qilish` remain clear primary/secondary actions.
- The window remains resizable.
- Do not add decorative images/charts solely for decoration; visual elements must support the installer workflow.

### Do not modify verified unrelated behavior

UI cleanup must not change the detection algorithm, uninstall behavior, product grouping, installer flow, or any condition already marked VERIFIED/ACCEPTED. UI changes must remain limited to presentation unless a separate acceptance requirement explicitly requires behavior changes.

### Acceptance

The visual change is not VERIFIED until the user builds and opens the actual Preflight executable and confirms that:

1. `Tarjimon Office UZ` is clearly displayed as the product name.
2. The previous technical `Muhim: 'Igor Pavlov'...` paragraph is absent.
3. The interface is clean and readable.
4. The detection list, checkboxes, and buttons still work.
5. Resizing still works.
