using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BitDiamond.Web.Infrastructure.Utils;
using BitDiamond.Data.EF;
using Axis.Pollux.RBAC.OAModule;
using Axis.Pollux.Authentication.OAModule;
using Axis.Pollux.Identity.OAModule;
using Axis.Jupiter.Europa;
using BitDiamond.Data.EF.Query;
using System.Linq;
using BitDiamond.Core.Models;

namespace BitDiamond.Test.Queries
{
    [TestClass]
    public class BlockChainQueryTests
    {
        [TestMethod]
        public void TestMethod1()
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

            var bcQuery = new BlockChainQuery(cxt);

            var r = bcQuery.GetPagedOutgoingUserTransactions(new Axis.Pollux.Identity.Principal.User { EntityId = "dev.bankai@gmail.com" }, 10, 0);

            r.Page.ToList().ForEach(_x =>
            {
                Console.WriteLine($"{_x.Amount}, {_x.Receiver}, {_x.Sender}, {_x.Status}, {_x.CreatedOn}");
            });
        }
    }
}
