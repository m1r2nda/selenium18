using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium18Tests.Pages.Main
{
	public class MainPage : PageBase
	{
		public string Url = "http://localhost:81/litecart";
		public string Title = "Online Store | My Store";

		public LatestProducts LatestProducts => new LatestProducts(webDriver, wait);

		public MainPage(IWebDriver webDriver, WebDriverWait wait) : base(webDriver,wait){ }

		public void GoToPage()
		{
			webDriver.Navigate().GoToUrl(Url);
			wait.Until(webDriver => webDriver.Title.Equals(Title));
		}
	}
}
