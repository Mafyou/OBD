namespace OBD.Mobile.Lib.Models;

/// <summary>What a sector holds, and is deleted with it: its notes (sensitive ones included) and its people.</summary>
public readonly record struct SectorContents(int Notes, int Persons)
{
    public bool IsEmpty => Notes is 0 && Persons is 0;

    /// <summary>The message of the delete confirmation: how many notes and people go with the sector, if any.</summary>
    public string DeleteMessage()
    {
        if (IsEmpty)
            return AppResources.GetString("Common_IrreversibleAction");

        List<string> parts = new(2);
        if (Notes > 0)
            parts.Add(Count(Notes, "SectorDetails_NoteCountOne", "SectorDetails_NoteCountMany"));
        if (Persons > 0)
            parts.Add(Count(Persons, "SectorDetails_PersonCountOne", "SectorDetails_PersonCountMany"));

        return AppResources.GetString("SectorDetails_DeleteContentsFormat", string.Join(AppResources.GetString("Common_And"), parts));
    }

    private static string Count(int count, string one, string many) =>
        count is 1 ? AppResources.GetString(one) : AppResources.GetString(many, count);
}
