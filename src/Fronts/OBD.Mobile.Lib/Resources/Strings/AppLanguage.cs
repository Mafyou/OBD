namespace OBD.Mobile.Lib.Resources.Strings;

/// <summary>The two languages of the app: French by default, then English. Pure rules, without MAUI, so they are
/// tested on their own.</summary>
public static class AppLanguage
{
    public const string French = "fr";

    public const string English = "en";

    /// <summary>Language of the app for a language code ("en", "en-GB", "de_DE", a saved preference…): English for an
    /// English code, French for any other code, an empty one or garbage. Never a third language.</summary>
    public static string FromCode(string? languageCode)
    {
        var language = languageCode?.Trim().Split('-', '_')[0];
        return string.Equals(language, English, StringComparison.OrdinalIgnoreCase) ? English : French;
    }

    /// <summary>Language of the app at start-up: the saved choice when there is one (a saved value that is neither
    /// fr nor en reads as French), otherwise the language of the phone (<see cref="FromCode"/>).</summary>
    public static string Resolve(string? savedCode, CultureInfo phoneCulture) =>
        FromCode(string.IsNullOrWhiteSpace(savedCode) ? phoneCulture.Name : savedCode);

    /// <summary>Culture the app runs in for <paramref name="language"/> on a phone set to
    /// <paramref name="phoneCulture"/>: the phone's own regional culture when it speaks that language (fr-CA, en-GB:
    /// its dates and numbers), fr-FR or en-US otherwise. Never a third language.</summary>
    public static CultureInfo CultureFor(string? language, CultureInfo phoneCulture)
    {
        var appLanguage = FromCode(language);
        if (string.Equals(phoneCulture.TwoLetterISOLanguageName, appLanguage, StringComparison.OrdinalIgnoreCase))
            return phoneCulture;
        return CultureInfo.GetCultureInfo(appLanguage == English ? "en-US" : "fr-FR");
    }
}
