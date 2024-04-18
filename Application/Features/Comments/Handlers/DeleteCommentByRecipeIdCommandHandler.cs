﻿using Application.Features.Comments.Commands;
using Application.Features.Comments.Validators;
using Domain.Exceptions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Comments.Handlers
{
    public class DeleteCommentByRecipeIdCommandHandler : IRequestHandler<DeleteCommentByRecipeIdCommand, Result<string>>
    {
        public DeleteCommentByRecipeIdCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCommentByRecipeIdCommandHandler> logger, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userService = userService;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCommentByRecipeIdCommandHandler> _logger;
        private readonly IUserService _userService;
        public async Task<Result<string>> Handle(DeleteCommentByRecipeIdCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Comment does not exist.");

            var userRequest = _userService.GetUser();

            if (userRequest.IsFaulted)
            {
                return new Result<string>(new UnAuthorizedException("El usuario no tiene permitido esta acción"));
            }

            var user = userRequest.Match(user => user, _ => null!);
            request.UserId = user.UserId;

            var validator = new DeleteCommentByRecipeIdCommandValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation Errors");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<string>(validationException);
            }
            _logger.LogInformation("Getting comment");


            var comment = await _unitOfWork.CommentRepository.FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken, false);

            if (comment == null)
            {
                _logger.LogInformation("Comment does not exist.");

                return new Result<string>(new NotFoundException("El comentario no existe"));
            }
            _logger.LogInformation("Deleting comment");

            _unitOfWork.CommentRepository.Remove(comment);

            var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!saved)
            {
                _logger.LogInformation("Can not save changes");

                return new Result<string>(new InfraestructureException("No fue posible borrar el comentario."));
            }
            _logger.LogInformation("Comment deleted.");

            return new Result<string>("Borrado satisfactoriamente");

        }
    }
}
