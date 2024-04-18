using Domain.Entities;

namespace Domain.Dto.Extensions
{
    public static class CommentExtension
    {
        public static CommentDto ToCommentDto (this Comment comment, int maxDepth, int currentDepth = 0)
        {
            var nextDepth = currentDepth + 1;
            return new CommentDto
            {
                Id = comment.Id,
                RecipeId = comment.Recipe,
                Content = comment.Comment1,
                Replies = currentDepth >= maxDepth ?
                    new List<CommentDto>()
                    : comment.InverseResponseNavigation.ToCommentDto(maxDepth, nextDepth).ToList(),
            };
        }
        public static IEnumerable<CommentDto> ToCommentDto(this IEnumerable<Comment> comment, int maxDepth, int currentDepth = 0)
        {
            return comment.Select(c => c.ToCommentDto(maxDepth, currentDepth));
        }

        
    }
}
