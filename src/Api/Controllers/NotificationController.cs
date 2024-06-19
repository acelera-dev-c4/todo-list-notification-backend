using Domain.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Api.Controllers;

[ApiController]
[Route("[Controller]")]
[EnableCors("AllowAllHeaders")]
[Authorize]
public class NotificationController : Controller
{
	private readonly INotificationService _notificationService;

	public NotificationController(INotificationService notificationService)
	{
		_notificationService = notificationService;
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] NotificationRequest notificationRequest)
	{
		var newNotification = await _notificationService.Create(notificationRequest);
		return Ok(newNotification);
	}

	[HttpGet]
	public async Task<IActionResult> Get()
	{
		var notifications = await _notificationService.List();
		return Ok(notifications);
	}

	

}

