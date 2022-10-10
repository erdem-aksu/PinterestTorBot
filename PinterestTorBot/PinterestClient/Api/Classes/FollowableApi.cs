using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PinterestTorBot.PinterestClient.Api.Responses;

namespace PinterestTorBot.PinterestClient.Api.Classes
{
    internal class FollowableApi : IFollowableApi
    {
        private PinterestApi Api { get; }

        private string EntityIdName { get; }

        private string FollowUrl { get; }

        private string UnFollowUrl { get; }

        private string FollowersUrl { get; }

        private string FollowersFor { get; }

        public FollowableApi(PinterestApi api, string entityIdName, string followUrl, string unFollowUrl,
            string followersUrl = null, string followersFor = null)
        {
            Api = api;
            EntityIdName = entityIdName;
            FollowUrl = followUrl;
            UnFollowUrl = unFollowUrl;
            FollowersFor = followersFor;
            FollowersUrl = followersUrl;
        }

        public async Task FollowAsync(object entityId)
        {
            if (string.IsNullOrEmpty(FollowUrl)) return;

            await FollowCallAsync(FollowUrl, entityId);
        }

        public async Task UnFollowAsync(object entityId)
        {
            if (string.IsNullOrEmpty(UnFollowUrl)) return;

            await FollowCallAsync(UnFollowUrl, entityId);
        }

        public async Task<PagedResponse<JObject>> GetFollowersAsync(string followersFor,
            int limit = 0, int skip = 0)
        {
            if (string.IsNullOrEmpty(FollowersUrl) || string.IsNullOrEmpty(FollowersFor)) return null;

            var data = new Dictionary<string, object> {{FollowersFor, followersFor}};

            return await Api.GetPagedAsync<JObject>(FollowersUrl, limit, skip, data, true);
        }

        private async Task FollowCallAsync(string resourceUrl, object entityId)
        {
            await Api.PostAsync(resourceUrl, CreateFollowData(entityId), true);
        }

        private object CreateFollowData(object entityId)
        {
            var data = new Dictionary<string, object> {{EntityIdName, entityId}};

            if (EntityIdName.Equals("interest_id"))
            {
                data.Add("interest_list", "favorited");
            }

            return data;
        }
    }
}