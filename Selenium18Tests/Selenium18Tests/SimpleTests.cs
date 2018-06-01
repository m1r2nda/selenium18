using System;
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
	}
}
