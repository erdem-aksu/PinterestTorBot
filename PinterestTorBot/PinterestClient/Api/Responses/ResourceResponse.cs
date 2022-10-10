namespace PinterestTorBot.PinterestClient.Api.Responses
{
    public class ResourceResponse<T>
    {
        public string Status { get; set; }
        
        public string Message { get; set; }

        public int Code { get; set; }
        
        public T Data { get; set; }

        public ErrorResponse Error { get; set; }
    }
}
