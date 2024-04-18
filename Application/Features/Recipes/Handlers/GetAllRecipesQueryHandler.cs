using Application.Features.Recipes.Queries;
using Domain.Dto;
using Domain.Dto.Extensions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Recipes.Handlers
{
    public class GetAllRecipesQueryHandler : IRequestHandler<GetAllRecipesQuery, Result<IEnumerable<RecipeDto>>>
    {
        public GetAllRecipesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllRecipesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllRecipesQueryHandler> _logger;
        public async Task<Result<IEnumerable<RecipeDto>>> Handle(GetAllRecipesQuery request, CancellationToken cancellationToken)
        {
            var favorites = await
                _unitOfWork.RecipeRepository.Where(r => r.RecipeId != null).Include(r => r.CreatedByNavigation).ToListAsync();

            return new Result<IEnumerable<RecipeDto>>(favorites.ToRecipeDto());

        }
    }
}
