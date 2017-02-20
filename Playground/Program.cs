using Axis.Jupiter.Europa;
using Axis.Luna.Extensions;
using Axis.Pollux.Authentication.OAModule;
using Axis.Pollux.Identity.OAModule;
using Axis.Pollux.Identity.Principal;
using Axis.Pollux.RBAC.OAModule;
using BitDiamond.Data.EF;
using System;
using System.Linq;

namespace Playground
{
    public class Program
    {
        static void Main(string[] args)
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
                europa.Store<User>().Query.ForAll((cnt, next) =>
                {
                    Console.WriteLine(next.EntityId);
                });
            });
        }
    }
}
