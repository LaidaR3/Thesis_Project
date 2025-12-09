namespace UserService.Models
{
    public class User
    {
        public int Id { get; set; }
        public int AuthUserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Department { get; set; }
        public string? Notes { get; set; }
    }
}
