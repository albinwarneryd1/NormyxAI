using Sylvaro.Application.Abstractions;

namespace Sylvaro.Api.Utilities;

public static class TenantContext
{
    public static Guid RequireTenantId(ICurrentUserContext user)
    {
        if (user.TenantId is null)
        {
            throw new InvalidOperationException("Tenant claim missing");
        }

        return user.TenantId.Value;
    }

    public static Guid RequireUserId(ICurrentUserContext user)
    {
        if (user.UserId is null)
        {
            throw new InvalidOperationException("User claim missing");
        }

        return user.UserId.Value;
    }
}
