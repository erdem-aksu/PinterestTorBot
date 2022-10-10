using System;

namespace PinterestTorBot.PinterestClient.Models
{
    public class Profile
    {
        public string ImpressumUrl { get; set; }

        public string LastName { get; set; }

        public string EmailSettings { get; set; }

        public string Locale { get; set; }

        public bool IsKnownFacebookUser { get; set; }

        public bool HasPassword { get; set; }

        public bool ThirdPartyMarketingTrackingEnabled { get; set; }

        public string CustomGender { get; set; }

        public string Gender { get; set; }

        public string NewsSettings { get; set; }

        public string Id { get; set; }

        public bool IsWriteBanned { get; set; }

        public string FirstName { get; set; }

        public string PushSettings { get; set; }

        public bool PersonalizeFromOffsiteBrowsing { get; set; }

        public bool FacebookTimelineEnabled { get; set; }

        public string EmailChangingTo { get; set; }

        public bool PersonalizeNuxFromOffsiteBrowsing { get; set; }

        public bool HasConfirmedEmail { get; set; }

        public bool IsPartner { get; set; }

        public string Type { get; set; }

        public string Email { get; set; }

        public string WebsiteUrl { get; set; }

        public string Location { get; set; }

        public string Username { get; set; }

        public string About { get; set; }

        public string ProfileImageUrl { get; set; }

        public bool EmailBounced { get; set; }

        public bool AdsCustomizeFromConversion { get; set; }

        public string[] AdditionalWebsiteUrls { get; set; }

        public bool FacebookPublishStreamEnabled { get; set; }

        public bool IsHighRisk { get; set; }

        public bool ShowImpressum { get; set; }

        public int? Age { get; set; }

        public bool ExcludeFromSearch { get; set; }

        public DateTime? Birthdate { get; set; }

        public bool PfyPreference { get; set; }

        public string EmailBizSettings { get; set; }

        public string Country { get; set; }

        public bool HideFromNews { get; set; }

        public bool IsMarketingSettingEnabled { get; set; }
    }
}