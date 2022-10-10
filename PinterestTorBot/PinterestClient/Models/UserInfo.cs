using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PinterestTorBot.PinterestClient.Models
{
    public class UserInfo
    {
        public bool ShowCreatorProfile { get; set; }

        public string ImpressumUrl { get; set; }

        public string LastName { get; set; }

        public bool DomainVerified { get; set; }

        public int FollowingCount { get; set; }

        public bool IsTastemaker { get; set; }

        public int ProfileReach { get; set; }

        public string ImageMediumUrl { get; set; }

        public int StoryPinCount { get; set; }

        public string ImageXlargeUrl { get; set; }

        public string FullName { get; set; }

        public string ImageSmallUrl { get; set; }

        public string Id { get; set; }

        public bool StorefrontManagementEnabled { get; set; }

        public string FirstName { get; set; }

        public string DomainUrl { get; set; }

        public bool ExplicitlyFollowedByMe { get; set; }

        public bool HasCatalog { get; set; }

        public string Location { get; set; }

        public bool Indexed { get; set; }

        public string ProfileDiscoveredPublic { get; set; }

        public string Type { get; set; }

        public string WebsiteUrl { get; set; }

        public int BoardCount { get; set; }

        public string Username { get; set; }

        public JObject VerifiedIdentity { get; set; }

        public int VideoPinCount { get; set; }

        public int NativePinCount { get; set; }

        public bool HasShowcase { get; set; }

        public DateTime? LastPinSaveTime { get; set; }

        public int FollowerCount { get; set; }

        public bool IsPartner { get; set; }

        public int PinsDoneCount { get; set; }

        public bool HasCustomBoardSortingOrder { get; set; }
        
        public int PinCount { get; set; }

        public int UserFollowingCount { get; set; }

        public string About { get; set; }

        public string ShowDiscoveredFeed { get; set; }

        public bool ShowImpressum { get; set; }

        public int JoinedCommunitiesCount { get; set; }

        public string NoindexReason { get; set; }

        public string ImageLargeUrl { get; set; }

        public JObject ProfileCover { get; set; }

        public bool BlockedByMe { get; set; }

        public List<JObject> RepinsFrom { get; set; }

        public bool HasBoard { get; set; }
    }
}