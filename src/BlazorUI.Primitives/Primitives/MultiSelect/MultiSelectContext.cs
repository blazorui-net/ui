using Microsoft.AspNetCore.Components;
using BlazorUI.Primitives.Contexts;

namespace BlazorUI.Primitives.MultiSelect;

/// <summary>
/// Represents the possible states for the Select All checkbox.
/// </summary>
public enum SelectAllState
{
    /// <summary>No items are selected.</summary>
    None,
    /// <summary>Some items are selected (indeterminate state).</summary>
    Indeterminate,
    /// <summary>All items are selected.</summary>
    All
}

/// <summary>
/// State for the MultiSelect primitive context.
/// </summary>
public class MultiSelectState
{
    /// <summary>
    /// Gets or sets whether the dropdown is open.
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// Gets or sets the list of selected values.
    /// </summary>
    public List<string> SelectedValues { get; set; } = new();

    /// <summary>
    /// Gets or sets the current search query.
    /// </summary>
    public string SearchQuery { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the index of the currently focused item for keyboard navigation.
    /// </summary>
    public int FocusedIndex { get; set; } = -1;

    /// <summary>
    /// Gets or sets whether the multiselect is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the trigger element reference for positioning.
    /// </summary>
    public ElementReference? TriggerElement { get; set; }

    /// <summary>
    /// Gets or sets the input element reference for focus management.
    /// </summary>
    public ElementReference? InputElement { get; set; }
}

/// <summary>
/// Metadata for a registered multiselect item.
/// </summary>
public class MultiSelectItemMetadata
{
    /// <summary>
    /// Gets or sets the value associated with this item.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the text used for search filtering.
    /// </summary>
    public string? SearchText { get; set; }

    /// <summary>
    /// Gets or sets the display text for this item.
    /// </summary>
    public string? DisplayText { get; set; }

    /// <summary>
    /// Gets or sets whether this item is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when this item is toggled.
    /// </summary>
    public EventCallback OnToggle { get; set; }
}

/// <summary>
/// Context for MultiSelect primitive component and its children.
/// Manages multiselect state, item registration, selection tracking, and keyboard navigation.
/// </summary>
public class MultiSelectContext : PrimitiveContextWithEvents<MultiSelectState>
{
    /// <summary>
    /// Callback invoked when the search query changes.
    /// </summary>
    public Action<string>? OnSearchQueryChange { get; set; }

    /// <summary>
    /// Callback invoked when the selected values change.
    /// </summary>
    public Action<IReadOnlyList<string>>? OnValuesChange { get; set; }

    /// <summary>
    /// Callback invoked when the open state changes.
    /// </summary>
    public Action<bool>? OnOpenChange { get; set; }

    /// <summary>
    /// Custom filter function. If null, uses default Contains matching.
    /// </summary>
    public Func<MultiSelectItemMetadata, string, bool>? FilterFunction { get; set; }

    /// <summary>
    /// Maximum number of tags to display before showing "+N more".
    /// </summary>
    public int MaxDisplayTags { get; set; } = 3;

    /// <summary>
    /// Initializes a new instance of the MultiSelectContext.
    /// </summary>
    public MultiSelectContext() : base(new MultiSelectState(), "multiselect")
    {
    }

    /// <summary>
    /// Gets the ID for the multiselect input element.
    /// </summary>
    public string InputId => GetScopedId("input");

    /// <summary>
    /// Gets the ID for the multiselect content container.
    /// </summary>
    public string ContentId => GetScopedId("content");

    /// <summary>
    /// Gets the ID for the multiselect trigger element.
    /// </summary>
    public string TriggerId => GetScopedId("trigger");

    /// <summary>
    /// Gets whether the dropdown is open.
    /// </summary>
    public bool IsOpen => State.IsOpen;

    /// <summary>
    /// Gets the current search query.
    /// </summary>
    public string SearchQuery => State.SearchQuery;

    /// <summary>
    /// Gets the selected values.
    /// </summary>
    public IReadOnlyList<string> SelectedValues => State.SelectedValues;

    /// <summary>
    /// Gets the currently focused item index.
    /// </summary>
    public int FocusedIndex => State.FocusedIndex;

