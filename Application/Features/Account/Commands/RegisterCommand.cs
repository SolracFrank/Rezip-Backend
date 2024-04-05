using LanguageExt.Common;
using MediatR;

namespace Application.Features.Account.Commands
{
    public class RegisterCommand : IRequest<Result<bool>>
    {
        public string? Name { get; set; } = null!;
        public string? SubId { get; set; } = null!;

    }
}
