using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SecureFileDelivery.API.Attributes;

// Attribute to require API key authentication on endpoints
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
{
    private const string ApiKeyHeaderName = "X-API-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Check if API key is provided in header
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "API Key is missing" });
            return;
        }

        // Get valid API keys from configuration
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var validApiKeys = configuration.GetSection("Security:ApiKeys").Get<string[]>() ?? Array.Empty<string>();

        // Validate API key
        if (!validApiKeys.Contains(extractedApiKey.ToString()))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Invalid API Key" });
            return;
        }

        // API key is valid, continue
        await next();
    }
}