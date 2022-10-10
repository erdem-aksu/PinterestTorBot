﻿namespace PinterestTorBot.PinterestClient.Api.Exceptions
{
    public class PinterestAuthorizationException : PinterestException
    {
        public PinterestAuthorizationException(string message)
            : base(message)
        {
            HttpStatusCode = 401;
        }
    }
}
