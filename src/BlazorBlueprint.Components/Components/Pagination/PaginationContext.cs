using BlazorBlueprint.Primitives.Table;

namespace BlazorBlueprint.Components.Pagination;

/// <summary>
/// Provides context state for Pagination component and its sub-components.
/// </summary>
/// <remarks>
/// <para>
/// PaginationContext is used internally by the Pagination component system to coordinate
/// state between the container (Pagination) and child components (PaginationFirst, PaginationPrevious,
/// PaginationNext, PaginationLast, PaginationInfo, PaginationPageSizeSelector, etc.) via Blazor's
/// CascadingValue mechanism.
/// </para>
/// <para>
/// This context pattern enables:
/// <list type="bullet">
/// <item>Shared pagination state across all sub-components</item>
/// <item>Automatic navigation behavior without manual wiring</item>
/// <item>Centralized page size and page change callbacks</item>
/// <item>Coordinated disabled states for navigation buttons</item>
/// </list>
/// </para>
/// </remarks>
public class PaginationContext
{
    /// <summary>
    /// Gets or sets the pagination state containing current page, page size, and total items.
    /// </summary>
    /// <value>
    /// The <see cref="PaginationState"/> instance, or <c>null</c> if using manual mode.
    /// </value>
    public PaginationState? State { get; set; }

    /// <summary>
    /// Gets or sets the available page size options for the page size selector.
    /// </summary>
    /// <value>
    /// An array of integers representing available page sizes. Default is [10, 20, 50, 100].
    /// </value>
    public int[] PageSizes { get; set; } = [10, 20, 50, 100];

    /// <summary>
    /// Gets or sets the callback invoked when the current page changes.
    /// </summary>
    /// <value>
    /// A function that accepts the new page number and returns a Task, or <c>null</c> if no callback is set.
    /// </value>
    public Func<int, Task>? OnPageChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the page size changes.
    /// </summary>
    /// <value>
    /// A function that accepts the new page size and returns a Task, or <c>null</c> if no callback is set.
    /// </value>
    public Func<int, Task>? OnPageSizeChanged { get; set; }
}
