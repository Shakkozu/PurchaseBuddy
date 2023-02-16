using NUnit.Framework;
using System;
using System.Net;
using TechTalk.SpecFlow;

namespace PurchaseBuddy.Specflow.StepDefinitions
{
    [Binding]
    public class UserShopsDefinitions
    {
		private string authToken = "";
		private HttpClient httpClient = new HttpClient();
		private AuthorizationController authorizationController = new AuthorizationController();

		[Given(@"the user shops list is empty")]
        public async Task GivenTheUserShopsListIsEmpty()
        {
			authToken = await RegisterAndLoginAsync();
            throw new PendingStepException();
        }

		[When(@"the user adds a shop")]
        public void WhenTheUserAddsAShop()
        {
            throw new PendingStepException();
        }
		
		[When(@"the user is registered")]
		public void WhenTheUserIsRegistered()
		{
			throw new PendingStepException();
		}

		[Then(@"the shop is added to the list")]
        public void ThenTheShopIsAddedToTheList()
        {
            throw new PendingStepException();
        }
		private async Task<string> RegisterAndLoginAsync()
		{
			var contentString = @"{
    ""email"": ""john.doe@example.com"",
    ""login"": ""shakkozu"",
    ""password"": ""zaq1@WSX""
}";
			var content = new StringContent(contentString);
			
			var baseUrl = "authorization/register";
			var response = await httpClient.PostAsync(baseUrl + "/register", content);

			return "";
		}
    }
}
