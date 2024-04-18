using LanguageExt.Common;
using MediatR;

namespace Application.Features.FavoriteRecipes.Command
{
    public class RemoveFavoriteByIdCommand : IRequest<Result<string>>
    {
        public string UserId { get; set; }
        public Guid RecipeId { get; set; }
    }
}
