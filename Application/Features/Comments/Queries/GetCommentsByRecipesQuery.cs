using Domain.Dto;
using Domain.Entities;
using LanguageExt.Common;
using MediatR;

namespace Application.Features.Comments.Queries
{
    public class GetCommentsByRecipesQuery : IRequest<Result<IEnumerable<CommentDto>>>
    {
        public Guid RecipeId { get; set; }
    }
}
