namespace OBD.Mobile.Lib.Text;

/// <summary>
/// Splits note content into plain-text and "#word" segments so a UI layer can render hashtags
/// as distinct, tappable spans without duplicating the parsing logic.
/// </summary>
public static partial class HashtagParser
{
    [GeneratedRegex(@"(#\w+)")]
    private static partial Regex HashtagRegex();

    /// <summary>
    /// Splits <paramref name="content"/> into alternating plain-text and "#word" segments.
    /// Returns an empty list when <paramref name="content"/> is null or empty.
    /// </summary>
    public static IReadOnlyList<string> Split(string content) =>
        string.IsNullOrEmpty(content) ? [] : HashtagRegex().Split(content);

    /// <summary>
    /// Attempts to extract the bare word (without the leading '#') from a segment produced by <see cref="Split"/>.
    /// </summary>
    public static bool TryGetWord(string segment, out string word)
    {
        if (!string.IsNullOrEmpty(segment) && segment[0] == '#' && segment.Length > 1)
        {
            word = segment[1..];
            return true;
        }

        word = string.Empty;
        return false;
    }
}
