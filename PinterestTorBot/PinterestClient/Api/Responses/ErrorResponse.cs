namespace PinterestTorBot.PinterestClient.Api.Responses
{
    public class ErrorResponse
    {
        public int ApiErrorCode { get; set; }
        
        public string Message { get; set; }

        public string Code { get; set; }
        
        public string Target { get; set; }
        
        public string HttpStatus { get; set; }
    }
}
