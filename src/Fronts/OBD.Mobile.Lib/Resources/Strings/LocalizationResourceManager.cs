namespace OBD.Mobile.Lib.Resources.Strings;

/// <summary>A language selectable from the Settings page.</summary>
public readonly record struct LanguageOption(string Code, string Name);

/// <summary>
/// Singleton that exposes localized strings as an indexer so XAML bindings (via <see cref="TranslateExtension"/>)
/// refresh live whenever <see cref="SetCulture(CultureInfo)"/> is called, without requiring a page reload.
/// </summary>
public sealed class LocalizationResourceManager : INotifyPropertyChanged
{
    private const string LanguagePreferenceKey = "app_language";

    public static LocalizationResourceManager Instance { get; } = new();

    public static IReadOnlyList<LanguageOption> SupportedLanguages { get; } =
    [
        new("fr", AppResources.GetString("Language_French")),
        new("en", AppResources.GetString("Language_English")),
    ];

    private LocalizationResourceManager()
    {
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string this[string key] => AppResources.GetString(key);

    public LanguageOption CurrentLanguage
    {
        get
        {
            var code = (AppResources.Culture ?? CultureInfo.CurrentUICulture).TwoLetterISOLanguageName;
            return SupportedLanguages.FirstOrDefault(l => l.Code == code, SupportedLanguages[0]);
        }
    }

    /// <summary>Restores the previously saved language preference, if any. Call once at app startup.</summary>
    public void RestoreSavedCulture()
    {
        var saved = Preferences.Default.Get(LanguagePreferenceKey, string.Empty);
        if (!string.IsNullOrEmpty(saved))
            SetCulture(new CultureInfo(saved), persist: false);
    }

    public void SetCulture(CultureInfo culture) => SetCulture(culture, persist: true);

    private void SetCulture(CultureInfo culture, bool persist)
    {
        if (persist)
            Preferences.Default.Set(LanguagePreferenceKey, culture.TwoLetterISOLanguageName);

        if (Equals(AppResources.Culture, culture))
            return;

        AppResources.Culture = culture;
        CultureInfo.CurrentUICulture = culture;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }
}
