namespace PinterestTorBot.PinterestClient.Models.Inputs
{
    public class RepinInput
    {
        public string PinId { get; set; }
        
        public string BoardId { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        public bool? IsVideo { get; set; } = null;
    }
}