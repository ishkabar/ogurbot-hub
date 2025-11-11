// File: Hub.Web/Program.cs
// Project: Hub.Web
// Namespace: N/A

using Ogur.Hub.Web.Middleware;
using Ogur.Hub.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Configure HttpClient for Hub API
var hubApiBaseUrl = builder.Configuration["HubApi:BaseUrl"] 
                    ?? throw new InvalidOperationException("HubApi:BaseUrl not configured");

builder.Services.AddHttpClient<IHubApiClient, HubApiClient>(client =>
{
    client.BaseAddress = new Uri(hubApiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add HttpClient for Traefik proxy
builder.Services.AddHttpClient();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".OgurHub.Session";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthenticationMiddleware();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
/*
app.MapFallback("/api/{**path}", async (HttpContext context, IHttpClientFactory httpClientFactory) =>
{
    var httpClient = httpClientFactory.CreateClient();
    var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
    
    var targetUrl = $"{apiBaseUrl}{context.Request.Path}{context.Request.QueryString}";
    
    var requestMessage = new HttpRequestMessage(
        new HttpMethod(context.Request.Method), 
        targetUrl);
    
    foreach (var header in context.Request.Headers)
    {
        if (header.Key.StartsWith("Content-", StringComparison.OrdinalIgnoreCase) ||
            header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }
        requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.AsEnumerable());
    }
    
    if (context.Request.ContentLength > 0)
    {
        var content = new StreamContent(context.Request.Body);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        requestMessage.Content = content;
    }
    
    // ResponseContentRead ładuje całą odpowiedź do bufora zamiast streamować
    var response = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead);

    context.Response.StatusCode = (int)response.StatusCode;

// Kopiuj tylko bezpieczne headery - pomiń Transfer-Encoding i Connection
    foreach (var header in response.Headers)
    {
        if (!header.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase) &&
            !header.Key.Equals("Connection", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Headers.TryAdd(header.Key, header.Value.ToArray());
        }
    }

    foreach (var header in response.Content.Headers)
    {
        if (!header.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase) &&
            !header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Headers.TryAdd(header.Key, header.Value.ToArray());
        }
    }

    if (response.StatusCode != System.Net.HttpStatusCode.NoContent && response.Content != null)
    {
        var content = await response.Content.ReadAsByteArrayAsync();
        await context.Response.Body.WriteAsync(content);
    }
});

*/
app.Run();