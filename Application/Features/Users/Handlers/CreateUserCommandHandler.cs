using Application.Features.Users.Commands;
using Application.Features.Users.Validators;
using Domain.Dto;
using Domain.Dto.Extensions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
    {
        public CreateUserCommandHandler(ILogger<GetUsersQueryHandler> logger, IUnitOfWork unitOfWork, IUserService userService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        private readonly ILogger<GetUsersQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var authResult = await _userService.GetUser(cancellationToken);
            if (authResult.IsFaulted)
            {
                return new Result<UserDto>(new UnAuthorizedException("Error al autenticar al usuario"));
            }

            var userAuthenticated = authResult.Match(user => user, _ => null!);

            var validator = new CreateUserCommandValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("User with request id not exists.");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<UserDto>(validationException);
            }

            var user = new User
            {
                UserId = Guid.NewGuid(),
                CreatedBy = userAuthenticated.UserId,
                Username = request.Username,
                Name = request.Name,
                Lastname = request.Lastname,
                Password = request.Password,
                CreationDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);

            var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!saved)
            {
                _logger.LogInformation("Can not save changes");

                return new Result<UserDto>(new InfraestructureException("No fue posible crear el usuario."));
            }

            return new Result<UserDto>(user.ToUserDto());
        }
    }
}
