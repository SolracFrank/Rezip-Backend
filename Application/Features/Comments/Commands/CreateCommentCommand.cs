using Domain.Entities;
using LanguageExt.Common;
using MediatR;

namespace Application.Features.Comments.Commands
{
    public class CreateCommentCommand : IRequest<Result<Comment>>
    {
        public string? User { get; set; }
        public string Comment { get; set; } = null!;
        public Guid? FatherComment { get; set; }
        public Guid FatherRecipe { get; set; }
    }
}
