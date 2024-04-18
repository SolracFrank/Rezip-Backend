using Domain.Dto;
using LanguageExt.Common;
using MediatR;

namespace Application.Features.FavoriteRecipes.Queries
{
    public class GetAllFavoritesRecipesQuery : IRequest<Result<IEnumerable<RecipeDto>>>
    {
        public string UserId { get; set; }
    }
}
