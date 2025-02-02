using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManagementLibrary.src.Services
{
    public class SystemService
    {
        public static string GetHWID()
        {
            var mc = new ManagementClass("win32_processor");
            var moc = mc.GetInstances();
            foreach (var mo in moc)
            {
                return mo.Properties["processorID"].Value.ToString();
            }
            return "";
        }

        public static string GetIP()
        {
            using var client = new WebClient();
            try { return client.DownloadString("http://api.ipify.org"); }
            catch { return ""; }
        }
    }
}
