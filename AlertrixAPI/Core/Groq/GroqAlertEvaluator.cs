using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class GroqAlertEvaluator
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public GroqAlertEvaluator(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Groq_AI_Key"]
                  ?? throw new Exception("GROQ_API_KEY missing");
    }

    public async Task<bool> IsAlertTriggeredAsync(string alertCondition)
    {
        var requestBody = new
        {
            model = "llama3-70b-8192",
            messages = new[]
            {
                new { role = "system", content = "Answer ONLY true or false." },
                new { role = "user", content = alertCondition }
            }
        };

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.groq.com/openai/v1/chat/completions"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _apiKey);

        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        return json.Contains("true", StringComparison.OrdinalIgnoreCase);
    }
}
