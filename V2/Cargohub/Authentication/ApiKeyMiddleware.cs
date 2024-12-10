namespace ApiKeyAuthentication.Authentication;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out
                var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Api-Key ontbreekt");
            return;
        }

        var apiKey = ApiKeyStorage.GetApiKeys().FirstOrDefault(k => k.Key == extractedApiKey);
        
        if (apiKey == null)
        {
            context.Response.StatusCode = 403; // Forbidden
            await context.Response.WriteAsync("Api-Key is ongeldig");
            return;
        } 

        context.Items["UserRole"] = apiKey.Role; // Attach role to the request
        context.Items["WarehouseID"] = apiKey.WarehouseID;
        await _next(context);  
    }
}