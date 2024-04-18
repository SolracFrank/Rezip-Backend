namespace Domain.Dto
{
    public class UserDto
    {
        public string UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string Username { get; set; } = null!;
        public string? CreatedBy { get; set; } = null!;

    }
}
