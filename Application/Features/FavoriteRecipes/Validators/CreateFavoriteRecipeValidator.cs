using Application.Features.FavoriteRecipes.Command;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.FavoriteRecipes.Validators
{
    internal class CreateFavoriteRecipeValidator : AbstractValidator<CreateFavoriteRecipeCommand>
    {
        public CreateFavoriteRecipeValidator(IUnitOfWork unitOfWork)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(r => r.UserId)
                .NotEmpty()
                .MustAsync(async (id, ctx) => await unitOfWork.UserRepository.AnyAsync(u => u.SubId == id, ctx))
                .WithMessage("El usuario solicitante no existe");

            RuleFor(r => r.RecipeId)
                .NotEmpty()
                .MustAsync(async (id, ctx) => await unitOfWork.RecipeRepository.AnyAsync(u => u.RecipeId == id, ctx))
                .WithMessage("La receta objetivo no existe");
        }
    }
}
