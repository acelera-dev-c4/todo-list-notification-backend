namespace Domain.Requests;

public class NotificationRequest
{
	public int SubscriptionId { get; set; }
	public string Message { get; set; } = "";
	public bool Readed { get; set; } = false;
	public int UserId {  get; set; }
}

public class NotificationUpdate
{
	public bool Readed { get; set; } = false;
}