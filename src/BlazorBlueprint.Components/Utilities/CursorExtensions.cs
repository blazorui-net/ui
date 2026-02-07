namespace BlazorBlueprint.Components.Utilities;

/// <summary>
/// Extension methods for converting <see cref="CursorType"/> to Tailwind CSS class strings.
/// </summary>
public static class CursorExtensions
{
    /// <summary>
    /// Converts a <see cref="CursorType"/> value to its corresponding Tailwind CSS cursor class.
    /// </summary>
    /// <param name="cursor">The cursor type to convert.</param>
    /// <returns>The Tailwind CSS class string (e.g., <c>"cursor-pointer"</c>).</returns>
    public static string ToClass(this CursorType cursor) => cursor switch
    {
        CursorType.Default => "cursor-default",
        CursorType.Pointer => "cursor-pointer",
        CursorType.NotAllowed => "cursor-not-allowed",
        CursorType.Crosshair => "cursor-crosshair",
        CursorType.Grab => "cursor-grab",
        CursorType.Grabbing => "cursor-grabbing",
        CursorType.ColResize => "cursor-col-resize",
        CursorType.RowResize => "cursor-row-resize",
        CursorType.EResize => "cursor-e-resize",
        CursorType.WResize => "cursor-w-resize",
        CursorType.Wait => "cursor-wait",
        CursorType.Text => "cursor-text",
        CursorType.Move => "cursor-move",
        CursorType.Help => "cursor-help",
        CursorType.None => "cursor-none",
        CursorType.Auto => "cursor-auto",
        _ => "cursor-default"
    };
}
