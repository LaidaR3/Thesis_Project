using System.Net.Http.Json;
using ResourceService.DTOs;

namespace ResourceService.Services
{
    public class AuditClient
    {
        private readonly HttpClient _httpClient;

        public AuditClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task LogAsync(
            string userId,
            string email,
            string role,
            string endpoint,
            string method,
            string result)
        {
            var log = new AuditLogDto
            {
                UserId = userId,
                Email = email,
                Role = role,
                ServiceName = "resource-service",
                Endpoint = endpoint,
                HttpMethod = method,
                Result = result
            };

            await _httpClient.PostAsJsonAsync(
                "http://localhost:5077/api/audit",
                log
            );
        }
    }
}
