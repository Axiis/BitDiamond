using Axis.Luna.Extensions;
using System.Configuration;
using System.Data.SqlClient;

namespace BitDiamond.Web.Infrastructure.Config.Hangfire
{
    public static class DBInitializer
    {
        public static string InitDb(string connectionStringName) => connectionStringName.UsingValue(_csn =>
        {
            var cstring = ConfigurationManager.ConnectionStrings[_csn];
            var builder = new SqlConnectionStringBuilder(cstring.ConnectionString);
            var originalCatalogue = builder.InitialCatalog;
            builder.InitialCatalog = "master";

            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                var stmt = connection.CreateCommand();
                stmt.CommandText = @"
IF (DB_ID('" + originalCatalogue + @"') is null)
BEGIN
CREATE DATABASE [" + originalCatalogue + @"];
END
";
                stmt.ExecuteNonQuery();
            }
        });
    }
}