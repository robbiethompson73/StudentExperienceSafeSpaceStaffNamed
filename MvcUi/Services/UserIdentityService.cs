using AyrshireCollege.Biis.UserIdentityLibrary.Interfaces;

namespace MvcUi.Services
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserIdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserIdentityFromHttpContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.User?.Identity?.Name ?? string.Empty;

            // In future (Azure AD), might change to:
            // return httpContext.User.FindFirst(ClaimTypes.Upn)?.Value;
        }
    }
}
