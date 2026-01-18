using System.Net.Http.Headers;
using AuthService.DTOs.Audit;

namespace AuthService.Services
{
    public class AuditClient
    {
        private readonly HttpClient _httpClient;
        private readonly JwtService _jwtService;

        public AuditClient(HttpClient httpClient, JwtService jwtService)
        {
            _httpClient = httpClient;
            _jwtService = jwtService;
        }

        public async Task LogAsync(CreateAuditLogDto dto)
        {
            var token = _jwtService.GenerateServiceToken("auth-service");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            await _httpClient.PostAsJsonAsync("/api/audit", dto);

        }
    }
}
