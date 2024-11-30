using LicenseManagementAPI.Core.Entities;
using LicenseManagementAPI.Presentation.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManagementAPI.Application.Interfaces
{
    public interface ISubscriptionService
    {
        Task<IActionResult> GetSubscriptionsByAppIdAsync(int appId);
        Task<IActionResult> AddSubscriptionAsync(AddSubscriptionDto addsubscriptionDto, int userId);
        Task<IActionResult> DeleteSubscriptionAsync(int subscriptionId, int userId);
    }

}
