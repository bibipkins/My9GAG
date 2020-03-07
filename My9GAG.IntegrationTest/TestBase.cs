using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace My9GAG.IntegrationTest
{
    public abstract class TestBase
    {
        private const string _TestCredentialsPath = "TestCredentials.txt";
        protected string Username { get; set; }
        protected string Password { get; set; }

        [TestInitialize]
        public async Task TestInit()
        {
            if (!File.Exists(_TestCredentialsPath))
            {
                await File.WriteAllLinesAsync(_TestCredentialsPath, new string[] { "username", "password" });
                throw new Exception($"{_TestCredentialsPath} not found, created a dummy file please edit this. Enter test credentials in it. First line should be username, second line should be password. This file is added to the gitignore and won't be committed");
            }

            var credentials = File.ReadAllLines(_TestCredentialsPath);
            Username = credentials[0].Trim();
            Password = credentials[1].Trim();
        }
    }
}
