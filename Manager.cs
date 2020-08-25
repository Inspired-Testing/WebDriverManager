using Microsoft.Win32;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using WebDriverManager.Controllers;
using WebDriverManager.Models;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace WebDriverManager
{
    /// <summary>
    /// This class is used to find and download the appropriate Selenium WebDriver
    /// You can set the browser type to use, as well as the operating system.
    /// The Manager class will scan your registry, and then lookup an appropriate version.
    /// At present browser objects can be automatically detected through Windows, Linux and MAC must be created manually.
    /// </summary>
    public class Manager
    {
        #region UrlsForRetrieving versions
        internal static string firefox_url = "https://api.github.com/repos/mozilla/geckodriver/releases/";
        internal static string chrome_url = "https://chromedriver.storage.googleapis.com/";
        internal static string edge_base = "https://msedgewebdriverstorage.blob.core.windows.net/edgewebdriver";
        internal static string edge_versions = "?delimiter=%2F&maxresults=1000&restype=container&comp=list&_=1597841359922&timeout=60000";
        internal static string edge_OS(string verison,string os) => "https://msedgedriver.azureedge.net/" + verison + "/edgedriver_" + os + ".zip";

        internal static string ie_url ="https://selenium-release.storage.googleapis.com/";
        #endregion

        #region Enums
        public enum BrowserType { CHROME, FIREFOX, INTERNET_EXPLORER, EDGE}
        public enum OperatingSystem { WINDOWS, LINUX, MAC}
        #endregion

        /// <summary>
        /// Retrieves a single browser object from the browsers found on the local machine
        /// This is extracted as a method for testing purposes.
        /// </summary>
        /// <param name="browsers"></param>
        /// <param name="browserType"></param>
        /// <returns></returns>
        internal static Browser GetBrowser(List<Browser> browsers, BrowserType browserType) 
        {
            return GetBrowsers().Where(x => x.Name.Replace(" ", "_").ToUpper().Contains(browserType.ToString())).FirstOrDefault();
        }

        /// <summary>
        /// Returns an appropraite WebDriver for a given browser and Operating System
        /// If using MAC or Linux, create a Browser object with name and Version to inject
        /// </summary>
        /// <param name="browserType">Supports Chrome, Firefox, Internet explorer and Edge</param>
        /// <param name="os">Supports Windows, Linux, and MAC - defaults to Windows</param>
        /// <returns></returns>
        public static string GetWebDriver(BrowserType browserType, OperatingSystem os = OperatingSystem.WINDOWS, Browser browser = null) 
        {
            if (browser == null) 
            {
                browser = GetBrowser(GetBrowsers(), browserType);
            }

            string driver;

            switch (browserType) 
            {
                case BrowserType.CHROME:                    

                    driver = GetVersionFromGoogleAPIS(chrome_url, browser,os);
                    return DownloadAndUnzip(chrome_url+ driver, os , browser, "chromedriver.exe", "webdriver.chrome.driver");

                case BrowserType.FIREFOX:
                   
                    driver = GetFireFoxURL(browser, os);
                    return DownloadAndUnzip(driver, os, browser, "geckodriver.exe", "webdriver.gecko.driver");

                case BrowserType.INTERNET_EXPLORER:
                    
                    driver = GetVersionFromGoogleAPIS(ie_url, browser,os);
                    return DownloadAndUnzip(ie_url + driver, os, browser, "IEDriverServer.exe", "webdriver.ie.driver");

                case BrowserType.EDGE:   
                    
                    return DownloadAndUnzip(GetEdgeURL(browser,os), os, browser, "msedgedriver.exe", "webdriver.edge.driver");

                default:
                    driver = GetVersionFromGoogleAPIS(chrome_url, browser,os);
                    return DownloadAndUnzip(chrome_url + driver, os, browser, "chromedriver.exe", "webdriver.chrome.driver");
            }           
        }

        /*
         * Gets the latest Firefox release from Github using the Github API
         * The latest Geckodriver supports backwards to version 60 of Firefox
         */
        internal static string GetFireFoxURL(Browser browser, OperatingSystem os) 
        {
            try
            {
                RestController restController = new RestController(firefox_url);
                var response = restController.Get("latest");
                GitHubRelease responseData = (GitHubRelease)response.DeserialiseResponseTo<GitHubRelease>();

                var osPlatform = Environment.OSVersion;
                bool sixtyfourBit = Environment.Is64BitOperatingSystem;


                if (os.Equals(OperatingSystem.WINDOWS))
                {
                    var asset = responseData.Assets.Where(x => x.Name.ToLower().Contains("win32")).FirstOrDefault();
                    return asset.browser_download_url.AbsoluteUri;
                }
                else if (os.Equals(OperatingSystem.MAC))
                {
                    var asset = responseData.Assets.Where(x => x.Name.ToLower().Contains("macos")).FirstOrDefault();
                    return asset.browser_download_url.AbsoluteUri;
                }
                else
                {
                    var asset = responseData.Assets.Where(x => x.Name.ToLower().Contains("linux" + (sixtyfourBit ? "64" : "32"))).FirstOrDefault();
                    return asset.browser_download_url.AbsoluteUri;
                }
            }
            catch (Exception e) 
            {
                throw;
            }


        }

        /*
         * Gets all the Edge Driver versions
         * Microsoft does not use an XML structure which deserialises neatly - open to suggestions - 
         * In order to extract the useful information we use Regex to find all the Name tags and then create Version objects to compare
         */
        internal static string GetEdgeURL(Browser browser, OperatingSystem os) 
        {
            RestController restController = new RestController(edge_base);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Accept", "application/xml");
            var response = restController.Get(edge_versions,headers);

            Regex versionRegex = new Regex("(?<=(<Name>))(.*?)(?=(</Name>))");
            var versionData = versionRegex.Matches(response.ResponseData);

            if (versionData.Count == 0) 
            {
                throw new Exception("Error retrieving Edge Versions - please create a support issue.");
            }

            var versions = versionData.Where(x => x.Value.Split(".")[0].Contains(browser.Version.Split(".")[0]));
            if (versions.Count() == 0) 
            {
                throw new Exception("Unable to find a compatible driver for Edge version "+browser.Version+", please create a support issue.");
            }

            Regex versionNumberRegex = new Regex(@"\d+(\.\d+)*");
            var versionNumbers = versionNumberRegex.Matches(String.Join(' ', versions));
            List<Version> driverVersions = new List<Version>();

            foreach(Match versionMatch in versionNumbers) 
            {
                Version versionNumber = null;
                Version.TryParse(versionMatch.Value, out versionNumber);

                if (versionNumber != null) 
                {
                    driverVersions.Add(versionNumber);
                }
            }

            var version = driverVersions.Max(x => x);

            var osPlatform = Environment.OSVersion;
            bool sixtyfourBit = Environment.Is64BitOperatingSystem;

            if (os.Equals(OperatingSystem.WINDOWS))
            {
                return edge_OS(version.ToString(), "win32");
            }
            else if (os.Equals(OperatingSystem.MAC))
            {

                return edge_OS(version.ToString(), "mac" + (sixtyfourBit ? "64" : "32"));
            }
            else
            {
                return edge_OS(version.ToString(), "arm" + (sixtyfourBit ? "64" : "32"));
            }


        }

        /*
         * Retrieves Chromedriver versions from the archive 
         * Filters for the major versions of the specified browser and returns the latest driver with the same major version
         */
        internal static string GetVersionFromGoogleAPIS(string url, Browser browser,OperatingSystem os) 
        {
            RestController restController = new RestController(url);
            var response = restController.Get("");
            var data = response.DeserialiseAsXML<ListBucketResult>();
            var version = data.Contents.Where(x => x.Key.Contains(browser.Version.Split(".")[0]));

            var osPlatform = Environment.OSVersion;
            bool sixtyfourBit = Environment.Is64BitOperatingSystem;

            if (!os.Equals(OperatingSystem.WINDOWS) && browser.Name.ToLower().Contains("explorer") && browser.Name.ToLower().Contains("internet")) 
            {
                throw new NotImplementedException();
            }

            if (version.Count() == 0) 
            {
                throw new Exception("Unable to find a compatible driver for Chrome version " + browser.Version + ", please create a support issue.");
            }
           

            if (os.Equals(OperatingSystem.WINDOWS))
            {
                return version.Where(x => x.Key.ToLower().Contains("win32")).FirstOrDefault().Key;
            }
            else if (os.Equals(OperatingSystem.MAC))
            {

                return version.Where(x => x.Key.ToLower().Contains("mac" + (sixtyfourBit ? "64" : "32"))).FirstOrDefault().Key;
            }
            else
            {
                return version.Where(x => x.Key.ToLower().Contains("linux" + (sixtyfourBit ? "64" : "32"))).FirstOrDefault().Key;
            }
        }


        private static string GetLastItemInSplit(string toSplit, string splitRegex)
        {
            string[] arr = toSplit.Split(splitRegex);
            return arr[arr.Length - 1];
        }

        /// <summary>
        /// Downloads the specified URL and then extracts the docs based on its extensions, supports .zip and .tar.gz 
        /// Uses the User directory\\WebDrivers folder to store extracted files
        /// </summary>
        /// <param name="url"></param>
        /// <param name="os"></param>
        /// <param name="browser"></param>
        /// <param name="filename"></param>
        /// <param name="environmentVariable"></param>
        /// <returns></returns>
        private static string DownloadAndUnzip(string url,OperatingSystem os, Browser browser, string filename, string environmentVariable)
        {
            string user = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string destination = user + "\\WebDrivers\\" + browser.Name + "\\"+os.ToString()+"\\" + browser.Version + "\\";
            Directory.CreateDirectory(destination);

            string zipFileName = GetLastItemInSplit(url, "/");
            using (var client = new WebClient())
            {
                if (!File.Exists(destination + filename) && !File.Exists(destination + Path.GetFileNameWithoutExtension(destination + filename)))
                {
                    Console.WriteLine("Downloading WebDriver from - " + url);
                    client.DownloadFile(url, destination + zipFileName);

                    if (zipFileName.Contains(".zip"))
                    {
                        ZipFile.ExtractToDirectory(destination + zipFileName, destination);
                    }
                    else if (zipFileName.Contains(".tar.gz"))
                    {
                        ExtractTGZ(destination + zipFileName, destination);
                    }



                }
                else 
                {
                    Console.WriteLine("Skipping download as driver is already in destination directory - "+destination);
                }

                //Selenium 3.141 requires MicrosoftWebDriver.exe
                //Therefore this creates a copy of the driver with that name
                if (File.Exists(destination + "msedgedriver.exe"))
                {
                    if (!File.Exists(destination + "MicrosoftWebDriver.exe"))
                    {
                        File.Copy(destination + "msedgedriver.exe", destination + "MicrosoftWebDriver.exe");
                    }
                }
            }

            System.Environment.SetEnvironmentVariable(environmentVariable, destination);
            return destination;
        }

        internal static void ExtractTGZ(String gzArchiveName, String destFolder)
        {
            Stream inStream = File.OpenRead(gzArchiveName);
            Stream gzipStream = new GZipInputStream(inStream);

            TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
            tarArchive.ExtractContents(destFolder);
            tarArchive.Close();

            gzipStream.Close();
            inStream.Close();
        }


        /*
         * Gets all the installed browsers and their versions from the WINDOWS Registry
         */
        internal static List<Browser> GetBrowsers()
        {
            //TO-DO
            //Need methods for getting browser versions on Linux and MAC
            //.net core runs on mac and linux, this should be possible

            try
            {
                RegistryKey browserKeys;
                //on 64bit the browsers are in a different location
                browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet");
                if (browserKeys == null)
                    browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");
                string[] browserNames = browserKeys.GetSubKeyNames();
                var browsers = new List<Browser>();
                for (int i = 0; i < browserNames.Length; i++)
                {
                    Browser browser = new Browser();
                    RegistryKey browserKey = browserKeys.OpenSubKey(browserNames[i]);
                    browser.Name = (string)browserKey.GetValue(null);
                    RegistryKey browserKeyPath = browserKey.OpenSubKey(@"shell\open\command");
                    browser.Path = (string)browserKeyPath.GetValue(null).ToString().StripQuotes();
                    RegistryKey browserIconPath = browserKey.OpenSubKey(@"DefaultIcon");
                    browser.IconPath = (string)browserIconPath.GetValue(null).ToString().StripQuotes();
                    browsers.Add(browser);
                    if (browser.Path != null)
                        browser.Version = FileVersionInfo.GetVersionInfo(browser.Path).FileVersion;
                    else
                        browser.Version = "unknown";
                }
                return browsers;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Registry Access Error");
            }

        }
    }

    internal static class Extensions
    {
        ///
        /// if string begins and ends with quotes, they are removed
        ///
        internal static String StripQuotes(this String s)
        {
            if (s.EndsWith("\"") && s.StartsWith("\""))
            {
                return s.Substring(1, s.Length - 2);
            }
            else
            {
                return s;
            }
        }
    }

   /// <summary>
   /// Class for encapsulating Browser information
   /// Name is important for IE: Use "Internet Explorer"
   /// The name will otherwise be used for storing drivers and archiving
   /// </summary>
    public class Browser
    {

        internal string Name { get; set; }
        internal string Path { get; set; }
        internal string IconPath { get; set; }
        internal string Version { get; set; }
    }
}
