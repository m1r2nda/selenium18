using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Selenium18Tests.Pages;
using Selenium18Tests.Pages.Main;
using Selenium18Tests.Pages.Product;

namespace Selenium18Tests
{
	public class TestBase
	{
		protected IWebDriver webDriver;
		protected WebDriverWait wait;

		[SetUp]
		public void SetUp()
		{
			webDriver = new ChromeDriver();

			wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(10));
		}

		[TearDown]
		public void TearDown()
		{
			webDriver.Close();
			webDriver.Quit();
			webDriver = null;
		}

		public MainPage GoToMainPage()
		{
			var page = new MainPage(webDriver, wait);
			page.GoToPage();
			return page;
		}

		public void AddProductsToCart(int count)
		{
			for (var i = 0; i < count; i++)
			{
				var mainPage = GoToMainPage();

				//Открываем первый продукт из списка (например, это список Latest Products) и добавляем его в корзину
				var productPage = mainPage.LatestProducts.Click(1);
				productPage.AddToCart();
			}
		}
	}
}
