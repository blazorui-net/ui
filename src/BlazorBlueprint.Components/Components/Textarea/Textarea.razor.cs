using BlazorBlueprint.Components.Input;
using BlazorBlueprint.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Linq.Expressions;

namespace BlazorBlueprint.Components.Textarea;

/// <summary>
/// A textarea component that follows the shadcn/ui design system.
/// </summary>
/// <remarks>
/// <para>
/// The Textarea component provides a customizable, accessible multi-line text input that
/// supports various states and features. It follows WCAG 2.1 AA standards
/// for accessibility and integrates with Blazor's data binding system.
/// </para>
/// <para>
/// Features:
/// - Multi-line text input with automatic content sizing
/// - Two-way data binding with Value/ValueChanged
/// - Character limit support via MaxLength parameter
/// - Error state visualization via aria-invalid attribute
/// - Smooth color and shadow transitions for state changes
/// - Disabled and required states
/// - Placeholder text support
/// - Full ARIA attribute support
/// - RTL (Right-to-Left) support
/// - Dark mode compatible via CSS variables
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;Textarea @bind-Value="description" Placeholder="Enter your description" /&gt;
///
/// &lt;Textarea Value="@comment" ValueChanged="HandleCommentChange" MaxLength="500" Required="true" AriaInvalid="@hasError" /&gt;
/// </code>
/// </example>
public partial class Textarea : ComponentBase, IDisposable
{
    private FieldIdentifier _fieldIdentifier;
    private EditContext? _editContext;
    private CancellationTokenSource? _debounceCts;

    /// <summary>
    /// Gets or sets the cascaded EditContext from a parent EditForm.
    /// </summary>
    [CascadingParameter]
    private EditContext? CascadedEditContext { get; set; }

    /// <summary>
    /// Gets or sets the current value of the textarea.
    /// </summary>
    /// <remarks>
    /// Supports two-way binding via @bind-Value syntax.
    /// </remarks>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the textarea value changes.
    /// </summary>
    /// <remarks>
    /// This event is fired on every keystroke (oninput event).
    /// Use with Value parameter for two-way binding.
    /// </remarks>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text displayed when the textarea is empty.
    /// </summary>
    /// <remarks>
    /// Provides a hint to the user about what to enter.
    /// Should not be used as a replacement for a label.
    /// </remarks>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets whether the textarea is disabled.
    /// </summary>
    /// <remarks>
    /// When disabled:
    /// - Textarea cannot be focused or edited
    /// - Cursor is set to not-allowed
    /// - Opacity is reduced for visual feedback
    /// </remarks>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether the textarea is required.
    /// </summary>
    /// <remarks>
    /// When true, the HTML5 required attribute is set.
    /// Works with form validation and :invalid CSS pseudo-class.
    /// </remarks>
    [Parameter]
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of characters allowed in the textarea.
    /// </summary>
    /// <remarks>
    /// When set, the HTML5 maxlength attribute is applied.
    /// Browser will prevent users from entering more than this many characters.
    /// </remarks>
    [Parameter]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the textarea.
    /// </summary>
    /// <remarks>
    /// Custom classes are appended after the component's base classes,
    /// allowing for style overrides and extensions.
    /// </remarks>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the HTML id attribute for the textarea element.
    /// </summary>
    /// <remarks>
    /// Used to associate the textarea with a label element via the label's 'for' attribute.
    /// This is essential for accessibility and allows clicking the label to focus the textarea.
    /// </remarks>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the ARIA label for the textarea.
    /// </summary>
    /// <remarks>
    /// Provides an accessible name for screen readers.
    /// Use when there is no visible label element.
    /// </remarks>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the ID of the element that describes the textarea.
    /// </summary>
    /// <remarks>
    /// References the id of an element containing help text or error messages.
    /// Improves screen reader experience by associating descriptive text.
    /// </remarks>
    [Parameter]
    public string? AriaDescribedBy { get; set; }

    /// <summary>
    /// Gets or sets whether the textarea value is invalid.
    /// </summary>
    /// <remarks>
    /// When true, aria-invalid="true" is set.
    /// Should be set based on validation state.
    /// Triggers destructive color styling for error states.
    /// </remarks>
    [Parameter]
    public bool? AriaInvalid { get; set; }

    /// <summary>
    /// Gets or sets the HTML name attribute for the textarea element.
    /// </summary>
    /// <remarks>
    /// When inside an EditForm and not explicitly set, the name is automatically
    /// derived from the ValueExpression (FieldIdentifier) to support SSR form postback.
    /// </remarks>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the bound value.
    /// </summary>
    [Parameter]
    public Expression<Func<string?>>? ValueExpression { get; set; }

    /// <summary>
    /// Gets or sets when <see cref="ValueChanged"/> fires.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><see cref="UpdateTiming.Immediate"/> — every keystroke (default, current behavior).</item>
    /// <item><see cref="UpdateTiming.OnChange"/> — only on blur / Enter.</item>
    /// <item><see cref="UpdateTiming.Debounced"/> — after typing pauses for <see cref="DebounceInterval"/> ms.</item>
    /// </list>
    /// </remarks>
    [Parameter]
    public UpdateTiming UpdateTiming { get; set; } = UpdateTiming.Immediate;

