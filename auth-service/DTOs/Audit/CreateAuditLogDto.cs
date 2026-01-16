namespace AuthService.DTOs.Audit
{
    public class CreateAuditLogDto
    {
        public string Action { get; set; } = string.Empty;
        public string TargetEndpoint { get; set; } = string.Empty;
        public string? Metadata { get; set; } 
    }
}
