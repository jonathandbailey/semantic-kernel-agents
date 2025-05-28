using System.Security.Claims;

namespace Todo.Api.Extensions
{
    public static class UserPrincipalExtensions
    {
        public static Guid Id(this ClaimsPrincipal user)
        {
            return Guid.Parse("B4C361C4-460C-4B1D-8DC7-34D5F3595AD1");
        }
    }
}
