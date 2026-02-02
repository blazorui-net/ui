namespace BlazorBlueprint.Components.FileUpload;

/// <summary>
/// Represents a file validation error.
/// </summary>
public class FileValidationError
{
    /// <summary>
    /// Gets or sets the file name.
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Gets or sets the error type.
    /// </summary>
    public required FileValidationErrorType ErrorType { get; init; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public required string Message { get; init; }
}

/// <summary>
/// Types of file validation errors.
/// </summary>
public enum FileValidationErrorType
{
    /// <summary>
    /// File exceeds the maximum size limit.
    /// </summary>
    FileTooLarge,

    /// <summary>
    /// File type is not accepted.
    /// </summary>
    InvalidType,

    /// <summary>
    /// Too many files selected.
    /// </summary>
    TooManyFiles
}
