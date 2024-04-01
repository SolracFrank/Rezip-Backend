using Domain.Dto;
using LanguageExt.Common;
using MediatR;

namespace Application.Features.Users.Queries
{
    public class GetUserByIdQuery : IRequest<Result<UserDto>>
    {
        public Guid UserId
        {
            get; set;
        }
    }
}
