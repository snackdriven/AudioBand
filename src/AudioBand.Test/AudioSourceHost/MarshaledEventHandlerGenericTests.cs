using System;
using Xunit;
using AudioSourceHost;

namespace AudioBand.Test.AudioSourceHost
{
    public class MarshaledEventHandlerGenericTests
    {
        [Fact]
        public void Handler_InvokesActionWithArgs()
        {
            // Arrange
            bool wasInvoked = false;
            string actualArg = null;
            var handler = new MarshaledEventHandler<string>(arg =>
            {
                wasInvoked = true;
                actualArg = arg;
            });

            // Act
            handler.Handler(this, "test_arg");

            // Assert
            Assert.True(wasInvoked);
            Assert.Equal("test_arg", actualArg);
        }

        [Fact]
        public void InitializeLifetimeService_ReturnsNull()
        {
            // Arrange
            var handler = new MarshaledEventHandler<string>(arg => { });

            // Act
            var result = handler.InitializeLifetimeService();

            // Assert
            Assert.Null(result);
        }
    }
}
