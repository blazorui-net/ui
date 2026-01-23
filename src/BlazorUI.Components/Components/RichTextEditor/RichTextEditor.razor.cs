using System.Text.Json;
using BlazorUI.Components.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorUI.Components.RichTextEditor;

/// <summary>
/// A rich text editor component built on Quill.js that follows the shadcn/ui design system.
/// </summary>
public partial class RichTextEditor : ComponentBase, IAsyncDisposable
{
    // === Private Fields ===
    private ElementReference _editorRef;
    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<RichTextEditor>? _dotNetRef;
    private string _editorId = Guid.NewGuid().ToString("N");
    private bool _jsInitialized;
    private string? _lastKnownValue;
    private bool _pendingValueUpdate;

    // === Format State Tracking ===
    private bool _isBold;
    private bool _isItalic;
    private bool _isUnderline;
    private bool _isStrike;
    private bool _isBulletList;
    private bool _isOrderedList;
    private bool _isBlockquote;
    private bool _isCodeBlock;
    private string _headerLevel = "";

    // === Parameters - Value Binding ===

    /// <summary>
    /// Gets or sets the HTML content of the editor.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the editor content changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the Delta (JSON) representation of the editor content.
    /// </summary>
    [Parameter]
    public string? DeltaValue { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the Delta content changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> DeltaValueChanged { get; set; }

    // === Parameters - Toolbar ===

    /// <summary>
    /// Gets or sets the toolbar preset configuration.
    /// </summary>
    [Parameter]
    public ToolbarPreset Toolbar { get; set; } = ToolbarPreset.Standard;

    /// <summary>
    /// Gets or sets custom toolbar content.
    /// </summary>
    [Parameter]
    public RenderFragment? ToolbarContent { get; set; }

    // === Parameters - Appearance ===

    /// <summary>
    /// Gets or sets the placeholder text displayed when the editor is empty.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the minimum height of the editor.
    /// </summary>
    [Parameter]
    public string MinHeight { get; set; } = "150px";

    /// <summary>
    /// Gets or sets the maximum height of the editor. Content will scroll when exceeded.
    /// </summary>
    [Parameter]
    public string? MaxHeight { get; set; }

    /// <summary>
    /// Gets or sets a fixed height for the editor.
    /// </summary>
    [Parameter]
    public string? Height { get; set; }

    /// <summary>
    /// Gets or sets additional CSS classes for the container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the HTML id attribute for the editor container.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    // === Parameters - State ===

    /// <summary>
    /// Gets or sets whether the editor is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets whether the editor is read-only.
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }

    // === Parameters - Accessibility ===

    /// <summary>
    /// Gets or sets the ARIA label for the editor.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Gets or sets the ID of the element that describes the editor.
    /// </summary>
    [Parameter]
    public string? AriaDescribedBy { get; set; }

    /// <summary>
    /// Gets or sets whether the editor value is invalid.
    /// </summary>
    [Parameter]
    public bool? AriaInvalid { get; set; }

    // === Parameters - Events ===

