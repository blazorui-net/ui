# Plan: SSR-Compatible Checkbox + SSR Demo App

## Problem

The Checkbox primitive renders `<button type="button" role="checkbox">`. In static SSR (no `@rendermode`), there is no Blazor runtime — no SignalR circuit, no WASM. The `@onclick` and `@onkeydown` handlers never fire. The checkbox renders but **cannot be toggled by the user**.

The same problem exists for the Components layer Checkbox, since it delegates to the Primitive.

## Solution Overview

1. **Refactor the Checkbox primitive** to use a native `<input type="checkbox">` inside a `<label>`, giving us browser-native toggle behavior, keyboard support, and form submission — all without JS or a Blazor runtime.

2. **Update the Components Checkbox** to use CSS-driven icon visibility (`peer-checked:`) instead of conditional C# rendering (`@if (Checked)`), so the checkmark responds to native input state changes in SSR.

3. **Create an SSR demo app** (`BlazorBlueprint.Demo.SSR`) that runs in pure static SSR mode to validate the changes.

---

## Part 1: Refactor Checkbox Primitive

### File: `src/BlazorBlueprint.Primitives/Primitives/Checkbox/Checkbox.razor`

**Before:**
```html
<button type="button" role="checkbox" aria-checked="@AriaCheckedValue" ... @onclick="HandleClick" @onkeydown="HandleKeyDown">
    @ChildContent
</button>
```

**After:**
```html
<label data-state="@DataState"
       aria-disabled="@(Disabled ? "true" : null)"
       @attributes="AdditionalAttributes">
    <input type="checkbox"
           id="@Id"
           name="@Name"
           checked="@Checked"
           disabled="@Disabled"
           value="@Value"
           aria-label="@AriaLabel"
           style="position:absolute;width:1px;height:1px;padding:0;margin:-1px;overflow:hidden;clip:rect(0,0,0,0);white-space:nowrap;border-width:0;"
           class="peer"
           @onchange="HandleChange" />
    @ChildContent
</label>
```

**Why this structure:**
- `<label>` wrapping `<input>`: clicking anywhere on the label toggles the checkbox natively (SSR-compatible, zero JS)
- Native `<input type="checkbox">`: browser handles toggle, Space key, form submission
- `class="peer"`: enables Tailwind `peer-checked:` variants on sibling content
- `style="..."` (sr-only inline): visually hides the input while keeping it accessible — inline styles because the Primitives layer is headless (no Tailwind dependency)
- `data-state` on label: preserved for CSS hooks and indeterminate state styling
- `@onchange`: fires in interactive mode; ignored in SSR (native behavior takes over)

### File: `src/BlazorBlueprint.Primitives/Primitives/Checkbox/Checkbox.razor.cs`

**Changes:**
- **Add** `Name` parameter (`string?`) — for HTML form submission
- **Add** `Value` parameter (`string`, default `"on"`) — form submission value (HTML standard default)
- **Replace** `HandleClick` + `HandleKeyDown` with `HandleChange(ChangeEventArgs)`:
  ```csharp
  private async Task HandleChange(ChangeEventArgs args)
  {
      var newChecked = (bool)(args.Value ?? false);
      if (Indeterminate)
      {
          Indeterminate = false;
          await IndeterminateChanged.InvokeAsync(false);
      }
      Checked = newChecked;
      await CheckedChanged.InvokeAsync(Checked);
  }
  ```
- **Remove** `shouldPreventDefault` field (native input handles this)
- **Keep** all existing parameters: `Checked`, `CheckedChanged`, `Indeterminate`, `IndeterminateChanged`, `Disabled`, `Id`, `AriaLabel`, `ChildContent`, `AdditionalAttributes`
- **Keep** `DataState` and `AriaCheckedValue` computed properties

### Behavioral comparison

| Concern | Before (button) | After (native input) |
|---------|-----------------|---------------------|
| SSR click toggle | Dead | Works natively |
| SSR Space key | Dead | Works natively |
| SSR form POST | Not possible | `name=value` submitted |
| Interactive toggle | `@onclick` handler | `@onchange` handler |
| Focus ring | On button | `peer-focus-visible:` on visual element |
| Screen reader | `role="checkbox"` + `aria-checked` | Implicit checkbox semantics |

---

## Part 2: Update Components Checkbox

### File: `src/BlazorBlueprint.Components/Components/Checkbox/Checkbox.razor`

