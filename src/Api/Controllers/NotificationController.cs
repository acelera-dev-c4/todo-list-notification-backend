using Domain.Requests;
using Infra.HttpClients;
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
    

    public NotificationController(INotificationService notificationService, INotificationHttpClient notificationHttpClient)
    {
        _notificationService = notificationService;
        
    }

    [HttpPost]
	public async Task<IActionResult> Create([FromBody] NotificationRequest notificationRequest)
	{
		var newNotification = await _notificationService.Create(notificationRequest);
		return Ok(newNotification);
	}

	[HttpGet("User/{userId}")]
	public async Task<IActionResult> GetByUserId([FromRoute]int userId)
	{
		var notifications = await _notificationService.GetByUserId(userId);
		if (notifications.Count == 0)
		{
			return NotFound("No notifications found for this user.");
		}
		return Ok(notifications);
	}
   /* [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook([FromBody] WebhookRequest request)
    {
        await _notificationService.HandleWebhookAsync(request);
        return Ok();
    }*/
}

