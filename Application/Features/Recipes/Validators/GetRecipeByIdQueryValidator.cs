using Application.Features.Recipes.Queries;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.Recipes.Validators
{
    internal class GetRecipeByIdQueryValidator : AbstractValidator<GetRecipeByIdQuery>
    {
        public GetRecipeByIdQueryValidator(IUnitOfWork unitOfWork)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(r => r.id)
                .NotEmpty()
                .MustAsync(async (id, ctx) => await unitOfWork.RecipeRepository.AnyAsync(x => x.RecipeId == id, ctx))
                .WithMessage("La receta a obtener no existe");

               
        }
    }
}
