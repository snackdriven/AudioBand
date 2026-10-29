using System;
using System.IO;
using AudioBand.Settings.Persistence;
using Newtonsoft.Json;
using Xunit;

namespace AudioBand.Test
{
    [CollectionDefinition("JSON defaults", DisableParallelization = true)]
    public class JsonDefaultsCollection
    {
    }

    [Collection("JSON defaults")]
    public class ProfileJsonSafetyTests
    {
        [Theory]
        [InlineData("{\"Name\":\"Local fixture\"}")]
        [InlineData("{\"$type\":\"Untrusted.Type, Missing.Assembly\",\"Name\":\"Local fixture\"}")]
        public void ReadProfile_IgnoresTypeMetadataEvenWithPolymorphicGlobalDefaults(string json)
        {
            var previousDefaults = JsonConvert.DefaultSettings;
            var path = Path.GetTempFileName();
            try
            {
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                File.WriteAllText(path, json);
                var profile = new PersistentSettings().ReadProfile(path);

                Assert.NotNull(profile);
                Assert.Equal("Local fixture", profile.Name);
            }
            finally
            {
                JsonConvert.DefaultSettings = previousDefaults;
                File.Delete(path);
            }
        }
    }
}
