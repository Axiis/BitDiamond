using Axis.Luna.Extensions;
using BitDiamond.Core.Models.Email;
using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.SqlServer;
using Hangfire.States;
using Newtonsoft.Json;
using SimpleInjector;
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
            var c = new Container();
            c.Register<ISomeService, SomeService>();

            var cstring = ConfigurationManager.ConnectionStrings["HangfireDb"];
            BuildDataBase(BuildConnectionString(cstring.ConnectionString));
            GlobalConfiguration.Configuration
                .UseFilter(new Interceptor())
                .UseSqlServerStorage(cstring.ConnectionString, new Hangfire.SqlServer.SqlServerStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromMinutes(1)
                })
                .UseActivator(new SimpleInjectorJobActivator(c))
                .UseLog4NetLogProvider();

            //using (var server = new BackgroundJobServer())
            //{
            //    string jid = null;
            //    Console.WriteLine(jid = BackgroundJob.Enqueue<ISomeService>(s => s.DoSomething(new Hex().GetString(), DateTime.Now)));

            //    //jid = "__$__";
            //    //storage.GetConnection().SetRangeInHash(jid, Enumerate("something".ValuePair("another value")));
            //    //RecurringJob.AddOrUpdate(jid, () => s.DoSomething(new Hex().GetString(), DateTime.Now), Cron.Minutely());

            //    //storage.GetConnection().SetRangeInHash(jid, new KeyValuePair<string, string>[0]);

            //    Console.ReadKey();
            //}

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            var mailModel = new UserWelcome
            {
                Link = "http://abc.xyz",
                From = "nobody@abc.com",
                LogoTextUrl = "stuff here",
                LogoUrl = "http://ble.com",
                Subject = "stuff",
                Target = "target@bullseye.crosshairs"
            };
            var json = JsonConvert.SerializeObject(mailModel);

            var obj = JsonConvert.DeserializeObject(json);
            var xobj = JsonConvert.DeserializeObject<MailModel>(json);
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

    public interface ISomeService
    {
        void DoSomething(string x, DateTime y);
    }

    public class SomeService: ISomeService
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





    public class SimpleInjectorJobActivator : JobActivator
    {
        private readonly Container _container;
        private readonly Lifestyle _lifestyle;

        public SimpleInjectorJobActivator(Container container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            _container = container;
        }

        public SimpleInjectorJobActivator(Container container, Lifestyle lifestyle)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            if (lifestyle == null)
            {
                throw new ArgumentNullException("lifestyle");
            }

            _container = container;
            _lifestyle = lifestyle;
        }

        public override object ActivateJob(Type jobType)
        {
            return _container.GetInstance(jobType);
        }

        public override JobActivatorScope BeginScope(JobActivatorContext context)
        {
            if (_lifestyle == null || _lifestyle != Lifestyle.Scoped)
            {
                return new SimpleInjectorScope(_container, SimpleInjector.Lifestyles.AsyncScopedLifestyle.BeginScope(_container));
            }
            return new SimpleInjectorScope(_container, new SimpleInjector.Lifestyles.AsyncScopedLifestyle().GetCurrentScope(_container));
        }
    }

    public class SimpleInjectorScope : JobActivatorScope
    {
        private readonly Container _container;
        private readonly Scope _scope;

        public SimpleInjectorScope(Container container, Scope scope)
        {
            _container = container;
            _scope = scope;
        }

        public override object Resolve(Type type)
        {
            return _container.GetInstance(type);
        }

        public override void DisposeScope()
        {
            if (_scope != null)
            {
                _scope.Dispose();
            }
        }
    }
}
