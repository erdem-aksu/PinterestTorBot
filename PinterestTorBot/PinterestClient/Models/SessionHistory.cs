using System;

namespace PinterestTorBot.PinterestClient.Models
{
    public class SessionHistory
    {
        public string Current { get; set; }

        public string Type { get; set; }

        public string DeviceName { get; set; }

        public string PlatformVersion { get; set; }

        public string PlatformType { get; set; }

        public string Location { get; set; }

        public int LastAccessedAt { get; set; }

        public DateTime AccessedAt { get; set; }

        public string IpAddress { get; set; }

        public string Id { get; set; }
    }
}