namespace PinterestTorBot.PinterestClient.Models.Inputs
{
    public class ProfileInput
    {
        public string LastName { get; set; }

        public string FirstName { get; set; }
        
        public string Username { get; set; }
        
        public string About { get; set; }

        public string Country { get; set; }

        public string WebsiteUrl { get; set; }
        
        public string Locale { get; set; }

        public string AccountType { get; set; }

        public string ExcludeFromSearch { get; set; }
        
        public string ProfileImageUrl { get; set; }

        public string ContactName { get; set; }

        public string Gender { get; set; }
    }
}