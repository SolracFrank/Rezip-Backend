using LanguageExt.Common;
using MediatR;

namespace Application.Features.Comments.Commands
{
    public class DeleteCommentByUserCommand : IRequest<Result<string>>
    {
        public Guid CommentId { get; set; }
        public string? UserId { get; set; }
    }
}
