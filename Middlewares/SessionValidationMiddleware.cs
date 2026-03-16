using Microsoft.AspNetCore.Http;
using RecetasAPINet.Services;
using System.Security.Claims;

namespace RecetasAPINet.Middlewares
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var sessionIdClaim = context.User.FindFirst("sid")?.Value;
                if (Guid.TryParse(sessionIdClaim, out var sessionId))
                {
                    var isValid = await authService.ValidateSessionAsync(sessionId);
                    if (!isValid)
                    {
                        // Limpiar cookie
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.None,
                            Expires = DateTime.UtcNow.AddDays(-1)
                        };
                        context.Response.Cookies.Append("auth_token", "", cookieOptions);
                        
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Sesión revocada o expirada");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
