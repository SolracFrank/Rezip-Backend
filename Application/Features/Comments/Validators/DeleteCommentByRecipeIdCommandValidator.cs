using Application.Features.Comments.Commands;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.Comments.Validators
{
    public class DeleteCommentByRecipeIdCommandValidator : AbstractValidator<DeleteCommentByRecipeIdCommand>
    {
        public DeleteCommentByRecipeIdCommandValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(r => r.UserId)
                .NotEmpty()
                .MustAsync(async (userid, ctx) => await unitOfWork.UserRepository.AnyAsync(u => u.SubId == userid, ctx))
                .WithMessage("El usuario solicitante no existe");

            RuleFor(r => r.RecipeId)
                .NotEmpty()
                .MustAsync(async (id, ctx) => await unitOfWork.RecipeRepository.AnyAsync(u => u.RecipeId == id, ctx)
                                              && await unitOfWork.CommentRepository.AnyAsync(c => c.Recipe == id, ctx))
                .WithMessage("La receta no existe o no pertenece al usuario");


        }
    }
}
