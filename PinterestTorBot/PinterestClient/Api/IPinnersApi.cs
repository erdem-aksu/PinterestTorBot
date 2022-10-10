using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PinterestTorBot.PinterestClient.Api.Classes;
using PinterestTorBot.PinterestClient.Api.Enums;
using PinterestTorBot.PinterestClient.Api.Responses;
using PinterestTorBot.PinterestClient.Models;

namespace PinterestTorBot.PinterestClient.Api
{
    public interface IPinnersApi
    {
        Task<UserInfo> GetUserInfoAsync(string username);
    }
}