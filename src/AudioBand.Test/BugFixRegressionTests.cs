using System;
using System.Collections.Generic;
using System.Linq;
using AudioBand.AudioSource;
using AudioBand.Messages;
using AudioBand.Models;
using AudioBand.Settings;
using AudioBand.Settings.Persistence;
using Moq;
using Xunit;

namespace AudioBand.Test
{
    public class BugFixRegressionTests
    {
        [Fact]
        public void AudioSession_SwitchAudioSource_UnsubscribesFromOldSourceAndSubscribesToNewSource()
        {
            var appSettingsMock = new Mock<IAppSettings>();
            appSettingsMock.SetupGet(x => x.AudioBandSettings).Returns(new AudioBandSettings { UseAutomaticIdleProfile = false });

            var oldSourceMock = new Mock<IInternalAudioSource>();
            var newSourceMock = new Mock<IInternalAudioSource>();

            var session = new AudioSession(appSettingsMock.Object)
            {
                CurrentAudioSource = oldSourceMock.Object,
            };

            oldSourceMock.Raise(x => x.TrackInfoChanged += null, oldSourceMock.Object, new TrackInfoChangedEventArgs { TrackName = "Old Song", Artist = "Old Artist" });
            Assert.Equal("Old Song", session.SongName);

            session.CurrentAudioSource = newSourceMock.Object;
            Assert.Null(session.SongName);

            // Raising events on the old source must no longer mutate AudioSession
            oldSourceMock.Raise(x => x.TrackInfoChanged += null, oldSourceMock.Object, new TrackInfoChangedEventArgs { TrackName = "Stale Old Song", Artist = "Old Artist" });
            oldSourceMock.Raise(x => x.VolumeChanged += null, oldSourceMock.Object, 99);
            Assert.Null(session.SongName);
            Assert.Equal(0, session.Volume);

            // Raising events on the new source updates AudioSession as expected
            newSourceMock.Raise(x => x.TrackInfoChanged += null, newSourceMock.Object, new TrackInfoChangedEventArgs { TrackName = "New Song", Artist = "New Artist" });
            newSourceMock.Raise(x => x.VolumeChanged += null, newSourceMock.Object, 42);
            Assert.Equal("New Song", session.SongName);
            Assert.Equal(42, session.Volume);
        }

        [Fact]
        public void GetUniqueProfileName_WhenCollisionExists_TerminatesAndAppendsIncrementingNumber()
        {
            var existing = new List<string> { "Default", "Default 1", "Default 2" };

            var unique = UserProfile.GetUniqueProfileName(existing, "Default");

            Assert.Equal("Default 3", unique);
            Assert.DoesNotContain(unique, existing);
        }

        [Fact]
        public void AppSettings_DeleteFirstProfile_AssignsRemainingProfileToIdleAndLastNonIdle()
        {
            var persistMock = new Mock<IPersistentSettings>();
            var messageBusMock = new Mock<IMessageBus>();

            var firstProfile = new UserProfile { Name = "FirstProfile" };
            var secondProfile = new UserProfile { Name = "SecondProfile" };

            persistMock.Setup(x => x.ReadSettings()).Returns(new Settings.Persistence.Settings
            {
                CurrentProfileName = "SecondProfile",
                AudioBandSettings = new AudioBandSettings
                {
                    IdleProfileName = "FirstProfile",
                    LastNonIdleProfileName = "FirstProfile",
                    UseAutomaticIdleProfile = true,
                    LastKnownVersion = "v1.2.5",
                },
            });
            persistMock.Setup(x => x.ReadProfiles()).Returns(new[] { firstProfile, secondProfile });

            var appSettings = new AppSettings(persistMock.Object, messageBusMock.Object);
            appSettings.AudioBandSettings.IdleProfileName = "FirstProfile";
            appSettings.AudioBandSettings.LastNonIdleProfileName = "FirstProfile";

            appSettings.DeleteProfile("FirstProfile");

            Assert.Equal("SecondProfile", appSettings.AudioBandSettings.IdleProfileName);
            Assert.Equal("SecondProfile", appSettings.AudioBandSettings.LastNonIdleProfileName);
            Assert.Single(appSettings.Profiles);
            Assert.Equal("SecondProfile", appSettings.Profiles.First().Name);
        }

