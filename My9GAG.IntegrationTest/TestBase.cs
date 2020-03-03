using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace My9GAG.IntegrationTest
{
    public abstract class TestBase
    {
        private const string _TestCredentialsPath = "TestCredentials.txt";
        protected string Username { get; set; }
        protected string Password { get; set; }

        [TestInitialize]
        public void TestInit()
        {
            if (!File.Exists(_TestCredentialsPath))
            {
                throw new Exception($"{_TestCredentialsPath} not found, make sure that you've created this file and entered test credentials in it. First line should be username, second line should be password. This file is added to the gitignore and won't be commited");
            }

            var credentials = File.ReadAllLines(_TestCredentialsPath);
            Username = credentials[0].Trim();
            Password = credentials[1].Trim();
        }
    }
}
