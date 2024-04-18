using Domain.Dto;
using LanguageExt.Common;
using MediatR;

namespace Application.Features.Recipes.Queries
{
    public class GetAllUserRecipesQuery : IRequest<Result<IEnumerable<RecipeDto>>>
    {
        public string userId { get; set; }
    }
}
