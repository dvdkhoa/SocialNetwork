﻿using SocialNetwork.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BLL.Services.Implements
{
    public class NotifyService : INotifyService
    {
        private readonly NotifyRepository _notifyRepository;

        public NotifyService(NotifyRepository notifyRepository)
        {
            _notifyRepository = notifyRepository;
        }

        public Task CreateAsync(string userId, string message, string thumnail, string intent)
        {
            return _notifyRepository.CreateAsync(userId, message, thumnail, intent);
        }
    }
}