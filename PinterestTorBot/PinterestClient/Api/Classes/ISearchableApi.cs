using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PinterestTorBot.PinterestClient.Api.Responses;

namespace PinterestTorBot.PinterestClient.Api.Classes
{
    public interface ISearchableApi
    {
        Task<PagedResponse<JObject>> SearchAsync(string query, int limit = 0,
            int skip = 0, bool shouldLogin = false);
    }
}