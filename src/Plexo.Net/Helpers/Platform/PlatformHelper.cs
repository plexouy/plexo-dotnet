using System.Runtime.InteropServices;

namespace Plexo.Client.SDK.Helpers.OperatingSystem
{
    public static class PlatformHelper
    {
        public static bool IsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsMacOS() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static bool IsDotNetDocker() =>
            string.Equals(System.Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", System.StringComparison.OrdinalIgnoreCase);
    }
}