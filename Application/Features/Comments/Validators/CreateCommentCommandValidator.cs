using Application.Features.Comments.Commands;
using Domain.Interfaces;
using FluentValidation;
using LanguageExt;

namespace Application.Features.Comments.Validators
{
    public class CreateCommentCommandValidator: AbstractValidator<CreateCommentCommand>
    {
        public CreateCommentCommandValidator(IUnitOfWork unitOfWork)
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(r => r.Comment)
                .NotEmpty()
                .MaximumLength(300).WithMessage("Carácteres máximos para un comentario es de 300");

            RuleFor(r => r.User)
                .NotEmpty()
                .MustAsync(async (id, ctx) => await unitOfWork.UserRepository.AnyAsync(x => x.SubId == id, ctx))
                .WithMessage("El comentador no existe");

            RuleFor(r => r.FatherRecipe)
                .NotEmpty()
                .MustAsync(async (id, ctx) => await unitOfWork.RecipeRepository.AnyAsync(x => x.RecipeId == id, ctx))
                .WithMessage("La receta a comentar no existe");

            RuleFor(r => r.FatherComment)
                .MustAsync(async (id, ctx) =>
                    id == null || await unitOfWork.CommentRepository.AnyAsync(x => x.Id == id, ctx))
                .WithMessage("El comentario a responder no existe");
        }
    }
}
