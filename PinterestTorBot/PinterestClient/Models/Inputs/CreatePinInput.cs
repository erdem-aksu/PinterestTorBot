namespace PinterestTorBot.PinterestClient.Models.Inputs
{
    public class CreatePinInput
    {
        public string Method => "uploaded";
        
        public string FieldSetKey => "create_success";
        
        public bool SkipPinCreateLog => true;

        public string Description { get; set; }

        public string Link { get; set; }

        public string ImageUrl { get; set; }

        public string BoardId { get; set; }

        public string Title { get; set; }

        public string Section { get; set; } = null;
    }
}