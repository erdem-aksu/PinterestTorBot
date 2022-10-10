using System;

namespace PinterestTorBot.PinterestClient.Http
{
    public class ProxyData
    {
        public Uri Address { get; set; }
        
        public string Username { get; set; }

        public string Password { get; set; }
    }
}