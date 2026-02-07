using System.Diagnostics.CodeAnalysis;

namespace BlazorBlueprint.Components.Utilities;

/// <summary>
/// Defines the cursor style to display when hovering over an element.
/// </summary>
/// <remarks>
/// Maps to standard CSS cursor values via the <see cref="CursorExtensions.ToClass"/> extension method.
/// Each value corresponds to a Tailwind CSS cursor utility class.
/// </remarks>
public enum CursorType
{
    /// <summary>
    /// The browser's default cursor, typically an arrow.
    /// Maps to <c>cursor-default</c>.
    /// </summary>
    Default,

    /// <summary>
    /// A pointing hand cursor indicating a clickable element.
    /// Maps to <c>cursor-pointer</c>.
    /// </summary>
    [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Pointer is the standard CSS cursor value name")]
    Pointer,

    /// <summary>
    /// A circle with a line through it, indicating the action is not allowed.
    /// Maps to <c>cursor-not-allowed</c>.
    /// </summary>
    NotAllowed,

    /// <summary>
    /// A crosshair cursor for precision selection.
    /// Maps to <c>cursor-crosshair</c>.
    /// </summary>
    Crosshair,

    /// <summary>
    /// An open hand cursor indicating an element can be grabbed.
    /// Maps to <c>cursor-grab</c>.
    /// </summary>
    Grab,

    /// <summary>
    /// A closed hand cursor indicating an element is being grabbed.
    /// Maps to <c>cursor-grabbing</c>.
    /// </summary>
    Grabbing,

    /// <summary>
    /// A column resize cursor for horizontal resizing.
    /// Maps to <c>cursor-col-resize</c>.
    /// </summary>
    ColResize,

    /// <summary>
    /// A row resize cursor for vertical resizing.
    /// Maps to <c>cursor-row-resize</c>.
    /// </summary>
    RowResize,

    /// <summary>
    /// An east-facing resize cursor.
    /// Maps to <c>cursor-e-resize</c>.
    /// </summary>
    EResize,

    /// <summary>
    /// A west-facing resize cursor.
    /// Maps to <c>cursor-w-resize</c>.
    /// </summary>
    WResize,

    /// <summary>
    /// A wait/loading cursor (typically a spinner or hourglass).
    /// Maps to <c>cursor-wait</c>.
    /// </summary>
    Wait,

    /// <summary>
    /// A text selection cursor (I-beam).
    /// Maps to <c>cursor-text</c>.
    /// </summary>
    Text,

    /// <summary>
    /// A move cursor indicating the element can be moved.
    /// Maps to <c>cursor-move</c>.
    /// </summary>
    Move,

    /// <summary>
    /// A help cursor (typically a question mark).
    /// Maps to <c>cursor-help</c>.
    /// </summary>
    Help,

    /// <summary>
    /// Hides the cursor entirely.
    /// Maps to <c>cursor-none</c>.
    /// </summary>
    None,

    /// <summary>
    /// The browser determines the cursor based on context.
    /// Maps to <c>cursor-auto</c>.
    /// </summary>
    Auto
}
