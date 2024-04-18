using Application.Features.Comments.Queries;
using Application.Features.Comments.Validators;
using Application.Features.Recipes.Handlers;
using Domain.Dto;
using Domain.Dto.Extensions;
using Domain.Interfaces;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Comments.Handlers
{
    public class GetCommentsByRecipesQueryHandler : IRequestHandler<GetCommentsByRecipesQuery, Result<IEnumerable<CommentDto>>>
    {
        public GetCommentsByRecipesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllUserRecipesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllUserRecipesQueryHandler> _logger;
        public async Task<Result<IEnumerable<CommentDto>>> Handle(GetCommentsByRecipesQuery request, CancellationToken cancellationToken)
        {

            var validator = new GetCommentsByRecipesQueryValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation Errors");

                var validationException = new FluentValidation.ValidationException(validationResult.Errors);

                return new Result<IEnumerable<CommentDto>>(validationException);
            }
            var allComments = await _unitOfWork.CommentRepository
                .Where(c => c.Recipe == request.RecipeId)
                .ToListAsync();

            var top = allComments.Where(c => c.Response == null);

            var commentDtos = top.Select(c => c.ToCommentDto(5, 0)).ToList();

            return new Result<IEnumerable<CommentDto>>(commentDtos);
        }
    }
}

