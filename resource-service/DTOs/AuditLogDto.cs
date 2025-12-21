namespace ResourceService.DTOs
{
    public class AuditLogDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public string ServiceName { get; set; }
        public string Endpoint { get; set; }
        public string HttpMethod { get; set; }

        public string Result { get; set; }
    }
}
