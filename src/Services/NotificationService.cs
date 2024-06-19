using Domain.Models;
using Domain.Requests;
using Infra.DB;

namespace Services;

public interface INotificationService
{
	Task<Notifications> Create(NotificationRequest notification);
}

public class NotificationService : INotificationService
{
	private readonly MyDBContext _myDBContext;

	public NotificationService(MyDBContext context)
	{
		_myDBContext = context;
	}

	public async Task<Notifications> Create(NotificationRequest notification)
	{
		var newNotification = new Notifications
		{
			SubscriptionId = notification.SubscriptionId,
			Message = notification.Message,
			Readed = notification.Readed,
		};

		_myDBContext.Notifications.Add(newNotification);
		await _myDBContext.SaveChangesAsync();

		return newNotification;
	}
}
