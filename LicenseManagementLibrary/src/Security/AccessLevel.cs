using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManagementLibrary.src.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AccessLevelAttribute : Attribute
    {
        public int RequiredLevel { get; }
        public AccessLevelAttribute(int requiredLevel) => RequiredLevel = requiredLevel;
    }
}
