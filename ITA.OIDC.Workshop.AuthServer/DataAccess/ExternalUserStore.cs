using Microsoft.AspNetCore.Identity;

namespace ITA.OIDC.Workshop.AuthServer.DataAccess;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class ExternalUserStore :
    IUserEmailStore<ExternalUser>,
    IUserPhoneNumberStore<ExternalUser>,
    IUserSecurityStampStore<ExternalUser>,
    IUserRoleStore<ExternalUser>
{
    public Task<string> GetUserIdAsync(ExternalUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id);
    }

    public Task<string> GetUserNameAsync(ExternalUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public Task SetUserNameAsync(ExternalUser user, string userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public Task<string> GetNormalizedUserNameAsync(ExternalUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedUserName);
    }

    public Task SetNormalizedUserNameAsync(ExternalUser user, string normalizedName,
        CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public Task<IdentityResult> CreateAsync(ExternalUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> UpdateAsync(ExternalUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(ExternalUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public async Task<ExternalUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await FindExternalUserAsync(userId, cancellationToken);
    }

    public async Task<ExternalUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return await FindExternalUserAsync(normalizedUserName, cancellationToken);
    }

    public Task SetEmailAsync(ExternalUser user, string email, CancellationToken cancellationToken)
    {
        user.Email = email;
        return Task.CompletedTask;
    }

    public Task<string> GetEmailAsync(ExternalUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(ExternalUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.EmailConfirmed);
    }

    public Task SetEmailConfirmedAsync(ExternalUser user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public async Task<ExternalUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return await FindExternalUserAsync(normalizedEmail, cancellationToken);
    }

    public Task<string> GetNormalizedEmailAsync(ExternalUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedEmail);
    }

    public Task SetNormalizedEmailAsync(ExternalUser user, string normalizedEmail, CancellationToken cancellationToken)
    {
        user.NormalizedEmail = normalizedEmail;
        return Task.CompletedTask;
    }

    public Task SetSecurityStampAsync(ExternalUser user, string stamp, CancellationToken cancellationToken)
    {
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    public Task<string> GetSecurityStampAsync(ExternalUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.SecurityStamp);
    }

    #region Implementation of IUserPhoneNumberStore<ExternalUser>

    public Task SetPhoneNumberAsync(ExternalUser user, string phoneNumber, CancellationToken cancellationToken)
    {
        user.PhoneNumber = phoneNumber;
        return Task.CompletedTask;
    }

    public Task<string> GetPhoneNumberAsync(ExternalUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PhoneNumber);
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(ExternalUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PhoneNumberConfirmed);
    }

    public Task SetPhoneNumberConfirmedAsync(ExternalUser user, bool confirmed, CancellationToken cancellationToken)
    {
        user.PhoneNumberConfirmed = confirmed;
        return Task.CompletedTask;
    }

    #endregion

    #region Implementation of IUserRoleStore<ExternalUser>


    public Task<IList<string>> GetRolesAsync(ExternalUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult<IList<string>>(user.Roles);
    }

    public Task<bool> IsInRoleAsync(ExternalUser user, string roleName, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Roles.Contains(roleName));
    }

    public Task AddToRoleAsync(ExternalUser user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("AddToRoleAsync is not supported");
    }

    public Task RemoveFromRoleAsync(ExternalUser user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("RemoveFromRoleAsync is not supported");
    }

    public Task<IList<ExternalUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("GetUsersInRoleAsync is not supported");
    }

    #endregion

    public void Dispose()
    {
        // Dispose any scoped resource
    }

    private Task<ExternalUser> FindExternalUserAsync(string login, CancellationToken cancellationToken)
    {
        throw new NotSupportedException($"FindExternalUserAsync is not supported. '{login}'");
    }
}

// ReSharper disable once ClassNeverInstantiated.Global