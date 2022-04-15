using Microsoft.AspNetCore.Identity;
using SocialNetwork.DAL.Identity.Models;
using SocialNetwork.DTO.Account;
using SocialNetwork.DTO.Entities;
using SocialNetwork.DTO.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BLL.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> CreateUserAsync(RegisterModel registerModel);
        Task<ApiResponse> LoginAsync(LoginModel loginModel);
        Task<DAL.Identity.Models.User> GetUserByIdAsync(string userId);
        Task<bool> FollowAsync(string userId, string destId);
        Task<Profile> GetProfileByIdAsync(string userId);

    }
}