    /// <summary>
    /// Gets whether the multiselect is disabled.
    /// </summary>
    public bool Disabled => State.Disabled;

    /// <summary>
    /// Gets the number of selected items that exceed MaxDisplayTags.
    /// </summary>
    public int OverflowCount => Math.Max(0, State.SelectedValues.Count - MaxDisplayTags);

    /// <summary>
    /// Gets the list of registered items for keyboard navigation.
    /// </summary>
    private List<MultiSelectItemMetadata> Items { get; } = new();

    /// <summary>
    /// Cached list of filtered items to avoid recomputing on every access.
    /// </summary>
    private List<MultiSelectItemMetadata>? _cachedFilteredItems;

    #region Open/Close Management

    /// <summary>
    /// Opens the dropdown.
    /// </summary>
    /// <param name="triggerElement">The trigger element reference for positioning.</param>
    public void Open(ElementReference? triggerElement = null)
    {
        if (State.Disabled) return;

        UpdateState(state =>
        {
            state.IsOpen = true;
            state.TriggerElement = triggerElement;
            state.FocusedIndex = -1;
        });

        OnOpenChange?.Invoke(true);
    }

    /// <summary>
    /// Closes the dropdown.
    /// </summary>
    public void Close()
    {
        UpdateState(state =>
        {
            state.IsOpen = false;
            state.SearchQuery = string.Empty;
            state.FocusedIndex = -1;
        });

        InvalidateFilterCache();
        OnOpenChange?.Invoke(false);
    }

    /// <summary>
    /// Toggles the dropdown open/closed.
    /// </summary>
    /// <param name="triggerElement">The trigger element reference for positioning.</param>
    public void Toggle(ElementReference? triggerElement = null)
    {
        if (State.IsOpen)
        {
            Close();
        }
        else
        {
            Open(triggerElement);
        }
    }

    #endregion

    #region Selection Management

    /// <summary>
    /// Checks if a value is currently selected.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the value is selected, false otherwise.</returns>
    public bool IsSelected(string? value)
    {
        if (value == null) return false;
        return State.SelectedValues.Contains(value);
    }

    /// <summary>
    /// Toggles the selection state of a value.
    /// </summary>
    /// <param name="value">The value to toggle.</param>
    public void ToggleValue(string? value)
    {
        if (value == null || State.Disabled) return;

        var item = Items.FirstOrDefault(i => i.Value == value);
        if (item?.Disabled == true) return;

        UpdateState(state =>
        {
            if (state.SelectedValues.Contains(value))
            {
                state.SelectedValues.Remove(value);
            }
            else
            {
                state.SelectedValues.Add(value);
            }
        });

        OnValuesChange?.Invoke(State.SelectedValues);
    }

    /// <summary>
    /// Selects a value (adds to selection if not already selected).
    /// </summary>
    /// <param name="value">The value to select.</param>
    public void SelectValue(string? value)
    {
        if (value == null || State.Disabled) return;

        var item = Items.FirstOrDefault(i => i.Value == value);
        if (item?.Disabled == true) return;

        if (!State.SelectedValues.Contains(value))
        {
            UpdateState(state =>
            {
                state.SelectedValues.Add(value);
            });

            OnValuesChange?.Invoke(State.SelectedValues);
        }
    }

    /// <summary>
    /// Deselects a value (removes from selection).
    /// </summary>
    /// <param name="value">The value to deselect.</param>
    public void DeselectValue(string? value)
    {
        if (value == null) return;

        if (State.SelectedValues.Contains(value))
        {
            UpdateState(state =>
            {
                state.SelectedValues.Remove(value);
            });

            OnValuesChange?.Invoke(State.SelectedValues);
        }
    }

    /// <summary>
    /// Selects all enabled items.
    /// </summary>
    public void SelectAll()
    {
        if (State.Disabled) return;

        UpdateState(state =>
        {
            foreach (var item in Items.Where(i => !i.Disabled && i.Value != null))
            {
                if (!state.SelectedValues.Contains(item.Value!))
                {
                    state.SelectedValues.Add(item.Value!);
                }
            }
        });

        OnValuesChange?.Invoke(State.SelectedValues);
    }

