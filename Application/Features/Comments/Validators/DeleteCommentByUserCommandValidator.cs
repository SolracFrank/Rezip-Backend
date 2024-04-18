using Application.Features.Comments.Commands;
using Domain.Interfaces;
using FluentValidation;

namespace Application.Features.Comments.Validators
{
    public class DeleteCommentByUserCommandValidator : AbstractValidator<DeleteCommentByUserCommand>
    {
        public DeleteCommentByUserCommandValidator(IUnitOfWork unitOfWork)
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleFor(r => r.UserId)
                .NotEmpty()
                .MustAsync(async (userid, ctx) => await unitOfWork.UserRepository.AnyAsync(u => u.SubId == userid, ctx))
                .WithMessage("El usuario solicitante no existe");

            RuleFor(r => r)
                .MustAsync(async (model, ctx) => await unitOfWork.CommentRepository
                    .AnyAsync(u => u.Id == model.CommentId && u.Owner == model.UserId, ctx))
                .WithMessage("El usuario solicitante no es dueño del comentario");
        }
    }
}
