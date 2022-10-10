using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PinterestTorBot.PinterestClient.Api.Responses;

namespace PinterestTorBot.PinterestClient.Api.Classes
{
    public interface IFollowableApi
    {
        Task FollowAsync(object entityId);

        Task UnFollowAsync(object entityId);

        Task<PagedResponse<JObject>> GetFollowersAsync(string followersFor,
            int limit = 0, int skip = 0);
    }
}