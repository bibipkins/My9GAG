using System;
using System.Threading.Tasks;

namespace My9GAG.Logic.SecureStorage
{
    class SecureStorage : ISecureStorage
    {
        public async Task<SecureStorageResult> GetAsync(string key)
        {
            var result = new SecureStorageResult();

            try
            {
                result.Value = await Xamarin.Essentials.SecureStorage.GetAsync(key);
                result.IsSuccessful = result.Value != null;
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }

            return result;
        }
        public async Task<SecureStorageResult> SetAsync(string key, string value)
        {
            var result = new SecureStorageResult();

            try
            {
                await Xamarin.Essentials.SecureStorage.SetAsync(key, value);
                result.IsSuccessful = true;
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }

            return result;
        }

        public SecureStorageResult Remove(string key)
        {
            var result = new SecureStorageResult();

            try
            {
                result.IsSuccessful = Xamarin.Essentials.SecureStorage.Remove(key);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }

            return result;
        }
        public SecureStorageResult Clear()
        {
            var result = new SecureStorageResult();

            try
            {
                Xamarin.Essentials.SecureStorage.RemoveAll();
                result.IsSuccessful = true;
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }

            return result;
        }
    }
}
