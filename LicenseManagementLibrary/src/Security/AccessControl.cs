using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LicenseManagementLibrary.src.Core;

namespace LicenseManagementLibrary.src.Security
{
    public static class AccessControl
    {
        public static bool CanAccess(object target, LicenseManager licenseManager)
        {
            // Check if class has access level attribute
            var classAttribute = target.GetType().GetCustomAttribute<AccessLevelAttribute>();
            if (classAttribute != null && !licenseManager.HasAccess(classAttribute.RequiredLevel))
            {
                return false;
            }
            return true;
        }

        public static bool CanAccess(MethodInfo method, LicenseManager licenseManager)
        {
            var attribute = method.GetCustomAttribute<AccessLevelAttribute>();
            if (attribute == null) return true;
            return licenseManager.HasAccess(attribute.RequiredLevel);
        }

        public static void EnsureAccess(object target, LicenseManager licenseManager)
        {
            if (!CanAccess(target, licenseManager))
            {
                throw new UnauthorizedAccessException($"Access Denied: Requires level {target.GetType().GetCustomAttribute<AccessLevelAttribute>()?.RequiredLevel}, but user has {licenseManager.Subscription.AccessLevel}");
            }
        }
        public static void EnsureMethodAccess(object target, string methodName, LicenseManager licenseManager)
        {
            var method = target.GetType().GetMethod(methodName);
            if (method == null)
            {
                throw new ArgumentException($"Method '{methodName}' not found on {target.GetType().Name}");
            }

            if (!CanAccess(method, licenseManager))
            {
                throw new UnauthorizedAccessException($"Access Denied: Requires level {method.GetCustomAttribute<AccessLevelAttribute>()?.RequiredLevel}, but user has {licenseManager.Subscription.AccessLevel}");
            }
        }
        public static void EnsureMethodAccess(object target, LicenseManager licenseManager, [CallerMemberName] string methodName = "")
        {
            var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method == null)
            {
                throw new ArgumentException($"Method '{methodName}' not found on {target.GetType().Name}");
            }

            if (!CanAccess(method, licenseManager))
            {
                throw new UnauthorizedAccessException($"Access Denied: Requires level {method.GetCustomAttribute<AccessLevelAttribute>()?.RequiredLevel}, but user has {licenseManager.Subscription.AccessLevel}");
            }
        }
    }
}
