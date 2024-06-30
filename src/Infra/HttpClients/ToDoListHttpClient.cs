using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Infra;

public interface IToDoListHttpClient
{
    Task<HttpResponseMessage> SetUrlWebhook(int _mainTaskId);
}

public class ToDoListHttpClient : IToDoListHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ToDoListHttpClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient("toDoClient");
    }

    public async Task<HttpResponseMessage> SetUrlWebhook(int _mainTaskId)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenSystemUserFromToDoList());

        var payload = new
        {
            url = $"{_configuration["NotificationApi:BaseUrl"]}Notification",
            mainTaskId = _mainTaskId,
        };

        string jsonPayload = JsonSerializer.Serialize(payload);
        HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var result = await _httpClient.PutAsync($"{_httpClient.BaseAddress}MainTask/SetUrlWebhook", content); //401

        return result;
    }

    private async Task<string> GetTokenSystemUserFromToDoList()
    {
        var login = new
        {
            email = _configuration["ToDoListApi:Email"],
            password = _configuration["ToDoListApi:Password"]
        };

        string jsonLogin = JsonSerializer.Serialize(login);
        HttpContent loginContent = new StringContent(jsonLogin, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}Auth", loginContent);

        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        string token;

        using (JsonDocument doc = JsonDocument.Parse(responseBody))
        {
            JsonElement root = doc.RootElement;
            JsonElement tokenElement = root.GetProperty("token");
            token = tokenElement.GetProperty("token").GetString()!;
        }

        return token;
    }
}