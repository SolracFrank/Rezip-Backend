using Domain.Dto;
using LanguageExt.Common;
using MediatR;

namespace Application.Features.Users.Commands
{
    public class CreateUserCommand : IRequest<Result<UserDto>>
    {

        public string Name { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

    }
}
