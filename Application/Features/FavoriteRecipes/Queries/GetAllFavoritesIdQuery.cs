using LanguageExt.Common;
using MediatR;

namespace Application.Features.FavoriteRecipes.Queries
{
    public class GetAllFavoritesIdQuery : IRequest<Result<IEnumerable<Guid>?>>
    {
        public string UserId { get; set; }
    }
}
