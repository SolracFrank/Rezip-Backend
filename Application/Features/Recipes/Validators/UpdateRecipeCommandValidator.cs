using Application.Features.Recipes.Commands;
using Domain.Custom;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.Recipes.Validators
{
    internal class UpdateRecipeCommandValidator : AbstractValidator<UpdateRecipeCommand>
    {
        public UpdateRecipeCommandValidator(IUnitOfWork unitOfWork, List<RecipeLogosTypes> recipeLogosTypes)
        {

            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(100).WithMessage("{propertyName} max length is 100");
            RuleFor(r => r.Description)
                .MaximumLength(255).WithMessage("{propertyName} max length is 255");

            RuleFor(r => r.Procedures)
                .NotEmpty();


            RuleFor(r => r.MimeType)
                .Must(type => recipeLogosTypes.Any(mime => type != null && mime.Type == type))
                .WithMessage("La imagen no tiene el formato correcto")
                .When(c => !string.IsNullOrEmpty(c.MimeType));
        }
    }
}
