using Infrastructure.Data;

namespace Application.Features.Users.Handlers;
using Application.Features.Users.Queries;
using Domain.Dto;
using Domain.Dto.Extensions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<IEnumerable<UserDto>>>
{
    private readonly ILogger<GetUsersQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RecipesDbContext _context;

    public GetUsersQueryHandler(ILogger<GetUsersQueryHandler> logger, IUnitOfWork unitOfWork, RecipesDbContext context)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _context = context;
    }
    public async Task<Result<IEnumerable<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.UserRepository.GetAllAsync(cancellationToken, true);

        //var usersDto = Mapper.Map<User, UserDto>(users);
        var usersDto = users.ToUserDto();

        return new Result<IEnumerable<UserDto>>(usersDto);
    }
}
