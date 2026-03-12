using Scim_v1.Models;

public class ScimAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly List<ScimClient> _clients;
    private readonly ILogger<ScimAuthMiddleware> _logger;

    public ScimAuthMiddleware(RequestDelegate next, IConfiguration config, ILogger<ScimAuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _clients = config.GetSection("Scim:Clients").Get<List<ScimClient>>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/scim"))
        {
            await _next(context);
            return;
        }
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            _logger.LogWarning("Token yok - IP: {IP}", context.Connection.RemoteIpAddress);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Token yok" });
            return;
        }

        var gelenToken = authHeader.Substring("Bearer ".Length).Trim();

        var client = _clients.FirstOrDefault(c => c.Token == gelenToken);

        if (client == null)
        {
            _logger.LogWarning("Geçersiz token - IP: {IP}", context.Connection.RemoteIpAddress);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Token geçersiz" });
            return;
        }

        _logger.LogInformation("{Client} bağlandı - {Time}", client.Name, DateTime.UtcNow);
        context.Items["ScimClient"] = client.Name;

        await _next(context);
    }
}