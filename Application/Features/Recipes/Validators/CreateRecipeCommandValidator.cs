using Application.Features.Recipes.Commands;
using Domain.Custom;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.Recipes.Validators
{
    public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
    {
        public CreateRecipeCommandValidator(IUnitOfWork unitOfWork, List<RecipeLogosTypes> recipeLogosTypes)
        {

            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(100).WithMessage("{propertyName} max length is 100");
            RuleFor(r => r.Description)
                .MaximumLength(255).WithMessage("{propertyName} max length is 255");

            RuleFor(r => r.Procedures)
                .NotEmpty();

            RuleFor(r => r.CreatedBy)
                .NotEmpty()
                .MustAsync(async (id, ctx) =>
                    await unitOfWork.UserRepository.AnyAsync(x => x.SubId == id, ctx))
                .WithMessage("Requesting user doesn't exist");

            RuleFor(r => r.Logo).NotEmpty();

            RuleFor(r => r.MimeType)
                .Must(type => recipeLogosTypes.Any(mime => mime.Type == type))
                .WithMessage("La imagen no tiene el formato correcto");

        }
    }
}