**Before** (conditional C# rendering):
```razor
<Checkbox ...>
    @if (Checked) { <svg>checkmark</svg> }
    else if (Indeterminate) { <svg>dash</svg> }
</Checkbox>
```

**After** (CSS-driven visibility):
```razor
<Checkbox ... class="@CssClass" Name="@EffectiveName">
    <svg class="hidden peer-checked:block text-current" ...>
        <polyline points="20 6 9 17 4 12" />
    </svg>
    <svg class="hidden group-data-[state=indeterminate]:block peer-checked:!hidden text-current" ...>
        <line x1="5" y1="12" x2="19" y2="12" />
    </svg>
</Checkbox>
```

**Why CSS-driven:**
- `peer-checked:block` — checkmark shows/hides when native input is toggled (works in SSR)
- `group-data-[state=indeterminate]:block` — dash shows when server renders indeterminate state
- `peer-checked:!hidden` — dash hides if user clicks an indeterminate checkbox (the input becomes checked, so the dash must hide and checkmark must show)

### File: `src/BlazorBlueprint.Components/Components/Checkbox/Checkbox.razor.cs`

**Changes:**
- **Add** `Name` parameter (`string?`)
- **Add** `EffectiveName` computed property (following existing Input pattern):
  ```csharp
  private string? EffectiveName => Name ??
      (_editContext != null && _fieldIdentifier.FieldName != null
          ? _fieldIdentifier.FieldName : null);
  ```
- **Update** `CssClass` — replace `peer` with `peer group` (add `group` for `group-data-[state=...]` variant)
- **Add** `relative` to `CssClass` — needed so the `position:absolute` hidden input is positioned within the checkbox bounds

---

## Part 3: SSR Demo Application

Create `demos/BlazorBlueprint.Demo.SSR/` — a pure static SSR Blazor app with **no interactive render mode**. This is the key difference from the existing Server demo (which uses `@rendermode="InteractiveServer"`).

### Why a separate demo (not reusing Demo.Shared layout)?

The existing Demo.Shared layout uses JS-dependent components (Sidebar, CommandSearch, ThemeService, DarkModeToggle). In pure SSR with no Blazor runtime, those JS interop calls would fail. The SSR demo needs its own minimal layout.

### New files:

1. **`demos/BlazorBlueprint.Demo.SSR/BlazorBlueprint.Demo.SSR.csproj`**
   - `Microsoft.NET.Sdk.Web`, `net8.0`
   - References: `BlazorBlueprint.Components`, `BlazorBlueprint.Primitives`
   - No interactive component packages needed

2. **`demos/BlazorBlueprint.Demo.SSR/Program.cs`**
   ```csharp
   builder.Services.AddRazorComponents(); // NO .AddInteractiveServerComponents()
   app.MapRazorComponents<App>();         // NO .AddInteractiveServerRenderMode()
   ```

3. **`demos/BlazorBlueprint.Demo.SSR/App.razor`**
   - Standard HTML shell with CSS links (blazorblueprint.css)
   - `<Routes />` with **no `@rendermode`** (pure static SSR)
   - Include `blazor.web.js` for enhanced navigation (optional but standard)

4. **`demos/BlazorBlueprint.Demo.SSR/Routes.razor`**
   - Router pointing to own assembly

5. **`demos/BlazorBlueprint.Demo.SSR/_Imports.razor`**
   - Standard imports + BlazorBlueprint namespaces

6. **`demos/BlazorBlueprint.Demo.SSR/Shared/MainLayout.razor`**
   - Simple layout with no JS dependencies
   - Basic nav header with links

7. **`demos/BlazorBlueprint.Demo.SSR/Pages/Index.razor`**
   - Landing page explaining what this demo validates

8. **`demos/BlazorBlueprint.Demo.SSR/Pages/CheckboxSSRDemo.razor`**
   - **Basic toggle**: Checkbox that toggles visually on click (SSR, no runtime)
   - **Form submission**: `<form method="post">` with checkbox + submit button, displays submitted values after POST using `[SupplyParameterFromForm]`
   - **Multiple checkboxes**: Group of named checkboxes in a form
   - **Disabled state**: Verify disabled renders correctly
   - **Primitive vs Styled**: Side-by-side of both

9. **`demos/BlazorBlueprint.Demo.SSR/Properties/launchSettings.json`**
   - Port 7175 (https) / 5186 (http) — next in the sequence

10. **`BlazorBlueprint.sln`** — add the new project to the solution

---

## Implementation Order

1. Refactor `Primitives/Checkbox/Checkbox.razor` + `.razor.cs` (the foundation)
2. Update `Components/Checkbox/Checkbox.razor` + `.razor.cs` (CSS-driven icons + Name)
3. Update `Primitives/CheckboxPrimitiveDemo.razor` (demo page uses `@if (Checked)` — update to CSS approach)
4. Create the SSR demo app (project, program, layout, pages)
5. Add SSR project to solution
6. Build and verify compilation
