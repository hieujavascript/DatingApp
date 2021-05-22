using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string getUserName(this ClaimsPrincipal user) {

            var username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return username;
        }
    }
}