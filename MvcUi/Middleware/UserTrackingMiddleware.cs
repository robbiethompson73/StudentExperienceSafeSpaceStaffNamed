using MvcUi.Services;

namespace MvcUi.Middleware
{
    public class UserTrackingMiddleware
    {
        private readonly RequestDelegate _next;

        public UserTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // This method is called for every HTTP request passing through the middleware pipeline.
        public async Task InvokeAsync(HttpContext context, UserTrackerService tracker)
        {
            // Determine a unique identifier for the user:
            // - If the user is authenticated, use their identity name (e.g., DOMAIN\username).
            // - Otherwise, fall back to their remote IP address.
            var userId = context.User.Identity?.IsAuthenticated == true
                ? context.User.Identity.Name
                : context.Connection.RemoteIpAddress?.ToString();

            // If a valid identifier was found, proceed with tracking.
            if (!string.IsNullOrEmpty(userId))
            {
                // Register the user as active.
                tracker.AddUser(userId);

                // When the response has finished, remove the user from the active list.
                // This ensures the user is only counted while the request is being handled.
                context.Response.OnCompleted(() =>
                {
                    tracker.RemoveUser(userId);
                    return Task.CompletedTask;
                });
            }

            // Pass control to the next middleware in the pipeline.
            await _next(context);
        }

    }
}
