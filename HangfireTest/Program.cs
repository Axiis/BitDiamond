using Axis.Luna.Extensions;
using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.SqlServer;
using Hangfire.States;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using static Axis.Luna.Extensions.EnumerableExtensions;

namespace HangfireTest
{
    public class Interceptor : JobFilterAttribute, IClientFilter, IServerFilter, IElectStateFilter
    {
        public void OnCreated(CreatedContext filterContext)
        {
            filterContext.Connection.SetJobParameter(filterContext.BackgroundJob.Id, "xyz", "other stuff here");
            //filterContext.BackgroundJob.
        }

        public void OnCreating(CreatingContext filterContext)
        {
        }

        public void OnPerformed(PerformedContext filterContext)
        {
        }

        public void OnPerforming(PerformingContext filterContext)
        {
        }

        public void OnStateElection(ElectStateContext context)
        {
            //context.CandidateState = new FailedState(new Exception());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var cstring = ConfigurationManager.ConnectionStrings["Playground_Hangfire"];
            BuildDataBase(BuildConnectionString(cstring.ConnectionString));
            GlobalConfiguration.Configuration
                .UseFilter(new Interceptor())
                .UseSqlServerStorage(cstring.ConnectionString, new Hangfire.SqlServer.SqlServerStorageOptions());

            var storage = new SqlServerStorage("Playground_Hangfire");

            using (var server = new BackgroundJobServer())
            {
                //var s = new SomeService();
                //string jid = null;
                //Console.WriteLine(jid = BackgroundJob.Enqueue(() => s.DoSomething(new Hex().GetString(), DateTime.Now)));

                //jid = "__$__";
                //storage.GetConnection().SetRangeInHash(jid, Enumerate("something".ValuePair("another value")));
                //RecurringJob.AddOrUpdate(jid, () => s.DoSomething(new Hex().GetString(), DateTime.Now), Cron.Minutely());

                //storage.GetConnection().SetRangeInHash(jid, new KeyValuePair<string, string>[0]);

                Console.ReadKey();
            }
        }

        static string BuildConnectionString(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            builder.InitialCatalog = "master";
            return builder.ConnectionString;
        }

        static void BuildDataBase(string cstring)
        {
            using (var connection = new SqlConnection(cstring))
            {
                connection.Open();

                var stmt = connection.CreateCommand();
                stmt.CommandText = @"
IF (DB_ID('Playground_HangfireDB') is null)
BEGIN
CREATE DATABASE [Playground_HangfireDB];
END
";
                stmt.ExecuteNonQuery();
            }
        }
    }

    public class SomeService
    {
        public SomeService()
        {

        }

        public void DoSomething(string param, DateTime otherParam)
        {
            Console.WriteLine($"something done: {param}, {otherParam}");
        }
    }

    public class Hex
    {
        public Hex()
        {
        }

        public string GetString() => GetHashCode().ToString();
    }
}
