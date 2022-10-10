using System.Collections.Generic;
using System.Threading.Tasks;

namespace PinterestTorBot.PinterestClient.Api.Classes
{
    internal class CanBeDeletedApi : ICanBeDeletedApi
    {
        private PinterestApi Api { get; }

        private string EntityIdName { get; }

        private string DeleteUrl { get; }

        public CanBeDeletedApi(PinterestApi api, string entityIdName, string deleteUrl)
        {
            Api = api;
            EntityIdName = entityIdName;
            DeleteUrl = deleteUrl;
        }

        public async Task DeleteAsync(object entityId)
        {
            await Api.PostAsync(DeleteUrl, new Dictionary<string, object> {{EntityIdName, entityId}}, true);
        }
    }
}