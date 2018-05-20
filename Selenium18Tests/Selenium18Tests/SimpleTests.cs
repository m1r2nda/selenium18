using NUnit.Framework;
using OpenQA.Selenium.Chrome;

namespace Selenium18Tests
{
	[TestFixture]
	public class SimpleTests
	{
		[Test]
		public void Google_GoToSearchPage_Success()
		{
			var url = "https://www.google.ru/search?q=Testing+with+Selenium+is+fun&oq=Testing+with+Selenium+is+fun";
			var ChromeDriver = new ChromeDriver();
			ChromeDriver.Navigate().GoToUrl(url);
			ChromeDriver.Close();
		}
	}
}
