// File: Ogur.Hub.Infrastructure/Persistence/Converters/ApiKeyConverter.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Persistence.Converters

using Ogur.Hub.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ogur.Hub.Infrastructure.Persistence.Converters;

/// <summary>
/// EF Core value converter for ApiKey value object.
/// </summary>
public sealed class ApiKeyConverter : ValueConverter<ApiKey, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyConverter"/> class.
    /// </summary>
    public ApiKeyConverter()
        : base(
            apiKey => apiKey.HashedValue,
            value => ApiKey.CreateFromHash(value))
    {
    }
}