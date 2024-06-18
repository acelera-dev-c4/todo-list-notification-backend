using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace todo_list_notification_backend.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/webhooks")]
    public class WebhookController : Controller
    {       
        [HttpPost("subscribesubtask")] // acesso direto do front
        public IActionResult CreateSubTaskSubscription(int subTaskId, int userId)
        {
            //return Ok(SubscriptionService.CreateSubTaskSubscription());
            return Ok($":placeholder: MainTask {subTaskId} subscribed to user {userId}");
        }

        [HttpPost("subscribemaintask")] // acesso direto do front
        public IActionResult CreateMainTaskSubscription(int mainTaskId, int userId)
        {
            //return Ok(SubscriptionService.CreateMainTaskSubscription());
            return Ok($":placeholder: MainTask {mainTaskId} subscribed to user {userId}");
        }
    }
}
