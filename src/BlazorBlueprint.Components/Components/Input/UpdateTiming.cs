namespace BlazorBlueprint.Components.Input;

/// <summary>
/// Controls when <c>ValueChanged</c> fires relative to user input.
/// </summary>
public enum UpdateTiming
{
    /// <summary>
    /// Fires <c>ValueChanged</c> on every keystroke (default, oninput event).
    /// </summary>
    Immediate,

    /// <summary>
    /// Fires <c>ValueChanged</c> only when the input loses focus or the user presses Enter (onchange event).
    /// </summary>
    OnChange,

    /// <summary>
    /// Fires <c>ValueChanged</c> after typing pauses for <c>DebounceInterval</c> milliseconds.
    /// </summary>
    Debounced
}
