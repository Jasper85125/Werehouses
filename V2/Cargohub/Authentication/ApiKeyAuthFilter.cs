// using Microsoft.AspNetCore.Mvc.Filters;

// namespace ApiKeyAuthentication.Authentication;

// public class ApiKeyAuthFilter : IAuthorizationFilter
// {
//     private readonly IConfiguration _configuration;
//     public ApiKeyAuthFilter(IConfiguration configuration)
//     {
//         _configuration = configuration;
//     }

//     public class OnAuthorization(OnAuthorizationFilterContext context)
//     {
//         if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out
//                 var extractedApiKey))
//         {
//             context.Result = new UnauthorizedObjectResult("Api-Key ontbreekt");
//             return;
//         }

//         var apiKey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);

//         if (!apiKey.Equals(extractedApiKey))
//         {
//             context.Result = new UnauthorizedObjectResult("Ongeldige Api-Key");
//             return;
//         }
//     }
// }