using Axis.Luna.Extensions;
using Axis.Pollux.RBAC.Auth;
using BitDiamond.Core.Services;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace BitDiamond.Core
{
    public static class Extensions
    {
        private static ConcurrentDictionary<MethodInfo, IEnumerable<string>> CachedDescriptors = new ConcurrentDictionary<MethodInfo, IEnumerable<string>>();

        #region UserContext Extensions
        public static PermissionProfile CurrentProcessPermissionProfile(this IUserContext context)
        {

            var frame = new StackFrame(1);
            var resources = CachedDescriptors.GetOrAdd(frame.GetMethod().As<MethodInfo>(),
                                                       mi => mi.GetFeatureAttributes().SelectMany(ratt => ratt.Resources).ToList());
            return new PermissionProfile
            {
                Principal = context.CurrentUser(),
                ResourcePaths = resources
            };
        }



        private static IEnumerable<ResourceAttribute> GetFeatureAttributes(this MethodInfo method)
            => method.DeclaringType
                     .GetInterfaces()
                     .SelectMany(i => i.GetMethods())
                     .Where(m => method.HasSameSignature(m))
                     .Select(m => m.GetCustomAttribute<ResourceAttribute>())
                     .Where(ratt => ratt != null);

        private static bool HasSameSignature(this MethodInfo method1, MethodInfo method2)
            => method1.Name == method2.Name &&
               method1.ReturnType == method2.ReturnType &&
               method1.GetParameters()
                      .Select(_p => _p.ParameterType)
                      .SequenceEqual(method2.GetParameters().Select(_p => _p.ParameterType));
        #endregion
    }
}
