namespace Application.Features.Users.Queries;

using Domain.Dto;
using Domain.Entities;
using LanguageExt.Common;
using MediatR;

public class GetUsersQuery : IRequest<Result<IEnumerable<UserDto>>>
{

}
