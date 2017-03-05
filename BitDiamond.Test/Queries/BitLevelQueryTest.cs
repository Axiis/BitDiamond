using Axis.Jupiter.Europa;
using Axis.Pollux.Authentication.OAModule;
using Axis.Pollux.Identity.OAModule;
using Axis.Pollux.RBAC.OAModule;
using BitDiamond.Data.EF;
using BitDiamond.Data.EF.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Test.Queries
{
    [TestClass]
    public class BitLevelQueryTest
    {
        [TestMethod]
        public void TestMethod()
        {
            var config = new ContextConfiguration<EuropaContext>()
                .WithConnection("server=(local);database=BitDiamondDb;user id=sa;password=developer;Max Pool Size=1000;Min Pool Size=10;pooling=true;multipleactiveresultsets=True;")
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

            var cxt = new EuropaContext(config);

            var q = new BitLevelQuery(cxt, new ReferralQuery(cxt));

            var x = q.BaseBitLevelQuery().Where(_jo => _jo.Level.UserId == "dev.bankai@gmail.com");
            Console.WriteLine(x.ToArray().Length);
            var r = x.AsEnumerable();
            var u = r.Select(_r => _r.ToBitLevel());
            var uar = u.ToArray();
            Console.WriteLine(uar.Length);
        }
    }
}
