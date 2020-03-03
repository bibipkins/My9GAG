using Microsoft.VisualStudio.TestTools.UnitTesting;
using My9GAG.Logic.Client;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace My9GAG.IntegrationTest.My9GAG
{
    [TestClass]
    public class ClientServiceTest
    {
        private const string _TestCredentialsPath = "TestCredentials.txt";
        private string _username;
        private string _password;

        [TestInitialize]
        public void TestInit()
        {
            if (!File.Exists(_TestCredentialsPath))
            {
                throw new Exception($"{_TestCredentialsPath} not found, make sure that you've created this file and entered test credentials in it. First line should be username, second line should be password. This file is added to the gitignore and won't be commited");
            }

            var credentials = File.ReadAllLines(_TestCredentialsPath);
            _username = credentials[0].Trim();
            _password = credentials[1].Trim();
        }

        /// <summary>
        /// Very basic test, more like a smoke test so that we can see if the happyflow returns anything and throws no errors
        /// </summary>
        [TestMethod]
        public async Task GetPostsAsync_HappyFlow_ShouldReturnPosts()
        {
            var clientService = new ClientService(logger: null, new DictionarySecureStorage(), generatePostMediaOnLoad: false);

            var resp = await clientService.LoginWithCredentialsAsync(_username, _password);
            Assert.IsTrue(resp.IsSuccessful);

            var result = await clientService.GetPostsAsync(Models.Post.PostCategory.Hot, 10);

            if (!result.IsSuccessful)
            {
                throw new InvalidOperationException("Get posts failed: " + result.Message);
            }

            Assert.AreEqual(10, clientService.Posts.Count);
            Assert.AreNotEqual(string.Empty, clientService.Posts.First().Title);
        }
    }
}
