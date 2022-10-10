using System.Collections.Generic;
using System.Threading.Tasks;
using PinterestTorBot.PinterestClient.Models;
using PinterestTorBot.PinterestClient.Models.Inputs;

namespace PinterestTorBot.PinterestClient.Api
{
    public interface IUsersApi
    {
        Task<Profile> GetProfileAsync();

        Task ChangePasswordAsync(string oldPassword, string newPassword);
        
        Task Deactivate(string reason = "other", string explanation = "");

        Task Invite(string email);

        Task ClearSearchHistory();

        Task<List<SessionHistory>> GetSessionHistory();
        
        Task<bool> IsBanned();

        Task<string> GetId();

        Task<string> GetUsername();
    }
}