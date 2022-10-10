using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Bogus.DataSets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using PinterestTorBot.PinterestClient.Api;
using PinterestTorBot.PinterestClient.Api.Session;
using PinterestTorBot.PinterestClient.Http;

namespace PinterestTorBot
{
    public class Tor
    {
        private const string ResourceRedirectUrl = "https://localhost/";
        private const string UsersFileName = "accounts.txt";
        private const string ApisFileName = "apis.txt";
        private const string IpsFileName = "ips.txt";

        private int _apiCount;

        public void Run(int count, int apiCount)
        {
            _apiCount = apiCount;

            for (var i = 0; i < count; i++)
            {
                try
                {
                    CreateAccountMobile();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                Console.WriteLine($"Kalan hesap: {count - (i + 1)}");
            }
        }

        protected void CreateAccount()
        {
            var faker = new Faker();

            var gender = faker.Person.Gender;
            var name = faker.Name.FirstName(gender);
            var surname = faker.Name.LastName(gender);
            var username =
                (name.ToLowerInvariant() + "_" + surname.ToLowerInvariant() + faker.Random.Number(99)).Replace(" ",
                    string.Empty);
            var email = faker.Internet.Email(name.ToLowerInvariant(), surname.ToLowerInvariant());
            var password = faker.Internet.Password(16);
            var businessName = $"{name} {surname}";
            var avatarUrl = faker.Internet.Avatar();

            var profile = new FirefoxProfile(@"C:\Tor Browser\Browser\TorBrowser\Data\Browser\profile.default");
            profile.SetPreference("general.useragent.override", TorConsts.HeaderUserAgent);
            profile.SetPreference("permissions.default.image", 2);

            using (var driver = new FirefoxDriver(new FirefoxBinary(@"C:\Tor Browser\Browser\firefox.exe"), profile,
                TimeSpan.FromSeconds(30)))
            {
                try
                {
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                    driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(30));
                    driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(30));
                    driver.Manage().Window.Maximize();

                    driver.Navigate().GoToUrl("https://api.myip.com/");

                    var ipPageSource = JsonConvert.DeserializeObject<JObject>(StripHtml(driver.PageSource));

                    using (var file =
                        new StreamWriter(
                            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                IpsFileName),
                            true))
                    {
                        file.WriteLine(ipPageSource.ToString());
                    }


                    Console.WriteLine($"Creating new account: {email}");

                    driver.Navigate().GoToUrl(TorConsts.UrlBase);

                    driver.FindElement(By.XPath("//div[contains(text(),'Create a business account')]")).Click();
                    driver.FindElement(By.Id("email")).SendKeys(email);
                    driver.FindElement(By.Id("password")).SendKeys(password);
                    driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                    var newUserLanguage = wait.Until(webDriver =>
                    {
                        try
                        {
                            return webDriver.FindElement(By.Id("newUserLanguage"));
                        }
                        catch (NoSuchElementException e)
                        {
                            return null;
                        }
                    });

                    newUserLanguage.Click();
                    driver.FindElement(By.CssSelector("#newUserLanguage option[value='en-US']")).Click();
                    driver.FindElement(By.Id("newUserCountry")).Click();
                    driver.FindElement(By.CssSelector("#newUserCountry option[value='US']")).Click();
                    driver.FindElement(By.CssSelector(".NuxContainer__NuxStepContainer button[type='submit']")).Click();

                    driver.FindElement(By.Id("name")).SendKeys(businessName);
                    driver.FindElement(By.CssSelector(".NuxContainer__NuxStepContainer button[type='button']")).Click();


                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            driver.FindElement(
                                By.CssSelector(".NuxContainer__NuxStepContainer button[type='submit']")).Click();
                            driver.FindElement(
                                By.CssSelector(".NuxContainer__NuxStepContainer button[type='button']")).Click();
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    driver.FindElement(By.CssSelector("label[for='adv_intentYes']")).Click();
                    driver.FindElement(By.CssSelector(".NuxContainer__NuxStepContainer button[type='button']")).Click();

                    driver.FindElement(By.CssSelector("label[for='contact_infoNo']")).Click();
                    driver.FindElement(
                        By.CssSelector(".NuxContainer__NuxStepContainer button[type='button']:not(:disabled)")).Click();

                    IWebElement interestEl;
                    IWebElement interestElSubmit;

                    try
                    {
                        interestEl = driver.FindElement(
                            By.CssSelector(".NuxContainer__NuxStepContainer .NuxInterest:first-child"));
                        interestElSubmit =
                            driver.FindElement(By.CssSelector(".NuxContainer__NuxStepContainer button[type='submit']"));
                    }
                    catch
                    {
                        interestEl = null;
                        interestElSubmit = null;
                    }

                    interestEl?.Click();
                    interestElSubmit?.Click();

                    driver.FindElement(By.CssSelector(".BizNuxExtensionUpsell__optionalSkip")).Click();

                    driver.Navigate().GoToUrl(TorConsts.UrlBase + "settings");

                    driver.FindElement(By.Id("username")).Clear();
                    driver.FindElement(By.Id("username")).SendKeys(username);
                    driver.FindElement(By.Id("about")).SendKeys(faker.Name.JobTitle());
//                driver.FindElement(By.XPath("//button[contains(text(),'Change')]")).Click();
                    driver.FindElement(By.CssSelector("div[data-test-id='done-button'] button[type='button']")).Click();
                    driver.Navigate().Refresh();

//                driver.FindElement(By.CssSelector("a[href='/settings/account-settings']")).Click();
//                driver.FindElement(By.Id("contact_name")).Clear();
//                driver.FindElement(By.Id("contact_name")).SendKeys(businessName);
//                driver.FindElement(By.CssSelector($"label[for='{(gender == Name.Gender.Male ? "male" : "female")}']"))
//                    .Click();
//                driver.FindElement(By.CssSelector("div[data-test-id='done-button'] button[type='button']")).Click();

                    using (var file =
                        new StreamWriter(
                            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                UsersFileName),
                            true))
                    {
                        file.WriteLine($"{email}\t{username}\t{password}");
                    }