    /// <summary>
    /// Gets or sets the debounce delay in milliseconds when <see cref="UpdateTiming"/> is <see cref="UpdateTiming.Debounced"/>.
    /// </summary>
    /// <remarks>
    /// Ignored when <see cref="UpdateTiming"/> is not <see cref="UpdateTiming.Debounced"/>. Default is 500 ms.
    /// </remarks>
    [Parameter]
    public int DebounceInterval { get; set; } = 500;

    /// <summary>
    /// Gets whether the textarea is in an invalid state based on EditContext validation.
    /// </summary>
    private bool IsInvalid
    {
        get
        {
            if (_editContext != null && ValueExpression != null && _fieldIdentifier.FieldName != null)
            {
                return _editContext.GetValidationMessages(_fieldIdentifier).Any();
            }
            return false;
        }
    }

    /// <summary>
    /// Gets the effective name attribute, falling back to the FieldIdentifier name when inside an EditForm.
    /// </summary>
    private string? EffectiveName => Name ?? (_editContext != null && _fieldIdentifier.FieldName != null ? _fieldIdentifier.FieldName : null);

    /// <summary>
    /// Gets the effective aria-invalid value combining manual AriaInvalid and EditContext validation.
    /// </summary>
    private string? EffectiveAriaInvalid
    {
        get
        {
            if (AriaInvalid == true || IsInvalid)
            {
                return "true";
            }
            return AriaInvalid?.ToString().ToLowerInvariant();
        }
    }

    /// <summary>
    /// Gets the computed CSS classes for the textarea element.
    /// </summary>
    /// <remarks>
    /// Combines shadcn/ui v4 textarea styles:
    /// - field-sizing-content for automatic content-based sizing
    /// - min-h-16 for minimum height (4rem)
    /// - Base styles (flex, rounded, border, padding, transitions)
    /// - Focus states with ring effects
    /// - aria-invalid pseudo-selector for error state styling
    /// - Dark mode support via CSS variables
    /// - Smooth transitions for color and box-shadow
    /// - Custom classes from the Class parameter
    /// Uses the cn() utility for intelligent class merging and Tailwind conflict resolution.
    /// </remarks>
    private string CssClass => ClassNames.cn(
        // Base textarea styles (from shadcn/ui v4)
        "flex field-sizing-content min-h-16 w-full rounded-md border border-input",
        "bg-transparent dark:bg-input/30 px-3 py-2 text-base shadow-xs",
        "placeholder:text-muted-foreground",
        // Focus states
        "outline-none focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50",
        // Error states (aria-invalid)
        "aria-[invalid=true]:border-destructive aria-[invalid=true]:ring-destructive/20",
        "dark:aria-[invalid=true]:ring-destructive/40",
        // Disabled state
        "disabled:cursor-not-allowed disabled:opacity-50",
        // Smooth transitions
        "transition-[color,box-shadow]",
        // Responsive text sizing
        "md:text-sm",
        // Custom classes (if provided)
        Class
    );

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (CascadedEditContext != null && ValueExpression != null)
        {
            _editContext = CascadedEditContext;
            _fieldIdentifier = FieldIdentifier.Create(ValueExpression);
        }
    }

    /// <summary>
    /// Handles the input event (fired on every keystroke).
    /// </summary>
    private async Task HandleInput(ChangeEventArgs args)
    {
        var newValue = args.Value?.ToString();
        if (string.IsNullOrWhiteSpace(newValue))
        {
            newValue = null;
        }

        Value = newValue;

        switch (UpdateTiming)
        {
            case UpdateTiming.Immediate:
                if (ValueChanged.HasDelegate)
                {
                    await ValueChanged.InvokeAsync(newValue);
                }
                break;

            case UpdateTiming.OnChange:
                // Display updates via Value assignment above; ValueChanged deferred to HandleChange.
                break;

            case UpdateTiming.Debounced:
                DebounceValueChanged(newValue);
                break;
        }

        if (_editContext != null && ValueExpression != null && _fieldIdentifier.FieldName != null)
        {
            _editContext.NotifyFieldChanged(_fieldIdentifier);
        }
    }

    /// <summary>
    /// Handles the change event (fired when textarea loses focus or Enter is pressed).
    /// </summary>
    private async Task HandleChange(ChangeEventArgs args)
    {
        if (UpdateTiming == UpdateTiming.OnChange)
        {
            var newValue = args.Value?.ToString();
            Value = newValue;

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(newValue);
            }
        }
    }

    /// <summary>
    /// Starts (or restarts) a debounce timer that fires <see cref="ValueChanged"/> after <see cref="DebounceInterval"/> ms.
    /// </summary>
    private async void DebounceValueChanged(string? value)
    {
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        _debounceCts = new CancellationTokenSource();

        try
        {
            await Task.Delay(DebounceInterval, _debounceCts.Token);

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }
        }
        catch (TaskCanceledException)
        {
            // Timer was cancelled — either by a new keystroke or disposal.
        }
    }

    public void Dispose()
    {
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        GC.SuppressFinalize(this);
    }
}
