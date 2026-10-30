using AudioSourceHost;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using Xunit;

namespace AudioBand.Test.AudioSourceHost
{
    public class AudioSourceLoggerTests : IDisposable
    {
        private MemoryTarget _target;

        public AudioSourceLoggerTests()
        {
            var config = new LoggingConfiguration();
            _target = new MemoryTarget("target");
            config.AddTarget(_target);
            config.AddRuleForAllLevels(_target);
            LogManager.Configuration = config;
        }

        public void Dispose()
        {
            LogManager.Configuration = null;
        }

        [Fact]
        public void Logger_DebugString_ForwardsToNLog()
        {
            var logger = new AudioSourceLogger("TestDebugString");
            logger.Debug("test debug message");
            Assert.Contains("test debug message", string.Join(" ", _target.Logs));
        }

        [Fact]
        public void Logger_DebugObject_ForwardsToNLog()
        {
            var logger = new AudioSourceLogger("TestDebugObject");
            logger.Debug(42);
            Assert.Contains("42", string.Join(" ", _target.Logs));
        }

        [Fact]
        public void Logger_InfoString_ForwardsToNLog()
        {
            var logger = new AudioSourceLogger("TestInfoString");
            logger.Info("test info message");
            Assert.Contains("test info message", string.Join(" ", _target.Logs));
        }

        [Fact]
        public void Logger_InfoObject_ForwardsToNLog()
        {
            var logger = new AudioSourceLogger("TestInfoObject");
            logger.Info(42);
            Assert.Contains("42", string.Join(" ", _target.Logs));
        }

        [Fact]
        public void Logger_WarnString_ForwardsToNLog()
        {
            var logger = new AudioSourceLogger("TestWarnString");
            logger.Warn("test warn message");
            Assert.Contains("test warn message", string.Join(" ", _target.Logs));
        }

        [Fact]
        public void Logger_WarnObject_ForwardsToNLog()
        {
            var logger = new AudioSourceLogger("TestWarnObject");
            logger.Warn(42);
            Assert.Contains("42", string.Join(" ", _target.Logs));
        }

        [Fact]
        public void Logger_ErrorString_ForwardsToNLog()
        {
            var logger = new AudioSourceLogger("TestErrorString");
            logger.Error("test error message");
            Assert.Contains("test error message", string.Join(" ", _target.Logs));
        }

        [Fact]
        public void Logger_ErrorObject_ForwardsToNLog()
        {
            var logger = new AudioSourceLogger("TestErrorObject");
            logger.Error(42);
            Assert.Contains("42", string.Join(" ", _target.Logs));
        }
    }
}
