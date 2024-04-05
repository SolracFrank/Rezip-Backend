using LanguageExt.Common;

namespace Domain.Interfaces
{
    public interface IAuth0Service
    {
        public Task<string> GetAuth0Token();
        public Task<Result<string>> GiveUserPermissions(string userId);
        public Task<Result<string>> DeleteUser(string userId);

    }
}
