using Microsoft.AspNetCore.Mvc;
using Services;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSubtaskAsync(int subtaskId)
    {
        var jwt = await _notificationService.GetJWTAsync();
        await _notificationService.UpdateSubtaskAsync(subtaskId, jwt);
        return Ok();
    }

}








