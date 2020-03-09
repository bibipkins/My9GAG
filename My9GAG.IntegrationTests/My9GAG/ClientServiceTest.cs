using Microsoft.VisualStudio.TestTools.UnitTesting;
using My9GAG.Logic.Client;
using NineGagApiClient.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace My9GAG.IntegrationTest.My9GAG
{
    [TestClass]
    public class ClientServiceTest : TestBase
    {
        /// <summary>
        /// Very basic test, more like a smoke test so that we can see if the happyflow returns anything and throws no errors
        /// </summary>
        [TestMethod]
        public async Task GetPostsAsync_HappyFlow_ShouldReturnPosts()
        {
            using var clientService = new ClientService(logger: null, new DictionarySecureStorage());

            var resp = await clientService.LoginWithCredentialsAsync(Username, Password);
            Assert.IsTrue(resp.IsSuccessful);

            var result = await clientService.GetPostsAsync(PostCategory.Hot, 10);

            if (!result.IsSuccessful)
            {
                throw new InvalidOperationException("Get posts failed: " + result.Message);
            }

            Assert.AreEqual(10, clientService.Posts.Count);
            Assert.AreNotEqual(string.Empty, clientService.Posts.First().Title);
        }
    }
}
