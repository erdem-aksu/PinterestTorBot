namespace PinterestTorBot
{
    public static class QueryStringExtensions
    {
        public static string AddQueryParam(this string original, string name, object value)
        {
            original += original.Contains("?") ? "&" : "?";
            original += $"{name}={value}";
            return original;
        }
    }
}