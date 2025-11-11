namespace Ogur.Hub.Web.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _publicPaths = new[] 
        { 
            "/account/login", 
            "/account/register",
            "/css/",
            "/js/",
            "/lib/",
            "/favicon.ico"
        };

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";
    
            // Check if path is public
            bool isPublicPath = _publicPaths.Any(p => path.StartsWith(p));
    
            if (context.Request.Method == "POST" && path == "/account/login")
            {
                await _next(context);
                return;
            }
    
            if (!isPublicPath)
            {
                var authToken = context.Session.GetString("AuthToken");
        
                if (string.IsNullOrEmpty(authToken))
                {
                    context.Response.Redirect("/Account/Login");
                    return;
                }
            }
            
            await _next(context);
        }
    }

    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
