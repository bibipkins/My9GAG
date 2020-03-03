using Microsoft.VisualStudio.TestTools.UnitTesting;
using My9GAG.NineGagApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My9GAG.IntegrationTest.My9GAG.NineGagApiClient
{
    [TestClass]
    public class ApiClientTest : TestBase
    {
        [TestMethod]
        public async Task GetPostsAsync_HappyFlow_10Posts()
        {
            using var apiClient = new ApiClient();
            await apiClient.LoginWithCredentialsAsync(Username, Password);

            //Act
            var posts = await apiClient.GetPostsAsync(Models.Post.PostCategory.Hot, 10);

            //Assert
            Assert.IsNotNull(posts);
            Assert.AreEqual(10, posts.Count());
            Assert.AreNotEqual(string.Empty, posts.First().Title);
        }

        [TestMethod]
        public async Task GetPostsAsync_PostsSince_OnlyPostsSince()
        {
            using var apiClient = new ApiClient();
            await apiClient.LoginWithCredentialsAsync(Username, Password);
            var top10posts = await apiClient.GetPostsAsync(Models.Post.PostCategory.Hot, 10);
            var post5 = top10posts[4];
            var post6 = top10posts[5];

            //Act
            var postsSince = await apiClient.GetPostsAsync(Models.Post.PostCategory.Hot, 10, post5.Id);

            //Assert
            Assert.IsNotNull(postsSince);
            Assert.AreEqual(10, postsSince.Count());
            Assert.AreNotEqual(string.Empty, postsSince.First().Title);

            Assert.AreEqual(post6.Id, postsSince[0].Id);
        }
    }
}
