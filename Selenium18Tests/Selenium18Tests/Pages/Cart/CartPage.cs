using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium18Tests.Pages.Cart
{
	public class CartPage
	{
		//локаторы 
		private By productCountLocator => By.XPath("//*[@id='order_confirmation-wrapper']//tr[2]/td[1]");
		private By updateButtonLocator => By.XPath("//*[@id='box-checkout-cart']//*[@name='update_cart_item']");
		private By quantityLocator => By.XPath("//*[@id='box-checkout-cart']//*[@name='quantity']");
		private By emptyCartTextLocator => By.XPath("//*[@id='checkout-cart-wrapper']//*[.='There are no items in your cart.']");

		public int Quantity
		{
			get => int.Parse(webDriver.FindElement(quantityLocator).Text);
			set
			{
				webDriver.FindElement(quantityLocator).Clear();
				webDriver.FindElement(quantityLocator).SendKeys(value.ToString());
			}
		}

		public string Title = "Checkout | My Store";

		private IWebDriver webDriver;
		private WebDriverWait wait;

		public CartPage(IWebDriver webDriver, WebDriverWait wait)
		{
			this.webDriver = webDriver;
			this.wait = wait;
		}
		
		/// <summary>
		/// Клик по кнопке "Update"
		/// </summary>
		public void Update()
		{
			webDriver.FindElement(updateButtonLocator).Click();
		}

		/// <summary>
		/// Удаляем товары из корзины по одному
		/// </summary>
		/// <param name="repeat"></param>
		public void RemoveByOne(int repeat)
		{
			var oldProductsCount = int.Parse(webDriver.FindElement(productCountLocator).Text);

			for (var i = oldProductsCount; i > (oldProductsCount-repeat); i--)
			{
				var currentProductsCount = webDriver.FindElement(productCountLocator);
				Quantity = i-1;
				Update();
				wait.Until(webDriver => ExpectedConditions.StalenessOf(currentProductsCount));
			}
		}

		/// <summary>
		/// Удаляем последний товар из корзины
		/// </summary>
		public void RemoveLastProduct()
		{
			wait.Until(ExpectedConditions.TextToBePresentInElementLocated(productCountLocator, "1"));
			var updateButtonElement = webDriver.FindElement(updateButtonLocator);

			Quantity = 0;
			Update();

			wait.Until(webDriver => ExpectedConditions.StalenessOf(updateButtonElement));
			wait.Until(webDriver => ExpectedConditions.VisibilityOfAllElementsLocatedBy(emptyCartTextLocator));
		}
}
}
