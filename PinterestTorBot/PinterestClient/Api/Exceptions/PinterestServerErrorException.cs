using System;

namespace PinterestTorBot.PinterestClient.Api.Exceptions
{
    public class PinterestServerErrorException : PinterestException
    {
        public PinterestServerErrorException()
        {

        }

        public PinterestServerErrorException(string message)
            : base(message)
        {

        }

        public PinterestServerErrorException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
