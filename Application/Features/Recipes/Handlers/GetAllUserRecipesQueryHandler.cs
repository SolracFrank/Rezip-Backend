using Application.Features.Recipes.Queries;
using Application.Features.Recipes.Validators;
using Domain.Dto;
using Domain.Dto.Extensions;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Recipes.Handlers
{
    public class GetAllUserRecipesQueryHandler : IRequestHandler<GetAllUserRecipesQuery, Result<IEnumerable<RecipeDto>>>
    {
        public GetAllUserRecipesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllUserRecipesQueryHandler> logger, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userService = userService;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllUserRecipesQueryHandler> _logger;
        private readonly IUserService _userService;
        public async Task<Result<IEnumerable<RecipeDto>>> Handle(GetAllUserRecipesQuery request, CancellationToken cancellationToken)
        {

            var userRequest = _userService.GetUser();

            if (userRequest.IsFaulted)
            {
                return new Result<IEnumerable<RecipeDto>>(new UnAuthorizedException("El usuario no tiene permitido esta acción"));
            }

            var user = userRequest.Match(user => user, _ => null!);
            request.userId = user.UserId;

            var validator = new GetAllUserRecipesQueryValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation Errors");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<IEnumerable<RecipeDto>>(validationException);
            }


            var recipes = await _unitOfWork.RecipeRepository.WhereAsync(r => r.CreatedBy == request.userId, cancellationToken, false);


            return new Result<IEnumerable<RecipeDto>>(recipes.ToRecipeDto());

        }
    }
}
