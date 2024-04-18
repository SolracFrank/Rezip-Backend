using LanguageExt.Common;
using MediatR;

namespace Application.Features.Recipes.Commands
{
    public class UpdateRecipeCommand : IRequest<Result<string>>
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; } = null!;

        public string Procedures { get; set; } = null!;

        public Guid RecipeId { get; set; }

        public byte[]? Logo { get; set; }

        public string? MimeType { get; set; } 

        public string? FileFormat { get; set; } 
    }
}
