using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PinterestTorBot.PinterestClient.Api.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BoardPrivacy
    {
        [EnumMember(Value = "public")] Public,

        [EnumMember(Value = "secret")] Secret
    }
}