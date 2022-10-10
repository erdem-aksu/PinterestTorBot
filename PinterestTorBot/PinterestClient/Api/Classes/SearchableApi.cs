using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PinterestTorBot.PinterestClient.Api.Responses;

namespace PinterestTorBot.PinterestClient.Api.Classes
{
    internal class SearchableApi : ISearchableApi
    {
        private PinterestApi Api { get; }

        private string SearchScope { get; }

        public SearchableApi(PinterestApi api, string searchScope)
        {
            Api = api;
            SearchScope = searchScope;
        }

        public async Task<PagedResponse<JObject>> SearchAsync(string query,
            int limit = 0, int skip = 0, bool shouldLogin = false)
        {
            return await Api.GetPagedAsync(async () =>
            {
                var path = Api.HasBookmark()
                    ? PinterestApiConstants.ResourceSearchWithPagination
                    : PinterestApiConstants.ResourceSearch;

                var res = await Api.GetAsync<object>(path, new
                {
//                    isPrefetch = false,
                    scope = SearchScope,
                    query,
//                    page_size = limit,
//                    redux_normalize_feed = true,
//                    auto_correction_disabled = false,
//                    rs = "typed"
                }, shouldLogin, null, true);

                if (res.GetType() == typeof(JArray))
                {
                    return ((JArray) res).ToObject<IList<JObject>>();
                }

                return ((JObject) res).TryGetValue("results", out var jToken)
                    ? jToken.ToObject<IList<JObject>>()
                    : default;
            }, limit, skip);
        }
    }
}