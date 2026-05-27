namespace OBD.Mobile.Lib.UnitTests.Models;

public class EntityDefaultsTests
{
    [Fact]
    public void Person_ShouldInitializeStringPropertiesWithEmptyValues()
    {
        var person = new Person();

        person.Name.ShouldBe(string.Empty);
        person.Position.ShouldBe(string.Empty);
        person.Memo.ShouldBe(string.Empty);
    }

    [Fact]
    public void Sector_ShouldInitializeNameWithEmptyValue()
    {
        var sector = new Sector();

        sector.Name.ShouldBe(string.Empty);
    }

    [Fact]
    public void WorkHabits_ShouldInitializePropertiesWithExpectedDefaults()
    {
        var workHabits = new WorkHabits();

        workHabits.RegularMeetings.ShouldBe(string.Empty);
        workHabits.RemoteWorkDays.ShouldBe(string.Empty);
        workHabits.ManagerId.ShouldBe(0);
        workHabits.Manager.ShouldBeNull();
    }

    [Fact]
    public void Note_ShouldInitializeCreatedAtCloseToUtcNow()
    {
        var before = DateTime.UtcNow;
        var note = new Note();
        var after = DateTime.UtcNow;

        note.CreatedAt.ShouldBeGreaterThanOrEqualTo(before);
        note.CreatedAt.ShouldBeLessThanOrEqualTo(after);
    }
}