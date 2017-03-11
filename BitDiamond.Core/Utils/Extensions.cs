using Axis.Luna;
using Axis.Luna.Extensions;
using Axis.Pollux.Identity.Principal;
using Axis.Pollux.RBAC.Auth;
using BitDiamond.Core.Services;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BitDiamond.Core
{
    public static class Extensions
    {
        private static ConcurrentDictionary<MethodInfo, IEnumerable<string>> CachedDescriptors = new ConcurrentDictionary<MethodInfo, IEnumerable<string>>();
        private static WeakCache Cache = new WeakCache();

        #region UserContext Extensions
        public static PermissionProfile CurrentProcessPermissionProfile(this IUserContext context)
        {

            var frame = new StackFrame(1);
            var resources = CachedDescriptors.GetOrAdd(frame.GetMethod().As<MethodInfo>(),
                                                       mi => mi.GetResourceAttribute().SelectMany(ratt => ratt.Resources).ToList());
            return new PermissionProfile
            {
                Principal = context.CurrentUser(),
                ResourcePaths = resources
            };
        }

        public static PermissionProfile CurrentPPP(this IUserContext context)
        {
            var frame = new StackFrame(1);
            var resources = CachedDescriptors.GetOrAdd(frame.GetMethod().As<MethodInfo>(),
                                                       mi => mi.GetResourceAttribute().SelectMany(ratt => ratt.Resources).ToList());
            return new PermissionProfile
            {
                Principal = context.CurrentUser(),
                ResourcePaths = resources
            };
        }

        /// <summary>
        /// This method is faster than the <c>CurrentProcessPermissionProfile</c> because it doesnt rely on the StackTrace method anymore. The downside of this
        /// is that the method it needs to get the resource from on the service specified cannot be an overloaded method.
        /// </summary>
        /// <typeparam name="Service">The service type whose method has been decorated with the <c>Axis.Pollux.RBAC.Auth.ResourceAttribute</c></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static PermissionProfile PermissionProfile<Service>(this Service service, User user, [CallerMemberName] string methodName = null)
        {
            var serviceType = typeof(Service);
            var resources = Cache.GetOrAdd($"{serviceType.MinimalAQName()}.{methodName}", _k =>
            {
                var mi = serviceType.GetMethod(methodName);
                return mi.GetResourceAttribute().SelectMany(ratt => ratt.Resources).ToList();
            });

            return new PermissionProfile
            {
                Principal = user,
                ResourcePaths = resources
            };
        }



        private static IEnumerable<ResourceAttribute> GetResourceAttribute(this MethodInfo method)
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

        public static string ResolveParameters(this string parametarized, params object[] @params) => string.Format(parametarized, @params);
    }
}
