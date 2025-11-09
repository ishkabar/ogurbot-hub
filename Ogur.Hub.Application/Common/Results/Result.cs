// File: Ogur.Hub.Application/Common/Results/Result.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Common.Results

namespace Ogur.Hub.Application.Common.Results;

/// <summary>
/// Represents the result of an operation.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; protected init; }

    /// <summary>
    /// Gets whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? Error { get; protected init; }

    /// <summary>
    /// Gets additional error details.
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; protected init; }

    protected Result(bool isSuccess, string? error, Dictionary<string, string[]>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">Error message.</param>
    /// <param name="errors">Validation errors.</param>
    public static Result Failure(string error, Dictionary<string, string[]>? errors = null) 
        => new(false, error, errors);
}

/// <summary>
/// Represents the result of an operation with a value.
/// </summary>
/// <typeparam name="T">Type of the value.</typeparam>
public class Result<T> : Result
{
    /// <summary>
    /// Gets the result value.
    /// </summary>
    public T? Value { get; private init; }

    private Result(bool isSuccess, T? value, string? error, Dictionary<string, string[]>? errors = null)
        : base(isSuccess, error, errors)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    /// <param name="value">Result value.</param>
    public static Result<T> Success(T value) => new(true, value, null);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">Error message.</param>
    /// <param name="errors">Validation errors.</param>
    public static new Result<T> Failure(string error, Dictionary<string, string[]>? errors = null) 
        => new(false, default, error, errors);
}