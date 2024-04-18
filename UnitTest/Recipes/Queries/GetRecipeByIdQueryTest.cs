using Application.Features.Recipes.Handlers;
using Application.Features.Recipes.Queries;
using Domain.Dto.Extensions;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Linq.Expressions;
using FluentValidation;
using Xunit;

namespace UnitTest.Recipes.Queries
{
    public class GetRecipeByIdQueryTest
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly ILogger<GetRecipeByIdQueryHandler> _logger = Substitute.For<ILogger<GetRecipeByIdQueryHandler>>();
        private readonly GetRecipeByIdQueryHandler sut;

        public GetRecipeByIdQueryTest( )
        {
            sut = new GetRecipeByIdQueryHandler(_unitOfWork,_logger);
        }

        [Fact]
        public async Task Get_Recipe_By_Id_Excepts_Correct()
        {
            //Arrange
            var query = new GetRecipeByIdQuery { id = Guid.NewGuid() };

            var recipe = new Recipe
            {
                Name = "Agua hervida",
                Description = "Agua hervida",
                Procedures = "Hervir el agua",
                RecipeId = query.id,
                CreatedBy = "Carlos",
            };

            _unitOfWork.RecipeRepository
                .AnyAsync(Arg.Any<Expression<Func<Recipe, bool>>>(), Arg.Any<CancellationToken>()).Returns(true);

            _unitOfWork.RecipeRepository.FindAsync(Arg.Any<CancellationToken>(), Arg.Any<object[]>()).Returns(recipe);

            //Act
            var result = await sut.Handle(query, CancellationToken.None);

            //Assert
            var recipeResult = result.Match(recipe => recipe, _ => null!);

            recipeResult.Should().NotBeNull();
            recipeResult.Should().BeEquivalentTo(recipe.ToRecipeDto());
        }
        [Fact]
        public async Task Get_Recipe_By_Id_Excepts_Incorrect_Does_Not_Exist()
        {
            //Arrange
            var query = new GetRecipeByIdQuery { id = Guid.NewGuid() };

            _unitOfWork.RecipeRepository
                .AnyAsync(Arg.Any<Expression<Func<Recipe, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);

            //Act
            var result = await sut.Handle(query, CancellationToken.None);

            //Assert
            var exception = result.Match(_ => null!, ex => ex);
            var validationException = exception as ValidationException;
            validationException.Should().NotBeNull();
            validationException.Errors.FirstOrDefault().ErrorMessage.Should().Be("La receta a obtener no existe");
        }
        [Fact]
        public async Task Get_Recipe_By_Id_Excepts_Incorrect_Id_Empty()
        {
            //Arrange

            //Act
            var result = await sut.Handle(new GetRecipeByIdQuery(), CancellationToken.None);

            //Assert
            var exception = result.Match(_ => null!, ex => ex);
            var validationException = exception as ValidationException;
            validationException.Should().NotBeNull();
            validationException.Errors.FirstOrDefault().ErrorMessage.Should().Be("'id' no debería estar vacío.");
        }
    }
}
