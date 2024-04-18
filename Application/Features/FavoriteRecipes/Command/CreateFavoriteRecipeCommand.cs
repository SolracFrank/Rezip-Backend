using LanguageExt.Common;
using MediatR;

namespace Application.Features.FavoriteRecipes.Command
{
    public class CreateFavoriteRecipeCommand : IRequest<Result<string>>
    {
        public Guid RecipeId { get; set; }
        public string UserId { get; set; }
    }
}
