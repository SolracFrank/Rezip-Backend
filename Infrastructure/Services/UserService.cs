using Domain.Custom;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;


        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<UserAuthenticated>> GetUser(CancellationToken ctx)
        {
            try
            {
                //mocking result until doing auth behavior
                Guid userId = Guid.Parse("0d80a7ea-334a-4295-8b53-fcfbf8a8c969");

                var user = await _unitOfWork.UserRepository.FindAsync(ctx, userId);

                if (user == null)
                {
                    return new Result<UserAuthenticated>(new NotFoundException("Usuario autenticado no existe"));
                }

                return new Result<UserAuthenticated>(new UserAuthenticated
                {
                    UserId = user.UserId,
                    Name = user.Username
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error authenticating user {ex.Message}");
                return new Result<UserAuthenticated>(new InfraestructureException("Error desconocido al autenticar"));
            }
        }
    }
}