        [Fact]
        public void AppSettings_SelectIdleProfile_WhenAutoIdleEnabled_PreservesLastNonIdleProfileName()
        {
            var persistMock = new Mock<IPersistentSettings>();
            var messageBusMock = new Mock<IMessageBus>();

            var mainProfile = new UserProfile { Name = "Essentials 2" };
            var idleProfile = new UserProfile { Name = "Idle" };

            persistMock.Setup(x => x.ReadSettings()).Returns(new Settings.Persistence.Settings
            {
                CurrentProfileName = "Essentials 2",
                AudioBandSettings = new AudioBandSettings
                {
                    IdleProfileName = "Idle",
                    LastNonIdleProfileName = "Essentials 2",
                    UseAutomaticIdleProfile = true,
                    LastKnownVersion = "v1.2.5",
                },
            });
            persistMock.Setup(x => x.ReadProfiles()).Returns(new[] { mainProfile, idleProfile });

            var appSettings = new AppSettings(persistMock.Object, messageBusMock.Object);
            Assert.Equal("Essentials 2", appSettings.AudioBandSettings.LastNonIdleProfileName);

            appSettings.SelectProfile("Idle");

            Assert.Equal("Idle", appSettings.CurrentProfile.Name);
            Assert.Equal("Essentials 2", appSettings.AudioBandSettings.LastNonIdleProfileName);
        }

        [Theory]
        [InlineData(null, true, 0, 0, 0, null)]
        [InlineData("", true, 0, 0, 0, null)]
        [InlineData("$version$", true, 0, 0, 0, null)]
        [InlineData("1.2.6", false, 1, 2, 6, null)]
        [InlineData("v1.2.6", false, 1, 2, 6, null)]
        [InlineData("V1.2.6", false, 1, 2, 6, null)]
        [InlineData("AudioBand v1.2.6", false, 1, 2, 6, null)]
        [InlineData("v1.2.6-beta.1", false, 1, 2, 6, "beta.1")]
        [InlineData("1.2.6-rc.2+build.42", false, 1, 2, 6, "rc.2")]
        public void SemanticVersion_ParsesVariousInputsWithoutThrowing(
            string input,
            bool expectedParseFailed,
            int expectedMajor,
            int expectedMinor,
            int expectedPatch,
            string expectedPreRelease)
        {
            var version = new SemanticVersion(input);

            Assert.Equal(expectedParseFailed, version.ParseFailed);
            Assert.Equal(expectedMajor, version.Major);
            Assert.Equal(expectedMinor, version.Minor);
            Assert.Equal(expectedPatch, version.Patch);
            Assert.Equal(expectedPreRelease, version.PreRelease);
        }

        [Fact]
        public void SemanticVersion_IsNewerVersionThan_HandlesPreReleasesAndNullSafely()
        {
            var stable = new SemanticVersion("v1.2.6");
            var prerelease = new SemanticVersion("v1.2.6-beta.1");
            var older = new SemanticVersion("v1.2.5");
            var invalid = new SemanticVersion((string)null);

            Assert.True(stable.IsNewerVersionThan(prerelease));
            Assert.False(prerelease.IsNewerVersionThan(stable));
            Assert.True(prerelease.IsNewerVersionThan(older));
            Assert.False(stable.IsNewerVersionThan(invalid));
            Assert.False(invalid.IsNewerVersionThan(stable));
            Assert.False(stable.IsNewerVersionThan(null));
        }

        [Fact]
        public void AppSettings_Constructor_PersistsCurrentAssemblyVersionBeforeSaving()
        {
            var persistMock = new Mock<IPersistentSettings>();
            var messageBusMock = new Mock<IMessageBus>();
            Settings.Persistence.Settings savedSettings = null;

            persistMock.Setup(x => x.ReadSettings()).Returns(new Settings.Persistence.Settings
            {
                CurrentProfileName = "Default Profile",
                AudioBandSettings = new AudioBandSettings
                {
                    IdleProfileName = "Idle",
                    LastNonIdleProfileName = "Default Profile",
                    UseAutomaticIdleProfile = false,
                    LastKnownVersion = "v1.2.5",
                },
            });
            persistMock.Setup(x => x.ReadProfiles()).Returns(new[]
            {
                new UserProfile { Name = "Default Profile" },
                new UserProfile { Name = "Idle" },
            });
            persistMock.Setup(x => x.WriteSettings(It.IsAny<Settings.Persistence.Settings>()))
                .Callback<Settings.Persistence.Settings>(s => savedSettings = s);

            var appSettings = new AppSettings(persistMock.Object, messageBusMock.Object);

            Assert.NotNull(savedSettings);
            Assert.NotEqual("v1.2.5", savedSettings.AudioBandSettings.LastKnownVersion);
            Assert.Equal(appSettings.AudioBandSettings.LastKnownVersion, savedSettings.AudioBandSettings.LastKnownVersion);
        }
    }
}
