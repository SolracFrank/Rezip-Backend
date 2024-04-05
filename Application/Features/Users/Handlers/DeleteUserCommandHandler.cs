using Application.Features.Users.Commands;
using Application.Features.Users.Validators;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Handlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<string>>
    {
        public DeleteUserCommandHandler(ILogger<GetUsersQueryHandler> logger, IUnitOfWork unitOfWork, IAuth0Service auth0Service)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _auth0Service = auth0Service;
        }

        private readonly ILogger<GetUsersQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuth0Service _auth0Service;
        public async Task<Result<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {

            var validator = new DeleteUserCommandValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("User with request id does not exists.");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<string>(validationException);
            }

            var deleteUser = await _auth0Service.DeleteUser(request.UserId);

            if (deleteUser.IsFaulted)
            {
                return new Result<string>(new InfraestructureException("Error eliminando al usuario"));
            }

            var user = await _unitOfWork.UserRepository.FindAsync(cancellationToken, request.UserId);

            _unitOfWork.UserRepository.Remove(user);

            var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!saved)
            {
                _logger.LogInformation("Can not save changes");

                return new Result<string>(new InfraestructureException("No fue posible borrar al usuario."));
            }

            return new Result<string>("Usuario borrado correctamente");
        }
    }
}
