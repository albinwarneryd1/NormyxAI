using Sylvaro.Domain.Entities;

namespace Sylvaro.Application.Abstractions;

public interface IJwtTokenService
{
    string CreateAccessToken(User user, IReadOnlyCollection<string> roles);
    string CreateRefreshToken();
}
