using Domain.Custom;
using LanguageExt.Common;

namespace Domain.Interfaces
{
    public interface IUserService
    {
        public Task<Result<UserAuthenticated>> GetUser(CancellationToken ctx);
    }
}
