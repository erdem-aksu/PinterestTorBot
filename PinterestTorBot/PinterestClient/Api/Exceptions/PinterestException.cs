using System;
using PinterestTorBot.PinterestClient.Api.Responses;

namespace PinterestTorBot.PinterestClient.Api.Exceptions
{
    public class PinterestException : Exception
    {
        public int? HttpStatusCode { get; internal set; }

        public string RequestUrl { get; internal set; }

        public string ResponseContent { get; internal set; }

        public ErrorResponse ErrorResponse { get; internal set; }

        public PinterestException()
        {
        }

        public PinterestException(string message)
            : base(message)
        {
        }

        public PinterestException(string message, Exception inner)
            : base(message, inner)
        {
        }

        internal static T Create<T>(string message, string requestUrl, string responseContent,
            ErrorResponse errorResponse = null, int? httpStatusCode = null)
            where T : PinterestException
        {
            var exception = (T) Activator.CreateInstance(typeof(T), message);
            exception.RequestUrl = requestUrl;
            exception.ResponseContent = responseContent;
            exception.HttpStatusCode = httpStatusCode;
            exception.ErrorResponse = errorResponse;
            return exception;
        }
    }
}