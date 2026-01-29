namespace BlazorUI.Components.CurrencyInput;

/// <summary>
/// Defines the properties of a currency.
/// </summary>
public class CurrencyDefinition
{
    /// <summary>
    /// Gets or sets the ISO 4217 currency code.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Gets or sets the currency symbol.
    /// </summary>
    public required string Symbol { get; init; }

    /// <summary>
    /// Gets or sets the number of decimal places.
    /// </summary>
    public required int DecimalPlaces { get; init; }

    /// <summary>
    /// Gets or sets the culture name for formatting.
    /// </summary>
    public required string CultureName { get; init; }

    /// <summary>
    /// Gets or sets the currency name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets whether the symbol appears before the amount.
    /// </summary>
    public bool SymbolBefore { get; init; } = true;
}
