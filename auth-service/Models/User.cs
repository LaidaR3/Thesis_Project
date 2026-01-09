namespace auth_service.Models
{
    public class User
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
