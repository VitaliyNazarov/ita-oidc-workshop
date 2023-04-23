using System.Collections.Immutable;
using System.Security.Claims;
using ITA.OIDC.Workshop.AuthServer.DataAccess;
using ITA.OIDC.Workshop.AuthServer.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ITA.OIDC.Workshop.AuthServer.Controllers;

public class AuthorizationController : Controller
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly SignInManager<ExternalUser> _signInManager;
    private readonly UserManager<ExternalUser> _userManager;

    public AuthorizationController(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        SignInManager<ExternalUser> signInManager,
        UserManager<ExternalUser> userManager)
    {
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
        _scopeManager = scopeManager;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    #region Authorization code, implicit and hybrid flows

    // Note: to support interactive flows like the code flow,
    // you must provide your own authorization endpoint action:

    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // Try to retrieve the user principal stored in the authentication cookie and redirect
        // the user agent to the login page (or to an external provider) in the following cases:
        //
        //  - If the user principal can't be extracted or the cookie is too old.
        //  - If prompt=login was specified by the client application.
        //  - If a max_age parameter was provided and the authentication cookie is not considered "fresh" enough.
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        if (result == null || !result.Succeeded || request.HasPrompt(Prompts.Login) ||
            (request.MaxAge != null && result.Properties?.IssuedUtc != null &&
             DateTimeOffset.UtcNow - result.Properties.IssuedUtc > TimeSpan.FromSeconds(request.MaxAge.Value)))
        {
            // If the client application requested promptless authentication,
            // return an error indicating that the user is not logged in.
            if (request.HasPrompt(Prompts.None))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
                    }!));
            }

            // To avoid endless login -> authorization redirects, the prompt=login flag
            // is removed from the authorization request payload before redirecting the user.
            var prompt = string.Join(" ", request.GetPrompts().Remove(Prompts.Login));

            var parameters = Request.HasFormContentType
                ? Request.Form.Where(parameter => parameter.Key != Parameters.Prompt).ToList()
                : Request.Query.Where(parameter => parameter.Key != Parameters.Prompt).ToList();

            parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));

            return Challenge(
                authenticationSchemes: IdentityConstants.ApplicationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(parameters)
                });
        }

        // Преобразуем информацию из ClaimsPrincipal в ExternalUser
        var user = ConvertTo(User);

        // Retrieve the application details from the database.
        var application = await _applicationManager.FindByClientIdAsync(request.ClientId!) ??
                          throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

        // Retrieve the permanent authorizations associated with the user and the calling client application.
        var authorizations = await _authorizationManager.FindAsync(
            subject: await _userManager.GetUserIdAsync(user),
            client: await _applicationManager.GetIdAsync(application) ?? string.Empty,
            status: Statuses.Valid,
            type: AuthorizationTypes.Permanent,
            scopes: request.GetScopes()).ToListAsync();

        var principal = await _signInManager.CreateUserPrincipalAsync(user);

        var authorization = authorizations.LastOrDefault() ?? await _authorizationManager.CreateAsync(
            principal: principal,
            subject: await _userManager.GetUserIdAsync(user),
            client: await _applicationManager.GetIdAsync(application) ?? string.Empty,
            type: AuthorizationTypes.Permanent,
            scopes: request.GetScopes());

        principal.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));

        await PreparePrincipalAsync(principal, user, request.GetScopes());

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    #endregion

    #region Authorization code flow

    [HttpPost("~/connect/token")]
    [IgnoreAntiforgeryToken]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
        {
            // Retrieve the claims principal stored in the authorization code/device code/refresh token.
            var authenticateResult = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                
            var user = ConvertTo(authenticateResult.Principal!);
                
            // Ensure the user is still allowed to sign in.
            if (!await _signInManager.CanSignInAsync(user))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                    }!));
            }

            var principal = authenticateResult.Principal!;
                
            await PreparePrincipalAsync(principal, user);

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new InvalidOperationException("The specified grant type is not supported.");
    }

    #endregion

    private ExternalUser ConvertTo(ClaimsPrincipal principal)
    {
        var userId = principal.FindFirstValue(Claims.Subject);
        var email = principal.FindFirstValue(Claims.Email);
        var userName = principal.FindFirstValue(Claims.Username);
        var roles = principal.FindAll(Claims.Role).Select(x => x.Value).ToArray();
        var phone = principal.FindFirstValue(Claims.PhoneNumber);
        var fullName = principal.FindFirstValue(Claims.Name);

        return new ExternalUser
        {
            Id = userId,
            UserName = userName,
            Email = email,
            Roles = roles,
            FullName = fullName,
            PhoneNumber = phone,

            EmailConfirmed = false,
            LockoutEnabled = false,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            NormalizedEmail = email.ToUpper(),
            NormalizedUserName = email.ToUpper(),
        };
    }
    
    private async Task PreparePrincipalAsync(
        ClaimsPrincipal principal,
        ExternalUser user,
        IEnumerable<string>? scopes = null,
        CancellationToken cancellationToken = default)
    {
        if (scopes != null)
        {
            principal.SetScopes(scopes);
            principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes(), cancellationToken).ToListAsync());
        }
            
        principal.SetClaim(Claims.Subject, user.Id);
        principal.SetClaim(Claims.Email, user.Email);
        principal.SetClaim(Claims.Name, user.FullName);
        principal.SetClaim(Claims.Username, user.UserName);
        principal.SetClaim(Claims.PhoneNumber, user.PhoneNumber);

        SetClaimsDestination(principal);
    }

    private void SetClaimsDestination(ClaimsPrincipal principal)
    {
        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(GetDestinations(claim, principal).ToArray());
        }
    }

    // scope=profile, claims =[organization_ogrn, organization_name, position, username, name]
    // scope=email,  claims =[email, email_verified]
    // scope=phone, claims=[phone, phone_verified]
    // scope=roles, claims=[role]
    private IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
    {
        switch (claim.Type)
        {
            case Claims.Issuer:
            case Claims.Subject:
            case Claims.Audience:
            case Claims.AuthorizedParty:
                yield return Destinations.AccessToken;
                yield return Destinations.IdentityToken;
                yield break;

            case Claims.ClientId:
                yield return Destinations.AccessToken;
                yield break;

            case Claims.Username:
            case Claims.Name:
                if (principal.HasScope(Scopes.Profile))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.EmailVerified:
                if (principal.HasScope(Scopes.Email))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Email:
                if (principal.HasScope(Scopes.Email))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.PhoneNumberVerified:
                if (principal.HasScope(Scopes.Phone))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.PhoneNumber:
                if (principal.HasScope(Scopes.Phone))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Role:
                yield return Destinations.AccessToken;

                if (principal.HasScope(Scopes.Roles))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Scope:
                yield return Destinations.AccessToken;
                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }
}
