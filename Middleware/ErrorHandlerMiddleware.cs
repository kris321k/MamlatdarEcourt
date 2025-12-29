namespace MyApp.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleError(context, ex);
        }
    }

    private Task HandleError(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";

        return context.Response.WriteAsJsonAsync(new
        {
            error = ex.Message
        });
    }
}
