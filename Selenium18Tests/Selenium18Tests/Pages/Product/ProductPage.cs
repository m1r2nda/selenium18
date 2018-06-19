using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium18Tests.Pages.Product
{
	public class ProductPage : PageBase
	{
		//локаторы 
		private By quantityProductsInCartLocator => By.XPath("//*[@id='cart']//*[@class='quantity']");
		private By addToCartButtonLocator => By.Name("add_cart_product");
		private By sizeSelector => By.Name("options[Size]");
		
		public string QuantityProductsInCart => webDriver.FindElement(quantityProductsInCartLocator).Text;

		public ProductPage(IWebDriver webDriver, WebDriverWait wait) : base(webDriver, wait) { }
		
		public void AddToCart()
		{
			if (webDriver.FindElement(sizeSelector).Displayed)
			{
				new SelectElement(webDriver.FindElement(sizeSelector)).SelectByIndex(1);
			}
			
			var quantityOld = QuantityProductsInCart;
			webDriver.FindElement(addToCartButtonLocator).Click();
			wait.Until(driver => !QuantityProductsInCart.Equals(quantityOld));
		}
	}
}
