using LanguageExt.Common;
using MediatR;

namespace Application.Features.Comments.Commands
{
    public class DeleteCommentByRecipeIdCommand : IRequest<Result<string>>
    {
        public string? UserId { get; set; }
        public Guid CommentId { get; set; }
        public Guid RecipeId { get; set; }
    }
}
