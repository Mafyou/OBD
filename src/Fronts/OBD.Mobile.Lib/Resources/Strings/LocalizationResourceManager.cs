namespace OBD.Mobile.Lib.Resources.Strings;

/// <summary>A language selectable from the Settings page.</summary>
public readonly record struct LanguageOption(string Code, string Name);

/// <summary>
/// Singleton that exposes localized strings as an indexer so XAML bindings (via <see cref="TranslateExtension"/>)
/// refresh live whenever <see cref="SetLanguage(string)"/> is called, without requiring a page reload.
/// </summary>
public sealed class LocalizationResourceManager : INotifyPropertyChanged
{
    private const string LanguagePreferenceKey = "app_language";

    public static LocalizationResourceManager Instance { get; } = new();

    /// <summary>French first, then English: the only two languages of the app.</summary>
    public static IReadOnlyList<LanguageOption> SupportedLanguages { get; } =
    [
        new(AppLanguage.French, AppResources.GetString("Language_French")),
        new(AppLanguage.English, AppResources.GetString("Language_English")),
    ];

    // The culture of the phone, read before the app sets its own.
    private readonly CultureInfo _phoneCulture = CultureInfo.CurrentUICulture;

    private LocalizationResourceManager()
    {
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string this[string key] => AppResources.GetString(key);

    public LanguageOption CurrentLanguage
    {
        get
        {
            var code = AppLanguage.FromCode((AppResources.Culture ?? CultureInfo.CurrentUICulture).Name);
            return SupportedLanguages.FirstOrDefault(l => l.Code == code, SupportedLanguages[0]);
        }
    }

    /// <summary>Applies the saved language, or on a first launch the language of the phone (English for an English
    /// phone, French for any other). Call once at app startup.</summary>
    public void RestoreSavedCulture()
    {
        var saved = Preferences.Default.Get(LanguagePreferenceKey, string.Empty);
        ApplyLanguage(AppLanguage.Resolve(saved, _phoneCulture), persist: false);
    }

    /// <summary>Switches the app to <paramref name="languageCode"/> (French for anything but English) and saves it.</summary>
    public void SetLanguage(string languageCode) => ApplyLanguage(AppLanguage.FromCode(languageCode), persist: true);

    private void ApplyLanguage(string language, bool persist)
    {
        if (persist)
            Preferences.Default.Set(LanguagePreferenceKey, language);

        var culture = AppLanguage.CultureFor(language, _phoneCulture);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        if (Equals(AppResources.Culture, culture))
            return;

        AppResources.Culture = culture;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }
}
