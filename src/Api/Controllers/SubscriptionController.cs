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
public class SubscriptionController : Controller
{
    private readonly IUnsubscriptionService _unsubscriptionService;
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(IUnsubscriptionService unsubscriptionService, ISubscriptionService subscriptionService)
    {
        _unsubscriptionService = unsubscriptionService;
        _subscriptionService = subscriptionService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SubscriptionsRequest subscriptionsRequest)
    {
        var newSubscription = await _subscriptionService.Create(subscriptionsRequest);        
        return Ok(newSubscription);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int subscriptionId)
    {
        await _unsubscriptionService.Delete(subscriptionId);
        return NoContent();
    }
}
