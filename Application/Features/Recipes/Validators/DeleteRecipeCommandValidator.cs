using Application.Features.Recipes.Commands;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.Recipes.Validators
{
    public class DeleteRecipeCommandValidator : AbstractValidator<DeleteRecipeCommand>
    {
        public DeleteRecipeCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(r => r.Id)
                .NotEmpty()
                .MustAsync(async (id, ctx) => await unitOfWork.RecipeRepository.AnyAsync(x => x.RecipeId == id, ctx))
                .WithMessage("La receta a eliminar no existe");
        }
    }
}
