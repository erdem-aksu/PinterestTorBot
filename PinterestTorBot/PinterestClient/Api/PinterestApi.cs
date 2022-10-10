using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PinterestTorBot.PinterestClient.Api.Exceptions;
using PinterestTorBot.PinterestClient.Api.Responses;
using PinterestTorBot.PinterestClient.Api.Session;
using PinterestTorBot.PinterestClient.Http;

namespace PinterestTorBot.PinterestClient.Api
{
    internal class PinterestApi
    {
        private readonly bool _autoLogin;

        private static readonly DefaultContractResolver ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = ContractResolver
        };

        private PinterestHttpClient Client { get; }

        private UserSessionData User { get; }
        
        public ProxyData ProxyData { get; }

        public PinterestApi(string username, string password, bool autoLogin = false, string sessionDataPath = null,
            ProxyData proxyData = null)
        {
            _autoLogin = autoLogin;

            User = new UserSessionData()
            {
                Username = username,
                Password = password
            };

            ProxyData = proxyData;

            Client = new PinterestHttpClient(User, _autoLogin, sessionDataPath, proxyData);
        }

        public async Task<T> GetAsync<T>(string path, object options = null, bool shouldLogin = false,
            JsonSerializerSettings serializerSettings = null, bool bookmarks = false)
        {
            using (var response = await Client.GetAsync(path, options, shouldLogin, serializerSettings, bookmarks)
                .ConfigureAwait(false))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return default;
                }

                if (!response.IsSuccessStatusCode)
                {
                    await ValidateResponse(response).ConfigureAwait(false);
                }

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var res = JsonConvert.DeserializeObject<BaseResponse<T>>(content, JsonSerializerSettings);

                return res.ResourceResponse.Data;
            }
        }

        public async Task GetAsync(string path, object options = null, bool shouldLogin = false,
            JsonSerializerSettings serializerSettings = null, bool bookmarks = false)
        {
            using (var response = await Client.GetAsync(path, options, shouldLogin, serializerSettings, bookmarks)
                .ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    await ValidateResponse(response).ConfigureAwait(false);
                }
            }
        }

        public async Task<PagedResponse<T>> GetPagedAsync<T>(string path, int limit = 0, int skip = 0,
            object options = null,
            bool shouldLogin = false, JsonSerializerSettings serializerSettings = null,
            Func<Task<IList<T>>> responseAction = null)
        {
            var data = new List<T>();
            var resultsNum = 0;
            var processed = 0;
            var pageSize = limit > PinterestApiConstants.DefaultPageSize
                ? PinterestApiConstants.DefaultPageSize
                : limit;
            var take = limit > 0 ? (int) Math.Ceiling(limit / (decimal) pageSize) : 1;

            while (true)
            {
                processed++;

                IList<T> res;

                if (responseAction != null)
                {
                    res = await responseAction();
                }
                else
                {
                    res = await GetAsync<IList<T>>(path, options, shouldLogin, serializerSettings, true);
                }

                if (res == null || !res.Any()) break;

                if (processed > skip)
                {
                    if (res.Count != pageSize)
                    {
                        pageSize = res.Count;
                        take = (int) Math.Ceiling(limit / (decimal) pageSize);
                    }

                    data.AddRange(res.TakeWhile((t, i) => limit <= 0 || i < limit - resultsNum * pageSize));

                    resultsNum++;
                }

                if (resultsNum >= take) break;

                if (!HasBookmark()) break;
            }

            return new PagedResponse<T>(data);
        }

        public async Task<PagedResponse<T>> GetPagedAsync<T>(Func<Task<IList<T>>> responseAction, int limit = 0,
            int skip = 0)
        {
            return await GetPagedAsync(null, limit, skip, null, false, null, responseAction);
        }

        public async Task PostAsync(string path, object value, bool shouldLogin = false, object options = null,
            JsonSerializerSettings serializerSettings = null)
        {
            await PostAsyncInternal(path, value, shouldLogin, options, serializerSettings).ConfigureAwait(false);
        }

        public async Task<T> PostAsync<T>(string path, object value, bool shouldLogin = false,
            object options = null, JsonSerializerSettings serializerSettings = null)
        {
            var content = await PostAsyncInternal(path, value, shouldLogin, options, serializerSettings)
                .ConfigureAwait(false);

            var res = JsonConvert.DeserializeObject<BaseResponse<T>>(content, JsonSerializerSettings);

            return res.ResourceResponse.Data;
        }

        private async Task<string> PostAsyncInternal(string path, object value, bool shouldLogin = false,
            object options = null, JsonSerializerSettings serializerSettings = null)
        {
            using (var response = await Client.PostAsync(path, value, options, shouldLogin, serializerSettings)
                .ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    await ValidateResponse(response).ConfigureAwait(false);
                }

                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        public async Task<T> PostUrlAsync<T>(string url, object value = null,
            object options = null, JsonSerializerSettings serializerSettings = null)
        {
            using (var response = await Client.PostUrlAsync(url, value, options, serializerSettings)
                .ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    await ValidateResponse(response).ConfigureAwait(false);
                }

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var res = JsonConvert.DeserializeObject<T>(content, JsonSerializerSettings);

                return res;
            }
        }

        public async Task DeleteAsync(string path)
        {
            using (var response = await Client.DeleteAsync($"{path}/").ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                    throw await CreateException(response).ConfigureAwait(false);
            }
        }

        public async Task Login()
        {
            await Client.Login();
        }

        public void Logout()
        {
            Client.Logout();
        }

        public List<Cookie> GetCookies()
        {
            return Client.GetCookies();
        }
        
        public bool HasBookmark()
        {
            return Client.HasBookmark();
        }

        public IEnumerable<string> GetBookmarks()
        {
            return Client.GetBookmarks();
        }

        public void SetBookmarks(IEnumerable<string> bookmarks)
        {
            Client.SetBookmarks(bookmarks);
        }

        private async Task ValidateResponse(HttpResponseMessage response)
        {
            if (_autoLogin && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Client.Logout();
            }

            throw await CreateException(response).ConfigureAwait(false);
        }

        private static async Task<Exception> CreateException(HttpResponseMessage response)
        {
            var url = response.RequestMessage.RequestUri.ToString();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            ErrorResponse jsonError = null;
            try
            {
                jsonError = JsonConvert.DeserializeObject<BaseResponse<dynamic>>(content, JsonSerializerSettings)
                    .ResourceResponse.Error;
            }
            catch (Exception)
            {
                // ignored
            }

            var message = jsonError?.Message ?? response.ReasonPhrase;
            var status = (int) response.StatusCode;
            switch (status)
            {
                case 400:
                    return PinterestException.Create<PinterestBadRequestException>(message, url, content, jsonError);
                case 401:
                    return PinterestException.Create<PinterestAuthorizationException>(message, url, content, jsonError);
                case 403:
                    return PinterestException.Create<PinterestForbiddenException>(message, url, content, jsonError);
                case 404:
                    return PinterestException.Create<PinterestNotFoundException>(message, url, content, jsonError);
                case 408:
                    return PinterestException.Create<PinterestTimeoutException>(message, url, content, jsonError);
                case 429:
                    return PinterestException.Create<PinterestRateLimitExceededException>(message, url, content,
                        jsonError);
                case 500:
                case 502:
                case 599:
                    return PinterestException.Create<PinterestServerErrorException>(message, url, content, jsonError,
                        status);
                default:
                    return new PinterestException(message)
                    {
                        RequestUrl = url,
                        ResponseContent = content,
                        ErrorResponse = jsonError,
                        HttpStatusCode = status
                    };
            }
        }
    }
}