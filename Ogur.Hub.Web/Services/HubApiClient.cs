// File: Hub.Web/Services/HubApiClient.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Services

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Ogur.Hub.Web.Models.ViewModels;

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

    public async Task<List<UserDto>?> GetUsersAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("/api/users");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get users failed: {StatusCode}", response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserDto>>>();
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
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
    
    public async Task<List<VpsContainerDto>?> GetContainersAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("/api/vps/containers");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get containers failed: {StatusCode}", response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<List<VpsContainerDto>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting containers");
            return null;
        }
    }

    public async Task<List<VpsWebsiteDto>?> GetWebsitesAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("/api/vps/websites");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get websites failed: {StatusCode}", response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<List<VpsWebsiteDto>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting websites");
            return null;
        }
    }

    public async Task<VpsResourceDto?> GetCurrentResourcesAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("/api/vps/resources/current");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get current resources failed: {StatusCode}", response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<VpsResourceDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current resources");
            return null;
        }
    }

    public async Task<List<VpsResourceDto>?> GetResourceHistoryAsync(string token, DateTime from, DateTime to)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"/api/vps/resources/history?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get resource history failed: {StatusCode}", response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<List<VpsResourceDto>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting resource history");
            return null;
        }
    }

    public async Task<bool> RefreshContainersAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync("/api/vps/containers/refresh", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing containers");
            return false;
        }
    }

    public async Task<bool> RefreshWebsitesAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync("/api/vps/websites/refresh", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing websites");
            return false;
        }
    }
    
    public async Task<DashboardStatsDto?> GetDashboardStatsAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("/api/dashboard/stats");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get dashboard stats failed: {StatusCode}", response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<DashboardStatsDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return null;
        }
    }
}

internal record ApiResponse<T>(bool Success, T? Data, string? Error, DateTime Timestamp);