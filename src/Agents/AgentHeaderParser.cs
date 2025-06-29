using System.Text.RegularExpressions;

namespace Agents;

public static class AgentHeaderParser
{
    private static readonly Regex StartHeaderRegex = new(@"^\[header-start\]\s*$", RegexOptions.Multiline);
    private static readonly Regex EndHeaderRegex = new(@"^\[header-end\]\s*$", RegexOptions.Multiline);

    public const string AgentInvokeHeader = "[agent-invoke]";
    public const string StreamToUserHeader = "[stream-to-user]";
    public const string TaskCompleteHeader = "[task-complete]";

    public static bool HasStartEndHeaders(string input)
    {
        var startMatch = StartHeaderRegex.Match(input);
        var endMatch = EndHeaderRegex.Match(input);

        return startMatch.Success && endMatch.Success;
    }

    public static bool HasHeader(string header, string content)
    {
        if (string.IsNullOrEmpty(content))
            return false;
        
        var headers = ExtractHeaders(content);

        var hasHeader = headers.Contains(header);

        return hasHeader;
    }

    public static List<string> ExtractHeaders(string input)
    {
        var startMatch = StartHeaderRegex.Match(input);
        var endMatch = EndHeaderRegex.Match(input);

        var headerBlock = input.Substring(startMatch.Index, endMatch.Index + endMatch.Length - startMatch.Index);
        var headerLines = headerBlock.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return [..headerLines];
    }

    public static string RemoveHeaders(string input)
    {
        var startMatch = StartHeaderRegex.Match(input);
        var endMatch = EndHeaderRegex.Match(input);

        if (startMatch.Success && endMatch.Success && endMatch.Index > startMatch.Index)
        {
            var afterHeaderIndex = endMatch.Index + endMatch.Length;
          
            while (afterHeaderIndex < input.Length && (input[afterHeaderIndex] == '\n' || input[afterHeaderIndex] == '\r'))
                afterHeaderIndex++;
            
            return input.Substring(afterHeaderIndex);
        }
        return input;
    }

    public static Dictionary<string, string> ExtractHeaderValues(List<string> headers, string headerKey)
    {
        var result = new Dictionary<string, string>();
        if (headers.Count == 0 || string.IsNullOrWhiteSpace(headerKey)) return result;

        var pattern = $@"\[{Regex.Escape(headerKey)}:([^\]]+)\]";
        var regex = new Regex(pattern);
        string? foundValue = null;

        foreach (var header in headers)
        {
            var match = regex.Match(header);
            if (match.Success && match.Groups.Count > 1)
            {
                if (foundValue != null)
                {
                    throw new InvalidOperationException($"Multiple values found for header '{headerKey}'. Only one value is allowed.");
                }
                foundValue = match.Groups[1].Value;
            }
        }
        if (foundValue != null)
        {
            result[headerKey] = foundValue;
        }
        return result;
    }
}