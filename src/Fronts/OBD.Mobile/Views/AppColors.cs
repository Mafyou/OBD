namespace OBD.Mobile.Views;

/// <summary>The colors of Colors.xaml, for the code that sets a color itself instead of a XAML binding.</summary>
public static class AppColors
{
    /// <summary>The color named <paramref name="key"/> in the resources of the app, or <paramref name="fallback"/> before they are loaded.</summary>
    public static Color Get(string key, Color fallback) =>
        Application.Current?.Resources.TryGetValue(key, out var value) is true && value is Color color ? color : fallback;
}