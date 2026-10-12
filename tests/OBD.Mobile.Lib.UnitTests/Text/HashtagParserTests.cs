namespace OBD.Mobile.Lib.UnitTests.Text;

public class HashtagParserTests
{
    [Fact]
    public void Split_ShouldReturnEmpty_WhenContentIsEmpty()
    {
        HashtagParser.Split(string.Empty).ShouldBeEmpty();
    }

    [Fact]
    public void Split_ShouldReturnSingleSegment_WhenContentHasNoHashtag()
    {
        var result = HashtagParser.Split("Plain text without tags");

        result.ShouldBe(["Plain text without tags"]);
    }

    [Fact]
    public void Split_ShouldSeparateHashtagsFromSurroundingText()
    {
        var result = HashtagParser.Split("See #architecture for details, also #roadmap.");

        result.ShouldBe(["See ", "#architecture", " for details, also ", "#roadmap", "."]);
    }

    [Theory]
    [InlineData("#architecture", true, "architecture")]
    [InlineData("#a", true, "a")]
    [InlineData("#", false, "")]
    [InlineData("plain", false, "")]
    [InlineData("", false, "")]
    public void TryGetWord_ShouldExtractWord_WhenSegmentIsHashtag(string segment, bool expectedResult, string expectedWord)
    {
        var result = HashtagParser.TryGetWord(segment, out var word);

        result.ShouldBe(expectedResult);
        word.ShouldBe(expectedWord);
    }
}
