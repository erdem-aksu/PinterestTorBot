using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace PinterestTorBot.PinterestClient.Models
{
    public class TopicInfo
    {
        public bool IsFollowed { get; set; }

        public bool KlpHasRelated { get; set; }

        public bool IsInterest { get; set; }

        public string ImageSignature { get; set; }

        public bool IsNew { get; set; }

        public JObject Images { get; set; }

        public bool HasRelated { get; set; }

        public string InterestStats { get; set; }

        public string BackgroundColor { get; set; }

        public string Id { get; set; }

        public string Description { get; set; }

        public string UrlName { get; set; }

        public string CanonicalUrl { get; set; }

        public bool IsKlpTerm { get; set; }

        public string Type { get; set; }

        public string ImageSource { get; set; }

        public List<JObject> RelatedKlps { get; set; }

        public int FollowerCount { get; set; }

        public string Key { get; set; }

        public int ImageSize { get; set; }

        public string CanonicalTerm { get; set; }

        public string Name { get; set; }

        public DateTime FeedUpdateTime { get; set; }
    }
}