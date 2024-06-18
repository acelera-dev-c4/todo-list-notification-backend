using System.Text;
using System.Text.Json;

namespace Infra;

public class ToDoListHttpClient
{
    private readonly HttpClient httpClient;

    public ToDoListHttpClient()
    {
        httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://localhost:7057/");
    }
    // exemplo, nao esta funcionando ainda
    public async void UpdateMainTaskAsFinished(int mainTaskId)
    {
        HttpContent? placeholder = null;
        await httpClient.PostAsync($"{httpClient.BaseAddress}Notification/{mainTaskId}", placeholder);
    }

    public async void AdviseToDoOfSubscription(int mainTaskId)
    {
        var payload = new
        {
            url = $"{httpClient.BaseAddress}Notification",
            mainTaskId = mainTaskId
        };

        string jsonPayload = JsonSerializer.Serialize(payload);

        HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        await httpClient.PostAsync($"{httpClient.BaseAddress}MainTask/SetUrlWebhook", content);
    }

}