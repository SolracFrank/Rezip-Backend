using Application.Features.FavoriteRecipes.Queries;
using Application.Features.FavoriteRecipes.Validators;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.FavoriteRecipes.Handlers
{
    public class GetAllFavoritesIdQueryHandler : IRequestHandler<GetAllFavoritesIdQuery, Result<IEnumerable<Guid>?>>
    {
        public GetAllFavoritesIdQueryHandler(ILogger<GetAllFavoritesRecipesQueryHandler> logger, IUnitOfWork unitOfWork, IUserService userService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        private readonly ILogger<GetAllFavoritesRecipesQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public async Task<Result<IEnumerable<Guid>>> Handle(GetAllFavoritesIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Validating User");

            var userRequest = _userService.GetUser();

            if (userRequest.IsFaulted)
            {
                return new Result<IEnumerable<Guid>>(new UnAuthorizedException("El usuario no tiene permitido esta acción"));
            }

            _logger.LogInformation("Validating request");
            var user = userRequest.Match(user => user, _ => null!);

            request.UserId = user.UserId;

            var validator = new GetAllFavoritesIdQueryValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation Errors");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<IEnumerable<Guid>>(validationException);
            }
            var favorites = await
                _unitOfWork.UserFavoriteRepository
                    .Where(r => r.SubId == request.UserId).Select(r => r.RecipeId)
                    .Where(id => id.HasValue).Select(id => id.Value)
                    .ToListAsync();

            return new Result<IEnumerable<Guid>>(favorites); 
        }
    }
}
