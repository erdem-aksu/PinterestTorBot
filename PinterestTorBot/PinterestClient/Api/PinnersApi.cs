using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PinterestTorBot.PinterestClient.Api.Classes;
using PinterestTorBot.PinterestClient.Api.Enums;
using PinterestTorBot.PinterestClient.Api.Responses;
using PinterestTorBot.PinterestClient.Models;

namespace PinterestTorBot.PinterestClient.Api
{
    internal class PinnersApi : IPinnersApi
    {
        private PinterestApi Api { get; }

        public PinnersApi(PinterestApi api)
        {
            Api = api;
        }

        public async Task<UserInfo> GetUserInfoAsync(string username)
        {
            return await Api.GetAsync<UserInfo>(PinterestApiConstants.ResourceUserInfo, new
            {
                field_set_key = "profile",
                username
            });
        }
    }
}