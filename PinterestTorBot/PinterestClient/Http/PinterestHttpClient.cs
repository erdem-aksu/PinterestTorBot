using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PinterestTorBot.PinterestClient.Api;
using PinterestTorBot.PinterestClient.Api.Exceptions;
using PinterestTorBot.PinterestClient.Api.Session;
using PinterestTorBot.PinterestClient.Api.SessionHandlers;
using PinterestTorBot.PinterestClient.Serialization;

namespace PinterestTorBot.PinterestClient.Http
{
    internal class PinterestHttpClient
    {
        private UserSessionData _user;

        private readonly StateData _stateData;

        private readonly bool _autoLogin;

        private readonly ISessionHandler _sessionHandler;

        private readonly CookieContainer _cookieContainer;

        private readonly Uri _baseAddress;

        private readonly ProxyData _proxyData;

        private static readonly IDictionary<string, string> RequestHeaders = new Dictionary<string, string>()
        {
            {"User-Agent", PinterestApiConstants.HeaderUserAgent},
            {"Accept", "application/json, text/javascript, */*; q=0.01"},
            {"Accept-Language", "en-US,en;q=0.5"},
            {"X-Pinterest-AppState", "active"},
            {"X-Requested-With", "XMLHttpRequest"}
        };

        private static readonly DefaultContractResolver ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = ContractResolver,
        };

        public PinterestHttpClient(UserSessionData user, bool autoLogin = false, string stateDataPath = null,
            ProxyData proxyData = null)
        {
            _user = user;
            _stateData = new StateData
            {
                User = _user
            };

            if (autoLogin && !string.IsNullOrEmpty(stateDataPath) && Directory.Exists(stateDataPath))
            {
                _autoLogin = true;
                _sessionHandler = new FileSessionHandler(this, Path.Combine(stateDataPath, _user.Username + ".bin"));
            }

            _cookieContainer = new CookieContainer();
            _baseAddress = new Uri(PinterestApiConstants.UrlBase);
            _proxyData = proxyData;
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri, object options = null,
            bool shouldLogin = false, JsonSerializerSettings serializerSettings = null, bool bookmarks = false)
        {
            if (shouldLogin)
            {
                await Login();
            }

            requestUri = BuildPath(requestUri, options, serializerSettings, bookmarks);

            var message = new HttpRequestMessage(HttpMethod.Get, requestUri);

            return await SendAsync(message, bookmarks);
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, object value, object options = null,
            bool shouldLogin = false, JsonSerializerSettings serializerSettings = null)
        {
            if (shouldLogin)
            {
                await Login();
            }

            requestUri = BuildPath(requestUri, options, serializerSettings);

            var message = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = GetFormUrlEncodedContent(value, serializerSettings),
            };

