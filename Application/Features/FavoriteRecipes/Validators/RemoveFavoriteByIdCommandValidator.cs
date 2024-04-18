using Application.Features.FavoriteRecipes.Command;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.FavoriteRecipes.Validators
{
    public class RemoveFavoriteByIdCommandValidator : AbstractValidator<RemoveFavoriteByIdCommand>
    {
        public RemoveFavoriteByIdCommandValidator(IUnitOfWork unitOfWork)
        {
             ClassLevelCascadeMode = CascadeMode.Stop;

             RuleFor(r => r.RecipeId)
                 .NotEmpty()
                 .MustAsync(async (id, ctx) => await unitOfWork.RecipeRepository.AnyAsync(x => x.RecipeId == id, ctx))
                 .WithMessage("La receta no existe");

             RuleFor(r => r.UserId)
                 .NotEmpty()
                 .MustAsync(async (id, ctx) => await unitOfWork.UserRepository.AnyAsync(x => x.SubId == id, ctx))
                 .WithMessage("El usuario no existe");
        }
    }
}
