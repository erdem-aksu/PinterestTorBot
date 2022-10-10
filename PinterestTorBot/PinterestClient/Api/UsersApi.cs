using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PinterestTorBot.PinterestClient.Models;
using PinterestTorBot.PinterestClient.Models.Inputs;

namespace PinterestTorBot.PinterestClient.Api
{
    internal class UsersApi : IUsersApi
    {
        private PinterestApi Api { get; }

        public UsersApi(PinterestApi api)
        {
            Api = api;
        }

        public async Task<Profile> GetProfileAsync()
        {
            return await Api.GetAsync<Profile>(PinterestApiConstants.ResourceGetUserSettings, shouldLogin: true);
        }

        public async Task ChangePasswordAsync(string oldPassword, string newPassword)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceChangePassword, new
            {
                old_password = oldPassword,
                new_password = newPassword,
                new_password_confirm = newPassword,
            }, true);
        }

        public async Task Deactivate(string reason = "other", string explanation = "")
        {
            var user = await GetProfileAsync();

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            await Api.PostAsync(PinterestApiConstants.ResourceDeactivateAccount, new
            {
                user_id = user.Id,
                reason,
                explanation
            }, true);
        }

        public async Task Invite(string email)
        {
            await Api.PostAsync(PinterestApiConstants.ResourceInvite, new
            {
                email,
                type = "email"
            }, true);
        }

        public async Task ClearSearchHistory()
        {
            await Api.PostAsync(PinterestApiConstants.ResourceClearSearchHistory, new { }, true);
        }

        public async Task<List<SessionHistory>> GetSessionHistory()
        {
            return await Api.GetAsync<List<SessionHistory>>(PinterestApiConstants.ResourceSessionsHistory,
                shouldLogin: true);
        }

        public async Task<bool> IsBanned()
        {
            return (await GetProfileAsync()).IsWriteBanned;
        }
        
        public async Task<string> GetId()
        {
            return (await GetProfileAsync()).Id;
        }
        
        public async Task<string> GetUsername()
        {
            return (await GetProfileAsync()).Username;
        }
    }
}