using Application.Features.Recipes.Queries;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.Recipes.Validators
{
    public class GetRecipeLogoQueryValidator : AbstractValidator<GetRecipeLogoQuery>
    {
        public GetRecipeLogoQueryValidator(IUnitOfWork unitOfWork)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(r => r.LogoId)
                .NotEmpty()
                .MustAsync(async (id, ctx) => await unitOfWork.RecipeRepository.AnyAsync(r => r.RecipeId == id, ctx))
                .WithMessage("No existe el logo seleccionado");
        }
    }
}
