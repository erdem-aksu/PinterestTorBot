using System.IO;
using PinterestTorBot.PinterestClient.Http;

namespace PinterestTorBot.PinterestClient.Api.SessionHandlers
{
    internal class FileSessionHandler : ISessionHandler
    {
        private PinterestHttpClient HttpClient { get; }

        private string FilePath { get; }

        public FileSessionHandler(PinterestHttpClient httpClient, string filePath)
        {
            HttpClient = httpClient;
            FilePath = filePath;
        }

        public void Load()
        {
            if (!File.Exists(FilePath)) return;

            using (var fs = File.OpenRead(FilePath))
            {
                HttpClient.LoadStateDataFromStream(fs);
            }
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(FilePath)) return;

            using (var state = HttpClient.GetStateDataAsStream())
            {
                using (var fileStream = File.Create(FilePath))
                {
                    state.Seek(0, SeekOrigin.Begin);
                    state.CopyTo(fileStream);
                }
            }
        }
    }
}