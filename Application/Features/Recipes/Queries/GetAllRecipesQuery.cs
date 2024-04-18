using Domain.Dto;
using LanguageExt.Common;
using MediatR;

namespace Application.Features.Recipes.Queries
{
    public class GetAllRecipesQuery :IRequest<Result<IEnumerable<RecipeDto>>>
    {
    }
}
