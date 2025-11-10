// File: Hub.Web/Controllers/TraefikController.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Web.Infrastructure;
using Ogur.Hub.Web.Models.ViewModels;
using System.Net.Http.Headers;
using System.Text;

namespace Ogur.Hub.Web.Controllers;

/// <summary>
/// Controller for Traefik dashboard integration
/// </summary>
public sealed class TraefikController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TraefikController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the TraefikController
    /// </summary>
    public TraefikController(
        IConfiguration configuration,
        ILogger<TraefikController> logger,
        IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Displays Traefik dashboard in iframe
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        var viewModel = new TraefikViewModel
        {
            Title = "Traefik Dashboard",
            Description = "Monitor reverse proxy and routing",
            Username = Username,
            IsAdmin = IsAdmin,
            DashboardUrl = "/Traefik/Proxy/",
            HasCredentials = false
        };

        return View(viewModel);
    }

    /// <summary>
    /// Proxies requests to Traefik dashboard with authentication
    /// </summary>
    [HttpGet]
    [Route("Traefik/Proxy/{**path}")]
    public async Task<IActionResult> Proxy(string? path)
    {
        var dashboardUrl = _configuration["Traefik:DashboardUrl"] ?? "http://localhost:8080";
        var username = _configuration["Traefik:Username"];
        var password = _configuration["Traefik:Password"];

        // Build target URL
        var baseUrl = dashboardUrl.TrimEnd('/');
        string targetPath;

        if (string.IsNullOrEmpty(path))
        {
            targetPath = "dashboard/";
        }
        else if (path.StartsWith("api/", StringComparison.OrdinalIgnoreCase))
        {
            targetPath = path;
        }
        else
        {
            targetPath = $"dashboard/{path}";
        }

        var targetUrl = $"{baseUrl}/{targetPath}";

        if (Request.QueryString.HasValue)
        {
            targetUrl += Request.QueryString.Value;
        }

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(30);

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            var authBytes = Encoding.UTF8.GetBytes($"{username}:{password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(authBytes));
        }

        try
        {
            var response = await client.GetAsync(targetUrl);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
            var content = await response.Content.ReadAsByteArrayAsync();

            if (contentType.Contains("text/html"))
            {
                var html = Encoding.UTF8.GetString(content);

                var injectionScript = @"
<script>
// Set theme IMMEDIATELY - Traefik uses 'traefik-dark' key with boolean value
(function() {
    const urlParams = new URLSearchParams(window.location.search);
    const urlTheme = urlParams.get('theme');
    
    if (urlTheme) {
        // Traefik uses 'traefik-dark' with boolean value
        const isDark = urlTheme === 'dark';
        localStorage.setItem('traefik-dark', isDark.toString());
        console.log('Set traefik-dark to:', isDark);
        
        // Remove theme from URL
        window.history.replaceState({}, '', window.location.pathname + window.location.hash);
    }
})();
</script>
<script>
(function() {
    window.addEventListener('message', function(event) {
        if (event.data && event.data.type === 'THEME_CHANGE') {
            const newTheme = event.data.theme;
            const currentIsDark = localStorage.getItem('traefik-dark') === 'true';
            const newIsDark = newTheme === 'dark';
            
            if (newIsDark !== currentIsDark) {
                console.log('Theme changing from', currentIsDark ? 'dark' : 'light', 'to', newTheme);
                window.location.href = window.location.pathname + '?theme=' + newTheme + window.location.hash;
            }
        }
    });
    
    if (window.parent !== window) {
        window.parent.postMessage({ type: 'TRAEFIK_READY' }, '*');
    }
    
    const originalFetch = window.fetch;
    window.fetch = function(url, options) {
        if (typeof url === 'string' && url.startsWith('/api/')) {
            url = '/Traefik/Proxy' + url;
        }
        return originalFetch(url, options);
    };
    
    const originalOpen = XMLHttpRequest.prototype.open;
    XMLHttpRequest.prototype.open = function(method, url, ...rest) {
        if (typeof url === 'string' && url.startsWith('/api/')) {
            url = '/Traefik/Proxy' + url;
        }
        return originalOpen.call(this, method, url, ...rest);
    };
})();
</script>";

                // Inject BEFORE everything - even before <html> if possible
                if (html.Contains("<!DOCTYPE") || html.Contains("<!doctype"))
                {
                    // Insert right after DOCTYPE
                    var doctypeEnd = html.IndexOf(">", html.IndexOf("<!DOCTYPE", StringComparison.OrdinalIgnoreCase)) +
                                     1;
                    html = html.Insert(doctypeEnd, injectionScript);
                }
                else if (html.Contains("<html"))
                {
                    // Insert right after <html>
                    var htmlTagEnd = html.IndexOf(">", html.IndexOf("<html", StringComparison.OrdinalIgnoreCase)) + 1;
                    html = html.Insert(htmlTagEnd, injectionScript);
                }
                else if (html.Contains("<head>"))
                {
                    html = html.Replace("<head>", "<head>" + injectionScript);
                }
                else
                {
                    html = injectionScript + html;
                }

                content = Encoding.UTF8.GetBytes(html);
            }

            var skipHeaders = new[]
                { "X-Frame-Options", "Content-Security-Policy", "Transfer-Encoding", "Content-Length" };

            foreach (var header in response.Headers)
            {
                if (!skipHeaders.Contains(header.Key, StringComparer.OrdinalIgnoreCase))
                {
                    Response.Headers.TryAdd(header.Key, header.Value.ToArray());
                }
            }

            foreach (var header in response.Content.Headers)
            {
                if (!skipHeaders.Contains(header.Key, StringComparer.OrdinalIgnoreCase))
                {
                    Response.Headers.TryAdd(header.Key, header.Value.ToArray());
                }
            }

            return File(content, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to proxy Traefik dashboard to {TargetUrl}", targetUrl);
            return StatusCode(500, "Failed to load Traefik dashboard");
        }
    }
}