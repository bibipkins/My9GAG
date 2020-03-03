using My9GAG.Logic.SecureStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace My9GAG.IntegrationTest.My9GAG
{
    public class DictionarySecureStorage : ISecureStorage
    {
        private readonly Dictionary<string, string> _dictionary;

        public DictionarySecureStorage()
        {
            _dictionary = new Dictionary<string, string>();
        }

        public SecureStorageResult Clear()
        {
            _dictionary.Clear();
            return new SecureStorageResult() { IsSuccessful = true };
        }

        public Task<SecureStorageResult> GetAsync(string key)
        {
            return Task.FromResult(new SecureStorageResult
            {
                IsSuccessful = true,
                Value = _dictionary[key]
            });
        }

        public SecureStorageResult Remove(string key)
        {
            var removed = _dictionary.Remove(key);
            return new SecureStorageResult() { IsSuccessful = removed };
        }

        public Task<SecureStorageResult> SetAsync(string key, string value)
        {
            _dictionary[key] = value;
            return Task.FromResult(new SecureStorageResult { IsSuccessful = true });
        }
    }
}
