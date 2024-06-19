using Domain.Models;
using Domain.Requests;
using Infra.DB;
using Domain.Mappers;

namespace Services;

public interface INotificationService
{
	Task<Notifications> Create(NotificationRequest notification);
	Task<List<Notifications>> List();
  Task<List<Notifications>> GetById(int mainTaskId);
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
		var newNotification = NotificationMapper.ToClass(notification);

		_myDBContext.Notifications.Add(newNotification);
		await _myDBContext.SaveChangesAsync();

		return newNotification;
	}

	public async Task<List<Notifications>> List() 
	{
		return await _myDBContext.Notifications.OrderByDescending(x=>x.Id).ToListAsync();
	}

	public async Task<List<Notifications>> GetById(int notificationId)
	{
		return await _myDBContext.Notifications.Where(x => x.Id == notificationId).ToListAsync();
	}
}
