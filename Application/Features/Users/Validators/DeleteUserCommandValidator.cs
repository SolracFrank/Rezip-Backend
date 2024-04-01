using Application.Features.Users.Commands;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.Users.Validators
{
    internal class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator(IUnitOfWork unitOfWork)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(r => r.UserId)
                .NotEmpty()
                .MustAsync(async (id, ctx) => await unitOfWork.UserRepository.AnyAsync(u => u.UserId == id, ctx))
                .WithMessage("El usuario a eliminar no existe");
        }
    }
}
