using Application.Features.Comments.Queries;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.Comments.Validators
{
    public class GetCommentsByRecipesQueryValidator :AbstractValidator<GetCommentsByRecipesQuery>
    {
        public GetCommentsByRecipesQueryValidator(IUnitOfWork unitOfWork)
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(r => r.RecipeId)
                .NotEmpty()
                .MustAsync(async (id, ctx) => await unitOfWork.RecipeRepository.AnyAsync(x => x.RecipeId == id, ctx))
                .WithMessage("La receta no existe");
        }
    }
}
