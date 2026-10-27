namespace OBD.Mobile.Lib.UnitTests.Data;

public class SqlLikeTests
{
    [Theory]
    [InlineData("abc", "%abc%")]
    [InlineData("10%", @"%10\%%")]
    [InlineData("file_name", @"%file\_name%")]
    [InlineData(@"C:\temp", @"%C:\\temp%")]
    [InlineData("", "%%")]
    public void Contains_ShouldEscapeWildcardsAndTheEscapeCharacter(string text, string expected) =>
        SqlLike.Contains(text).ShouldBe(expected);
}