using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string getUserName(this ClaimsPrincipal user) {

            var username = user.FindFirst(ClaimTypes.Name)?.Value;
            return username;
        }
        public static int getUserId(this ClaimsPrincipal user) {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return userId;
        }

        
    }
}