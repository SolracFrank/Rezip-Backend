using Domain.Dto;
using LanguageExt.Common;
using MediatR;

namespace Application.Features.Users.Commands
{
    public class UpdateUserCommand : IRequest<Result<UserDto>>
    {
        public Guid UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string Username { get; set; } = null!;

    }
}