            return await SendAsync(message);
        }

        public async Task<HttpResponseMessage> PostUrlAsync(string url, object value = null, object options = null,
            JsonSerializerSettings serializerSettings = null)
        {
            if (options != null)
            {
                url = options.GetType().GetProperties()
                    .ToDictionary(p => p.Name, p => p.GetValue(options).ToString())
                    .Aggregate(url, (current, pair) => Extensions.QueryStringExtensions.AddQueryParam(current, pair.Key, pair.Value));
            }

            var message = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = value != null ? GetFormUrlEncodedContent(value, serializerSettings) : null
            };

            using (var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 2,
                UseProxy = _proxyData != null,
                Proxy = _proxyData != null
                    ? new WebProxy(_proxyData.Address, false, new string[] { },
                        new NetworkCredential(_proxyData.Username, _proxyData.Password))
                    : null
            })
            using (var client = new HttpClient(handler))
            {
                var response = await client.SendAsync(message);

                return response;
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri, bool shouldLogin = false)
        {
            if (shouldLogin)
            {
                await Login();
            }

            return await SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri));
        }

        public async Task Login()
        {
            if (_autoLogin)
            {
                _sessionHandler.Load();
            }

            if (_stateData.IsLoggedIn) return;

            if (string.IsNullOrEmpty(_user.Username) || string.IsNullOrEmpty(_user.Password))
            {
                throw new ArgumentNullException(_user.Username);
            }

            using (var loginRes = await PostAsync(PinterestApiConstants.ResourceLogin,
                new {username_or_email = _user.Username, password = _user.Password}))
            {
                if (!loginRes.IsSuccessStatusCode)
                {
                    throw new PinterestAuthorizationException(loginRes.ReasonPhrase);
                }
            }

            _stateData.IsLoggedIn = true;

            if (_autoLogin)
            {
                _sessionHandler.Save();
            }
        }

        public void Logout()
        {
            _stateData.CsrfToken = string.Empty;
            _stateData.IsLoggedIn = false;

            foreach (Cookie co in _cookieContainer.GetCookies(_baseAddress))
            {
                co.Expires = DateTime.Now.Subtract(TimeSpan.FromDays(1));
            }

            _stateData.Cookies = _cookieContainer;
            _stateData.RawCookies = _cookieContainer.GetCookies(_baseAddress).Cast<Cookie>().ToList();

            if (_autoLogin)
            {
                _sessionHandler.Save();
            }
        }

        public List<Cookie> GetCookies()
        {
            return _stateData.RawCookies;
        }

        public bool HasBookmark()
        {
            return _stateData.Bookmarks != null && _stateData.Bookmarks.Any();
        }

        public IEnumerable<string> GetBookmarks()
        {
            return _stateData.Bookmarks;
        }

        public void SetBookmarks(IEnumerable<string> bookmarks)
        {
            _stateData.Bookmarks = bookmarks;
        }

        public Stream GetStateDataAsStream()
        {
            return SerializationHelper.SerializeToStream(_stateData);
        }

        public void LoadStateDataFromStream(Stream stream)
        {
            var data = SerializationHelper.DeserializeFromStream<StateData>(stream);

            if (_user.Password != data.User.Password)
            {
                return;
            }

            _user = data.User;

            foreach (var cookie in data.RawCookies)
            {
                _cookieContainer.Add(_baseAddress, cookie);
            }

            _stateData.RawCookies = data.RawCookies;
            _stateData.Bookmarks = data.Bookmarks;
            _stateData.AppVersion = data.AppVersion;
            _stateData.CsrfToken = data.CsrfToken;
            _stateData.IsLoggedIn = data.IsLoggedIn;
        }

        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, bool bookmarks = false)
        {
            await InitCsrfToken();

            SetHeaders(message);

            using (var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 2,
                UseCookies = true,
                UseProxy = _proxyData != null,
                Proxy = _proxyData != null
                    ? new WebProxy(_proxyData.Address, false, new string[] { },
                        new NetworkCredential(_proxyData.Username, _proxyData.Password))
                    : null
            })
            using (var client = new HttpClient(handler) {BaseAddress = _baseAddress})
            {
                var response = await client.SendAsync(message);

                _stateData.Cookies = handler.CookieContainer;
                _stateData.RawCookies = _stateData.Cookies.GetCookies(response.RequestMessage.RequestUri)
                    .Cast<Cookie>()
                    .ToList();
                _stateData.CsrfToken = _stateData.RawCookies
                                           .First(cookie => cookie.Name == PinterestApiConstants.CsrfCookieField)?.Value
                                       ?? string.Empty;
                _stateData.Bookmarks = null;

                if (response.Headers.Contains(PinterestApiConstants.AppVersionHeaderField))
                {
                    _stateData.AppVersion =
                        response.Headers.GetValues(PinterestApiConstants.AppVersionHeaderField).First();
                }

                if (bookmarks)
                {
                    try
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var res = JsonConvert.DeserializeObject<JObject>(content, JsonSerializerSettings);

                        _stateData.Bookmarks = res.SelectToken("resource.options.bookmarks").ToObject<List<string>>();
                    }
                    catch (Exception)
                    {
                        _stateData.Bookmarks = null;
                    }
                }

                return response;
            }
        }

        private async Task InitCsrfToken()
        {
            if (!string.IsNullOrEmpty(_stateData.CsrfToken)) return;

            using (var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 2,
                UseCookies = true,
                UseProxy = _proxyData != null,
                Proxy = _proxyData != null
                    ? new WebProxy(_proxyData.Address, false, new string[] { },
                        new NetworkCredential(_proxyData.Username, _proxyData.Password))
                    : null
            })
            using (var client = new HttpClient(handler) {BaseAddress = _baseAddress})
            {
                await client.GetAsync(client.BaseAddress);

                var responseCookies = handler.CookieContainer.GetCookies(client.BaseAddress);

                _stateData.CsrfToken = responseCookies[PinterestApiConstants.CsrfCookieField]?.Value ?? string.Empty;
            }
        }


        private static FormUrlEncodedContent GetFormUrlEncodedContent(object obj,
            JsonSerializerSettings serializerSettings = null)
        {
            var data = new
            {
                context = new object(),
                options = obj
            };

            var wrapper = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("source_url", string.Empty),
                new KeyValuePair<string, string>("data",
                    JsonConvert.SerializeObject(data, serializerSettings ?? JsonSerializerSettings))
            };

            return new FormUrlEncodedContent(wrapper);
        }

        private string BuildPath(string basePath, object options = null,
            JsonSerializerSettings serializerSettings = null, bool bookmarks = false)
        {
            if (options == null) return basePath;

            var path = basePath;

            if (!path.EndsWith("/"))
                path += "/";

            var optionsPairs = options.GetType() == typeof(Dictionary<string, object>)
                ? (Dictionary<string, object>) options
                : options.GetType().GetProperties()
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(options, null));

            if (bookmarks && _stateData.Bookmarks != null)
            {
                if (_stateData.Bookmarks.Any() && _stateData.Bookmarks.First() != "-end-")
                {
                    optionsPairs.Add("bookmarks", _stateData.Bookmarks);
                }
            }

            var data = new
            {
                context = new object(),
                options = optionsPairs
            };

            var wrapper = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("source_url", string.Empty),
                new KeyValuePair<string, string>("data",
                    JsonConvert.SerializeObject(data, serializerSettings ?? JsonSerializerSettings))
            };

            return wrapper.Aggregate(path, (current, pair) => Extensions.QueryStringExtensions.AddQueryParam(current, pair.Key, pair.Value));
        }

        private void SetHeaders(HttpRequestMessage request)
        {
            foreach (var header in RequestHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            request.Headers.Add("Referer", PinterestApiConstants.UrlBase);
            request.Headers.Add("Origin", PinterestApiConstants.UrlBase);
            request.Headers.Add("X-APP-VERSION", _stateData.AppVersion);
            request.Headers.Add("X-CSRFToken", _stateData.CsrfToken);
        }
    }
}