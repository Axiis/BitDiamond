using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axis.Jupiter.Europa;
using Axis.Pollux.Identity.OAModule;
using Axis.Pollux.Authentication.OAModule;
using Axis.Pollux.RBAC.OAModule;
using BitDiamond.Data.EF;
using Axis.Luna.Extensions;
using Axis.Pollux.Identity.Principal;
using Axis.Pollux.RBAC.Auth;
using BitDiamond.Web.Infrastructure.Utils;
using BitDiamond.Core.Models;
using System.Linq;
using BitDiamond.Core.Utils;

namespace BitDiamond.Test
{
    [TestClass]
    public class DbMigrationTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            try
            {
                var config = new ContextConfiguration<EuropaContext>()
                    .WithConnection("server=(local);database=BitDiamondDb_Test;user id=sa;password=developer;Max Pool Size=1000;Min Pool Size=10;pooling=true;multipleactiveresultsets=True;")
                    .WithEFConfiguraton(_efc =>
                    {
                        _efc.LazyLoadingEnabled = false;
                        _efc.ProxyCreationEnabled = false;
                    })
                    .WithInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<EuropaContext>())
                    .UsingModule(new IdentityAccessModuleConfig())
                    .UsingModule(new AuthenticationAccessModuleConfig())
                    .UsingModule(new RBACAccessModuleConfig())
                    .UsingModule(new BitDiamondModuleConfig());

                new EuropaContext(config).Using(europa =>
                {
                    var ss = europa.Store<SystemSetting>().Query.FirstOrDefault(_ss => _ss.Name == Constants.Settings_MaxBitLevel);
                    Console.WriteLine(ss?.Data ?? "null");
                });
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

        }

        [TestMethod]
        public void TestMethod2()
        {
            var role = new Role { RoleName = "Something" };
            var role2 = new Role { RoleName = "Something" };

            Console.WriteLine(role == role2);
            Console.WriteLine(role.Equals(role2));
        }
    }
}
