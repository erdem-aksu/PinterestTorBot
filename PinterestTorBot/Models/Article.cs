namespace PinterestTorBot.Models
{
    public class Article
    {
        public int Id { get; set; }

        public string Text { get; set; }
        
        public string Html { get; set; }
        
        public string Summary { get; set; }
        
        public string Keywords { get; set; }
    }
}