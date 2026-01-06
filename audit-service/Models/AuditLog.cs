namespace AuditService.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string? UserId { get; set; }  
        public string? Email { get; set; }
        public string? Role { get; set; }

        public string ServiceName { get; set; }
        public string Endpoint { get; set; }
        public string HttpMethod { get; set; }

        public string Result { get; set; } // Allowed / Denied
        public DateTime Timestamp { get; set; }
    }
}
