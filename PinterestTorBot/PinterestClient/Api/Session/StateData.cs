using System;
using System.Collections.Generic;
using System.Net;

namespace PinterestTorBot.PinterestClient.Api.Session
{
    [Serializable]
    public class StateData
    {
        public UserSessionData User { get; set; }
        
        public bool IsLoggedIn { get; set; }

        public CookieContainer Cookies { get; set; }
        
        public List<Cookie> RawCookies { get; set; }

        public string CsrfToken { get; set; } = string.Empty;
        
        public string AppVersion { get; set; } = PinterestApiConstants.HeaderXAppVersion;
        
        public IEnumerable<string> Bookmarks { get; set; }
    }
}