namespace Service;
public class WebhooksService
{
    private readonly HttpClient httpClient;

    public WebhooksService()
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
}