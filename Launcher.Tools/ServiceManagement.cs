using Launcher.Consts;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Launcher.Tools
{
    public class PathManagement
    {
        [SupportedOSPlatform("Windows")]
        public static string? GetSmartMixPath()
        {
            string ServiceName = ApplicationNames.Service;

            using (ManagementObject wmiService = new ManagementObject("Win32_Service.Name='" + ServiceName + "'"))
            {
                wmiService.Get();
                return wmiService["PathName"]
                    .ToString()?
                    .Replace("\"", "")
                    .Replace(ApplicationNames.Server, "") ?? throw new ArgumentNullException("Путь до SmartMix не найден");
            }
        }

        public static string CombinePath(string applicationsPath, string executionName)
            => Path.Combine(applicationsPath, executionName);

    }
}