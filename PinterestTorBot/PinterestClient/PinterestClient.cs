using PinterestTorBot.PinterestClient.Api;
using PinterestTorBot.PinterestClient.Http;

namespace PinterestTorBot.PinterestClient
{
    public class PinterestClient
    {
        private PinterestApi Api { get; }

        public IAuthApi Auth { get; }

        public IPinnersApi Pinners { get; }

        public IUsersApi Users { get; }

        public PinterestClient(string username, string password, bool autoLogin = false, string sessionDataPath = null,
            ProxyData proxyData = null)
        {
            Api = new PinterestApi(username, password, autoLogin, sessionDataPath, proxyData);

            Auth = new AuthApi(Api);
            Users = new UsersApi(Api);
            Pinners = new PinnersApi(Api);
        }
    }
}