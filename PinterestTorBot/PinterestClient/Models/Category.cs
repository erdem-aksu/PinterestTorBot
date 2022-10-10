using Newtonsoft.Json.Linq;

namespace PinterestTorBot.PinterestClient.Models
{
    public class Category
    {
        public string CategoryType;

        public string Type;

        public string Name;

        public string Key;

        public JObject Images;
    }
}