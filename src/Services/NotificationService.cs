using Domain.Models;
using Domain.Requests;
using Infra.DB;
using Domain.Mappers;
using Microsoft.EntityFrameworkCore;
using Infra.HttpClients;

namespace Services;

public interface INotificationService
{
	Task<Notifications> Create(NotificationRequest notification);
	Task<List<Notifications>> GetByUserId(int userId);
}

public class NotificationService : INotificationService
{
	private readonly MyDBContext _myDBContext;
    private readonly INotificationHttpClient _notificationHttpClient;

    public NotificationService(MyDBContext context,INotificationHttpClient notificationHttpClient)
	{
		_myDBContext = context;
		_notificationHttpClient = notificationHttpClient;
	}

	public async Task<Notifications> Create(NotificationRequest notification)
	{
		var newNotification = NotificationMapper.ToClass(notification);

		_myDBContext.Notifications.Add(newNotification);
		await _myDBContext.SaveChangesAsync();

		return newNotification;
	}

    public async Task<List<Notifications>> GetByUserId(int userId)
    {
		var jwt = await _notificationHttpClient.GetJWTAsync();
		await _notificationHttpClient.GetMainTaskByUserId(userId);





        return await _myDBContext.Notifications.Where(n => n.SubscriptionId== userId).OrderByDescending(nameof => nameof.Id).ToListAsync();//Corrigir
    }

	public int GetUserId(int userId)
	{

		var Id = userId;
		return Id;


	}

	
	
}
