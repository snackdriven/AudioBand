using AudioBand.AudioSource;
using AudioBand.TextFormatting;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AudioBand.Test.TextFormatting
{
    public class TextPlaceholderTests
    {
        private class TestTextPlaceholder : TextPlaceholder
        {
            public TestTextPlaceholder(IEnumerable<TextPlaceholderParameter> parameters, IAudioSession audioSession) : base(parameters, audioSession)
            {
            }

            public override string GetText()
            {
                return "";
            }

            public string ExposeGetParameter(string parameterName)
            {
                return GetParameter(parameterName);
            }
        }

        [Fact]
        public void GetParameter_GivenValidParameter_ReturnsCorrectValue()
        {
            var parameters = new List<TextPlaceholderParameter>
            {
                new TextPlaceholderParameter { Name = "test", Value = "value" },
                new TextPlaceholderParameter { Name = "key", Value = "123" }
            };

            var sessionMock = new Mock<IAudioSession>();

            var placeholder = new TestTextPlaceholder(parameters, sessionMock.Object);

            Assert.Equal("value", placeholder.ExposeGetParameter("test"));
            Assert.Equal("123", placeholder.ExposeGetParameter("key"));
        }

        [Fact]
        public void GetParameter_GivenInvalidParameter_ReturnsNull()
        {
            var parameters = new List<TextPlaceholderParameter>
            {
                new TextPlaceholderParameter { Name = "test", Value = "value" }
            };

            var sessionMock = new Mock<IAudioSession>();

            var placeholder = new TestTextPlaceholder(parameters, sessionMock.Object);

            Assert.Null(placeholder.ExposeGetParameter("nonexistent"));
        }

        [Fact]
        public void GetParameter_GivenNullParameters_ReturnsNull()
        {
            var sessionMock = new Mock<IAudioSession>();
            var placeholder = new TestTextPlaceholder(null, sessionMock.Object);
            Assert.Null(placeholder.ExposeGetParameter("test"));
        }
    }
}
