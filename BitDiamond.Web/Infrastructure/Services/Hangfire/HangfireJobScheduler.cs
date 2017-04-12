using Axis.Luna;
using BitDiamond.Core.Services;
using Hangfire;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;

namespace BitDiamond.Web.Infrastructure.Services.Hangfire
{

    public class HangfireJobScheduler : IBackgroundOperationScheduler
    {
        public static readonly string CustomJobParameterKey = "BitDiamond.Config.Hangfire.CustomParam";

        private IUserContext _userContext;

        public HangfireJobScheduler(IUserContext _userContext)
        {
            this._userContext = _userContext;
        }

        public Operation EnqueueOperation(Expression<Action> opInvocation, TimeSpan? delay = default(TimeSpan?)) => Operation.Try(() =>
        {
            //Cache all necessary contextual data
            CallContext.LogicalSetData(CustomJobParameterKey, new SerializableUserContext
            {
                _currentUserRoles = _userContext.CurrentUserRoles().ToList(),
                _currentUserLogon = _userContext.CurrentUserLogon(),
                _currentUser = _userContext.CurrentUser()
            });

            if (delay == null) BackgroundJob.Enqueue(opInvocation);
            else BackgroundJob.Schedule(opInvocation, delay.Value);
        });

        public Operation RepeatOperation(Expression<Action> opInvocation, ScheduleInterval interval) => Operation.Try(() =>
        {
            switch (interval)
            {
                case ScheduleInterval.Daily: RecurringJob.AddOrUpdate(opInvocation, Cron.Daily()); break;
                case ScheduleInterval.Hourly: RecurringJob.AddOrUpdate(opInvocation, Cron.Hourly()); break;
                case ScheduleInterval.Minutely: RecurringJob.AddOrUpdate(opInvocation, Cron.Minutely()); break;
                case ScheduleInterval.Monthly: RecurringJob.AddOrUpdate(opInvocation, Cron.Monthly()); break;
                case ScheduleInterval.Weekly: RecurringJob.AddOrUpdate(opInvocation, Cron.Weekly()); break;
                case ScheduleInterval.Yearly: RecurringJob.AddOrUpdate(opInvocation, Cron.Yearly()); break;
                default: throw new Exception("unknown interval");
            }
        });
    }
}