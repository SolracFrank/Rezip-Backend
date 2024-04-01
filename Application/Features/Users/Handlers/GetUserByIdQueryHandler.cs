using Application.Features.Users.Queries;
using Application.Features.Users.Validators;
using Domain.Dto;
using Domain.Dto.Extensions;
using Domain.Interfaces;
using FluentValidation;
using Infrastructure.Data;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
    {
        public GetUserByIdQueryHandler(ILogger<GetUsersQueryHandler> logger, IUnitOfWork unitOfWork, RecipesDbContext context)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        private readonly ILogger<GetUsersQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RecipesDbContext _context;

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {

            var validator = new GetUserByIdQueryValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Company with request id not exists.");

                var validationException = new ValidationException(validationResult.Errors);

                return new Result<UserDto>(validationException);
            }


            var user = await _unitOfWork.UserRepository.FindAsync(cancellationToken, request.UserId);
            
            await _context.Entry(user).Reference(user => user.CreatedByNavigation)
                .LoadAsync();

            _logger.LogInformation($"User {user.Username} created by {user?.CreatedByNavigation?.Username}");

            return new Result<UserDto>(user.ToUserDto());
        }
    }
}
