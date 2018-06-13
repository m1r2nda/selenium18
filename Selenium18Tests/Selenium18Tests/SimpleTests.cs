using System;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Selenium18Tests
{
	[TestFixture]
	public class SimpleTests
	{
		private IWebDriver webDriver;
		private WebDriverWait wait;

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

		[Test]
		public void Google_GoToSearchPage_Success()
		{
			var url = "https://www.google.ru/search?q=Testing+with+Selenium+is+fun&oq=Testing+with+Selenium+is+fun";
			webDriver.Navigate().GoToUrl(url);
		}

		[Test]
		public void AdminPage_Login_Success()
		{
			//Переходим на страницу логина в админку
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart/admin/");
			wait.Until(webDriver => webDriver.Title.Equals("My Store"));

			//Вводим логин-пароль
			webDriver.FindElement(By.XPath("//input[@name='username']")).SendKeys("admin");
			webDriver.FindElement(By.XPath("//input[@name='password']")).SendKeys("admin");

			//Нажимаем "Login"
			webDriver.FindElement(By.XPath("//button[@name='login']")).Click();

			//ASSERT
			var isSuccessMessageDisplayed = webDriver.FindElement(By.XPath("//*[@class='notice success']")).Displayed;
			var successMessage = webDriver.FindElement(By.XPath("//*[@class='notice success']")).Text;

			Assert.IsTrue(isSuccessMessageDisplayed, "Не отобразилось сообщение об успешном входе в админку");
			Assert.AreEqual("You are now logged in as admin", successMessage, "Отличается от ожидаемого приветственный текст при входе в админку");
		}

		[Test]
		public void AdminPage_GoToLeftPanelMenuItem_Success()
		{
			//Переходим на страницу логина в админку
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart/admin/");
			wait.Until(webDriver => webDriver.Title.Equals("My Store"));

			//Авторизуемся в админке
			webDriver.FindElement(By.XPath("//input[@name='username']")).SendKeys("admin");
			webDriver.FindElement(By.XPath("//input[@name='password']")).SendKeys("admin");
			webDriver.FindElement(By.XPath("//button[@name='login']")).Click();

			//Кликаем на все ссылки в левой панели и проверяем наличие заголовка
			var leftPanelMenuItemsCount = webDriver.FindElements(By.XPath("//*[@id='app-']")).Count;
			for (var i=0; i < leftPanelMenuItemsCount; i++)
			{
				webDriver.FindElements(By.XPath("//*[@id='app-']"))[i].Click();

				//ASSERT
				Assert.IsTrue(IsElementPresent(By.TagName("h1")), "Не отобразился заголовок при переходе в меню из левой панели админки");

				//Прокликиваем так же подпункты меню последовательно
				var leftPanelMenuSubItemsCount = webDriver.FindElements(By.XPath("//*[@id='app-']//li")).Count;
				for (var j = 0; j < leftPanelMenuSubItemsCount; j++)
				{
					webDriver.FindElements(By.XPath("//*[@id='app-']//li"))[j].Click();

					//ASSERT
					Assert.IsTrue(IsElementPresent(By.TagName("h1")), "Не отобразился заголовок при переходе в меню из левой панели админки");
				}
			}
		}

		[Test]
		public void HomePage_EachProduct_HasOnlyOneSticker()
		{
			//Переходм на главную страницу
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart");
			wait.Until(webDriver => webDriver.Title.Equals("Online Store | My Store"));

			//Находим все товары на странице и проверяем, что у каждого товара ровно 1 стикер
			var products = webDriver.FindElements(By.CssSelector("li.product"));
			foreach (var product in products)
			{
				var stickersCount = product.FindElements(By.ClassName("sticker")).Count;
				Assert.AreEqual(1, stickersCount, "У товара не один стикер");
			}
		}

		[Test]
		public void HomePage_GoToCampaignProduct_Success()
		{
			//Переходм на главную страницу
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart");
			wait.Until(webDriver => webDriver.Title.Equals("Online Store | My Store"));

			//Находим первый товар из раздела Campaigns и запоминаем данные о нем
			var productNameOnMainPage = webDriver.FindElement(By.XPath("(//*[@id='box-campaigns']//li)[1]//*[@class='name']")).Text;
			var campaignPriceOnMainPage = webDriver.FindElement(By.XPath("(//*[@id='box-campaigns']//li)[1]//*[@class='campaign-price']")).Text;
			var campaignPriceColorOnMainPage = webDriver.FindElement(By.XPath("(//*[@id='box-campaigns']//li)[1]//*[@class='campaign-price']")).GetCssValue("color").Replace("rgba(","").Replace(")","").Replace(" ","").Split(',');
			var campaignPriceTagNameOnMainPage = webDriver.FindElement(By.XPath("(//*[@id='box-campaigns']//li)[1]//*[@class='campaign-price']")).TagName;
			var campaignPriceFontSizeOnMainPage = webDriver.FindElement(By.XPath("(//*[@id='box-campaigns']//li)[1]//*[@class='campaign-price']")).Size.Height;

			var regularPriceColorOnMainPage = webDriver.FindElement(By.XPath("(//*[@id='box-campaigns']//li)[1]//*[@class='regular-price']")).GetCssValue("color").Replace("rgba(", "").Replace(")", "").Replace(" ","").Split(','); 
			var regularPriceFontSizeOnMainPage = webDriver.FindElement(By.XPath("(//*[@id='box-campaigns']//li)[1]//*[@class='regular-price']")).Size.Height;
			var regularPriceOnMainPage = webDriver.FindElement(By.XPath("(//*[@id='box-campaigns']//li)[1]//*[@class='regular-price']")).Text;
			var regularPriceTagNameOnMainPage = webDriver.FindElement(By.XPath("(//*[@id='box-campaigns']//li)[1]//*[@class='regular-price']")).TagName;
			
			//Переходим в товар
			webDriver.FindElement(By.XPath("(//*[@id='box-campaigns']//li)[1]//a[@class='link']")).Click();

			//Проверяем данные о товаре
			var productNameOnProductPage = webDriver.FindElement(By.XPath("//*[@id='box-product']//h1[@class='title']")).Text;
			var campaignPriceTagNameOnProductPage = webDriver.FindElement(By.XPath("//*[@id='box-product']//*[@class='campaign-price']")).TagName;
			var campaignPriceColorOnProductPage = webDriver.FindElement(By.XPath("//*[@id='box-product']//*[@class='campaign-price']")).GetCssValue("color").Replace("rgba(", "").Replace(")", "").Replace(" ","").Split(',');
			var campaignPriceOnProductPage = webDriver.FindElement(By.XPath("//*[@id='box-product']//*[@class='campaign-price']")).Text;
			var campaignPriceFontSizeOnProductPage = webDriver.FindElement(By.XPath("//*[@id='box-product']//*[@class='campaign-price']")).Size.Height;

			var regularPriceOnProductPage = webDriver.FindElement(By.XPath("//*[@id='box-product']//*[@class='regular-price']")).Text;
			var regularPriceColorOnProductPage = webDriver.FindElement(By.XPath("//*[@id='box-product']//*[@class='regular-price']")).GetCssValue("color").Replace("rgba(", "").Replace(")", "").Replace(" ","").Split(',');
			var regularPriceFontSizeOnProductPage = webDriver.FindElement(By.XPath("//*[@id='box-product']//*[@class='regular-price']")).Size.Height;
			var regularPriceTagNameOnProductPage = webDriver.FindElement(By.XPath("//*[@id='box-product']//*[@class='regular-price']")).TagName;

			Assert.AreEqual(productNameOnMainPage, productNameOnProductPage, "Не совпадает название товара на главной и на странице самого товара");
			Assert.AreEqual(regularPriceOnMainPage, regularPriceOnProductPage, "Не совпадает обычная цена товара на главной и на странице самого товара");
			Assert.AreEqual(campaignPriceOnMainPage, campaignPriceOnProductPage, "Не совпадает скидочная цена товара на главной и на странице самого товара");
			Assert.AreEqual("s", regularPriceTagNameOnMainPage, "Не зачеркнута обычная цена товара на главной странице");
			Assert.AreEqual("s", regularPriceTagNameOnProductPage, "Не зачеркнута обычная цена товара на странице самого товара");
			Assert.IsTrue(campaignPriceFontSizeOnMainPage - regularPriceFontSizeOnMainPage > 0, "Аукционная цена не больше обычной на главной странице");
			Assert.IsTrue(campaignPriceFontSizeOnProductPage - regularPriceFontSizeOnProductPage > 0, "Аукционная цена не больше обычной на странице самого товара");
			Assert.IsTrue(regularPriceColorOnProductPage[0].Equals(regularPriceColorOnProductPage[1]) && regularPriceColorOnProductPage[1].Equals(regularPriceColorOnProductPage[2]), "Обычная цена не серого цвета на странице самого товара");
			Assert.IsTrue(regularPriceColorOnMainPage[0].Equals(regularPriceColorOnMainPage[1]) && regularPriceColorOnMainPage[1].Equals(regularPriceColorOnMainPage[2]), "Обычная цена не серого цвета на главной странице");
			Assert.AreEqual("strong", campaignPriceTagNameOnMainPage, "Аукционная цена не жирная на главной странице");
			Assert.AreEqual("strong", campaignPriceTagNameOnProductPage, "Аукционная цена не жирная на странице самого товара");
			Assert.IsTrue(campaignPriceColorOnProductPage[1]=="0" && campaignPriceColorOnProductPage[2]=="0" && campaignPriceColorOnProductPage[0]!="0", "Цвет аукционной цены товара не красного цвета на странице самого товара");
			Assert.IsTrue(campaignPriceColorOnMainPage[1]=="0" && campaignPriceColorOnMainPage[2]=="0" && campaignPriceColorOnMainPage[0]!="0", "Цвет аукционной цены товара не красного цвета на главной странице");
		}


		[Test]
		public void AdminPage_Countries_AreInAlphabeticalOrder()
		{
			//Переходим на страницу логина в админку
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart/admin/");
			wait.Until(webDriver => webDriver.Title.Equals("My Store"));

			//Авторизуемся в админке
			webDriver.FindElement(By.XPath("//input[@name='username']")).SendKeys("admin");
			webDriver.FindElement(By.XPath("//input[@name='password']")).SendKeys("admin");
			webDriver.FindElement(By.XPath("//button[@name='login']")).Click();
			
			//Переходим на страницу стран
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart/admin/?app=countries&doc=countries");
			wait.Until(webDriver => webDriver.Title.Equals("Countries | My Store"));

			//Проверяем, что список стран расположен в алфавитном порядке
			var countries = webDriver.FindElements(By.CssSelector("tr.row a:not([title=Edit])"));
			for (var i = 0; i < countries.Count-1; i++)
			{
				Assert.IsTrue(String.CompareOrdinal(countries[i].Text, countries[i+1].Text) <=0, "В списке стран не соблюдается алфавитный порядок");
			}
		}

		[Test]
		public void AdminPage_ZonesOfCountries_AreInAlphabeticalOrder()
		{
			//Переходим на страницу логина в админку
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart/admin/");
			wait.Until(webDriver => webDriver.Title.Equals("My Store"));

			//Авторизуемся в админке
			webDriver.FindElement(By.XPath("//input[@name='username']")).SendKeys("admin");
			webDriver.FindElement(By.XPath("//input[@name='password']")).SendKeys("admin");
			webDriver.FindElement(By.XPath("//button[@name='login']")).Click();

			//Переходим на страницу стран
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart/admin/?app=countries&doc=countries");
			wait.Until(webDriver => webDriver.Title.Equals("Countries | My Store"));

			//Переходим только к тем странам, у которых кол-во зон не равно нулю
			var countriesUrls = webDriver.FindElements(By.XPath("//tr[contains(@class,'row')]//td[6][not(contains(.,0))]/../*/a[@title='Edit']")).Select(e => e.GetAttribute("href")).ToList();
			foreach (var url in countriesUrls)
			{
				webDriver.Navigate().GoToUrl(url);
				//Проверяем, что список зон расположен в алфавитном порядке
				var zones = webDriver.FindElements(By.XPath("//*[@id='table-zones']//input[contains(@name,'name') and not(@value='')]/.."));
				for (var i = 0; i < zones.Count - 1; i++)
				{
					Assert.IsTrue(String.CompareOrdinal(zones[i].Text, zones[i + 1].Text) <= 0, "В списке зон не соблюдается алфавитный порядок");
				}
			}
		}

		[Test]
		public void AdminPage_EditCountry_WasOpenedInNewWindow()
		{
			//Переходим на страницу логина в админку
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart/admin/");
			wait.Until(webDriver => webDriver.Title.Equals("My Store"));

			//Авторизуемся в админке
			webDriver.FindElement(By.XPath("//input[@name='username']")).SendKeys("admin");
			webDriver.FindElement(By.XPath("//input[@name='password']")).SendKeys("admin");
			webDriver.FindElement(By.XPath("//button[@name='login']")).Click();

			//Переходим на страницу добавления страны на вкладке Countries
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart/admin/?app=countries&doc=edit_country");
			wait.Until(webDriver => webDriver.Title.Equals("Add New Country | My Store"));

			//Прокликиваем все ссылки на внешние источники
			var itemsCount = webDriver.FindElements(By.XPath("//*[contains(@class,'fa-external-link')]/..")).Count;
			for (var j = 0; j < itemsCount; j++)
			{
				var oldWindowsCount = webDriver.WindowHandles.Count;
				var oldWindows = webDriver.WindowHandles;
				var mainWindow = webDriver.CurrentWindowHandle;

				webDriver.FindElements(By.XPath("//*[contains(@class,'fa-external-link')]/.."))[j].Click();

				//ASSERT
				var newWindow = wait.Until(AnyWindowOtherThan(oldWindows));
				Assert.AreEqual(oldWindowsCount+1, webDriver.WindowHandles.Count, "Не открылась ссылка в новом окне");

				//Закрываем окно и возвращаемся назад
				webDriver.SwitchTo().Window(newWindow).Close();
				webDriver.SwitchTo().Window(mainWindow);
			}
		}

		private Func<IWebDriver, string> AnyWindowOtherThan(ReadOnlyCollection<string> oldWindows)
		{
			return webDriver =>
			{
				var windowsHandles = webDriver.WindowHandles;
				var newWindows = windowsHandles.Except(oldWindows);
				return newWindows.Any() ? newWindows.FirstOrDefault() : string.Empty;
			};
		}

		[Test]
		public void AdminPage_AddNewProduct_Success()
		{
			//Переходим на страницу логина в админку
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart/admin/");
			wait.Until(webDriver => webDriver.Title.Equals("My Store"));

			//Авторизуемся в админке
			webDriver.FindElement(By.XPath("//input[@name='username']")).SendKeys("admin");
			webDriver.FindElement(By.XPath("//input[@name='password']")).SendKeys("admin");
			webDriver.FindElement(By.XPath("//button[@name='login']")).Click();

			//Переходим на вкладку Catalog
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart/admin/?app=catalog&doc=catalog");
			wait.Until(webDriver => webDriver.Title.Equals("Catalog | My Store"));

			//Нажимаем "Add New Product" 
			//Заполняем данные о товаре на вкладке General
			webDriver.FindElement(By.XPath("//*[@id='content']//a[contains(.,'Add New Product')]")).Click();
			webDriver.FindElement(By.XPath("//*[@id='tab-general']//*[@name='status'][@value='1']")).Click();
			var productName = Guid.NewGuid().ToString();
			webDriver.FindElement(By.XPath("//*[@id='tab-general']//*[@name='name[en]']")).SendKeys(productName);
			webDriver.FindElement(By.XPath("//*[@id='tab-general']//*[@name='code']")).SendKeys("code");
			webDriver.FindElement(By.XPath("//*[@id='tab-general']//input[@name='product_groups[]'][@value='1-3']")).Click();
			webDriver.FindElement(By.XPath("//*[@id='tab-general']//*[@name='quantity']")).Clear();
			webDriver.FindElement(By.XPath("//*[@id='tab-general']//*[@name='quantity']")).SendKeys("10");
			webDriver.FindElement(By.XPath("//*[@id='tab-general']//*[@name='new_images[]']")).SendKeys($"{TestContext.CurrentContext.TestDirectory}\\new-product.png");
			webDriver.FindElement(By.XPath("//*[@id='tab-general']//*[@name='date_valid_from']")).SendKeys(DateTime.Today.ToString("dd.MM.yyyy"));
			webDriver.FindElement(By.XPath("//*[@id='tab-general']//*[@name='date_valid_to']")).SendKeys(DateTime.Today.AddDays(7).ToString("dd.MM.yyyy"));

			//Переключаемся на вкладку Information
			webDriver.FindElement(By.XPath("//*[@id='content']//a[@href='#tab-information']")).Click();
			wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='tab-information']//select[@name='manufacturer_id']")));

			//Заполняем данные о товаре на вкладке Information
			new SelectElement(webDriver.FindElement(By.Name("manufacturer_id"))).SelectByIndex(1);
			webDriver.FindElement(By.Name("keywords")).SendKeys("keywords");
			webDriver.FindElement(By.Name("short_description[en]")).SendKeys("short_description[en]");
			webDriver.FindElement(By.XPath("//*[@class='trumbowyg-editor']")).SendKeys("description");
			webDriver.FindElement(By.Name("head_title[en]")).SendKeys("head_title[en]");
			webDriver.FindElement(By.Name("meta_description[en]")).SendKeys("meta_description[en]");

			//Переключаемся на вкладку Prices
			webDriver.FindElement(By.XPath("//*[@id='content']//a[@href='#tab-prices']")).Click();
			wait.Until(ExpectedConditions.ElementIsVisible(By.Name("purchase_price")));

			//Заполняем данные о товаре на вкладке Prices
			webDriver.FindElement(By.Name("purchase_price")).Clear();
			webDriver.FindElement(By.Name("purchase_price")).SendKeys("20");
			new SelectElement(webDriver.FindElement(By.Name("purchase_price_currency_code"))).SelectByValue("USD");
			webDriver.FindElement(By.Name("prices[USD]")).Clear();
			webDriver.FindElement(By.Name("prices[USD]")).SendKeys("20");
			webDriver.FindElement(By.Name("gross_prices[USD]")).Clear();
			webDriver.FindElement(By.Name("gross_prices[USD]")).SendKeys("20");
			webDriver.FindElement(By.Name("prices[EUR]")).Clear();
			webDriver.FindElement(By.Name("prices[EUR]")).SendKeys("20");
			webDriver.FindElement(By.Name("gross_prices[EUR]")).Clear();
			webDriver.FindElement(By.Name("gross_prices[EUR]")).SendKeys("20");

			//Сохраняем
			webDriver.FindElement(By.Name("save")).Click();

			//Проверяем, что товар появился в каталоге (админке)
			//Переходим на вкладку Catalog
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart/admin/?app=catalog&doc=catalog");
			wait.Until(webDriver => webDriver.Title.Equals("Catalog | My Store"));
			Assert.IsTrue(webDriver.FindElement(By.XPath($"//*[@id='content']//a[.='{productName}']")).Displayed, "Новый товар не появился в каталоге после создания");
		}

		[Test]
		public void AdminPage_GeoZonesOfCountries_AreInAlphabeticalOrder()
		{
			//Переходим на страницу логина в админку
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart/admin/");
			wait.Until(webDriver => webDriver.Title.Equals("My Store"));

			//Авторизуемся в админке
			webDriver.FindElement(By.XPath("//input[@name='username']")).SendKeys("admin");
			webDriver.FindElement(By.XPath("//input[@name='password']")).SendKeys("admin");
			webDriver.FindElement(By.XPath("//button[@name='login']")).Click();

			//Переходим на страницу гео зон
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart/admin/?app=geo_zones&doc=geo_zones");
			wait.Until(webDriver => webDriver.Title.Equals("Geo Zones | My Store"));

			//Переходим к каждой стране
			var countriesUrls = webDriver.FindElements(By.XPath("//table//a[@title='Edit']")).Select(e => e.GetAttribute("href")).ToList();
			foreach (var url in countriesUrls)
			{
				webDriver.Navigate().GoToUrl(url);
				//Проверяем, что список зон расположен в алфавитном порядке
				var zones = webDriver.FindElements(By.XPath("//*[@id='table-zones']//select[contains(@name,'zone_code')]/option[@selected]"));
				for (var i = 0; i < zones.Count - 1; i++)
				{
					Assert.IsTrue(String.CompareOrdinal(zones[i].Text, zones[i + 1].Text) <= 0, "В списке зон не соблюдается алфавитный порядок");
				}
			}
		}

		[Test]
		public void Registration_CreateAccount_Success()
		{
			//Переходим на страницу создания аккаунта в админку
			webDriver.Navigate().GoToUrl("http://localhost:81/litecart/create_account");
			wait.Until(webDriver => webDriver.Title.Equals("Create Account | My Store"));

			//Заполняем все необходимые поля для регистарции
			var email = $"{Guid.NewGuid()}@user.ru";
			var password = "password";
			webDriver.FindElement(By.Name("firstname")).SendKeys("firstname");
			webDriver.FindElement(By.Name("lastname")).SendKeys("lastname");
			webDriver.FindElement(By.Name("address1")).SendKeys("address1");
			webDriver.FindElement(By.Name("postcode")).SendKeys("12345");
			webDriver.FindElement(By.Name("city")).SendKeys("city");
			new SelectElement(webDriver.FindElement(By.Name("country_code"))).SelectByText("United States");
			webDriver.FindElement(By.Name("phone")).SendKeys("+19033870573");
			webDriver.FindElement(By.Name("email")).SendKeys(email);
			webDriver.FindElement(By.Name("password")).SendKeys(password);
			webDriver.FindElement(By.Name("confirmed_password")).SendKeys(password);
			webDriver.FindElement(By.Name("create_account")).Click();

			//После регистрации переходим на главную автоматически, просто ждем этого
			wait.Until(webDriver => webDriver.Title.Equals("Online Store | My Store"));

			//Разлогиниваемся
			var logoutLocator = By.XPath("(//a[.='Logout'])[1]");
			webDriver.FindElement(logoutLocator).Click();

			//Повторный вход в только что созданную учетную запись
			webDriver.FindElement(By.Name("email")).SendKeys(email);
			webDriver.FindElement(By.Name("password")).SendKeys(password);
			webDriver.FindElement(By.Name("login")).Click();

			//И еще раз разлогиниваемся
			webDriver.FindElement(logoutLocator).Click();
		}

		[Test]
		public void HomePage_AddProductToCart_Success()
		{
			for (int i = 0; i < 3; i++)
			{
				//Переходим на главную страницу
				webDriver.Navigate().GoToUrl("http://localhost:81/litecart");
				wait.Until(webDriver => webDriver.Title.Equals("Online Store | My Store"));

				//Открываем первый продукт из списка (например, это список Latest Products)
				webDriver.FindElement(By.XPath("(//*[@id='box-latest-products']//li[contains(@class,'product')])[1]")).Click();
				wait.Until(webDriver => !webDriver.Title.Equals("Online Store | My Store"));
				var quantityOld = webDriver.FindElement(By.XPath("//*[@id='cart']//*[@class='quantity']")).Text;
				webDriver.FindElement(By.Name("add_cart_product")).Click();
				wait.Until(webDriver => !webDriver.FindElement(By.XPath("//*[@id='cart']//*[@class='quantity']")).Text.Equals(quantityOld));
			}

			//Переходим в корзину
			webDriver.FindElement(By.XPath("//*[@id='cart']//a[@class='link']")).Click();
			wait.Until(webDriver => webDriver.Title.Equals("Checkout | My Store"));

			//Поштучно удаляем два товара путем уменьшения их кол-ва. Предполагаем, что сейчас в корзине 3 единицы одного и того же товара
			for (var i = 3; i > 1; i--)
			{
				var productCount = webDriver.FindElement(By.XPath($"//*[@id='order_confirmation-wrapper']//tr[2]/td[1]"));
				
				//удаляем
				webDriver.FindElement(By.XPath("//*[@id='box-checkout-cart']//*[@name='quantity']")).Clear();
				webDriver.FindElement(By.XPath("//*[@id='box-checkout-cart']//*[@name='quantity']")).SendKeys($"{i-1}");
				webDriver.FindElement(By.XPath("//*[@id='box-checkout-cart']//*[@name='update_cart_item']")).Click();
				wait.Until(webDriver => ExpectedConditions.StalenessOf(productCount));
			}

			//удаляем последний третий товар
			wait.Until(ExpectedConditions.TextToBePresentInElementLocated(By.XPath($"//*[@id='order_confirmation-wrapper']//tr[2]/td[1]"),"1"));
			var updateButtonElement = webDriver.FindElement(By.XPath("//*[@id='box-checkout-cart']//*[@name='update_cart_item']"));
			webDriver.FindElement(By.XPath("//*[@id='box-checkout-cart']//*[@name='quantity']")).Clear();
			webDriver.FindElement(By.XPath("//*[@id='box-checkout-cart']//*[@name='quantity']")).SendKeys("0");
			webDriver.FindElement(By.XPath("//*[@id='box-checkout-cart']//*[@name='update_cart_item']")).Click();

			wait.Until(webDriver => ExpectedConditions.StalenessOf(updateButtonElement));
			wait.Until(webDriver => ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//*[@id='checkout-cart-wrapper']//*[.='There are no items in your cart.']")));
		}

		private bool IsElementPresent(By locator)
		{
			return webDriver.FindElements(locator).Count > 0;
		}
	}
}
