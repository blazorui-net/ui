using BlazorBlueprint.Components.Utilities;
using Microsoft.AspNetCore.Components;

namespace BlazorBlueprint.Components.Timeline;

/// <summary>
/// Container for the main content area of a timeline item.
/// </summary>
/// <remarks>
/// Wraps the header and description of a timeline item in a flex column layout.
/// </remarks>
public partial class TimelineContent : ComponentBase
{
    /// <summary>
    /// Gets or sets additional CSS classes.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the content to render.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Captures any additional HTML attributes.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private string CssClass => ClassNames.cn(
        "flex flex-col gap-2 pl-2",
        Class
    );
}
