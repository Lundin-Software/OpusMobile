namespace Opus.Mobile.ViewModels;

public interface ILoginPreferencesStore
{
    bool ContainsKey(string key);

    T Get<T>(string key, T defaultValue);

    void Set<T>(string key, T value);

    void Remove(string key);
}

internal sealed class LoginPreferencesStore : ILoginPreferencesStore
{
    public bool ContainsKey(string key) => Preferences.ContainsKey(key);

    public T Get<T>(string key, T defaultValue) =>
        Preferences.Default.Get(key, defaultValue);

    public void Set<T>(string key, T value) =>
        Preferences.Default.Set(key, value);

    public void Remove(string key) =>
        Preferences.Default.Remove(key);
}
