using Application.Features.Users.Queries;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.Users.Validators
{
    public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
    {
        public GetUserByIdQueryValidator(IUnitOfWork unitOfWork)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(u => u.UserId)
                .NotEmpty()
                .MustAsync(async (id, cancellationToken) => await unitOfWork.UserRepository.AnyAsync(u => u.UserId == id, cancellationToken))
                .WithMessage("Usuario no encontrado");
        }
    }
}
