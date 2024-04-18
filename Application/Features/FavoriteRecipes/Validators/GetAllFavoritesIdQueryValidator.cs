using Application.Features.FavoriteRecipes.Queries;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.FavoriteRecipes.Validators
{
    public class GetAllFavoritesIdQueryValidator : AbstractValidator<GetAllFavoritesIdQuery>
    {
        public GetAllFavoritesIdQueryValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(r => r.UserId)
                .NotEmpty()
                .MustAsync(async (id, ctx) => await unitOfWork.UserRepository.AnyAsync(u => u.SubId == id, ctx))
                .WithMessage("El usuario solicitante no existe");
        }
    }
}
