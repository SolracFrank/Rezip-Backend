using Application.Features.FavoriteRecipes.Queries;
using Application.Features.FavoriteRecipes.Validators;
using Domain.Dto;
using Domain.Dto.Extensions;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.FavoriteRecipes.Handlers
{
    public class GetAllFavoritesRecipesQueryHandler : IRequestHandler<GetAllFavoritesRecipesQuery, Result<IEnumerable<RecipeDto>>>
    {
        public GetAllFavoritesRecipesQueryHandler(ILogger<GetAllFavoritesRecipesQueryHandler> logger, IUnitOfWork unitOfWork, IUserService userService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        private readonly ILogger<GetAllFavoritesRecipesQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;


        public async Task<Result<IEnumerable<RecipeDto>>> Handle(GetAllFavoritesRecipesQuery request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("Validating User");

            var userRequest = _userService.GetUser();

            if (userRequest.IsFaulted)
            {
                return new Result<IEnumerable<RecipeDto>>(new UnAuthorizedException("El usuario no tiene permitido esta acción"));
            }

            _logger.LogInformation("Validating request");
            var user = userRequest.Match(user => user, _ => null!);

            request.UserId = user.UserId;

            var validator = new GetAllFavoriteRecipesQueryValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation Errors");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<IEnumerable<RecipeDto>>(validationException);
            }

            var favorites = await
                _unitOfWork.UserFavoriteRepository.Where(r => r.SubId == request.UserId).Include(r => r.Recipe.CreatedByNavigation).Select(r => r.Recipe).ToListAsync();

            return new Result<IEnumerable<RecipeDto>>(favorites.ToRecipeDto());

        }
    }
}


//var favorites =
//    await _unitOfWork.UserFavoriteRepository.WhereAsync(r => r.SubId == request.UserId, cancellationToken,
//        false);