using Axis.Luna;
using Axis.Pollux.Identity.Principal;
using System;
using System.Linq.Expressions;

namespace BitDiamond.Core.Services
{
    public interface IBackgroundOperationScheduler
    {
        Operation<string> EnqueueOperation<Service>(Expression<Action<Service>> opInvocation, TimeSpan? delay = null);
        Operation RepeatOperation<Service>(string uniqueOpId, Expression<Action<Service>> opInvocation, ScheduleInterval interval);
        Operation<string> EnqueueOperation<Service>(IUserContext principal, Expression<Action<Service>> opInvocation, TimeSpan? delay = null);
        Operation RepeatOperation<Service>(string uniqueOpId, IUserContext principal, Expression<Action<Service>> opInvocation, ScheduleInterval interval);
    }

    public enum ScheduleInterval
    {
        Minutely,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
}
