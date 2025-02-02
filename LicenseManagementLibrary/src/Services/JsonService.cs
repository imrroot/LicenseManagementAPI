using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LicenseManagementLibrary.src.Services
{
    internal class JsonService
    {
        public static async Task<string> SerializeDto(object data)
        {
            var json = JsonSerializer.Serialize(data);
            return json;
        }
    }
}
