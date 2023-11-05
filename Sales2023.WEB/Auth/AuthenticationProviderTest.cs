using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Sales2023.WEB.Auth
{
    public class AuthenticationProviderTest : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var anonimous = new ClaimsIdentity();
            var mzamoraUser = new ClaimsIdentity(new List<Claim>
            {
                new Claim("FirstName", "Michael"),
                new Claim("LastName", "Zamora"),
                new Claim(ClaimTypes.Name, "mzamora@yopmail.com"),
                new Claim(ClaimTypes.Role, "Admin")
            });
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(mzamoraUser)));
        }
    }
}
