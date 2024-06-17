using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Api.Controllers;

[ApiController]
[Route("[Controller]")]
[EnableCors("AllowAllHeaders")]
public class SubscriptionController : Controller
{
    private readonly IUnsubscriptionService _unsubscriptionService;

    public SubscriptionController(IUnsubscriptionService unsubscriptionService)
    {
        _unsubscriptionService = unsubscriptionService;
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int subscriptionId)
    {
        await _unsubscriptionService.Delete(subscriptionId);
        return NoContent();
    }
}
