using Application.Features.Users.Commands;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.Users.Validators
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(r => r.Username)
                .NotEmpty()
                .MaximumLength(100).WithMessage("La longitud máxima de {PropertyName} es de 100")
                .MustAsync(async (username, ctx) =>
                    !await unitOfWork.UserRepository.AnyAsync(u => u.Username == username, ctx))
                .WithMessage("{PropertyValue} ya existe");

            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(100).WithMessage("La longitud máxima de {PropertyName} es de 100");

            RuleFor(r => r.Lastname)
                .NotEmpty()
                .MaximumLength(100).WithMessage("La longitud máxima de {PropertyName} es de 100");

            RuleFor(r => r.Password)
                .NotEmpty()
                .MinimumLength(8)
                .WithMessage("La contraseña debe tener al menos 8 caracteres")
                .MaximumLength(25)
                .WithMessage("La contraseña debe máximo 25 caracteres")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,25}$")
                .WithMessage("La contraseña debe contener al menos una letra en mayúscula, una en minúscula, un número y un carácter especial.");

        }
    }
}
