using System.Net.Http.Json;
using AuthService.DTOs.Audit;

public class AuditClient
{
    private readonly HttpClient _client;

    public AuditClient(HttpClient client)
    {
        _client = client;
    }

   
    public async Task LogAsync(string result)
    {
        await _client.PostAsJsonAsync("/api/audit", new
        {
            Result = result
        });
    }

    public async Task LogAsync(CreateAuditLogDto dto)
    {
        await _client.PostAsJsonAsync("/api/audit", dto);
    }

    public void SetAuthorization(string token)
    {
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
}
