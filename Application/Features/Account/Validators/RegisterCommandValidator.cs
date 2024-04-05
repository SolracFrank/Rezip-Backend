using Application.Features.Account.Commands;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.Account.Validators
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator(IUnitOfWork unitOfWork)
        { 
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(r => r.SubId)
                .NotEmpty()
                .MaximumLength(255).WithMessage("La longitud máxima de {PropertyName} es de 255");
        }
    }
}
