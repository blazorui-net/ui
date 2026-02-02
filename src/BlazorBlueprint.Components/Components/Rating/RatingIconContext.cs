namespace BlazorBlueprint.Components.Rating;

/// <summary>
/// Context provided to custom icon templates.
/// </summary>
public class RatingIconContext
{
    /// <summary>
    /// The index of the icon (1-based).
    /// </summary>
    public int Index { get; init; }

    /// <summary>
    /// The fill percentage (0, 0.5, or 1).
    /// </summary>
    public double Fill { get; init; }

    /// <summary>
    /// Whether this icon is currently being hovered.
    /// </summary>
    public bool IsHovered { get; init; }

    /// <summary>
    /// The active color.
    /// </summary>
    public string? ActiveColor { get; init; }

    /// <summary>
    /// The inactive color.
    /// </summary>
    public string? InactiveColor { get; init; }
}
