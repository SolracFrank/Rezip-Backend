using Application.Features.Comments.Commands;
using Application.Features.Comments.Validators;
using Application.Features.Recipes.Handlers;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Comments.Handlers
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<Comment>>
    {
        public CreateCommentCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateRecipeCommandHandler> logger, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userService = userService;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateRecipeCommandHandler> _logger;
        private readonly IUserService _userService;
        public async Task<Result<Comment>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Validating request user");
            var userRequest = _userService.GetUser();

            if (userRequest.IsFaulted)
            {
                _logger.LogInformation("User is not authorized");

                return new Result<Comment>(new UnAuthorizedException("El usuario no tiene permitido esta acción"));
            }

            var user = userRequest.Match(user => user, _ => null!);
            request.User = user.UserId;

            var validator = new CreateCommentCommandValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            _logger.LogInformation("Validating errors");

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation Errors");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<Comment>(validationException);
            }
            _logger.LogInformation("Validation passed");

            var comment = new Comment
            {
                Recipe = request.FatherRecipe,
                Comment1 = request.Comment,
                Id = Guid.NewGuid(),
                Owner = request.User,
                Response = request.FatherComment,
            };
            _logger.LogInformation("Trying to save changes");

            await _unitOfWork.CommentRepository.AddAsync(comment, cancellationToken);
            var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!saved)
            {
                _logger.LogInformation("Can not save changes");

                return new Result<Comment>(new InfraestructureException("No fue posible crear la receta."));
            }
            _logger.LogInformation("Comment created.");

            return new Result<Comment>(comment);
        }
    }
}
