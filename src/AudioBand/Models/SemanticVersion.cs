using System.Text.RegularExpressions;

namespace AudioBand.Models
{
    /// <summary>
    /// A class to represent Semantic Versioning.
    /// </summary>
    public class SemanticVersion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <param name="patch">The patch version.</param>
        public SemanticVersion(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <param name="version">Version in format v1.0.0 or 1.0.0.</param>
        public SemanticVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                ParseFailed = true;
                return;
            }

            var regex = Regex.Match(
                version,
                @"(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+(?<build>[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?");

            if (!regex.Success
                || !int.TryParse(regex.Groups["major"].Value, out var major)
                || !int.TryParse(regex.Groups["minor"].Value, out var minor)
                || !int.TryParse(regex.Groups["patch"].Value, out var patch))
            {
                ParseFailed = true;
                return;
            }

            Major = major;
            Minor = minor;
            Patch = patch;
            PreRelease = regex.Groups["prerelease"].Success ? regex.Groups["prerelease"].Value : null;
        }

        /// <summary>
        /// Gets the Major version.
        /// </summary>
        public int Major { get; private set; } = 0;

        /// <summary>
        /// Gets the Minor version.
        /// </summary>
        public int Minor { get; private set; } = 0;

        /// <summary>
        /// Gets the Patch version.
        /// </summary>
        public int Patch { get; private set; } = 0;

        /// <summary>
        /// Gets the optional pre-release identifier.
        /// </summary>
        public string PreRelease { get; private set; }

        /// <summary>
        /// Gets whether the parsing failed.
        /// </summary>
        public bool ParseFailed { get; private set; } = false;

        /// <summary>
        /// Compares whether one version is newer than another.
        /// </summary>
        /// <param name="other">The other version to compare against.</param>
        /// <returns>Whether or not the current version is latest.</returns>
        public bool IsNewerVersionThan(SemanticVersion other)
        {
            if (other == null || ParseFailed || other.ParseFailed)
            {
                return false;
            }

            if (Major != other.Major)
            {
                return Major > other.Major;
            }

            if (Minor != other.Minor)
            {
                return Minor > other.Minor;
            }

            if (Patch != other.Patch)
            {
                return Patch > other.Patch;
            }

            // Per SemVer 2.0.0 §11, a normal release has higher precedence than a pre-release version.
            return string.IsNullOrEmpty(PreRelease) && !string.IsNullOrEmpty(other.PreRelease);
        }
    }
}
