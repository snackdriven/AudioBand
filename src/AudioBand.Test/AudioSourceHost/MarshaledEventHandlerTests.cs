using System;
using Xunit;
using AudioSourceHost;

namespace AudioBand.Test.AudioSourceHost
{
    public class MarshaledEventHandlerTests
    {
        [Fact]
        public void Handler_InvokesAction()
        {
            // Arrange
            bool wasInvoked = false;
            var handler = new MarshaledEventHandler(() => wasInvoked = true);

            // Act
            handler.Handler(this, EventArgs.Empty);

            // Assert
            Assert.True(wasInvoked);
        }

        [Fact]
        public void InitializeLifetimeService_ReturnsNull()
        {
            // Arrange
            var handler = new MarshaledEventHandler(() => { });

            // Act
            var result = handler.InitializeLifetimeService();

            // Assert
            Assert.Null(result);
        }
    }
}
