namespace FluentPaint.Core.Parsers;

public class ColorChannelsParser
{
    public List<string> Parse(string colorChannels)
    {
        if (!colorChannels.Contains("And"))
        {
            return colorChannels == "All"
                ? new List<string> { "First", "Second", "Third" }
                : new List<string> { colorChannels };
        }

        var andIndex = colorChannels.IndexOf("And", StringComparison.Ordinal);
        var firstValue = colorChannels[..andIndex];
        var secondValue = colorChannels[(andIndex + 3)..];

        return new List<string> { firstValue, secondValue };
    }
}