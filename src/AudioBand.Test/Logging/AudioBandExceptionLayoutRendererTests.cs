using System;
using System.Text;
using AudioBand.Logging;
using NLog;
using Xunit;

namespace AudioBand.Test.Logging
{
    public class AudioBandExceptionLayoutRendererTests
    {
        [Fact]
        public void Render_ExceptionIsNull_ReturnsEmptyString()
        {
            var renderer = new AudioBandExceptionLayoutRenderer();
            var logEvent = new LogEventInfo { Exception = null };

            var result = renderer.Render(logEvent);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Render_ExceptionIsNotNull_ReturnsFormattedString()
        {
            var renderer = new AudioBandExceptionLayoutRenderer();
            var exception = new Exception("Test exception message");
            var logEvent = new LogEventInfo { Exception = exception };

            var result = renderer.Render(logEvent);

            var sb = new StringBuilder();
            sb.AppendLine("---START OF EXCEPTION---");
            sb.AppendLine(exception.ToString());
            sb.AppendLine("---END OF EXCEPTION");
            var expected = sb.ToString();

            Assert.Equal(expected, result);
        }
    }
}
