using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium18Tests.Pages.Product;

namespace Selenium18Tests.Pages.Main
{
	public class LatestProducts
	{
		private IWebDriver webDriver;
		private WebDriverWait wait;

		public LatestProducts(IWebDriver webDriver, WebDriverWait wait)
		{
			this.webDriver = webDriver;
			this.wait = wait;
		}

		private By GetProductLocator(int order)
		{
			return By.XPath($"(//*[@id='box-latest-products']//li[contains(@class,'product')])[{order}]");
		}

		private By GetProductNameLocator(int order)
		{
			return By.XPath($"(//*[@id='box-latest-products']//li[contains(@class,'product')])[{order}]//*[@class='name']");
		}

		public ProductPage Click(int order)
		{
			var productName = webDriver.FindElement(GetProductNameLocator(order)).Text;
			webDriver.FindElement(GetProductLocator(order)).Click();
			wait.Until(driver => driver.Title.Contains(productName));
			return new ProductPage(webDriver, wait);
		}
	}
}
