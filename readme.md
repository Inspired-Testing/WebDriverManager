# Welcome to Inspired Testings WebDriverManager

## Purpose
This project is heavily inspired by Bonigarcia and his Java WebDriverManager found at: https://github.com/bonigarcia/webdrivermanager
The idea behind it is that as an organisation providing Automation Testing we need a way to seemlessly manager Selenium WebDrivers without having to maintain and explain - but also in C#!

### Usage

Usage is fairly simple, calling the GetWebDriver command will return the directory of the Webdriver exe file which is downloaded for you if it is not already installed.
The Environment variable is also set, so you should be able to instantiate a new driver with ease.

`Manager.GetWebDriver(Manager.BrowserType.CHROME);`
`Manager.GetWebDriver(Manager.BrowserType.CHROME, Manager.OperatingSystem.WINDOWS);`

Using this format you are able to specify Browsers from the list
*Chrome
*Firefox
*Internet Explorer
*Edge (Chromium)

You are also able to choose from the following Operating Systems:
*Windows
*Linux
*MAC

Automatic browser detection is currently only supported for windows.
For Mac and Linux you will need to create a new Browser object and pass it into the GetWebDriver Method

`   
    Browser browser = new Browser() 
    {
        Name = "Google Chrome",
        Version = "84.0.4147.135"
    };

    var chromedriverDir = Manager.GetWebDriver(Manager.BrowserType.CHROME, Manager.OperatingSystem.LINUX, browser);
`


<br/><br/><br/>
> Developer: Stephen Beck

