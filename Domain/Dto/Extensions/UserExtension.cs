using Domain.Entities;

namespace Domain.Dto.Extensions
{
    public static class UserExtension
    {
        public static UserDto ToUserDto(this User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Lastname =user.Lastname,
                Name = user.Name,
                Username = user.Username,
                CreatedBy = user.CreatedByNavigation?.Username
            };
        }
        public static IEnumerable<UserDto> ToUserDto(this IEnumerable<User> files)
        {
            return files.Select(file => file.ToUserDto());
        }
    }
}
