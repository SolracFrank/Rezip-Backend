using Application.Features.Users.Commands;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.Users.Validators
{
    public class UpdateUserComamandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserComamandValidator(IUnitOfWork unitOfWork)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(u => u.UserId)
                .NotEmpty()
                .MustAsync(async (id, cancellationToken) => await unitOfWork.UserRepository.AnyAsync(u => u.UserId == id, cancellationToken))
                .WithMessage("Usuario no encontrado");

            RuleFor(r => r.Username)
                .MaximumLength(100).WithMessage("La longitud máxima de {PropertyName} es de 100")
                .MustAsync(async (username, ctx) =>
                    !await unitOfWork.UserRepository.AnyAsync(u => u.Username == username, ctx))
                .WithMessage("{PropertyValue} ya existe");

            RuleFor(r => r.Name)
                .MaximumLength(100).WithMessage("La longitud máxima de {PropertyName} es de 100");

            RuleFor(r => r.Lastname)
                .MaximumLength(100).WithMessage("La longitud máxima de {PropertyName} es de 100");

        }
    }
}
