// Declared in the namespace of the view models: their sources (linked from the app) find these types before the MAUI
// ones of the same name, which only work on a phone. Each one hands its work to AppPlatform.
namespace OBD.Mobile.ViewModels;

public static class Shell
{
    public static FakeShell Current { get; } = new();
}

public sealed class FakeShell
{
    public Task GoToAsync(string route)
    {
        AppPlatform.Routes.Add(route);
        return Task.CompletedTask;
    }
}

public static class Preferences
{
    public static FakePreferences Default { get; } = new();
}

public sealed class FakePreferences
{
    public void Set<T>(string key, T value) => AppPlatform.Preferences[key] = value;

    public T Get<T>(string key, T defaultValue) =>
        AppPlatform.Preferences.TryGetValue(key, out var value) && value is T typed ? typed : defaultValue;
}

public static class AppInfo
{
    public static FakeAppInfo Current { get; } = new();
}

public sealed class FakeAppInfo
{
    public string VersionString => "2.8";

    public string BuildString => "24";
}

public static class DeviceInfo
{
    public static FakeDeviceInfo Current { get; } = new();
}

public sealed class FakeDeviceInfo
{
    public string Platform => "Android";

    public string VersionString => "16";

    public string Manufacturer => "Contoso";

    public string Model => "Phone 1";
}

public static class Launcher
{
    public static Task<bool> OpenAsync(string uri)
    {
        AppPlatform.OpenedUris.Add(uri);
        return Task.FromResult(true);
    }
}

public static class FileSystem
{
    public static string CacheDirectory => AppPlatform.CacheDirectory;
}

public static class Share
{
    public static FakeShare Default { get; } = new();
}

public sealed class FakeShare
{
    public Task RequestAsync(ShareFileRequest request)
    {
        AppPlatform.SharedFiles.Add(request);
        AppPlatform.SharedFileContents.Add(File.ReadAllBytes(request.File!.FullPath));
        return Task.CompletedTask;
    }
}

public static class MediaPicker
{
    public static Task<FileResult?> CapturePhotoAsync() => AppPlatform.CapturePhoto();

    public static Task<List<FileResult>> PickPhotosAsync() => AppPlatform.PickPhotos();
}
