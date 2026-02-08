## What's New in v2.4.0

### New Features
- **InputField\<TValue\> component** — generic typed input with automatic type conversion, formatting, validation, and parse error handling
- **InputConverter\<TValue\> system** — global, instance, and built-in default converter resolution for 15+ types
- **EditForm integration** — cascading EditContext, ValueExpression, field validation tracking, and aria-invalid state for 14 input components (Input, Textarea, InputGroupInput, InputGroupTextarea, NumericInput, CurrencyInput, MaskedInput, Select, Combobox, NativeSelect, DatePicker, TimePicker, ColorPicker, InputOTP)
- **UpdateTiming and debounce** — Immediate, OnChange, and Debounced modes for Textarea, InputGroupInput, InputGroupTextarea, CommandInput, NumericInput, and CurrencyInput
- **Cursor utilities** — CursorType enum and CursorExtensions.ToClass() for mapping cursor types to Tailwind CSS classes; Button now shows cursor-pointer by default

### Bug Fixes
- DataTable global search now uses the column's Format string when matching values
- Text input components normalize empty/whitespace strings to null for consistent validation on nullable string properties
- Command palette keyboard shortcut changed from Ctrl+K to Ctrl+I to avoid browser address bar conflict
- Timeline connector uses dynamic height instead of fixed, with symmetric spacing and proper z-index
- Fixed dropdown mispositioning when switching between multiple open MultiSelect components
- Fixed stale portal content when interacting inside open popovers
- Fixed ContextMenu keyboard navigation breaking on repeated right-clicks

### Improvements
- blazorblueprint.css removed from git tracking — rebuilt automatically during CI build
- Release script enhanced with --yes, --update-primitives, and --release-notes flags
