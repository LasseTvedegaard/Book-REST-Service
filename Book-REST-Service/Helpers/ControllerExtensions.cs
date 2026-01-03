using System.Security.Claims;

namespace Book_REST_Service.Helpers
{
    public static class ControllerExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(claim))
                throw new UnauthorizedAccessException("UserId claim missing");

            return claim;
        }
    }
}
