using Domain.Dto;
using LanguageExt.Common;
using MediatR;

namespace Application.Features.Recipes.Queries
{
    public class GetRecipeByIdQuery : IRequest<Result<RecipeDto>>
    {
        public Guid id { get; set; }
    }
}
