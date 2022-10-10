namespace PinterestTorBot.PinterestClient.Models.Inputs
{
    public class UpdatePinInput : CreatePinInput
    {
        public string Id { get; set; }

        public new string Method => null;

        public new string ImageUrl = null;

        public new string Section => null;

        public string BoardSectionId { get; set; } = null;
    }
}