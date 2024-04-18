
using Domain.Entities;

namespace Domain.Dto.Extensions
{
    public static class RecipeLogoExtension
    {
        public static RecipeLogoDto ToRecipeLogoDto(this RecipeLogo recipe)
        {
            return new RecipeLogoDto
            {
                Id = recipe?.RecipeId,
                Logo = recipe?.Logo
            };
        }
        public static IEnumerable<RecipeLogoDto> ToRecipeLogoDto(this IEnumerable<RecipeLogo> files)
        {
            return files.Select(file => file.ToRecipeLogoDto());
        }
    }
}
