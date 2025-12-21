using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AuthService.Services
{
    public class AuditClient
    {
        private readonly HttpClient _http;

        public AuditClient(HttpClient http)
        {
            _http = http;
        }

        public void SetAuthorization(string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task LogAsync(string result)
        {
            await _http.PostAsJsonAsync("/api/audit", new
            {
                result = result
            });
        }
    }
}
