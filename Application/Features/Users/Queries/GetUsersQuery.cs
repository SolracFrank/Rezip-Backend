namespace Application.Features.Users.Queries;

using Domain.Dto;
using LanguageExt.Common;
using MediatR;

public class GetUsersQuery : IRequest<Result<IEnumerable<UserDto>>>
{

}
