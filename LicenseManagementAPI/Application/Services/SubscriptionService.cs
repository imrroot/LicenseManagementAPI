﻿using LicenseManagementAPI.Application.Interfaces;
using LicenseManagementAPI.Core.Entities;
using LicenseManagementAPI.Core.Interfaces;
using LicenseManagementAPI.Infrastructure.Repositories;
using LicenseManagementAPI.Presentation.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LicenseManagementAPI.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IAppRepository _appRepository;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository, IAppRepository appRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _appRepository = appRepository;
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsByAppIdAsync(int appId)
        {
            return await _subscriptionRepository.GetSubscriptionsByAppIdAsync(appId);
        }

        public async Task<IActionResult> AddSubscriptionAsync(AddSubscriptionDto addsubscriptionDto, int userId)
        {
            var app = await _appRepository.GetAppByIdWithSubscriptionsAsync(addsubscriptionDto.ApplicationId, userId);

            if (app == null) return new NotFoundObjectResult(new {Message = "Application not found"});
            var exists = app.Subscriptions.Any(s=> s.Name == addsubscriptionDto.Name && s.AppId == addsubscriptionDto.ApplicationId);
            if(exists) return new BadRequestObjectResult(new {Message = "Subscription already exists"});
            var subscription = new Subscription
            {
                Name = addsubscriptionDto.Name,
                Level = addsubscriptionDto.AccessLevel,
                AppId = addsubscriptionDto.ApplicationId
            };

            await _subscriptionRepository.AddSubscriptionAsync(subscription);
            return new OkObjectResult(new {Message ="Subscription added successfully"});
        }

        public async Task<IActionResult> DeleteSubscriptionAsync(int subscriptionId,int userId)
        { 
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId, userId);
            if (subscription == null) return new NotFoundObjectResult(new{message = "Subscription not found"});
            await _subscriptionRepository.DeleteSubscriptionAsync(subscription);
           return new OkObjectResult(new {Message = "Subscription deleted successfully"});
        }
    }

}