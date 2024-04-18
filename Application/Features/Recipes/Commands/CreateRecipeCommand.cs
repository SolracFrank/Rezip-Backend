using Domain.Dto;
using LanguageExt.Common;
using MediatR;

namespace Application.Features.Recipes.Commands
{
    public class CreateRecipeCommand : IRequest<Result<RecipeDto>>
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string Procedures { get; set; } = null!;

        public string CreatedBy { get; set; } = null!;

        public Guid RecipeId { get; set; }

        public byte[]? Logo { get; set; }

        public string MimeType { get; set; } = null!;

        public string FileFormat { get; set; } = null!;

    }
}
