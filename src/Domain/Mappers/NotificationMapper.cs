using Domain.Requests;
using Domain.Models;

namespace Domain.Mappers;

public class NotificationMapper
{
	public static Notifications ToClass(NotificationRequest notificationRequest) => new Notifications
	{
		Id = null,
		SubscriptionId = notificationRequest.SubscriptionId,
		Message = notificationRequest.Message,
		Readed = notificationRequest.Readed
	};
}
