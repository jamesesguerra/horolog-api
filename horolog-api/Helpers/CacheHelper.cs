namespace horolog_api.Helpers;

public static class CacheHelper
{
    internal static async ValueTask<object?> AddDayCache(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        context.HttpContext.Response.Headers["cache-control"] = "public,max-age=86400";
        var result = await next(context);
        return result;
    }
}