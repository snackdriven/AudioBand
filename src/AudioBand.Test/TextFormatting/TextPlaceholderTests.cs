using System.Collections.Generic;
using AudioBand.AudioSource;
using AudioBand.TextFormatting;
using Moq;
using Xunit;

namespace AudioBand.Test
{
    public class TextPlaceholderTests
    {
        [Fact]
        public void Parameters_RetainValuesAndReturnNullForMissingNames()
        {
            var placeholder = new ParameterPlaceholder(new[]
            {
                new TextPlaceholderParameter { Name = "format", Value = "mm:ss" },
                new TextPlaceholderParameter { Name = "empty", Value = "" },
            });

            Assert.Equal("mm:ss", placeholder.Parameter("format"));
            Assert.Equal("", placeholder.Parameter("empty"));
            Assert.Null(placeholder.Parameter("missing"));
            Assert.Null(placeholder.Parameter("FORMAT"));
        }

        [Fact]
        public void Parameters_NullSequenceIsEmpty()
        {
            Assert.Null(new ParameterPlaceholder(null).Parameter("format"));
        }

        [Fact]
        public void Parameters_LastDuplicateWinsAndConstructorSnapshotsValues()
        {
            var parameter = new TextPlaceholderParameter { Name = "format", Value = "ss" };
            var parameters = new List<TextPlaceholderParameter>
            {
                new TextPlaceholderParameter { Name = "format", Value = "mm:ss" },
                parameter,
            };
            var placeholder = new ParameterPlaceholder(parameters);
            parameter.Value = "changed";
            parameters.Clear();

            Assert.Equal("ss", placeholder.Parameter("format"));
        }

        private sealed class ParameterPlaceholder : TextPlaceholder
        {
            public ParameterPlaceholder(IEnumerable<TextPlaceholderParameter> parameters)
                : base(parameters, new Mock<IAudioSession>().Object)
            {
            }

            public string Parameter(string name) => GetParameter(name);

            public override string GetText() => "fixture";
        }
    }
}
