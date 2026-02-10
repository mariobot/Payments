using Hyip_Payments.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Hyip_Payments.Web.Identity
{
    /// <summary>
    /// Custom claims factory to add additional claims (email, username) to the user principal
    /// </summary>
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public CustomUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            // Add email claim if not already present
            if (!identity.HasClaim(c => c.Type == ClaimTypes.Email) && !string.IsNullOrEmpty(user.Email))
            {
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            }

            // Add additional email claim with short name for compatibility
            if (!string.IsNullOrEmpty(user.Email))
            {
                identity.AddClaim(new Claim("email", user.Email));
            }

            // Add username claim
            if (!string.IsNullOrEmpty(user.UserName))
            {
                identity.AddClaim(new Claim("username", user.UserName));
            }

            // Add name claim (can be username or email)
            if (!identity.HasClaim(c => c.Type == ClaimTypes.Name))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? "Unknown"));
            }

            return identity;
        }
    }
}
