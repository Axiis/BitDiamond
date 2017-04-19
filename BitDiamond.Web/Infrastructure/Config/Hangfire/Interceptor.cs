using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using System.Runtime.Remoting.Messaging;
using Axis.Luna.Extensions;
using Newtonsoft.Json;
using BitDiamond.Web.Infrastructure.Services.Hangfire;
using System;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Luna;
using System.Linq;

namespace BitDiamond.Web.Infrastructure.Config.Hangfire
{
    public class Interceptor : JobFilterAttribute, IClientFilter, IServerFilter
    {
        public static readonly string CallContextParameters_UserContext = "Bitdiamond.Hangfire.CallContextParams.UserContext";

        public static readonly string DependencyResolverScope = "Bitdiamond.Hangfire.DependencyResolverScope";


        public void OnPerforming(PerformingContext filterContext)
        {
            //for recurring jobs
            var uniqueOpId = filterContext.GetJobParameter<string>("RecurringJobId");
            if (!string.IsNullOrWhiteSpace(uniqueOpId))
            {
                filterContext.Connection
                    .GetAllEntriesFromHash($"{HangfireJobScheduler.RecurrentJobKeyPrefix}::{uniqueOpId}")
                    .Where(_kvp => _kvp.Key == HangfireJobScheduler.CustomJobProperty_UserContext)
                    .Select(_kvp => _kvp.Value)
                    .FirstOrDefault()
                    .Do(_v => CallContext.LogicalSetData(CallContextParameters_UserContext, JsonConvert.DeserializeObject<SerializableUserContext>(_v)));
            }

            //create a new resolution scope for use while invoking the job function
            CallContext.LogicalSetData(DependencyResolverScope, _scopeGenerator.Invoke());
        }
        public void OnPerformed(PerformedContext filterContext)
        {
            try
            {
                //if we are dealing with an operation, resolve it so its failures, if any, are "visible" to hangfire
                if (filterContext.Result?.GetType().HasGenericAncestor(typeof(Operation<>)) == true)
                    filterContext.Result.AsDynamic().Resolve();

                //dispose the resolution scope
                CallContext.LogicalGetData(DependencyResolverScope)
                           .As<IDisposable>()
                           .Dispose();
            }
            finally
            {
                //free the data slot in the call context
                CallContext.FreeNamedDataSlot(DependencyResolverScope);
                CallContext.FreeNamedDataSlot(CallContextParameters_UserContext);
            }
        }


        public void OnCreated(CreatedContext filterContext)
        {
        }
        public void OnCreating(CreatingContext filterContext)
        {
        }


        private Func<IDisposable> _scopeGenerator;
        public Interceptor(Func<IDisposable> dependencyScopeGenerator)
        {
            ThrowNullArguments(() => dependencyScopeGenerator);

            _scopeGenerator = dependencyScopeGenerator;
        }
    }
}