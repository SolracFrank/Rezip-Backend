using Domain.Custom;
using LanguageExt.Common;

namespace Domain.Interfaces
{
    public interface IUserService
    {
        public Result<UserAuthenticated> GetUser();

    }
}
