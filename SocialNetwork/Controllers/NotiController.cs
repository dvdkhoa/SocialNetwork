using Microsoft.AspNetCore.Mvc;
using SocialNetwork.BLL.Services;

namespace SocialNetwork.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotiController : ControllerBase
    {
        private readonly INotifyService _notifyService;

        public NotiController(INotifyService notifyService)
        {
            _notifyService = notifyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotification(string userId)
        {
            var notifications = await _notifyService.GetNotifycationByUserAsync(userId);

            return Ok(notifications);
        }
    }
}
