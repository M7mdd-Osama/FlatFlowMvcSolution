using System.Security.Claims;

namespace FlatFlow.PL.Helpers
{
    public static class UserHelper
    {
        public static string GetCurrentUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value))
            {
                return userIdClaim.Value;
            }
            throw new UnauthorizedAccessException("User not logged in");
        }
    }
}