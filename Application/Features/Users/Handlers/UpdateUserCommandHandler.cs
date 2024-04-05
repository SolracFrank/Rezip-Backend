using Application.Features.Users.Commands;
using Application.Features.Users.Validators;
using Domain.Dto;
using Domain.Dto.Extensions;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Handlers
{
    internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
    {
        public UpdateUserCommandHandler(ILogger<UpdateUserCommandHandler> logger, IUnitOfWork unitOfWork, IUserService userService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        private readonly ILogger<UpdateUserCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var authResult =  _userService.GetUser();
            if (authResult.IsFaulted)
            {
                return new Result<UserDto>(new UnAuthorizedException("Error al autenticar al usuario"));
            }

            var userAuthenticated = authResult.Match(user => user, _ => null!);


            var validator = new UpdateUserComamandValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("User with request id not exists.");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<UserDto>(validationException);
            }

            var user = await _unitOfWork.UserRepository.FindAsync(cancellationToken, request.UserId);
            var updatesCount = 0;

            foreach (var property in request.GetType().GetProperties())
            {
                if (property.Name == "UserId") continue;
                if (property.GetValue(request) != null) updatesCount++;
            }

            if (updatesCount == 0)
            {
                return new Result<UserDto>(user.ToUserDto());
            }

            user.Username = request.Username;
            user.Name = request.Name;
            user.Lastname = request.Lastname;
            user.UpdateDate = DateTime.UtcNow;

            _unitOfWork.UserRepository.Update(user);

            var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!saved)
            {
                _logger.LogInformation("Can not save changes");

                return new Result<UserDto>(new InfraestructureException("No fue posible actualizar el usuario."));
            }

            return new Result<UserDto>(user.ToUserDto());
        }
    }
}
