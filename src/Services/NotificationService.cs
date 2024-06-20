using Domain.Models;
using Domain.Requests;
using Infra.DB;
using Domain.Mappers;
using Microsoft.EntityFrameworkCore;
using Domain.Exceptions;

namespace Services;

public interface INotificationService
{
	Task<Notifications> Create(NotificationRequest notification);
}

public class NotificationService : INotificationService
{
	private readonly MyDBContext _myDBContext;
	private readonly SubtaskHttpClient _subtaskHttpClient;

	public NotificationService(MyDBContext context, SubtaskHttpClient subtaskHttpClient)
	{
		_myDBContext = context;
		_subtaskHttpClient = subtaskHttpClient;
	}

	public async Task<Notifications> Create(NotificationRequest notification)
	{
		var newNotification = NotificationMapper.ToClass(notification);

		_myDBContext.Notifications.Add(newNotification);
		await _myDBContext.SaveChangesAsync();
		await  UpdateSubtaskAsync(notification.SubscriptionId);

		return newNotification;
	}

    private async Task UpdateSubtaskAsync(int subscriptionId)
    {
		var subtaskId = await GetSubtaskAsync(subscriptionId);				
        var jwt = await _subtaskHttpClient.GetJWTAsync();
        await _subtaskHttpClient.UpdateSubtaskAsync(subtaskId, jwt);
    }

    private async Task<int> GetSubtaskAsync(int subscriptionId)
    {
		var subscription = await _myDBContext.Subscriptions.FindAsync(subscriptionId);

        if (subscription != null)
        {
			return subscription.SubTaskIdSubscriber;
        }
        else
        {
            throw new NotFoundException("Subscription not found");
        }

    }
}
