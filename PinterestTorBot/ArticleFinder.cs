using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using PinterestTorBot.EntityFramework;
using PinterestTorBot.Models;

namespace PinterestTorBot
{
    public class ArticleFinder
    {
        public async Task Run()
        {
            var profile = new FirefoxProfile(@"C:\Tor Browser\Browser\TorBrowser\Data\Browser\profile.default");
            profile.SetPreference("general.useragent.override", TorConsts.HeaderUserAgent);
            profile.SetPreference("permissions.default.image", 2);

            var workingDir = Path.GetDirectoryName(GetType().Assembly.Location);
            var bookmarkFile = Path.Combine(workingDir, "bookmark.txt");
            var idsFile = Path.Combine(workingDir, "ids.txt");

            var bookmark = 1;

            if (File.Exists(bookmarkFile))
            {
                bookmark = Convert.ToInt32(File.ReadAllText(bookmarkFile));
            }

            var ids = File.ReadAllLines(idsFile)
                .Select(s => Convert.ToInt32(s))
                .OrderBy(i => i)
                .Where(i => i > bookmark)
                .ToList();

            var latestId = 0;
            var tryCount = 0;

            begin:

            using (var driver = new FirefoxDriver(new FirefoxBinary(@"C:\Tor Browser\Browser\firefox.exe"), profile,
                TimeSpan.FromSeconds(30)))
            {

                foreach (var id in ids.Where(i => i >= latestId))
                {
                    File.WriteAllText(bookmarkFile, id.ToString());
                    
                    driver.Navigate().GoToUrl($"https://ezinearticles.com/ezinepublisher/?id={id}");
                    Console.WriteLine($"{DateTime.Now:G} -> {id}");

                    if (driver.Title.Contains("Page not found"))
                    {
                        Console.WriteLine($"{DateTime.Now:G} -> 404");
                        continue;
                    }

                    string plainArticle;

                    try
                    {
                        plainArticle = driver.FindElementById("plain-article").Text;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        driver.Manage().Cookies.DeleteAllCookies();
                        RefreshTorIdentity();

                        if (tryCount > 3)
                        {
                            Console.WriteLine($"{DateTime.Now:G} -> Retry count exited, continue...");
                            tryCount = 0;
                            continue;
                        }

                        tryCount++;
                        latestId = id;
                        driver.Close();
                        driver.Dispose();
                        goto begin;
                    }
                    
                    tryCount = 0;

                    using (var context = new AppDbContext())
                    {
                        context.Add(new Article
                        {
                            Text = plainArticle,
                            Html = driver.FindElementById("formatted-article").Text,
                            Summary = driver.FindElementById("article-summary").Text,
                            Keywords = driver.FindElementById("article-keywords").GetAttribute("value")
                        });

                        await context.SaveChangesAsync();
                        Console.WriteLine($"{DateTime.Now:G} -> {id}: Article saved.");
                    }
                }
            }
        }

        public async Task Run2()
        {
            var profile = new FirefoxProfile(@"C:\Tor Browser\Browser\TorBrowser\Data\Browser\profile.default");
            profile.SetPreference("general.useragent.override", TorConsts.HeaderUserAgent);
            profile.SetPreference("permissions.default.image", 2);

            var workingDir = Path.GetDirectoryName(GetType().Assembly.Location);
            var bookmarkFile = Path.Combine(workingDir, "bookmark.txt");
            var idsFile = Path.Combine(workingDir, "ids.txt");

            var bookmark = 1;

            if (File.Exists(bookmarkFile))
            {
                bookmark = Convert.ToInt32(File.ReadAllText(bookmarkFile));
            }

            var ids = File.ReadAllLines(idsFile)
                .Select(s => Convert.ToInt32(s))
                .OrderBy(i => i)
                .Where(i => i > bookmark)
                .ToList();

            var latestId = 0;
            var tryCount = 0;

            begin:

            using (var driver = new FirefoxDriver(new FirefoxBinary(@"C:\Tor Browser\Browser\firefox.exe"), profile,
                TimeSpan.FromSeconds(30)))
            {

                foreach (var id in ids.Where(i => i >= latestId))
                {
                    File.WriteAllText(bookmarkFile, id.ToString());
                    
                    var url = "https://translate.google.com/translate?hl=en&ie=UTF8&prev=_t&sl=ar&tl=en&u=" +
                              HttpUtility.UrlEncode($"https://ezinearticles.com/ezinepublisher/?id={id}");

                    driver.Navigate().GoToUrl(url);
                    Console.WriteLine($"{DateTime.Now:G} -> {id}");

                    if (driver.Title.Contains("Page not found"))
                    {
                        Console.WriteLine($"{DateTime.Now:G} -> 404");
                        continue;
                    }

                    string plainArticle;

                    try
                    {
                        var plainTextWait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                        plainArticle = plainTextWait.Until(webDriver =>
                        {
                            try
                            {
                                return webDriver.FindElement(By.Id("plain-article"));
                            }
                            catch (NoSuchElementException e)
                            {
                                return null;
                            }
                        }).Text;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        driver.Manage().Cookies.DeleteAllCookies();
                        RefreshTorIdentity();

                        if (tryCount > 3)
                        {
                            Console.WriteLine($"{DateTime.Now:G} -> Retry count exited, continue...");
                            tryCount = 0;
                            continue;
                        }

                        tryCount++;
                        latestId = id;
                        driver.Dispose();
                        goto begin;
                    }
                    
                    tryCount = 0;

                    using (var context = new AppDbContext())
                    {
                        context.Add(new Article
                        {
                            Text = plainArticle,
                            Html = driver.FindElementById("formatted-article").Text,
                            Summary = driver.FindElementById("article-summary").Text,
                            Keywords = driver.FindElementById("article-keywords").GetAttribute("value")
                        });

                        await context.SaveChangesAsync();
                        Console.WriteLine($"{DateTime.Now:G} -> {id}: Article saved.");
                    }
                }
            }
        }

        public void IdentityRefreshTest()
        {
            var profile = new FirefoxProfile(@"C:\Tor Browser\Browser\TorBrowser\Data\Browser\profile.default");
            profile.SetPreference("general.useragent.override", TorConsts.HeaderMobileUserAgent);
            profile.SetPreference("permissions.default.image", 2);

            begin:

            using (var driver = new FirefoxDriver(new FirefoxBinary(@"C:\Tor Browser\Browser\firefox.exe"), profile,
                TimeSpan.FromSeconds(30)))
            {
                driver.Navigate().GoToUrl("https://api.myip.com/");

                var ipPageSource = JsonConvert.DeserializeObject<JObject>(StripHtml(driver.PageSource));

                IPAddress.TryParse(ipPageSource.Value<string>("ip"), out var ipAddress);

                Console.WriteLine(ipAddress);
                RefreshTorIdentity();
                goto begin;
            }
        }

        public static string StripHtml(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
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
    }
}