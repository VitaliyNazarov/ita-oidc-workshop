using Microsoft.AspNetCore.Identity;

namespace ITA.OIDC.Workshop.AuthServer.DataAccess;

internal sealed class ExternalRoleStore : IRoleStore<ExternalRole>
{
    public Task<IdentityResult> CreateAsync(ExternalRole role, CancellationToken cancellationToken)
    {
        throw new NotSupportedException($"Operation '{nameof(CreateAsync)}' is not supported");
    }

    public Task<IdentityResult> UpdateAsync(ExternalRole role, CancellationToken cancellationToken)
    {
        throw new NotSupportedException($"Operation '{nameof(UpdateAsync)}' is not supported");
    }

    public Task<IdentityResult> DeleteAsync(ExternalRole role, CancellationToken cancellationToken)
    {
        throw new NotSupportedException($"Operation '{nameof(DeleteAsync)}' is not supported");
    }

    public Task<string> GetRoleIdAsync(ExternalRole role, CancellationToken cancellationToken)
    {
        throw new NotSupportedException($"Operation '{nameof(GetRoleIdAsync)}' is not supported");
    }

    public Task<string> GetRoleNameAsync(ExternalRole role, CancellationToken cancellationToken)
    {
        throw new NotSupportedException($"Operation '{nameof(GetRoleNameAsync)}' is not supported");
    }

    public Task SetRoleNameAsync(ExternalRole role, string roleName, CancellationToken cancellationToken)
    {
        throw new NotSupportedException($"Operation '{nameof(SetRoleNameAsync)}' is not supported");
    }

    public Task<string> GetNormalizedRoleNameAsync(ExternalRole role, CancellationToken cancellationToken)
    {
        throw new NotSupportedException($"Operation '{nameof(GetNormalizedRoleNameAsync)}' is not supported");
    }

    public Task SetNormalizedRoleNameAsync(ExternalRole role, string normalizedName,
        CancellationToken cancellationToken)
    {
        throw new NotSupportedException($"Operation '{nameof(SetNormalizedRoleNameAsync)}' is not supported");
    }

    public Task<ExternalRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        throw new NotSupportedException($"Operation '{nameof(FindByIdAsync)}' is not supported");
    }

    public Task<ExternalRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        throw new NotSupportedException($"Operation '{nameof(FindByNameAsync)}' is not supported");
    }

    public void Dispose()
    {
    }
}