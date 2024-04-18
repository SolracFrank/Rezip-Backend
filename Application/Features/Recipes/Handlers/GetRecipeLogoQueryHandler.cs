using Application.Features.Recipes.Queries;
using Application.Features.Recipes.Validators;
using Domain.Dto;
using Domain.Dto.Extensions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Recipes.Handlers
{
    public class GetRecipeLogoQueryHandler : IRequestHandler<GetRecipeLogoQuery, Result<RecipeLogoDto>>
    {
        public GetRecipeLogoQueryHandler(IUnitOfWork unitOfWork, ILogger<GetRecipeLogoQuery> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRecipeLogoQuery> _logger;

        public async Task<Result<RecipeLogoDto>> Handle(GetRecipeLogoQuery request, CancellationToken cancellationToken)
        {
            var validator = new GetRecipeLogoQueryValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation Errors");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<RecipeLogoDto>(validationException);
            }

            var recipeLogo = await _unitOfWork.RecipeLogoRepository.FindAsync(cancellationToken, request.LogoId);
            var recipe = await _unitOfWork.RecipeRepository.FindAsync(cancellationToken, request.LogoId);
       

            return new Result<RecipeLogoDto>(new RecipeLogoDto
            {
                Id = request.LogoId,
                Logo = recipeLogo.Logo,
                FileName = recipe.Name,
                FileFormat = recipeLogo.FileFormat,
                MimeType = recipeLogo.MimeType
            });
        }
    }
}