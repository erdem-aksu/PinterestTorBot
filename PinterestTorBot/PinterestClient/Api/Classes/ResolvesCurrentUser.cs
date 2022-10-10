using System.Threading.Tasks;
using PinterestTorBot.PinterestClient.Models;

namespace PinterestTorBot.PinterestClient.Api.Classes
{
    internal class ResolvesCurrentUser : IResolvesCurrentUser
    {
        private PinterestApi Api { get; }

        public ResolvesCurrentUser(PinterestApi api)
        {
            Api = api;
        }
       
        public async Task<string> GetUsernameAsync()
        {
            return (await GetCurrentUserAsync()).Username;
        }

        public async Task<string> GetIdAsync()
        {
            return (await GetCurrentUserAsync()).Id;
        }

        private async Task<Profile> GetCurrentUserAsync()
        {
            return await Api.GetAsync<Profile>(PinterestApiConstants.ResourceGetUserSettings, shouldLogin: true);
        }
    }
}