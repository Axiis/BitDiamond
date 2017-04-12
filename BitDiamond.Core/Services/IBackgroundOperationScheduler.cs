using Axis.Luna;
using System;
using System.Linq.Expressions;

namespace BitDiamond.Core.Services
{
    public interface IBackgroundOperationScheduler
    {
        Operation EnqueueOperation(Expression<Action> opInvocation, TimeSpan? delay = null);
        Operation RepeatOperation(Expression<Action> opInvocation, ScheduleInterval interval);
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