                    if (_apiCount > 0)
                    {
                        Task.Delay(TimeSpan.FromSeconds(2)).Wait();

                        driver.Navigate().GoToUrl(TorConsts.DeveloperUrlApps);

                        if (!driver.Url.StartsWith(TorConsts.DeveloperUrlApps))
                        {
                            throw new Exception("Pinterest api create login error.");
                        }

                        var termsWait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                        IWebElement termsEl;

                        try
                        {
                            termsEl = termsWait.Until(webDriver =>
                            {
                                try
                                {
                                    return webDriver.FindElement(By.Name("terms"));
                                }
                                catch (NoSuchElementException e)
                                {
                                    return null;
                                }
                            });
                        }
                        catch
                        {
                            termsEl = null;
                        }

                        termsEl?.Click();
                    }

                    for (var i = 0; i < _apiCount; i++)
                    {
                        try
                        {
                            driver.FindElement(By.XPath("//button[contains(text(),'Create app')]")).Click();
                            driver.FindElement(By.Name("name")).SendKeys(Guid.NewGuid().ToString("N"));
                            driver.FindElement(By.Name("description")).SendKeys(Guid.NewGuid().ToString("N"));
                            driver.FindElement(By.CssSelector(".modal.createApp footer button")).Click();

                            var devwait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

                            var devFirstEl = devwait.Until(webDriver =>
                            {
                                try
                                {
                                    return webDriver.FindElement(By.CssSelector(".row.appInfo .appId input"));
                                }
                                catch (NoSuchElementException e)
                                {
                                    return null;
                                }
                            });

                            var appId = devFirstEl.GetAttribute("value");
                            driver.FindElement(By.CssSelector(".row.appInfo .secret div button")).Click();
                            var secret = driver.FindElement(By.CssSelector(".row.appInfo .secret input"))
                                .GetAttribute("value");

                            var urlWait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

                            var urlInput = urlWait.Until(webDriver =>
                            {
                                try
                                {
                                    return webDriver.FindElement(
                                        By.XPath(
                                            "//span[contains(text(),'Redirect URIs')]/following-sibling::div//input"));
                                }
                                catch (NoSuchElementException e)
                                {
                                    return null;
                                }
                            });

                            urlInput.SendKeys(ResourceRedirectUrl + Keys.Enter);
                            driver.ExecuteScript(
                                "$(\"span:contains('Redirect URIs') ~ div input\")[0].dispatchEvent(new KeyboardEvent('keyup', { bubbles: true, cancelable: true, keyCode: 13, which: 13, key: 'Enter' }));");
                            driver.FindElement(By.CssSelector(".banner .container .save.row button")).Click();

                            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

                            var apiPath = TorConsts.OauthUrlBase;

                            apiPath = new Dictionary<string, string>()
                            {
                                {"response_type", "code"},
                                {"redirect_uri", ResourceRedirectUrl},
                                {"client_id", appId},
                                {"scope", "read_public,write_public,read_relationship,write_relationship"},
                                {"state", ""}
                            }.Aggregate(apiPath, (current, pair) => current.AddQueryParam(pair.Key, pair.Value));

                            driver.Navigate().GoToUrl(apiPath);
                            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

                            var code = Regex.Match(driver.Url, @"code=(.*)").Groups[1].Value;

                            var tokenPath = TorConsts.OauthAccessUrlBase;

                            tokenPath = new Dictionary<string, string>()
                            {
                                {"grant_type", "authorization_code"},
                                {"client_id", appId},
                                {"client_secret", secret},
                                {"code", code}
                            }.Aggregate(tokenPath, (current, pair) => current.AddQueryParam(pair.Key, pair.Value));

                            driver.ExecuteScript(
                                $"var form = document.createElement('form'); form.method = 'post'; form.action = '{tokenPath}'; document.body.appendChild(form); form.submit();");


                            var accessToken = Regex.Match(driver.PageSource,
                                    "\"access_token\": \"((.(?<!((\"|\')\\)|(\"|\')\\s\\+|(\"|\')\\,)))*)\"").Groups[1]
                                .Value;

                            using (var file =
                                new StreamWriter(
                                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                        ApisFileName),
                                    true))
                            {
                                if (!string.IsNullOrEmpty(accessToken))
                                {
                                    file.WriteLine($"{username}\t{appId}\t{secret}\t{accessToken}");
                                }
                            }

                            driver.Navigate().GoToUrl(TorConsts.DeveloperUrlApps);
                        }
                        catch
                        {
                            driver.Navigate().GoToUrl(TorConsts.DeveloperUrlApps);
                        }
                    }

