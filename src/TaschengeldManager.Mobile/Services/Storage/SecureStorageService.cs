namespace TaschengeldManager.Mobile.Services.Storage;

/// <summary>
/// Implementation of secure storage using MAUI SecureStorage
/// </summary>
public class SecureStorageService : ISecureStorageService
{
    public async Task SetAsync(string key, string value)
    {
        await SecureStorage.Default.SetAsync(key, value);
    }

    public async Task<string?> GetAsync(string key)
    {
        return await SecureStorage.Default.GetAsync(key);
    }

    public Task RemoveAsync(string key)
    {
        SecureStorage.Default.Remove(key);
        return Task.CompletedTask;
    }

    public async Task<bool> ContainsKeyAsync(string key)
    {
        var value = await SecureStorage.Default.GetAsync(key);
        return value != null;
    }

    public Task ClearAllAsync()
    {
        SecureStorage.Default.RemoveAll();
        return Task.CompletedTask;
    }
}
