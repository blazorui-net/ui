using BlazorBlueprint.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorBlueprint.Components.Timeline;

/// <summary>
/// Renders a vertical connector line between timeline items.
/// </summary>
/// <remarks>
/// The connector color adapts based on the item's status:
/// - Completed: solid primary color
/// - InProgress: gradient from primary to muted
/// - Pending: muted color
/// A custom color can override the status-based coloring.
/// </remarks>
public partial class TimelineConnector : ComponentBase
{
    /// <summary>
    /// Gets or sets the status to determine connector styling.
    /// </summary>
    [Parameter]
    public TimelineStatus Status { get; set; } = TimelineStatus.Completed;

    /// <summary>
    /// Gets or sets an explicit color override for the connector.
    /// When set, this takes precedence over the status-based color.
    /// </summary>
    [Parameter]
    public TimelineColor? Color { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the connector.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Captures any additional HTML attributes.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string CssClass => ClassNames.cn(
        "w-0.5",
        Color switch
        {
            TimelineColor.Primary => "bg-primary",
            TimelineColor.Secondary => "bg-secondary",
            TimelineColor.Muted => "bg-muted",
            TimelineColor.Accent => "bg-accent",
            TimelineColor.Destructive => "bg-destructive",
            _ => Status switch
            {
                TimelineStatus.Completed => "bg-primary",
                TimelineStatus.InProgress => "bg-linear-to-b from-primary to-muted",
                TimelineStatus.Pending => "bg-muted",
                _ => "bg-primary"
            }
        },
        Class
    );
}
