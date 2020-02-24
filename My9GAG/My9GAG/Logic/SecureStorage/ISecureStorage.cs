using System.Threading.Tasks;

namespace My9GAG.Logic.SecureStorage
{
    public interface ISecureStorage
    {
        Task<SecureStorageResult> GetAsync(string key);
        Task<SecureStorageResult> SetAsync(string key, string value);
        SecureStorageResult Remove(string key);
        SecureStorageResult Clear();
    }
}
