namespace BlazorBlueprint.Components.Timeline;

/// <summary>
/// Defines the status of a timeline item.
/// </summary>
/// <remarks>
/// Controls the visual appearance of the timeline icon and connector line
/// to indicate progress through a sequence of events.
/// </remarks>
public enum TimelineStatus
{
    /// <summary>
    /// The item has been completed. Displays with primary color.
    /// </summary>
    Completed,

    /// <summary>
    /// The item is currently in progress. Displays with a gradient connector.
    /// </summary>
    InProgress,

    /// <summary>
    /// The item is pending and has not started. Displays with muted color.
    /// </summary>
    Pending
}
