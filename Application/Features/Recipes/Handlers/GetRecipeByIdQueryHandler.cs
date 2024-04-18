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
    public class GetRecipeByIdQueryHandler : IRequestHandler<GetRecipeByIdQuery, Result<RecipeDto>>
    {
        public GetRecipeByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetRecipeByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRecipeByIdQueryHandler> _logger;
        public async Task<Result<RecipeDto>> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
        {

            var validator = new GetRecipeByIdQueryValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation Errors");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<RecipeDto>(validationException);
            }

            var recipe = await _unitOfWork.RecipeRepository.FindAsync(cancellationToken, request.id);

            return new Result<RecipeDto>(recipe!.ToRecipeDto());
        }
    }
}
