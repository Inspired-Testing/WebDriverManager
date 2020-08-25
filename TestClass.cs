using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace WebDriverManager
{
    [TestFixture]    
    public class TestClass
    {
        [OneTimeSetUp]
        public void Setup() 
        {
            string user = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string destination = user + "\\WebDrivers\\";
            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }
           
        }

        [Test]
        public void GoogleChromeWindows() 
        {
            string chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.CHROME, Manager.OperatingSystem.WINDOWS);
            var files = Directory.GetFiles(chromedriverDir);
            var filenames = files.Select(x => Path.GetFileName(x));
            Assert.IsTrue(filenames.Contains("chromedriver.exe"));
        }

        [Test]
        public void FireFoxWindows()
        {
            string chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.FIREFOX, Manager.OperatingSystem.WINDOWS);
            var files = Directory.GetFiles(chromedriverDir);
            var filenames = files.Select(x => Path.GetFileName(x));
            Assert.IsTrue(filenames.Contains("geckodriver.exe"));
        }

        [Test]
        public void InternetExplorerWindows()
        {
            string chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.INTERNET_EXPLORER, Manager.OperatingSystem.WINDOWS);
            var files = Directory.GetFiles(chromedriverDir);
            var filenames = files.Select(x => Path.GetFileName(x));
            Assert.IsTrue(filenames.Contains("IEDriverServer.exe"));
        }

        [Test]
        public void EdgeWindows()
        {
            string chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.EDGE, Manager.OperatingSystem.WINDOWS);
            var files = Directory.GetFiles(chromedriverDir);
            var filenames = files.Select(x => Path.GetFileName(x));
            Assert.IsTrue(filenames.Contains("msedgedriver.exe"));
        }

        [Test]
        public void GoogleChromeLinux()
        {
            string chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.CHROME, Manager.OperatingSystem.LINUX);
            var files = Directory.GetFiles(chromedriverDir);
            var filenames = files.Select(x => Path.GetFileName(x));
            Assert.IsTrue(filenames.Contains("chromedriver"));
        }

        [Test]
        public void FireFoxLinux()
        {
            string chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.FIREFOX, Manager.OperatingSystem.LINUX);
            var files = Directory.GetFiles(chromedriverDir);
            var filenames = files.Select(x => Path.GetFileName(x));
            Assert.IsTrue(filenames.Contains("geckodriver"));
        }

        [Test]
        public void InternetExplorerLinux()
        {
            Assert.Throws<NotImplementedException>(() => Manager.GetWebDriver(Manager.BrowserType.INTERNET_EXPLORER, Manager.OperatingSystem.LINUX));
            
        }

        [Test]
        public void EdgeLinux()
        {
            string chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.EDGE, Manager.OperatingSystem.LINUX);
            chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.EDGE, Manager.OperatingSystem.LINUX);
            var files = Directory.GetFiles(chromedriverDir);
            var filenames = files.Select(x => Path.GetFileName(x));
            Assert.IsTrue(filenames.Contains("msedgedriver.exe"));
        }

        [Test]
        public void GoogleChromeWindows_PreDownload()
        {
            string chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.CHROME, Manager.OperatingSystem.WINDOWS);
            chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.CHROME, Manager.OperatingSystem.WINDOWS);
            var files = Directory.GetFiles(chromedriverDir);
            var filenames = files.Select(x => Path.GetFileName(x));
            Assert.IsTrue(filenames.Contains("chromedriver.exe"));
        }

        [Test]
        public void FireFoxWindows_PreDownload()
        {
            string chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.FIREFOX, Manager.OperatingSystem.WINDOWS);
            chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.FIREFOX, Manager.OperatingSystem.WINDOWS);
            var files = Directory.GetFiles(chromedriverDir);
            var filenames = files.Select(x => Path.GetFileName(x));
            Assert.IsTrue(filenames.Contains("geckodriver.exe"));
        }

        [Test]
        public void InternetExplorerWindows_PreDownload()
        {
            string chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.INTERNET_EXPLORER, Manager.OperatingSystem.WINDOWS);
            chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.INTERNET_EXPLORER, Manager.OperatingSystem.WINDOWS);
            var files = Directory.GetFiles(chromedriverDir);
            var filenames = files.Select(x => Path.GetFileName(x));
            Assert.IsTrue(filenames.Contains("IEDriverServer.exe"));
        }

        [Test]
        public void EdgeWindows_PreDownload()
        {
            string chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.EDGE, Manager.OperatingSystem.WINDOWS);
            chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.EDGE, Manager.OperatingSystem.WINDOWS);
            var files = Directory.GetFiles(chromedriverDir);
            var filenames = files.Select(x => Path.GetFileName(x));
            Assert.IsTrue(filenames.Contains("msedgedriver.exe"));
        }

        [Test]
        public void GoogleChromeLinux_PreDownload()
        {
            string chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.CHROME, Manager.OperatingSystem.LINUX);
            chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.CHROME, Manager.OperatingSystem.LINUX);
            var files = Directory.GetFiles(chromedriverDir);
            var filenames = files.Select(x => Path.GetFileName(x));
            Assert.IsTrue(filenames.Contains("chromedriver"));
        }

        [Test]
        public void FireFoxLinux_PreDownload()
        {
            string chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.FIREFOX, Manager.OperatingSystem.LINUX);
            chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.FIREFOX, Manager.OperatingSystem.LINUX);
            var files = Directory.GetFiles(chromedriverDir);
            var filenames = files.Select(x => Path.GetFileName(x));
            Assert.IsTrue(filenames.Contains("geckodriver"));
        }        

        [Test]
        public void EdgeLinux_PreDownload()
        {
            string chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.EDGE, Manager.OperatingSystem.LINUX);
            chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.EDGE, Manager.OperatingSystem.LINUX);
            var files = Directory.GetFiles(chromedriverDir);
            var filenames = files.Select(x => Path.GetFileName(x));
            Assert.IsTrue(filenames.Contains("msedgedriver.exe"));
        }

        [Test]
        public void Chrome_NonExistentVersion() 
        {
            Browser browser = Manager.GetBrowser(Manager.GetBrowsers(), Manager.BrowserType.CHROME);
            browser.Version = "1000.00.00";
            Assert.Throws<Exception>(()=>Manager.GetVersionFromGoogleAPIS(Manager.chrome_url, browser, Manager.OperatingSystem.WINDOWS));
        }        

        [Test]
        public void IE_NonExistentVersion()
        {
            Browser browser = Manager.GetBrowser(Manager.GetBrowsers(), Manager.BrowserType.INTERNET_EXPLORER);
            browser.Version = "1000.00.00";
            Assert.Throws<Exception>(() => Manager.GetVersionFromGoogleAPIS(Manager.ie_url, browser, Manager.OperatingSystem.WINDOWS));
        }

        [Test]
        public void Edge_NonExistentVersion()
        {
            Browser browser = Manager.GetBrowser(Manager.GetBrowsers(), Manager.BrowserType.EDGE);
            browser.Version = "1000.00.00";
            Assert.Throws<Exception>(() => Manager.GetEdgeURL(browser, Manager.OperatingSystem.WINDOWS));
        }

        [Test]
        public void Chrome_Linux_ManualBrowser() 
        {
            Browser browser = new Browser() 
            {
                Name = "Google Chrome",
                Version = "84.0.4147.135"
            };

            var chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.CHROME, Manager.OperatingSystem.LINUX, browser);
            var files = Directory.GetFiles(chromedriverDir);
            var filenames = files.Select(x => Path.GetFileName(x));
            Assert.IsTrue(filenames.Contains("chromedriver"));

        }
    }
}
