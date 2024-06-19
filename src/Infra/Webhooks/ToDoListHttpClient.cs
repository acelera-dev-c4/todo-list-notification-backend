using System.Text;
using System.Text.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;


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

	//public async void AdviseToDoOfSubscription(int mainTaskId)
	//{
	//    var payload = new
	//    {
	//        url = $"{httpClient.BaseAddress}Notification",
	//        mainTaskId = mainTaskId
	//    };

	//    string jsonPayload = JsonSerializer.Serialize(payload);

	//    HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
	//    await httpClient.PostAsync($"{httpClient.BaseAddress}MainTask/SetUrlWebhook", content);
	//}


	public async Task<string> GetAuthTokenAsync()
	{
		using (HttpClient client = new HttpClient())
		{
			client.BaseAddress = new Uri("https://localhost:7057");

			// Criar o objeto com as credenciais
			var loginData = new
			{
				email = "system@mail.com",
				password = "SecurePassword123"
			};

			// Serializar o objeto para JSON
			string json = JsonConvert.SerializeObject(loginData);
			HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

			// Enviar a solicitação POST com o corpo da requisição
			HttpResponseMessage response = await client.PostAsync("/Auth", content);

			if (response.IsSuccessStatusCode)
			{
				string responseBody = await response.Content.ReadAsStringAsync();
				AuthResponse authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseBody);
				return authResponse.token.token;
			}
			else
			{
				throw new Exception("Failed to authenticate");
			}
		}
	}


	public async Task SetUrlWebhookAsync(string token)
	{
		using (HttpClient client = new HttpClient())
		{
			client.BaseAddress = new Uri("https://localhost:7057");
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

			var data = new
			{
				url = "https://localhost:7057/Notification",
				mainTaskId = 16
			};
			string json = JsonConvert.SerializeObject(data);
			HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

			HttpResponseMessage response = await client.PostAsync("/MainTask/SetUrlWebhook", content);

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to set URL webhook");
			}
		}
	}

	public async Task ExecuteAsync()
	{
		try
		{
			string token = await GetAuthTokenAsync();
			await SetUrlWebhookAsync(token);
			Console.WriteLine("URL webhook set successfully.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred: {ex.Message}");
		}
	}

}


public class Token
{
	public string token { get; set; }
	public DateTime expiration { get; set; }
}

public class UserData
{
	public int id { get; set; }
	public string name { get; set; }
	public string email { get; set; }
}

public class AuthResponse
{
	public Token token { get; set; }
	public UserData userData { get; set; }
}


