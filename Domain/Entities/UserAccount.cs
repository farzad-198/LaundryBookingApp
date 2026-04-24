using Domain.Enums;

namespace Domain.Entities
{
    public class UserAccount
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public Guid PersonId { get; set; }

        public Person Person { get; set; } = null!;
    }
}