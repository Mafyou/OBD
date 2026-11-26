namespace OBD.Mobile.Lib.UnitTests.Models;

[Collection("AppResourcesCulture")]
public sealed class SectorContentsTests : IDisposable
{
    private readonly CultureInfo? _originalCulture = AppResources.Culture;

    public void Dispose() => AppResources.Culture = _originalCulture;

    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(1, 0, false)]
    [InlineData(0, 1, false)]
    public void IsEmpty_ShouldBeTrueOnlyWithoutNotesNorPeople(int notes, int persons, bool expected) =>
        new SectorContents(notes, persons).IsEmpty.ShouldBe(expected);

    [Theory]
    [InlineData(0, 0, "Cette action est irréversible.")]
    [InlineData(1, 0, "Ce secteur et tout son contenu (1 note) seront supprimés. Cette action est irréversible.")]
    [InlineData(0, 1, "Ce secteur et tout son contenu (1 personne) seront supprimés. Cette action est irréversible.")]
    [InlineData(3, 2, "Ce secteur et tout son contenu (3 notes et 2 personnes) seront supprimés. Cette action est irréversible.")]
    public void DeleteMessage_ShouldSayHowManyNotesAndPeopleGo_InFrench(int notes, int persons, string expected)
    {
        AppResources.Culture = CultureInfo.GetCultureInfo("fr-FR");

        new SectorContents(notes, persons).DeleteMessage().ShouldBe(expected);
    }

    [Theory]
    [InlineData(0, 0, "This action cannot be undone.")]
    [InlineData(1, 1, "This sector and everything in it (1 note and 1 person) will be deleted. This action cannot be undone.")]
    [InlineData(4, 0, "This sector and everything in it (4 notes) will be deleted. This action cannot be undone.")]
    [InlineData(0, 5, "This sector and everything in it (5 people) will be deleted. This action cannot be undone.")]
    public void DeleteMessage_ShouldSayHowManyNotesAndPeopleGo_InEnglish(int notes, int persons, string expected)
    {
        AppResources.Culture = CultureInfo.GetCultureInfo("en-US");

        new SectorContents(notes, persons).DeleteMessage().ShouldBe(expected);
    }
}
