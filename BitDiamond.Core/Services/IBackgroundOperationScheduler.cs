using Axis.Luna;
using Axis.Pollux.Identity.Principal;
using System;
using System.Linq.Expressions;

namespace BitDiamond.Core.Services
{
    public interface IBackgroundOperationScheduler
    {
        Operation<string> EnqueueOperation(Expression<Action> opInvocation, TimeSpan? delay = null);
        Operation RepeatOperation(string uniqueOpId, Expression<Action> opInvocation, ScheduleInterval interval);
        Operation<string> EnqueueOperation(IUserContext principal, Expression<Action> opInvocation, TimeSpan? delay = null);
        Operation RepeatOperation(string uniqueOpId, IUserContext principal, Expression<Action> opInvocation, ScheduleInterval interval);
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
