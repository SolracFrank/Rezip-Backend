using Domain.Dto;
using LanguageExt.Common;
using MediatR;

namespace Application.Features.Recipes.Queries
{
    public class GetRecipeLogoQuery : IRequest<Result<RecipeLogoDto>>
    {
        public Guid LogoId { get; set; }
    }
}
