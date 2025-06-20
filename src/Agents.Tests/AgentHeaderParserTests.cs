
namespace Agents.Tests
{
    public class AgentHeaderParserTests
    {
        [Fact]
        public void HasHeaders_WithMultipleHeaders_ReturnsTrue()
        {
            const string header = "[header-start]\n[stream-to-user]\n[header-end]";

            var match = AgentHeaderParser.HasStartEndHeaders(header);

            Assert.True(match);
        }

        [Fact]
        public void ExtractHeaders_WithMultipleHeaders_ReturnsStreamToUserHeader()
        {
            const string header = "[header-start]\n[stream-to-user]\n[header-end]";

            var headerContent = AgentHeaderParser.ExtractHeaders(header);

            Assert.Contains(AgentHeaderParser.StreamToUserHeader, headerContent);
        }

        [Fact]
        public void RemoveHeaders_WithStartAndEndHeaders_ReturnsContent()
        {
            const string header = "[header-start]\n[stream-to-user]\n[header-end]\nThis is the content\and more content";

            string content = AgentHeaderParser.RemoveHeaders(header);

            Assert.Equal("This is the content\and more content", content);
        }

        [Fact]
        public void HasHeader_WithMatchingHeader_ReturnsTrue()
        {
            const string input = "[header-start]\n[stream-to-user]\n[header-end]\nThis is the content\and more content";

            var hasHeader = AgentHeaderParser.HasHeader("[stream-to-user]", input);

            Assert.True(hasHeader);

        }
    }

}