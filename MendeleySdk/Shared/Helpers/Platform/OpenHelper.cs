using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MendeleySdk.Shared.Helpers.Platform
{
    public class OpenHelper : IOpener
    {
        public void OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                StartWithOutputRedirectToNull("cmd", $"/c start {url}");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                StartWithOutputRedirectToNull("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                StartWithOutputRedirectToNull("open", url);
            }
        }

        private static void StartWithOutputRedirectToNull(string fileName, string args)
        {
            ProcessStartInfo psi = new()
                                   {
                                       FileName = fileName,
                                       Arguments = args,
                                       RedirectStandardOutput = true,
                                       RedirectStandardError = true
                                   };
            Process.Start(psi);
        }
    }
}