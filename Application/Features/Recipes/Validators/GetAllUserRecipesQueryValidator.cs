using Application.Features.Recipes.Queries;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.Recipes.Validators
{
    public class GetAllUserRecipesQueryValidator :AbstractValidator<GetAllUserRecipesQuery>
    {
        public GetAllUserRecipesQueryValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(r => r.userId).MustAsync(async (id, ctx) =>
                    await unitOfWork.UserRepository.AnyAsync(x => x.SubId == id, ctx))
                .WithMessage("El usuario no existe");
        }
    }
}