    /// <summary>
    /// Deselects all items.
    /// </summary>
    public void DeselectAll()
    {
        if (State.SelectedValues.Count == 0) return;

        UpdateState(state =>
        {
            state.SelectedValues.Clear();
        });

        OnValuesChange?.Invoke(State.SelectedValues);
    }

    /// <summary>
    /// Toggles the select all state.
    /// If all are selected, deselects all. Otherwise, selects all.
    /// </summary>
    public void ToggleSelectAll()
    {
        var state = GetSelectAllState();
        if (state == SelectAllState.All)
        {
            DeselectAll();
        }
        else
        {
            SelectAll();
        }
    }

    /// <summary>
    /// Gets the current state of the Select All checkbox.
    /// </summary>
    /// <returns>The select all state (None, Indeterminate, or All).</returns>
    public SelectAllState GetSelectAllState()
    {
        var enabledItems = Items.Where(i => !i.Disabled && i.Value != null).ToList();
        if (enabledItems.Count == 0) return SelectAllState.None;

        var selectedCount = enabledItems.Count(i => IsSelected(i.Value));

        if (selectedCount == 0) return SelectAllState.None;
        if (selectedCount == enabledItems.Count) return SelectAllState.All;
        return SelectAllState.Indeterminate;
    }

    /// <summary>
    /// Sets the selected values (for controlled mode).
    /// </summary>
    /// <param name="values">The values to set as selected.</param>
    public void SetSelectedValues(IEnumerable<string>? values)
    {
        UpdateState(state =>
        {
            state.SelectedValues.Clear();
            if (values != null)
            {
                state.SelectedValues.AddRange(values);
            }
        });
    }

    #endregion

    #region Search/Filter Management

    /// <summary>
    /// Updates the search query and resets focus.
    /// </summary>
    /// <param name="query">The new search query.</param>
    public void UpdateSearchQuery(string query)
    {
        UpdateState(state =>
        {
            state.SearchQuery = query;
            state.FocusedIndex = -1;
        });

        InvalidateFilterCache();
        OnSearchQueryChange?.Invoke(query);
    }

    /// <summary>
    /// Clears the search query.
    /// </summary>
    public void ClearSearch()
    {
        UpdateSearchQuery(string.Empty);
    }

    /// <summary>
    /// Gets the list of filtered items based on the current search query.
    /// </summary>
    public List<MultiSelectItemMetadata> GetFilteredItems()
    {
        if (_cachedFilteredItems != null)
        {
            return _cachedFilteredItems;
        }

        if (string.IsNullOrWhiteSpace(State.SearchQuery))
        {
            _cachedFilteredItems = Items.ToList();
        }
        else if (FilterFunction != null)
        {
            _cachedFilteredItems = Items.Where(item =>
                FilterFunction(item, State.SearchQuery)
            ).ToList();
        }
        else
        {
            _cachedFilteredItems = Items.Where(item =>
                item.SearchText?.Contains(State.SearchQuery, StringComparison.OrdinalIgnoreCase) ?? false
            ).ToList();
        }

        return _cachedFilteredItems;
    }

    /// <summary>
    /// Invalidates the cached filtered items.
    /// </summary>
    private void InvalidateFilterCache()
    {
        _cachedFilteredItems = null;
    }

    #endregion

    #region Item Registration

    /// <summary>
    /// Gets an item by its index in the registered items list.
    /// </summary>
    /// <param name="index">The index of the item.</param>
    /// <returns>The item metadata, or null if the index is out of range.</returns>
    public MultiSelectItemMetadata? GetItemByIndex(int index)
    {
        if (index < 0 || index >= Items.Count)
            return null;
        return Items[index];
    }

    /// <summary>
    /// Gets an item by its value.
    /// </summary>
    /// <param name="value">The value of the item.</param>
    /// <returns>The item metadata, or null if not found.</returns>
    public MultiSelectItemMetadata? GetItemByValue(string? value)
    {
        if (value == null) return null;
        return Items.FirstOrDefault(i => i.Value == value);
    }

