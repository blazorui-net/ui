namespace BlazorUI.Components.ColorPicker;

/// <summary>
/// Defines the color format for the ColorPicker.
/// </summary>
public enum ColorFormat
{
    /// <summary>
    /// Hexadecimal format (#RRGGBB or #RRGGBBAA).
    /// </summary>
    Hex,

    /// <summary>
    /// RGB format (rgb(r, g, b) or rgba(r, g, b, a)).
    /// </summary>
    RGB,

    /// <summary>
    /// HSL format (hsl(h, s%, l%) or hsla(h, s%, l%, a)).
    /// </summary>
    HSL
}
