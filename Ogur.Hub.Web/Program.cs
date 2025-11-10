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

app.Run();