    /// <summary>
    /// Registers an item with the multiselect context for filtering and keyboard navigation.
    /// </summary>
    /// <param name="value">The value of the item.</param>
    /// <param name="searchText">The text used for filtering.</param>
    /// <param name="displayText">The display text for the item.</param>
    /// <param name="disabled">Whether the item is disabled.</param>
    /// <param name="onToggle">The callback to invoke when the item is toggled.</param>
    /// <returns>The index of the registered item.</returns>
    public int RegisterItem(string? value, string? searchText, string? displayText, bool disabled, EventCallback onToggle)
    {
        var metadata = new MultiSelectItemMetadata
        {
            Value = value,
            SearchText = searchText ?? displayText ?? value,
            DisplayText = displayText ?? value,
            Disabled = disabled,
            OnToggle = onToggle
        };
        Items.Add(metadata);
        InvalidateFilterCache();
        return Items.Count - 1;
    }

    /// <summary>
    /// Unregisters an item from the multiselect context.
    /// </summary>
    /// <param name="index">The index of the item to unregister.</param>
    public void UnregisterItem(int index)
    {
        if (index >= 0 && index < Items.Count)
        {
            Items.RemoveAt(index);
            InvalidateFilterCache();
        }
    }

    /// <summary>
    /// Clears all registered items.
    /// </summary>
    public void ClearItems()
    {
        Items.Clear();
        InvalidateFilterCache();
    }

    /// <summary>
    /// Gets an item ID for ARIA attributes.
    /// </summary>
    /// <param name="index">The index of the item.</param>
    public string GetItemId(int index) => GetScopedId($"item-{index}");

    #endregion

    #region Keyboard Navigation

    /// <summary>
    /// Sets the disabled state.
    /// </summary>
    /// <param name="disabled">Whether the multiselect is disabled.</param>
    public void SetDisabled(bool disabled)
    {
        UpdateState(state =>
        {
            state.Disabled = disabled;
        });
    }

    /// <summary>
    /// Sets the focused item index for keyboard navigation.
    /// </summary>
    /// <param name="index">The index of the item to focus.</param>
    public void SetFocusedIndex(int index)
    {
        UpdateState(state =>
        {
            state.FocusedIndex = index;
        });
    }

    /// <summary>
    /// Moves focus to the next or previous item that is not disabled.
    /// </summary>
    /// <param name="direction">1 for next, -1 for previous.</param>
    public void MoveFocus(int direction)
    {
        var filteredItems = GetFilteredItems();
        if (filteredItems.Count == 0) return;

        int startIndex = State.FocusedIndex;
        int newIndex = startIndex;

        // Find next non-disabled item
        do
        {
            newIndex += direction;

            // Wrap around
            if (newIndex < 0)
                newIndex = filteredItems.Count - 1;
            else if (newIndex >= filteredItems.Count)
                newIndex = 0;

            // Avoid infinite loop if all items are disabled
            if (newIndex == startIndex)
                break;

        } while (newIndex >= 0 && newIndex < filteredItems.Count && filteredItems[newIndex].Disabled);

        SetFocusedIndex(newIndex);
    }

    /// <summary>
    /// Moves focus to the first enabled item.
    /// </summary>
    public void FocusFirst()
    {
        var filteredItems = GetFilteredItems();
        for (int i = 0; i < filteredItems.Count; i++)
        {
            if (!filteredItems[i].Disabled)
            {
                SetFocusedIndex(i);
                return;
            }
        }
    }

    /// <summary>
    /// Moves focus to the last enabled item.
    /// </summary>
    public void FocusLast()
    {
        var filteredItems = GetFilteredItems();
        for (int i = filteredItems.Count - 1; i >= 0; i--)
        {
            if (!filteredItems[i].Disabled)
            {
                SetFocusedIndex(i);
                return;
            }
        }
    }

    /// <summary>
    /// Toggles the currently focused item.
    /// </summary>
    public async Task ToggleFocusedItem()
    {
        var filteredItems = GetFilteredItems();
        if (State.FocusedIndex >= 0 && State.FocusedIndex < filteredItems.Count)
        {
            var item = filteredItems[State.FocusedIndex];
            if (!item.Disabled && item.Value != null)
            {
                ToggleValue(item.Value);
                await item.OnToggle.InvokeAsync();
            }
        }
    }

    /// <summary>
    /// Toggles the focused item and closes the dropdown.
    /// </summary>
    public async Task ToggleFocusedItemAndClose()
    {
        await ToggleFocusedItem();
        Close();
    }

    #endregion
}
