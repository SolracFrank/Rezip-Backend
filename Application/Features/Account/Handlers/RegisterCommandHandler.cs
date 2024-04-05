using Application.Features.Account.Commands;
using Application.Features.Account.Validators;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Account.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<bool>>
    {
        public RegisterCommandHandler(ILogger<RegisterCommandHandler> logger, IUnitOfWork unitOfWork, IUserService userService, IAuth0Service auth0Service)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _auth0Service = auth0Service;
        }

        private readonly ILogger<RegisterCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IAuth0Service _auth0Service;
      

        public async Task<Result<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Checking if user is authenticated");
            var authResult = _userService.GetUser();

            if (authResult.IsFaulted)
            {
                _logger.LogInformation("User is not authenticated");

                return new Result<bool>(new UnAuthorizedException("Error al autenticar al usuario"));
            }
            _logger.LogInformation("User authenticated");

            var user = authResult.Match(user => user, _ => null!);

            request.SubId = user.UserId;
            request.Name = user.Name;

            _logger.LogInformation("Validating user");

            var validator = new RegisterCommandValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation Errors");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<bool>(validationException);
            }
            _logger.LogInformation("Checking if user already exists");

            var userExist =await  _unitOfWork.UserRepository.AnyAsync(u => u.SubId == request.SubId, cancellationToken);

            if (userExist)
            {
                _logger.LogInformation("User exists");

                return new Result<bool>(false);
            }
            _logger.LogInformation("User doesn't exist");

            _logger.LogInformation("Giving user permissions in auth0");
            var permissionUser = await  _auth0Service.GiveUserPermissions(user.UserId);


            if (permissionUser.IsFaulted)
            {
                _logger.LogInformation("Giving user permissions have failed");

                return new Result<bool>(new BadRequestException("Error al darle permisos al usuario"));
            }

            _logger.LogInformation("User permissions has been granted");

            _logger.LogInformation("Creating user in database");

            var newUser = new User
            {
                Username = user.Name,
                Email = user.Name,
                SubId = user.UserId,
                UserId = Guid.NewGuid(),
            };

            await _unitOfWork.UserRepository.AddAsync(newUser, cancellationToken);
            var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!saved)
            {
                _logger.LogInformation("Can not save changes");

                return new Result<bool>(new InfraestructureException("No fue posible crear el usuario."));
            }
            _logger.LogInformation("User created");
            return new Result<bool>(true);

        }
    }
}
