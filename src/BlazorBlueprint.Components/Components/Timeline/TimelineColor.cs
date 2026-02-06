namespace BlazorBlueprint.Components.Timeline;

/// <summary>
/// Defines the color theme for Timeline icon and connector components.
/// </summary>
/// <remarks>
/// Uses CSS custom properties for theming, consistent with the shadcn/ui design system.
/// </remarks>
public enum TimelineColor
{
    /// <summary>
    /// Primary color theme. Uses --primary and --primary-foreground CSS variables.
    /// </summary>
    Primary,

    /// <summary>
    /// Secondary color theme. Uses --secondary and --secondary-foreground CSS variables.
    /// </summary>
    Secondary,

    /// <summary>
    /// Muted color theme. Uses --muted and --muted-foreground CSS variables.
    /// </summary>
    Muted,

    /// <summary>
    /// Accent color theme. Uses --accent and --accent-foreground CSS variables.
    /// </summary>
    Accent,

    /// <summary>
    /// Destructive color theme. Uses --destructive and --destructive-foreground CSS variables.
    /// </summary>
    Destructive
}
