using Domain.Entities;

namespace Domain.Dto
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string? Owner { get; set; }
        public Guid RecipeId { get; set; }
        public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
    }

}
