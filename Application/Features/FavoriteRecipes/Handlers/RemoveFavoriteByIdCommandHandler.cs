using Application.Features.FavoriteRecipes.Command;
using Application.Features.FavoriteRecipes.Validators;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.FavoriteRecipes.Handlers
{
    public class RemoveFavoriteByIdCommandHandler : IRequestHandler<RemoveFavoriteByIdCommand, Result<string>>
    {
        public RemoveFavoriteByIdCommandHandler(IUnitOfWork unitOfWork, IUserService userService, ILogger<RemoveFavoriteByIdCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _logger = logger;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ILogger<RemoveFavoriteByIdCommandHandler> _logger;
        public async Task<Result<string>> Handle(RemoveFavoriteByIdCommand request, CancellationToken cancellationToken)
        {

            var userResult = _userService.GetUser();

            if (userResult.IsFaulted)
            {
                _logger.LogInformation("Usuario solicitante no encontrado");
                return new Result<string>(new UnAuthorizedException("El usuario no puede realizar esa acción"));
            }

            var user = userResult.Match(user => user, _ => null!);

            request.UserId = user.UserId;

            var validator = new RemoveFavoriteByIdCommandValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation Errors");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<string>(validationException);
            }
            _logger.LogInformation("Checking if favorite already exist");


            var favorite =
                await _unitOfWork.UserFavoriteRepository.FirstOrDefaultAsync(r =>
                    r.RecipeId == request.RecipeId && r.SubId == request.UserId, cancellationToken);

            if (favorite.IsNull())
            {
                return new Result<string>("Not changed");
            }


            _unitOfWork.UserFavoriteRepository.Remove(favorite);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!result)
            {
                return new Result<string>(new InfraestructureException("Érror deleting favorite has occurred"));
            }
            return new Result<string>("Favorite removido con éxito");

        }
    }
}
