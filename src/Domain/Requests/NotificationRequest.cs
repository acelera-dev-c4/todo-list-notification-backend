namespace Domain.Requests;

public class NotificationRequest
{
	public int SubscriptionId { get; set; }
	public string Message { get; set; } = "";
	public bool Readed { get; set; } = false;
}
