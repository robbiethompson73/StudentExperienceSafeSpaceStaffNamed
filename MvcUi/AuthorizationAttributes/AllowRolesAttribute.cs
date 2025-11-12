using Microsoft.AspNetCore.Mvc;

namespace MvcUi.AuthorizationAttributes
{
    public class AllowRolesAttribute : TypeFilterAttribute
    {
        public AllowRolesAttribute(params string[] roles)
            : base(typeof(AllowRolesAuthorizationFilter))
        {
            // Pass roles as arguments to the filter
            Arguments = new object[] { roles };
        }
    }
}
