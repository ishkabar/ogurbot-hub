// File: Ogur.Hub.Web/Services/HubApiClient.cs
// Project: Ogur.Hub.Web
// Namespace: Ogur.Hub.Web.Services

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Ogur.Hub.Web.Services;

/// <summary>
/// HTTP client for Ogur Hub API.
/// </summary>
public sealed class HubApiClient : IHubApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HubApiClient> _logger;

    public HubApiClient(HttpClient httpClient, ILogger<HubApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        try
        {
            var request = new { username, password };
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Login failed: {StatusCode}", response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return null;
        }
    }

    public async Task<List<ApplicationDto>?> GetApplicationsAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("/api/applications");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get applications failed: {StatusCode}", response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ApplicationDto>>>();
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting applications");
            return null;
        }
    }

    public async Task<List<LicenseDto>?> GetLicensesAsync(string token, int? applicationId = null)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var url = applicationId.HasValue 
                ? $"/api/licenses?applicationId={applicationId}" 
                : "/api/licenses";
            
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get licenses failed: {StatusCode}", response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<LicenseDto>>>();
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting licenses");
            return null;
        }
    }

    public async Task<List<DeviceDto>?> GetDevicesAsync(string token, int? licenseId = null)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var url = licenseId.HasValue 
                ? $"/api/devices?licenseId={licenseId}" 
                : "/api/devices";
            
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get devices failed: {StatusCode}", response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<DeviceDto>>>();
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting devices");
            return null;
        }
    }

    public async Task<LicenseDto?> CreateLicenseAsync(string token, CreateLicenseRequest request)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync("/api/licenses", request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Create license failed: {StatusCode}", response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<LicenseDto>>();
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating license");
            return null;
        }
    }
}

internal record ApiResponse<T>(bool Success, T? Data, string? Error, DateTime Timestamp);