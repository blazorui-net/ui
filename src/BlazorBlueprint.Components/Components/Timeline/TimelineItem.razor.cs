using BlazorBlueprint.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorBlueprint.Components.Timeline;

/// <summary>
/// Represents a single item/entry in a Timeline.
/// </summary>
/// <remarks>
/// Each TimelineItem renders a date column, icon/connector column, and content column.
/// It supports custom icons, status-based styling, and optional connector lines.
/// </remarks>
public partial class TimelineItem : ComponentBase
{
    /// <summary>
    /// Gets or sets the color theme for the icon.
    /// </summary>
    [Parameter]
    public TimelineColor IconColor { get; set; } = TimelineColor.Primary;

    /// <summary>
    /// Gets or sets the current status of the item.
    /// </summary>
    [Parameter]
    public TimelineStatus Status { get; set; } = TimelineStatus.Completed;

    /// <summary>
    /// Gets or sets the color theme for the connector line.
    /// </summary>
    [Parameter]
    public TimelineColor? ConnectorColor { get; set; }

    /// <summary>
    /// Gets or sets whether to show the connector line below this item.
    /// </summary>
    [Parameter]
    public bool ShowConnector { get; set; } = true;

    /// <summary>
    /// Gets or sets the size of the icon.
    /// </summary>
    [Parameter]
    public TimelineSize IconSize { get; set; } = TimelineSize.Medium;

    /// <summary>
    /// Gets or sets additional CSS classes to apply to the item.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the main content (TimelineContent with header and description).
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets custom icon content to replace the default icon.
    /// </summary>
    [Parameter]
    public RenderFragment? IconContent { get; set; }

    /// <summary>
    /// Gets or sets the time/date content displayed in the left column.
    /// </summary>
    [Parameter]
    public RenderFragment? TimeContent { get; set; }

    /// <summary>
    /// Captures any additional HTML attributes.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string CssClass => ClassNames.cn(
        "relative w-full mb-8 last:mb-0",
        Class
    );

    private string ContentGridClass => ClassNames.cn(
        "grid grid-cols-[1fr_auto_1fr] gap-4 items-start",
        Status == TimelineStatus.InProgress ? "aria-current-step" : null
    );
}
