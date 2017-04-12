using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using System.Runtime.Remoting.Messaging;
using Axis.Luna.Extensions;
using Newtonsoft.Json;
using BitDiamond.Core.Utils;
using BitDiamond.Web.Infrastructure.Services.Hangfire;
using System;

using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Web.Infrastructure.Config.Hangfire
{
    public class Interceptor : JobFilterAttribute, IClientFilter, IServerFilter
    {
        public static readonly string CallContextParameters = "Bitdiamond.Hangfire.CallContextParams";
        public static readonly string DependencyResolverScope = "Bitdiamond.Hangfire.DependencyResolverScope";

        public void OnCreating(CreatingContext filterContext)
        {
            //extract contextual data
            var _userContext = CallContext
                .LogicalGetData(HangfireJobScheduler.CustomJobParameterKey)
                .As<SerializableUserContext>()
                .ThrowIfNull("invalid user context");

            //free up the slot in the call context param map
            CallContext.FreeNamedDataSlot(HangfireJobScheduler.CustomJobParameterKey);

            //serialise the context data
            var json = JsonConvert.SerializeObject(_userContext, Constants.Misc_DefaultJsonSerializerSettings);

            filterContext.SetJobParameter(CallContextParameters, json);
        }
        public void OnPerforming(PerformingContext filterContext)
        {
            //get the usercontext from the job's custom param and set it in the call context
            var json = filterContext.GetJobParameter<string>(HangfireJobScheduler.CustomJobParameterKey);
            var contextData = JsonConvert.DeserializeObject<SerializableUserContext>(json, Constants.Misc_DefaultJsonSerializerSettings);
            CallContext.LogicalSetData(CallContextParameters, contextData);
            
            //create a new resolution scope for use while invoking the job function
            CallContext.LogicalSetData(DependencyResolverScope, _scopeGenerator.Invoke());
        }
        public void OnPerformed(PerformedContext filterContext)
        {
            try
            {
                //dispose the resolution scope
                CallContext.LogicalGetData(DependencyResolverScope)
                           .As<IDisposable>()
                           .Dispose();
            }
            finally
            {
                //free the data slot in the call context
                CallContext.FreeNamedDataSlot(DependencyResolverScope);
            }
        }


        public void OnCreated(CreatedContext filterContext)
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