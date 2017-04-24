using Axis.Luna;
using Axis.Luna.Extensions;
using BitDiamond.Core.Services;
using Hangfire;
using Hangfire.Storage;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Expressions;

using static Axis.Luna.Extensions.EnumerableExtensions;

namespace BitDiamond.Web.Infrastructure.Services.Hangfire
{

    public class HangfireJobScheduler : IBackgroundOperationScheduler
    {
        public static readonly string EnqueuedJobKeyPrefix = "BitDiamond.Enqueued";
        public static readonly string RecurrentJobKeyPrefix = "BitDiamond.Recurrent";

        public static readonly string CustomJobProperty_UserContext = "UserContext";


        private IUserContext _userContext;
        private IStorageConnection _connection;

        public HangfireJobScheduler(IUserContext userContext, IStorageConnection connection)
        {
            _userContext = userContext;
            _connection = connection;
        }

        public Operation<string> EnqueueOperation<Service>(Expression<Action<Service>> opInvocation, TimeSpan? delay = default(TimeSpan?)) => EnqueueOperation(_userContext, opInvocation, delay);

        public Operation<string> EnqueueOperation<Service>(IUserContext principal, Expression<Action<Service>> opInvocation, TimeSpan? delay = default(TimeSpan?)) => Operation.Try(() =>
        {
            //Cache all necessary contextual data
            var json = JsonConvert.SerializeObject(new SerializableUserContext
            {
                _currentUserRoles = principal.CurrentUserRoles().ToList(),
                _currentUserLogon = principal.CurrentUserLogon(),
                _currentUser = principal.CurrentUser()
            });

            string jid = null;
            if (delay == null)
                jid = BackgroundJob.Enqueue(opInvocation);
            else
                jid = BackgroundJob.Schedule(opInvocation, delay.Value);

            //persist the user context
            _connection.SetJobParameter(jid, CustomJobProperty_UserContext, json);

            return jid;
        });

        public Operation RepeatOperation<Service>(string uniqueOpId, Expression<Action<Service>> opInvocation, ScheduleInterval interval) => RepeatOperation(uniqueOpId, _userContext, opInvocation, interval);

        public Operation RepeatOperation<Service>(string uniqueOpId, IUserContext principal, Expression<Action<Service>> opInvocation, ScheduleInterval interval) => Operation.Try(() =>
        {
            //Cache all necessary contextual data
            var json = JsonConvert.SerializeObject(new SerializableUserContext
            {
                _currentUserRoles = principal.CurrentUserRoles().ToList(),
                _currentUserLogon = principal.CurrentUserLogon(),
                _currentUser = principal.CurrentUser()
            });

            //persist the user context
            _connection.SetRangeInHash($"{RecurrentJobKeyPrefix}::{uniqueOpId}", Enumerate(CustomJobProperty_UserContext.ValuePair(json)));

            switch (interval)
            {
                case ScheduleInterval.Daily: RecurringJob.AddOrUpdate(uniqueOpId, opInvocation, Cron.Daily()); break;
                case ScheduleInterval.Hourly: RecurringJob.AddOrUpdate(uniqueOpId, opInvocation, Cron.Hourly()); break;
                case ScheduleInterval.Minutely: RecurringJob.AddOrUpdate(uniqueOpId, opInvocation, Cron.Minutely()); break;
                case ScheduleInterval.Monthly: RecurringJob.AddOrUpdate(uniqueOpId, opInvocation, Cron.Monthly()); break;
                case ScheduleInterval.Weekly: RecurringJob.AddOrUpdate(uniqueOpId, opInvocation, Cron.Weekly()); break;
                case ScheduleInterval.Yearly: RecurringJob.AddOrUpdate(uniqueOpId, opInvocation, Cron.Yearly()); break;
                default: throw new Exception("unknown interval");
            }
        });
    }
}