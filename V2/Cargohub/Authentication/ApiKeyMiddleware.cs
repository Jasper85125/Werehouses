namespace ServicesV2;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ApiKeyStorage _apiKeyStorage; 


    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
        _apiKeyStorage = new ApiKeyStorage();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }
        
        if (!context.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out
                var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Api-Key ontbreekt");
            return;
        }

        var apiKey = _apiKeyStorage.GetApiKeys().FirstOrDefault(k => k.Key == extractedApiKey);
        
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