using Axis.Pollux.Identity.Principal;
using Reinforced.Typings.Fluent;
using System.Linq;

using Axis.Luna.Extensions;
using BitDiamond.Core.Models;

namespace BitDiamond.Web.Infrastructure.Config
{
    /// <summary>
    /// This class is called by the ReinforcedTypings build extension everytime the project is built.
    /// </summary>
    public static class ReinforcedTypings
    {
        public static void Configure(ConfigurationBuilder config)
        {
            #region  pollux models

            var polluxBase = typeof(PolluxEntity<>);
            polluxBase
                .Assembly
                .GetTypes()
                .Where(_t => _t.HasGenericAncestor(polluxBase))
                .Do(_tarr => config.ExportAsInterfaces(_tarr, _icb =>
                {
                    _icb.WithPublicProperties()
                        .OverrideNamespace("Pollux.Models");
                }));

            polluxBase
                .Assembly
                .GetTypes()
                .Where(_t => _t.IsEnum)
                .Do(_tarr => config.ExportAsInterfaces(_tarr, _icb =>
                {
                }));
            #endregion


            #region BitDiamond models

            var bitDiamondBase = typeof(BaseModel<>);
            bitDiamondBase
                .Assembly
                .GetTypes()
                .Where(_t => _t.HasGenericAncestor(bitDiamondBase))
                .Do(_tarr => config.ExportAsInterfaces(_tarr, _icb =>
                {
                    _icb.WithPublicProperties()
                        .OverrideNamespace("BitDiamond.Models");
                }));

            bitDiamondBase = typeof(BaseModel<>);
            bitDiamondBase
                .Assembly
                .GetTypes()
                .Where(_t => _t.IsEnum)
                .Do(_tarr => config.ExportAsInterfaces(_tarr, _icb =>
                {
                }));
            #endregion
        }
    }
}