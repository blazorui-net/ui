namespace BlazorUI.Components.MaskedInput;

/// <summary>
/// Predefined mask patterns.
/// </summary>
public enum MaskPreset
{
    /// <summary>
    /// Custom mask pattern (use Mask parameter).
    /// </summary>
    Custom,

    /// <summary>
    /// US Phone: (999) 999-9999
    /// </summary>
    Phone,

    /// <summary>
    /// Social Security Number: 999-99-9999
    /// </summary>
    SSN,

    /// <summary>
    /// Credit Card: 9999 9999 9999 9999
    /// </summary>
    CreditCard,

    /// <summary>
    /// Date: 99/99/9999
    /// </summary>
    Date,

    /// <summary>
    /// Time: 99:99
    /// </summary>
    Time,

    /// <summary>
    /// 24-Hour Time with seconds: 99:99:99
    /// </summary>
    Time24,

    /// <summary>
    /// US ZIP Code: 99999 or 99999-9999
    /// </summary>
    ZipCode,

    /// <summary>
    /// Employer Identification Number: 99-9999999
    /// </summary>
    EIN
}
