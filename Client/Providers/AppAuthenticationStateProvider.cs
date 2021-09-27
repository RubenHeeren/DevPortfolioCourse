using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client.Providers;

public class AppAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
    internal const string LocalStorageBearerTokenKeyName = "bearerToken";

    public AppAuthenticationStateProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            string savedToken = await _localStorageService.GetItemAsync<string>(LocalStorageBearerTokenKeyName);

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            JwtSecurityToken jwtSecurityToken = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);
            DateTime expiry = jwtSecurityToken.ValidTo;

            if (expiry < DateTime.UtcNow)
            {
                await _localStorageService.RemoveItemAsync(LocalStorageBearerTokenKeyName);
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Get claims from token and build an authenticated user object
            var claims = ParseClaims(jwtSecurityToken);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            return new AuthenticationState(user);
        }
        catch (System.Exception)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    internal async Task SignIn()
    {
        string savedToken = await _localStorageService.GetItemAsync<string>(LocalStorageBearerTokenKeyName);

        JwtSecurityToken jwtSecurityToken = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);

        var claims = ParseClaims(jwtSecurityToken);
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

        Task<AuthenticationState> authenticationState = Task.FromResult(new AuthenticationState(user));
        NotifyAuthenticationStateChanged(authenticationState);
    }

    internal void SignOut()
    {
        ClaimsPrincipal nobody = new ClaimsPrincipal(new ClaimsIdentity());
        Task<AuthenticationState> authenticationState = Task.FromResult(new AuthenticationState(nobody));
        NotifyAuthenticationStateChanged(authenticationState);
    }

    private IList<Claim> ParseClaims(JwtSecurityToken jwtSecurityToken)
    {
        IList<Claim> claims = jwtSecurityToken.Claims.ToList();
        // The value of tokenContent.Subject is the user's email.
        claims.Add(new Claim(ClaimTypes.Name, jwtSecurityToken.Subject));
        return claims;
    }
}
