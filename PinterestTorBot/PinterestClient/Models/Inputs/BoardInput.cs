using PinterestTorBot.PinterestClient.Api.Enums;

namespace PinterestTorBot.PinterestClient.Models.Inputs
{
    public class BoardInput
    {
        public string BoardId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public BoardPrivacy Privacy { get; set; }

        public string Category { get; set; } = "other";
    }
}