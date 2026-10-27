namespace OBD.Mobile.Lib.Data;

/// <summary>
/// Patterns for a "contains" search with <c>LIKE ? ESCAPE '\'</c> (<see cref="EscapeClause"/>): the % and _ the user types are
/// searched as themselves instead of matching any text.
/// </summary>
public static class SqlLike
{
    public const string EscapeClause = @"ESCAPE '\'";

    /// <summary>The pattern of the rows holding <paramref name="text"/> anywhere.</summary>
    public static string Contains(string text) => $"%{Escape(text)}%";

    private static string Escape(string text) =>
        text.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_");
}