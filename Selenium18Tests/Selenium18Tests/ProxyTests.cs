using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Selenium18Tests
{
	public class ProxyTests
	{
		[Test]
		public void Google_GoToSearchPageWithProxy_Success()
		{
			Proxy proxy = new Proxy();
			proxy.SslProxy = "localhost:8888";
			proxy.Kind = ProxyKind.Manual;
			proxy.HttpProxy = "localhost:8888";
			ChromeOptions options = new ChromeOptions();
			options.Proxy = proxy;
			IWebDriver driver = new ChromeDriver(options);
			var url = "https://www.google.ru/search?q=Testing+with+Selenium+is+fun&oq=Testing+with+Selenium+is+fun";
			driver.Navigate().GoToUrl(url);
			driver.Close();
			driver.Quit();
		}
	}
}
