using System;
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

		private bool IsElementPresent(By locator)
		{
			return webDriver.FindElements(locator).Count > 0;
		}
	}
}
