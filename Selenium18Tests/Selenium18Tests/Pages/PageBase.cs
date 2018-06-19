using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium18Tests.Pages.Cart;

namespace Selenium18Tests.Pages
{
	public class PageBase
	{
		public PageBase(IWebDriver webDriver, WebDriverWait wait)
		{
			this.webDriver = webDriver;
			this.wait = wait;
		}

		private By goToCartLinkSelector => By.XPath("//*[@id='cart']//a[@class='link']");

		protected IWebDriver webDriver;
		protected WebDriverWait wait;

		public CartPage GoToCart()
		{
			var cartPage = new CartPage(webDriver, wait);
			webDriver.FindElement(goToCartLinkSelector).Click();
			wait.Until(webDriver => webDriver.Title.Equals(cartPage.Title));
			return cartPage;
		}
	}
}
