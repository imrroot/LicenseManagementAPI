using LicenseManagementAPI.Application.Interfaces;
using LicenseManagementAPI.Presentation.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManagementAPI.Presentation.Controllers
{
    //[Authorize]
    [Route("api/subscription")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddSubscription([FromBody] AddSubscriptionDto addSubscriptionDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = int.Parse(User.FindFirst("Id").Value); 
            await _subscriptionService.AddSubscriptionAsync(addSubscriptionDto, userId);
            return Ok(new { message = "Subscription added successfully" });
        }
        [HttpPost("list/{id}")]
        public async Task<IActionResult> GetSubscription(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            //var userId = int.Parse(User.FindFirst("Id").Value); 
            var list = await _subscriptionService.GetSubscriptionsByAppIdAsync(id);
            return Ok(list);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSubscription(int id)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
             await _subscriptionService.DeleteSubscriptionAsync(id, userId);
            return Ok(new { message = "Subscription deleted successfully" });
        }
    }

}
