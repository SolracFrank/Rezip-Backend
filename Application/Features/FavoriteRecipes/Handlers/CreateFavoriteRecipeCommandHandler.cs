using Application.Features.FavoriteRecipes.Command;
using Application.Features.FavoriteRecipes.Validators;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.FavoriteRecipes.Handlers
{
    internal class CreateFavoriteRecipeCommandHandler: IRequestHandler<CreateFavoriteRecipeCommand, Result<string>>
    {
        public CreateFavoriteRecipeCommandHandler(ILogger<CreateFavoriteRecipeCommandHandler> logger, IUnitOfWork unitOfWork, IUserService userService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        private readonly ILogger<CreateFavoriteRecipeCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public async Task<Result<string>> Handle(CreateFavoriteRecipeCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Validating User");

            var userRequest = _userService.GetUser();


            if (userRequest.IsFaulted)
            {
                return new Result<string>(new UnAuthorizedException("El usuario no tiene permitido esta acción"));
            }

            var user = userRequest.Match(user => user, _ => null!);

            request.UserId = user.UserId;
            _logger.LogInformation("Validating Favorites");

            var validator = new CreateFavoriteRecipeValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation Errors");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<string>(validationException);
            }
            _logger.LogInformation("Checking if favorite already exist");

            var favoriteExists =
                await _unitOfWork.UserFavoriteRepository.AnyAsync(
                    r => r.RecipeId == request.RecipeId && r.SubId == request.UserId, cancellationToken);

            if (favoriteExists)
            {
                _logger.LogInformation("Favorite already exist");

                return new Result<string>("Guardado sin cammbios");
            }

            _logger.LogInformation("Trying to Add Favorites");


            await _unitOfWork.UserFavoriteRepository.AddAsync(new UserFavoritesRecipe
            {
                Id = Guid.NewGuid(),
                SubId = request.UserId,
                RecipeId = request.RecipeId,
            }, cancellationToken);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result)
            {
                _logger.LogInformation("Addition successful");

                return new Result<string>("Cambios guardados satisfactoriamente");
            }
            _logger.LogInformation("Addition User Favorite failed");

            return new Result<string>(new InfraestructureException("Error al actualizar User Favorite"));
        }
    }
}
