namespace OBD.Mobile.Lib.Abstractions;

/// <summary>
/// The notes of the notebook. The lists (<see cref="GetBySectorAsync"/>, <see cref="GetSensitiveAsync"/>,
/// <see cref="GetVisualsAsync"/>, <see cref="SearchAsync"/>) leave the image of a sketch out: they only show its title.
/// Open a note with <see cref="GetAsync"/> before changing it, never save a note of a list.
/// </summary>
public interface INoteService
{
    Task<IReadOnlyList<Note>> GetBySectorAsync(int sectorId);
    Task<IReadOnlyList<Note>> GetSensitiveAsync();

    /// <summary>Every sketch and photo, sensitive ones included, without their images.</summary>
    Task<IReadOnlyList<Note>> GetVisualsAsync();

    Task<IReadOnlyList<Note>> SearchAsync(string query);
    Task<Note?> GetAsync(int id);
    Task<int> InsertAsync(Note note);
    Task<int> UpdateAsync(Note note);

    /// <summary>Moves a note into the sensitive section, or out of it, without rewriting the rest of it.</summary>
    Task<int> SetSensitiveAsync(int noteId, bool isSensitive);

    /// <summary>Deletes the note and the hashtag links from it or to it.</summary>
    Task<int> DeleteAsync(Note note);
}