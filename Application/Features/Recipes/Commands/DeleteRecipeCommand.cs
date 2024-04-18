using LanguageExt.Common;
using MediatR;

namespace Application.Features.Recipes.Commands
{
    public class DeleteRecipeCommand :IRequest<Result<string>>
    {
        public Guid Id { get; set; }
    }
}