    /// <summary>
    /// Gets or sets the callback invoked when the editor content changes.
    /// </summary>
    [Parameter]
    public EventCallback<TextChangeEventArgs> OnTextChange { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the selection changes.
    /// </summary>
    [Parameter]
    public EventCallback<SelectionChangeEventArgs> OnSelectionChange { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the editor gains focus.
    /// </summary>
    [Parameter]
    public EventCallback OnFocus { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the editor loses focus.
    /// </summary>
    [Parameter]
    public EventCallback OnBlur { get; set; }

    // === Lifecycle Methods ===

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InitializeJsAsync();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        // If Value changed externally, update the editor
        if (_jsInitialized && Value != _lastKnownValue && !_pendingValueUpdate)
        {
            _pendingValueUpdate = true;
            await SetHtmlAsync(Value);
            _lastKnownValue = Value;
            _pendingValueUpdate = false;
        }
    }

    private async Task InitializeJsAsync()
    {
        if (_jsInitialized) return;

        try
        {
            _jsModule = await JS.InvokeAsync<IJSObjectReference>("import",
                "./_content/BlazorUI.Components/js/quill-interop.js");
            _dotNetRef = DotNetObjectReference.Create(this);

            var options = BuildEditorOptions();
            await _jsModule.InvokeVoidAsync("initializeEditor",
                _editorRef, _dotNetRef, _editorId, options);
            _jsInitialized = true;

            // Set initial content
            if (!string.IsNullOrEmpty(Value))
            {
                await _jsModule.InvokeVoidAsync("setHtml", _editorId, Value);
                _lastKnownValue = Value;
            }

            // Apply disabled state
            if (Disabled)
            {
                await _jsModule.InvokeVoidAsync("disable", _editorId);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to initialize RichTextEditor JS: {ex.Message}");
        }
    }

    // === JSInvokable Callbacks ===

    [JSInvokable]
    public async Task OnTextChangeCallback(TextChangeEventArgs args)
    {
        _lastKnownValue = args.Html;
        Value = args.Html;
        await ValueChanged.InvokeAsync(args.Html);

        DeltaValue = args.Delta;
        await DeltaValueChanged.InvokeAsync(args.Delta);

        await OnTextChange.InvokeAsync(args);
    }

    [JSInvokable]
    public async Task OnSelectionChangeCallback(SelectionChangeEventArgs args)
    {
        // Update format state from selection
        if (args.Format != null)
        {
            UpdateFormatState(args.Format);
        }

        // Detect focus/blur from selection (null range = lost focus)
        if (args.Range == null && args.OldRange != null)
        {
            await OnBlur.InvokeAsync();
        }
        else if (args.Range != null && args.OldRange == null)
        {
            await OnFocus.InvokeAsync();
        }

        await OnSelectionChange.InvokeAsync(args);
    }

    private static bool GetFormatBool(Dictionary<string, object?> format, string key)
    {
        if (!format.TryGetValue(key, out var value) || value == null)
            return false;

        if (value is bool b)
            return b;

        if (value is JsonElement je && je.ValueKind == JsonValueKind.True)
            return true;

        return false;
    }

    private static string GetFormatString(Dictionary<string, object?> format, string key)
    {
        if (!format.TryGetValue(key, out var value) || value == null)
            return "";

        if (value is string s)
            return s;

        if (value is JsonElement je)
        {
            if (je.ValueKind == JsonValueKind.String)
                return je.GetString() ?? "";
            if (je.ValueKind == JsonValueKind.Number)
                return je.GetInt32().ToString();
        }

        return value.ToString() ?? "";
    }

    // === Toolbar Actions ===

    private async Task ToggleFormatAsync(string format, object? value = null)
    {
        if (_jsModule == null || !_jsInitialized || Disabled) return;

        // Toggle: if already active, remove; otherwise apply
        bool isActive = format switch
        {
            "bold" => _isBold,
            "italic" => _isItalic,
            "underline" => _isUnderline,
            "strike" => _isStrike,
            "blockquote" => _isBlockquote,
            "code-block" => _isCodeBlock,
            "list" when value?.ToString() == "bullet" => _isBulletList,
            "list" when value?.ToString() == "ordered" => _isOrderedList,
            _ => false
        };

        var newValue = isActive ? false : (value ?? true);

        // Use formatAndGetState for all formats to ensure immediate state sync
        var formatState = await _jsModule.InvokeAsync<Dictionary<string, object?>>(
            "formatAndGetState", _editorId, format, newValue);
        UpdateFormatState(formatState);

        // Refocus the editor after toolbar button click
        await _jsModule.InvokeVoidAsync("focus", _editorId);
    }

    private void UpdateFormatState(Dictionary<string, object?> format)
    {
        if (format == null) return;

        _isBold = GetFormatBool(format, "bold");
        _isItalic = GetFormatBool(format, "italic");
        _isUnderline = GetFormatBool(format, "underline");
        _isStrike = GetFormatBool(format, "strike");
        _isBlockquote = GetFormatBool(format, "blockquote");
        _isCodeBlock = GetFormatBool(format, "code-block");

        var listValue = GetFormatString(format, "list");
        _isBulletList = listValue == "bullet";
        _isOrderedList = listValue == "ordered";

        _headerLevel = GetFormatString(format, "header");

        StateHasChanged();
    }

    private async Task HandleHeaderChange(ChangeEventArgs e)
    {
        if (_jsModule == null || !_jsInitialized || Disabled) return;

        var value = e.Value?.ToString();
        if (string.IsNullOrEmpty(value))
        {
            await _jsModule.InvokeVoidAsync("format", _editorId, "header", false);
        }
        else
        {
            await _jsModule.InvokeVoidAsync("format", _editorId, "header", int.Parse(value));
        }

        // Refocus the editor after dropdown change
        await _jsModule.InvokeVoidAsync("focus", _editorId);
    }

    private async Task InsertLinkAsync()
    {
        if (_jsModule == null || !_jsInitialized || Disabled) return;

        // Use the JS prompt to get URL from user
        await _jsModule.InvokeVoidAsync("promptLink", _editorId);
        await _jsModule.InvokeVoidAsync("focus", _editorId);
    }

    // === Public API Methods ===

    /// <summary>
    /// Focuses the editor.
    /// </summary>
    public async Task FocusAsync()
    {
        if (_jsModule != null && _jsInitialized)
        {
            await _jsModule.InvokeVoidAsync("focus", _editorId);
        }
    }

    /// <summary>
    /// Removes focus from the editor.
    /// </summary>
    public async Task BlurAsync()
    {
        if (_jsModule != null && _jsInitialized)
        {
            await _jsModule.InvokeVoidAsync("blur", _editorId);
        }
    }

    /// <summary>
    /// Gets the current selection range.
    /// </summary>
    public async Task<EditorRange?> GetSelectionAsync()
    {
        if (_jsModule != null && _jsInitialized)
        {
            return await _jsModule.InvokeAsync<EditorRange?>("getSelection", _editorId);
        }
        return null;
    }

    /// <summary>
    /// Sets the selection range.
    /// </summary>
    public async Task SetSelectionAsync(int index, int length = 0)
    {
        if (_jsModule != null && _jsInitialized)
        {
            await _jsModule.InvokeVoidAsync("setSelection", _editorId, index, length);
        }
    }

    /// <summary>
    /// Applies formatting to the current selection.
    /// </summary>
    public async Task FormatAsync(string formatName, object? value = null)
    {
        if (_jsModule != null && _jsInitialized)
        {
            await _jsModule.InvokeVoidAsync("format", _editorId, formatName, value ?? true);
        }
    }

    /// <summary>
    /// Gets the plain text content of the editor.
    /// </summary>
    public async Task<string> GetTextAsync()
    {
        if (_jsModule != null && _jsInitialized)
        {
            return await _jsModule.InvokeAsync<string>("getText", _editorId) ?? "";
        }
        return "";
    }

    /// <summary>
    /// Gets the length of the editor content.
    /// </summary>
    public async Task<int> GetLengthAsync()
    {
        if (_jsModule != null && _jsInitialized)
        {
            return await _jsModule.InvokeAsync<int>("getLength", _editorId);
        }
        return 0;
    }

    /// <summary>
    /// Gets the HTML content of the editor.
    /// </summary>
    public async Task<string> GetHtmlAsync()
    {
        if (_jsModule != null && _jsInitialized)
        {
            return await _jsModule.InvokeAsync<string>("getHtml", _editorId) ?? "";
        }
        return "";
    }

    /// <summary>
    /// Sets the HTML content of the editor.
    /// </summary>
    public async Task SetHtmlAsync(string? html)
    {
        if (_jsModule != null && _jsInitialized)
        {
            await _jsModule.InvokeVoidAsync("setHtml", _editorId, html ?? "");
        }
    }

    // === Private Helper Methods ===

    private object BuildEditorOptions() => new
    {
        placeholder = Placeholder ?? "",
        readOnly = Disabled || ReadOnly
    };

    // === CSS Classes ===

    private string ContainerCssClass => ClassNames.cn(
        "flex flex-col rounded-md border border-input bg-background",
        "focus-within:border-ring focus-within:ring-[3px] focus-within:ring-ring/50",
        ClassNames.when(AriaInvalid == true, "border-destructive ring-destructive/20"),
        ClassNames.when(Disabled, "opacity-50 cursor-not-allowed"),
        Class
    );

    private string ToolbarCssClass => ClassNames.cn(
        "flex flex-wrap items-center gap-1 px-3 py-2 border-b border-input bg-muted/40"
    );

    private string EditorCssClass => ClassNames.cn(
        "text-base md:text-sm",
        ClassNames.when(Disabled, "cursor-not-allowed")
    );

    private string EditorStyle
    {
        get
        {
            var styles = new List<string>();

            if (!string.IsNullOrEmpty(Height))
            {
                styles.Add($"height: {Height}");
                styles.Add("overflow-y: auto");
            }
            else
            {
                styles.Add($"min-height: {MinHeight}");
                if (!string.IsNullOrEmpty(MaxHeight))
                {
                    styles.Add($"max-height: {MaxHeight}");
                    styles.Add("overflow-y: auto");
                }
            }

            return string.Join("; ", styles);
        }
    }

    private string ToolbarButtonCssClass => ClassNames.cn(
        "inline-flex items-center justify-center rounded-md h-8 w-8",
        "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
        "transition-colors disabled:opacity-50 disabled:pointer-events-none"
    );

    private string GetButtonCssClass(bool isActive) => ClassNames.cn(
        ToolbarButtonCssClass,
        ClassNames.when(isActive, "bg-accent text-accent-foreground border border-ring")
    );

    private string DropdownCssClass => ClassNames.cn(
        "h-8 px-2 rounded-md border border-input bg-background text-sm",
        "focus:outline-none focus:ring-2 focus:ring-ring",
        "disabled:opacity-50 disabled:pointer-events-none"
    );

    // === Dispose ===

    public async ValueTask DisposeAsync()
    {
        if (_jsModule != null && _jsInitialized)
        {
            try
            {
                await _jsModule.InvokeVoidAsync("disposeEditor", _editorId);
                await _jsModule.DisposeAsync();
            }
            catch { }
        }
        _dotNetRef?.Dispose();
    }
}
