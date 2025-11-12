using MvcUi.AuthorizationAttributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedServicesLibrary.AppSettings;

namespace MvcUi.Controllers
{
    public class ErrorController : Controller
    {
        private readonly GlobalSettings _globalSettings;
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(IOptions<GlobalSettings> globalSettings,
                                ILogger<ErrorController> logger)
        {
            _globalSettings = globalSettings.Value;
            _logger = logger;
        }

        [AllowRoles("BiisAdmin", "Admin", "Staff", "Student")]
        [Route("Error/400")]
        public IActionResult BadRequestPage()
        {
            // Log a warning when a 400 Bad Request occurs, including the request path and query string for context
            _logger.LogWarning("400 Bad Request encountered. Path: {Path}, QueryString: {QueryString}",
                HttpContext.Request.Path, HttpContext.Request.QueryString);

            ViewData["AppName"] = _globalSettings.MyGlobalName;
            return View("BadRequest");
        }

        [AllowRoles("BiisAdmin", "Admin", "Staff", "Student")]
        [Route("Error/404")]
        public IActionResult NotFoundPage()
        {
            // Log a warning when a 404 Not Found error occurs, capturing the requested path and query string
            _logger.LogWarning("404 Not Found encountered. Path: {Path}, QueryString: {QueryString}",
                HttpContext.Request.Path, HttpContext.Request.QueryString);

            ViewData["AppName"] = _globalSettings.MyGlobalName;
            return View("NotFound");
        }

        [AllowRoles("BiisAdmin", "Admin", "Staff", "Student")]
        [Route("Error/500")]
        public IActionResult ServerError()
        {
            // For 500 Server Errors, attempt to retrieve the exception details and log the error with stack trace.
            // If no exception info is available, log a generic server error with request details.
            var exceptionFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
            if (exceptionFeature?.Error != null)
            {
                _logger.LogError(exceptionFeature.Error,
                    "500 Server Error encountered. Path: {Path}, QueryString: {QueryString}",
                    HttpContext.Request.Path, HttpContext.Request.QueryString);
            }
            else
            {
                _logger.LogError("500 Server Error encountered without exception details. Path: {Path}, QueryString: {QueryString}",
                    HttpContext.Request.Path, HttpContext.Request.QueryString);
            }

            ViewData["AppName"] = _globalSettings.MyGlobalName;
            return View("ServerError");
        }

        [AllowRoles("BiisAdmin", "Admin", "Staff", "Student")]
        [Route("Error/{statusCode}")]
        public IActionResult GenericError(int statusCode)
        {
            // Log a warning for any other generic HTTP errors with status code, path, and query string details
            _logger.LogWarning("Generic error with status code {StatusCode} encountered. Path: {Path}, QueryString: {QueryString}",
                statusCode, HttpContext.Request.Path, HttpContext.Request.QueryString);

            ViewData["AppName"] = _globalSettings.MyGlobalName;
            return View("GenericError", statusCode);
        }
    }
}
