using Domain.Models;
using Domain.Requests;
using Infra.DB;
using Domain.Mappers;
using Microsoft.EntityFrameworkCore;
using Domain.Exceptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Services;

public interface INotificationService
{
	Task<Notifications> Create(NotificationRequest notification);
	Task<List<Notifications>> List();
    Task<Notifications?> GetById(int notificationId);
	Task<List<Notifications>> GetByUserId(int userId);

	Task<Notifications> Update(NotificationUpdate notificationUpdate, int notificationId);
}

public class NotificationService : INotificationService
{
	private readonly MyDBContext _myDBContext;
	private readonly SubtaskHttpClient _subtaskHttpClient;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public NotificationService(MyDBContext context, SubtaskHttpClient subtaskHttpClient, IHttpContextAccessor httpContextAccessor)
	{
		_myDBContext = context;
		_subtaskHttpClient = subtaskHttpClient;
		_httpContextAccessor = httpContextAccessor;
	}

	public async Task<Notifications> Create(NotificationRequest notification)
	{
		var newNotification = NotificationMapper.ToClass(notification);

		_myDBContext.Notifications.Add(newNotification);
		await _myDBContext.SaveChangesAsync();
		await  UpdateSubtaskAsync(notification.SubscriptionId);

		return newNotification;
	}

	public async Task<List<Notifications>> List() 
	{
		return await _myDBContext.Notifications.OrderByDescending(x=>x.Id).ToListAsync();
	}

	public async Task<Notifications?> GetById(int notificationId)
	{
		return await _myDBContext.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);
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

	public async Task<List<Notifications>> GetByUserId(int userId)
	{
		return await _myDBContext.Notifications.Where(x => x.UserId == userId).OrderByDescending(x => x.Id).ToListAsync();
	}

	public async Task<Notifications> Update(NotificationUpdate notificationUpdate, int notificationId)
	{
		var notification = await GetById(notificationId);

		if (notification is null)
			throw new NotFoundException("notification not found!");

		var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (notification.UserId.ToString() != userId)
		{
			throw new UnauthorizedAccessException("You don't have permission to update this notification.");
		}

		notification.Readed = notificationUpdate.Readed;
		await _myDBContext.Notifications
			.Where(n => n.Id == notificationId)
			.ExecuteUpdateAsync(n => n
				.SetProperty(notification => notification.Readed, notificationUpdate.Readed)
			);
		await _myDBContext.SaveChangesAsync();

		return notification;
	}
}
