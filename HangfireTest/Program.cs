using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace HangfireTest
{
    public class Interceptor : JobFilterAttribute, IClientFilter, IServerFilter
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
    }

    class Program
    {
        static void Main(string[] args)
        {
            var cstring = ConfigurationManager.ConnectionStrings["Playground_Hangfire"];
            BuildDataBase(BuildConnectionString(cstring.ConnectionString));
            GlobalConfiguration.Configuration
                .UseFilter(new Interceptor())
                .UseSqlServerStorage(cstring.ConnectionString, new Hangfire.SqlServer.SqlServerStorageOptions
                {
                    SchemaName = "SteveBabu"
                });

            using (var server = new BackgroundJobServer())
            {
                var s = new SomeService();

                Console.WriteLine(BackgroundJob.Enqueue(() => s.DoSomething(new Hex().GetString(), DateTime.Now)));

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
