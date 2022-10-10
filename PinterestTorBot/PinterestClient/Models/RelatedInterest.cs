using System;
using Newtonsoft.Json.Linq;

namespace PinterestTorBot.PinterestClient.Models
{
    public class RelatedInterest
    {
        public bool IsFollowed {get; set;}
        
        public string UrlName {get; set;}
        
        public string ImageSource {get; set;}
        
        public string BackgroundColor {get; set;}
        
        public bool IsInterest {get; set;}
        
        public string ImageSignature {get; set;}
        
        public DateTime FeedUpdateTime {get; set;}
        
        public bool IsNew {get; set;}
        
        public string CanonicalUrl {get; set;}
        
        public int FollowerCount {get; set;}
        
        public string Key {get; set;}
        
        public int ImageSize {get; set;}
        
        public JObject Images {get; set;}
        
        public string Type {get; set;}
        
        public string Id {get; set;}
        
        public string Name {get; set;}
        
        public string PillColor {get; set;}
        
    }
}
