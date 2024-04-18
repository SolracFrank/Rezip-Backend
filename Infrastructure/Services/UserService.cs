using Domain.Custom;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly Auth0Claims _auth0Claims;
        private readonly IUnitOfWork _unitOfWork;


        public UserService( ILogger<UserService> logger, IHttpContextAccessor contextAccessor, IOptions<Auth0Claims> auth0Claims, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
            _auth0Claims = auth0Claims.Value;

        }
        public Result<UserAuthenticated> GetUser()
        {
            try
            {
                _logger.LogInformation("Trying to get the claims principal");

                var user = _contextAccessor.HttpContext?.User;

                if (user == null)
                {
                    _logger.LogInformation("Claims principal are empty.");

                    return new Result<UserAuthenticated>(new UnAuthorizedException("User is not authenticated."));
                }

                _logger.LogInformation("Claims principal obtained");

                var userEmail = user.Claims.FirstOrDefault(c => c.Type == _auth0Claims.Email)?.Value;

                _logger.LogInformation("Checking if Email claim exists");

                if (userEmail == null)
                {
                    _logger.LogInformation("Email claim not exists");

                    return new Result<UserAuthenticated>(new UnAuthorizedException("El usuario no tiene Email"));
                }
                _logger.LogInformation($"Email exists {userEmail}");

                var subId = user.Identity.Name;
                _logger.LogInformation("Checking if sub  claim exists");

                if (subId == null)
                {
                    _logger.LogInformation("Sub claim not exists");

                    return new Result<UserAuthenticated>(new UnAuthorizedException("El usuario no tiene Sub"));
                }

                _logger.LogInformation($"Sub exists {subId}");

                return new Result<UserAuthenticated>(new UserAuthenticated { Name = userEmail, UserId = subId});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error authenticating user {ex.Message}");
                return new Result<UserAuthenticated>(new InfraestructureException("Error desconocido al autenticar"));
            }
        }
    }
}
