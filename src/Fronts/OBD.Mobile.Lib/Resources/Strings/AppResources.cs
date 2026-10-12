namespace OBD.Mobile.Lib.Resources.Strings;

/// <summary>
/// Thin wrapper around the generated resource manager for AppResources.resx / AppResources.en.resx.
/// Avoids depending on the Visual Studio designer code-generator so it also works from the CLI.
/// </summary>
public static class AppResources
{
    private static readonly ResourceManager resourceManager =
        new("OBD.Mobile.Lib.Resources.Strings.AppResources", typeof(AppResources).Assembly);

    /// <summary>
    /// Explicit culture override. When null, resource lookup falls back to <see cref="CultureInfo.CurrentUICulture"/>.
    /// </summary>
    public static CultureInfo? Culture { get; set; }

    public static string GetString(string name) => resourceManager.GetString(name, Culture) ?? name;

    public static string GetString(string name, params object[] args) =>
        string.Format(Culture, GetString(name), args);
}