                    driver.Manage().Cookies.DeleteAllCookies();
                }
                finally
                {
                    Task.Run(RefreshTorIdentity).Wait();
                    driver.Manage().Cookies.DeleteAllCookies();
                    driver.Quit();
                }
            }
        }

        protected void CreateAccountMobile()
        {
            var faker = new Faker();

            var gender = faker.Person.Gender;
            var name = faker.Name.FirstName(gender);
            var surname = faker.Name.LastName(gender);
            var username =
                (name.ToLowerInvariant() + "_" + surname.ToLowerInvariant() + faker.Random.Number(99)).Replace(" ",
                    string.Empty);
            var email = faker.Internet.Email(name.ToLowerInvariant(), surname.ToLowerInvariant(), null,
                faker.Random.Number(99).ToString());
            var password = faker.Internet.Password(16);
            var businessName = $"{name} {surname}";
            var avatarUrl = faker.Internet.Avatar();

            var profile = new FirefoxProfile(@"C:\Tor Browser\Browser\TorBrowser\Data\Browser\profile.default");
            profile.SetPreference("general.useragent.override", TorConsts.HeaderMobileUserAgent);
            profile.SetPreference("permissions.default.image", 2);

            using (var driver = new FirefoxDriver(new FirefoxBinary(@"C:\Tor Browser\Browser\firefox.exe"), profile,
                TimeSpan.FromSeconds(30)))
            {
                try
                {
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                    driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(30));
                    driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(30));
                    driver.Manage().Window.Size = new Size(414, 736);

//                    driver.Navigate().GoToUrl("https://api.myip.com/");

//                    var ipPageSource = JsonConvert.DeserializeObject<JObject>(StripHtml(driver.PageSource));
//
//                    if (ipPageSource.Value<string>("country") == "Unknown")
//                    {
//                        throw new Exception("Country is unknown");
//                    }
//
//                    IPAddress.TryParse(ipPageSource.Value<string>("ip"), out var ipAddress);
//
//                    if (ipAddress.IsIPv6Multicast || ipAddress.IsIPv6Teredo)
//                    {
//                        throw new Exception("ipv6 skipping");
//                    }
//
//                    using (var file =
//                        new StreamWriter(
//                            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
//                                IpsFileName),
//                            true))
//                    {
//                        file.WriteLine(ipPageSource.ToString());
//                    }

                    Console.WriteLine($"Creating new account: {email}");

                    driver.Navigate().GoToUrl(TorConsts.UrlBase);

                    var crebtn = (RemoteWebElement) driver.FindElement(By.XPath("//*[contains(text(),'Create a business account')]"));
                    driver.Mouse.MouseMove(crebtn.Coordinates);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    driver.Mouse.Click(crebtn.Coordinates);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    var crebtn2 = (RemoteWebElement) driver.FindElement(By.XPath("//*[contains(text(),'Create account')]"));
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    driver.Mouse.MouseMove(crebtn2.Coordinates);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    driver.Mouse.Click(crebtn2.Coordinates);
                    
                    var emailInput = (RemoteWebElement) driver.FindElement(By.Id("email"));
                    driver.Mouse.MouseMove(emailInput.Coordinates);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    driver.Mouse.Click(emailInput.Coordinates);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    driver.Keyboard.SendKeys(email);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    driver.Keyboard.PressKey(Keys.Enter);
                    driver.Keyboard.ReleaseKey(Keys.Enter);
                    
                    var passwordInput = (RemoteWebElement) driver.FindElement(By.Id("password"));
                    driver.Mouse.MouseMove(passwordInput.Coordinates);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    driver.Mouse.Click(passwordInput.Coordinates);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    driver.Keyboard.SendKeys(password);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    driver.Keyboard.PressKey(Keys.Enter);
                    driver.Keyboard.ReleaseKey(Keys.Enter);
                    
                    var nameInput = (RemoteWebElement) driver.FindElement(By.Id("name"));
                    driver.Mouse.MouseMove(nameInput.Coordinates);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    driver.Mouse.Click(nameInput.Coordinates);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    driver.Keyboard.SendKeys(businessName);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    driver.Keyboard.PressKey(Keys.Enter);
                    driver.Keyboard.ReleaseKey(Keys.Enter);

                    try
                    {
                        var fwait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

                        fwait.Until(webDriver =>
                        {
                            try
                            {
                                return webDriver.FindElement(By.XPath("//*[contains(text(),'No thanks')]"));
                            }
                            catch (NoSuchElementException e)
                            {
                                return null;
                            }
                        }).Click();
                    }
                    catch
                    {
                        // ignored
                    }

                    try
                    {
                        var fwait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

                        fwait.Until(webDriver =>
                        {
                            try
                            {
                                return webDriver.FindElement(By.XPath("//*[contains(text(),\"I'll do it later\")]"));
                            }
                            catch (NoSuchElementException e)
                            {
                                return null;
                            }
                        }).Click();
                    }
                    catch
                    {
                        // ignored
                    }

                    try
                    {
                        var fwait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

                        var interestEls = fwait.Until(webDriver =>
                        {
                            try
                            {
                                return webDriver
                                    .FindElements(By.CssSelector("div[data-test-id='nux-picker-topic']"))
                                    .Take(12)
                                    .OrderBy(o => Guid.NewGuid())
                                    .Take(7)
                                    .ToList();
                            }
                            catch (NoSuchElementException e)
                            {
                                return null;
                            }
                        });

                        foreach (var interestEl in interestEls)
                        {
                            interestEl.Click();
                        }

                        driver.FindElement(By.CssSelector("button[type='button']")).Click();
                    }
                    catch
                    {
                        // ignored
                    }

                    driver.Navigate().GoToUrl(TorConsts.UrlBase + "settings/edit/");

                    driver.FindElement(By.Id("country")).Click();
                    driver.FindElement(By.CssSelector("#country option[value='US']")).Click();

                    driver.Navigate().GoToUrl(TorConsts.UrlBase + "settings/profile/");
                    driver.FindElement(By.Id("username")).Clear();
                    driver.FindElement(By.Id("username")).SendKeys(username);
                    driver.FindElement(By.Id("username")).SendKeys(username.Substring(3));
                    driver.FindElement(By.Id("about")).SendKeys(faker.Name.JobTitle());

                    driver.Navigate().Refresh();

                    var httpClient = new PinterestClient.PinterestClient(string.Empty, string.Empty);

                    var userInfo = httpClient.Pinners.GetUserInfoAsync(username).Result;

                    if (userInfo == null || userInfo.Indexed != true)
                    {
                        throw new Exception("Hesap bulunamadı veya shadowlu.");
                    }

                    using (var file =
                        new StreamWriter(
                            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                UsersFileName),
                            true))
                    {
                        file.WriteLine($"{email}\t{username}\t{password}");
                    }

                    if (_apiCount > 0)
                    {
                        Task.Delay(TimeSpan.FromSeconds(5)).Wait();

                        driver.Manage().Window.Size = new Size(1024, 1366);

                        driver.Navigate().GoToUrl(TorConsts.DeveloperUrlApps);

                        if (!driver.Url.StartsWith(TorConsts.DeveloperUrlApps))
                        {
                            throw new Exception("Pinterest api create login error.");
                        }

                        var termsWait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                        IWebElement termsEl;

                        try
                        {
                            termsEl = termsWait.Until(webDriver =>
                            {
                                try
                                {
                                    return webDriver.FindElement(By.Name("terms"));
                                }
                                catch (NoSuchElementException e)
                                {
                                    return null;
                                }
                            });
                        }
                        catch
                        {
                            termsEl = null;
                        }

                        termsEl?.Click();
                    }

                    for (var i = 0; i < _apiCount; i++)
                    {
                        try
                        {
                            driver.FindElement(By.XPath("//button[contains(text(),'Create app')]")).Click();
                            driver.FindElement(By.Name("name")).SendKeys(Guid.NewGuid().ToString("N"));
                            driver.FindElement(By.Name("description")).SendKeys(Guid.NewGuid().ToString("N"));
                            driver.FindElement(By.CssSelector(".modal.createApp footer button")).Click();

                            var devwait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

                            var devFirstEl = devwait.Until(webDriver =>
                            {
                                try
                                {
                                    return webDriver.FindElement(By.CssSelector(".row.appInfo .appId input"));
                                }
                                catch (NoSuchElementException e)
                                {
                                    return null;
                                }
                            });

                            var appId = devFirstEl.GetAttribute("value");
                            driver.FindElement(By.CssSelector(".row.appInfo .secret div button")).Click();
                            var secret = driver.FindElement(By.CssSelector(".row.appInfo .secret input"))
                                .GetAttribute("value");

                            var urlWait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

                            var urlInput = urlWait.Until(webDriver =>
                            {
                                try
                                {
                                    return webDriver.FindElement(
                                        By.XPath(
                                            "//span[contains(text(),'Redirect URIs')]/following-sibling::div//input"));
                                }
                                catch (NoSuchElementException e)
                                {
                                    return null;
                                }
                            });

                            urlInput.SendKeys(ResourceRedirectUrl + Keys.Enter);
                            driver.ExecuteScript(
                                "$(\"span:contains('Redirect URIs') ~ div input\")[0].dispatchEvent(new KeyboardEvent('keyup', { bubbles: true, cancelable: true, keyCode: 13, which: 13, key: 'Enter' }));");
                            driver.FindElement(By.CssSelector(".banner .container .save.row button")).Click();

                            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

                            var apiPath = TorConsts.OauthUrlBase;

                            apiPath = new Dictionary<string, string>()
                            {
                                {"response_type", "code"},
                                {"redirect_uri", ResourceRedirectUrl},
                                {"client_id", appId},
                                {"scope", "read_public,write_public,read_relationship,write_relationship"},
                                {"state", ""}
                            }.Aggregate(apiPath, (current, pair) => current.AddQueryParam(pair.Key, pair.Value));

                            driver.Navigate().GoToUrl(apiPath);
                            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                            Task.Delay(TimeSpan.FromSeconds(2)).Wait();

                            var code = Regex.Match(driver.Url, @"code=(.*)").Groups[1].Value;

                            var tokenPath = TorConsts.OauthAccessUrlBase;

                            tokenPath = new Dictionary<string, string>()
                            {
                                {"grant_type", "authorization_code"},
                                {"client_id", appId},
                                {"client_secret", secret},
                                {"code", code}
                            }.Aggregate(tokenPath, (current, pair) => current.AddQueryParam(pair.Key, pair.Value));

                            driver.ExecuteScript(
                                $"var form = document.createElement('form'); form.method = 'post'; form.action = '{tokenPath}'; document.body.appendChild(form); form.submit();");


                            var accessToken = Regex.Match(driver.PageSource,
                                    "\"access_token\": \"((.(?<!((\"|\')\\)|(\"|\')\\s\\+|(\"|\')\\,)))*)\"").Groups[1]
                                .Value;

                            using (var file =
                                new StreamWriter(
                                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                        ApisFileName),
                                    true))
                            {
                                if (!string.IsNullOrEmpty(accessToken))
                                {
                                    file.WriteLine($"{username}\t{appId}\t{secret}\t{accessToken}");
                                }
                            }

                            driver.Navigate().GoToUrl(TorConsts.DeveloperUrlApps);
                        }
                        catch
                        {
                            driver.Navigate().GoToUrl(TorConsts.DeveloperUrlApps);
                        }
                    }

                    driver.Manage().Cookies.DeleteAllCookies();
                }
                finally
                {
                    Task.Run(RefreshTorIdentity).Wait();
                    driver.Manage().Cookies.DeleteAllCookies();
                    driver.Quit();
                }
            }
        }

        public void RefreshTorIdentity()
        {
            Console.WriteLine("Identity Refreshing...");
            var ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9151);
            var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                try
                {
                    client.Connect(ip);
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Unable to connect to server of Tor.");
                    throw;
                }

                client.Send(Encoding.ASCII.GetBytes("AUTHENTICATE \"123456\"\n"));
                var data = new byte[1024];
                var receivedDataLength = client.Receive(data);
                var stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);

                if (stringData.Contains("250"))
                {
                    client.Send(Encoding.ASCII.GetBytes("SIGNAL NEWNYM\r\n"));
                    data = new byte[1024];
                    receivedDataLength = client.Receive(data);
                    stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);
                    if (!stringData.Contains("250"))
                    {
                        throw new Exception("Unable to signal new user to server of Tor.");
                    }
                }
                else
                {
                    throw new Exception("Unable to authenticate to server of Tor.");
                }
            }
            catch (Exception e)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                Console.WriteLine(e.Message);
                throw;
            }

            Console.WriteLine("Identity Refreshed.");
        }

        public static string StripHtml(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        protected void CreateAccountWithJs()
        {
            var faker = new Faker();

            var gender = faker.Person.Gender;
            var name = faker.Name.FirstName(gender);
            var surname = faker.Name.LastName(gender);
            var username =
                (name.ToLowerInvariant() + "_" + surname.ToLowerInvariant() + faker.Random.Number(99)).Replace(" ",
                    string.Empty);
            var email = faker.Internet.Email(name.ToLowerInvariant(), surname.ToLowerInvariant());
            var password = faker.Internet.Password(16);
            var businessName = $"{name} {surname}";
            var avatarUrl = faker.Internet.Avatar();

            var profile = new FirefoxProfile(@"C:\Tor Browser\Browser\TorBrowser\Data\Browser\profile.default");
            profile.SetPreference("general.useragent.override", TorConsts.HeaderUserAgent);
            profile.SetPreference("permissions.default.image", 2);

            var indexFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "index.html");
            using (var driver = new FirefoxDriver(new FirefoxBinary(@"C:\Tor Browser\Browser\firefox.exe"), profile,
                TimeSpan.FromSeconds(30)))
            {
                try
                {
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                    driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(30));
                    driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(30));

                    driver.Navigate().GoToUrl(indexFile);

                    var jsClient = new PinterestJsClient(new UserSessionData(), driver);

                    jsClient.InitCsrfToken();

                    Task.Delay(TimeSpan.FromSeconds(60)).Wait();

//                    driver.ExecuteScript(
//                        $"");

                    driver.Manage().Cookies.DeleteAllCookies();
                }
                finally
                {
                    Task.Run(RefreshTorIdentity).Wait();
                    driver.Quit();
                }
            }
        }
    }
}