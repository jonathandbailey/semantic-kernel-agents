
namespace Agents.Tests
{
    public class AgentHeaderParserTests
    {
        private const string ContentWithStreamToUserHeader =
            "[header-start]\n[stream-to-user]\n[header-end]\nThis is the content\and more content";

        private const string TaskIdKey = "task-id";
        private const string TaskIdGuidKey = "2E66793C-C7F6-4512-8106-C5372CFB9029";

        private const string ContentWithTaskKeyHeader =
            "[header-start]\n[task-id:{0}]\n[header-end]\nThis is the content\and more content";

        [Fact]
        public void HasHeaders_WithMultipleHeaders_ReturnsTrue()
        {
            var match = AgentHeaderParser.HasStartEndHeaders(ContentWithStreamToUserHeader);

            Assert.True(match);
        }

        [Fact]
        public void ExtractHeaders_WithMultipleHeaders_ReturnsStreamToUserHeader()
        {
            var headerContent = AgentHeaderParser.ExtractHeaders(ContentWithStreamToUserHeader);

            Assert.Contains(AgentHeaderParser.StreamToUserHeader, headerContent);
        }

        [Fact]
        public void RemoveHeaders_WithStartAndEndHeaders_ReturnsContent()
        {
            var content = AgentHeaderParser.RemoveHeaders(ContentWithStreamToUserHeader);

            Assert.Equal("This is the content\and more content", content);
        }

        [Fact]
        public void HasHeader_WithMatchingHeader_ReturnsTrue()
        {
            var hasHeader = AgentHeaderParser.HasHeader(AgentHeaderParser.StreamToUserHeader, ContentWithStreamToUserHeader);

            Assert.True(hasHeader);

        }

        [Fact]
        public void ExtractHeaderValues_WithTaskId_ReturnsKeyValuePair()
        {
            var headerInput = string.Format(ContentWithTaskKeyHeader, TaskIdGuidKey);
            
            var headers = AgentHeaderParser.ExtractHeaders(headerInput);

            var keys = AgentHeaderParser.ExtractHeaderValues(headers, TaskIdKey);

            Assert.True(keys.ContainsKey(TaskIdKey));
            Assert.True(keys.ContainsValue(TaskIdGuidKey));
        }
    }

}