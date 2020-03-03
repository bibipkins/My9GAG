using Microsoft.VisualStudio.TestTools.UnitTesting;
using My9GAG.Logic.Client;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace My9GAG.IntegrationTest
{
    [TestClass]
    public class UnitTest1
    {
        private const string _TestCredentialsPath = "TestCredentials.txt";
        private string _username;
        private string _password;

        [TestInitialize]
        public void TestInit()
        {
            if(!File.Exists(_TestCredentialsPath))
            {
                throw new Exception($"{_TestCredentialsPath} not found, make sure that you've created this file and entered test credentials in it. First line should be username, second line should be password. This file is added to the gitignore and won't be commited");
            }

            var credentials = File.ReadAllLines(_TestCredentialsPath);
            _username = credentials[0].Trim();
            _password = credentials[1].Trim();
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            var a = new ClientService(null, new DictionarySecureStorage());

            await a.LoginWithCredentialsAsync(_username, _password);

            var result = await a.GetPostsAsync(Models.Post.PostCategory.Hot, 10);
            
            if(!result.IsSuccessful)
            {
                throw new InvalidOperationException("Get posts failed: " + result.Message);
            }

            Assert.AreNotEqual(String.Empty, a.Posts.First().Title);
        }
    }
}
