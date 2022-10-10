namespace PinterestTorBot.PinterestClient.Models
{
    public class LookUpCountry
    {
        public string Key { get; set; }
        
        public string Name { get; set; }
    }
    
    public class LookUpLocale
    {
        public string Key { get; set; }
        
        public string Name { get; set; }
    }
    
    public class LookUpAccountType
    {
        public string Key { get; set; }
        
        public string Name { get; set; }
        
        public string Help { get; set; }
    }
    
    public class LookUpSection
    {
        public string Id { get; set; }
        
        public string Title { get; set; }
    }
    
    public class LookUpBoard
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
    }
}