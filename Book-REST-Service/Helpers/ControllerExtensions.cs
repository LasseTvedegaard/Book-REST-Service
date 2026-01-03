using System.Security.Claims;

namespace Book_REST_Service.Helpers
{
    public static class ControllerExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var claim =
                user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(claim))
                throw new UnauthorizedAccessException("UserId claim missing");

            if (!Guid.TryParse(claim, out var userId))
                throw new UnauthorizedAccessException("Invalid UserId claim");

            return userId;
        }
    }
}
