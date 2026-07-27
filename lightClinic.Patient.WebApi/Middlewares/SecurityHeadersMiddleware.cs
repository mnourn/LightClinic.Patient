namespace lightClinic.Patient.WebApi.Middlewares;

public class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;

        // Prevent clickjacking
        headers["X-Frame-Options"] = "DENY";

        // Prevent MIME-type sniffing
        headers["X-Content-Type-Options"] = "nosniff";

        // Control referrer info sent to other sites
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // Force HTTPS for 1 year on HTTPS requests.
        if (context.Request.IsHttps)
            headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";

        // Content Security Policy — tighten based on your frontend needs
        headers["Content-Security-Policy"] =
            "default-src 'self'; " +
            "script-src 'self'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data:; " +
            "frame-ancestors 'none';";

        // Remove server info leakage
        headers.Remove("Server");
        headers.Remove("X-Powered-By");

        await next(context);
    }
}
