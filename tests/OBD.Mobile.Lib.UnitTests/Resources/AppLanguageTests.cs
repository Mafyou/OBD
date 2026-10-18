namespace OBD.Mobile.Lib.UnitTests.Resources;

public class AppLanguageTests
{
    [Theory]
    [InlineData("en", AppLanguage.English)]
    [InlineData("en-GB", AppLanguage.English)]
    [InlineData("EN_us", AppLanguage.English)]
    [InlineData("fr", AppLanguage.French)]
    [InlineData("fr-CA", AppLanguage.French)]
    [InlineData("de", AppLanguage.French)]
    [InlineData("es-ES", AppLanguage.French)]
    [InlineData("it", AppLanguage.French)]
    [InlineData("", AppLanguage.French)]
    [InlineData("garbage", AppLanguage.French)]
    [InlineData(null, AppLanguage.French)]
    public void FromCode_ShouldBeEnglishOnlyForEnglish_AndFrenchOtherwise(string? code, string expected) =>
        AppLanguage.FromCode(code).ShouldBe(expected);

    [Theory]
    [InlineData("en-US", AppLanguage.English)]
    [InlineData("en-GB", AppLanguage.English)]
    [InlineData("fr-FR", AppLanguage.French)]
    [InlineData("de-DE", AppLanguage.French)]
    [InlineData("es-ES", AppLanguage.French)]
    public void Resolve_ShouldFollowThePhone_OnAFirstLaunch(string phone, string expected) =>
        AppLanguage.Resolve(string.Empty, CultureInfo.GetCultureInfo(phone)).ShouldBe(expected);

    [Theory]
    [InlineData("fr", "en-US", AppLanguage.French)]
    [InlineData("en", "fr-FR", AppLanguage.English)]
    [InlineData("en", "de-DE", AppLanguage.English)]
    [InlineData("de", "en-US", AppLanguage.French)]
    [InlineData("zz-garbage", "en-US", AppLanguage.French)]
    public void Resolve_ShouldKeepAValidSavedChoice_AndReadAnyOtherAsFrench(string saved, string phone, string expected) =>
        AppLanguage.Resolve(saved, CultureInfo.GetCultureInfo(phone)).ShouldBe(expected);

    [Theory]
    [InlineData(AppLanguage.French, "fr-CA", "fr-CA")]
    [InlineData(AppLanguage.French, "de-DE", "fr-FR")]
    [InlineData(AppLanguage.French, "en-GB", "fr-FR")]
    [InlineData(AppLanguage.English, "en-GB", "en-GB")]
    [InlineData(AppLanguage.English, "fr-FR", "en-US")]
    [InlineData("de", "de-DE", "fr-FR")]
    public void CultureFor_ShouldNeverBeAThirdLanguage(string language, string phone, string expected) =>
        AppLanguage.CultureFor(language, CultureInfo.GetCultureInfo(phone)).Name.ShouldBe(expected);
}
