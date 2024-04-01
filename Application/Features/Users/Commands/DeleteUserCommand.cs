using LanguageExt.Common;
using MediatR;

namespace Application.Features.Users.Commands
{
    public class DeleteUserCommand : IRequest<Result<string>>
    {
        public Guid UserId { get; set; }
    }
}
