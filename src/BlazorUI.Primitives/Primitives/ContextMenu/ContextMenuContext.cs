using BlazorUI.Primitives.Contexts;

namespace BlazorUI.Primitives.ContextMenu;

/// <summary>
/// State for the ContextMenu primitive context.
/// </summary>
public class ContextMenuState
{
    /// <summary>
    /// Gets or sets whether the context menu is open.
    /// </summary>
    public bool IsOpen { get; set; } = false;

    /// <summary>
    /// Gets or sets the X coordinate of the context menu.
    /// </summary>
    public double X { get; set; } = 0;

    /// <summary>
    /// Gets or sets the Y coordinate of the context menu.
    /// </summary>
    public double Y { get; set; } = 0;
}

/// <summary>
/// Context for ContextMenu primitive component and its children.
/// Manages open state and position. Navigation is handled by ContextMenuContent.
/// </summary>
public class ContextMenuContext : PrimitiveContextWithEvents<ContextMenuState>
{
    /// <summary>
    /// Initializes a new instance of the ContextMenuContext.
    /// </summary>
    public ContextMenuContext() : base(new ContextMenuState(), "ctx-menu")
    {
    }

    /// <summary>
    /// Gets the ID for the context menu trigger.
    /// </summary>
    public string TriggerId => GetScopedId("trigger");

    /// <summary>
    /// Gets the ID for the context menu content.
    /// </summary>
    public string ContentId => GetScopedId("content");

    /// <summary>
    /// Gets whether the context menu is open.
    /// </summary>
    public bool IsOpen => State.IsOpen;

    /// <summary>
    /// Gets the X position of the context menu.
    /// </summary>
    public double X => State.X;

    /// <summary>
    /// Gets the Y position of the context menu.
    /// </summary>
    public double Y => State.Y;

    /// <summary>
    /// Gets or sets the currently active submenu context.
    /// Used to close submenus when hovering over other items.
    /// </summary>
    public ContextMenuSubContext? ActiveSubMenu { get; set; }

    /// <summary>
    /// Opens the context menu at the specified position.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    public void Open(double x, double y)
    {
        UpdateState(state =>
        {
            state.IsOpen = true;
            state.X = x;
            state.Y = y;
        });
    }

    /// <summary>
    /// Closes the context menu.
    /// </summary>
    public void Close()
    {
        // Close any active submenu first
        ActiveSubMenu?.Close();
        ActiveSubMenu = null;

        UpdateState(state =>
        {
            state.IsOpen = false;
        });
    }

    /// <summary>
    /// Closes any active submenu. Called when hovering over sibling menu items.
    /// </summary>
    public void CloseActiveSubMenu()
    {
        if (ActiveSubMenu != null)
        {
            ActiveSubMenu.Close();
            ActiveSubMenu = null;
        }
    }
}

/// <summary>
/// Interface for context menu items that support keyboard navigation.
/// </summary>
public interface IContextMenuItem
{
    /// <summary>
    /// Gets whether the item is disabled.
    /// </summary>
    bool Disabled { get; }

    /// <summary>
    /// Focuses the item.
    /// </summary>
    Task FocusAsync();
}
