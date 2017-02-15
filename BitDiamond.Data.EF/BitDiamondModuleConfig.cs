using Axis.Jupiter.Europa;
using Axis.Jupiter.Europa.Module;
using Axis.Luna.Extensions;
using BitDiamond.Core.Models;
using BitDiamond.Core.Utils;
using System;
using System.Linq;

namespace BitDiamond.Data.EF
{
    public class BitDiamondModuleConfig : BaseModuleConfigProvider
    {
        public override string ModuleName => "BitDiamond.Data.EF.OAModule";

        protected override void Initialize()
        {
            //load all mappings
            var asm = GetType().Assembly;
            asm.GetTypes()
               .Where(t => t.IsEntityMap())
               .ForAll((cnt, t) => this.UsingConfiguration(Activator.CreateInstance(t).AsDynamic()));

            ///Seed Data
            #region 1. Settings data
            UsingContext(cxt =>
            {
                if (cxt.Store<SystemSetting>().Query.Any()) return;

                new SystemSetting[]
                {
                    new SystemSetting
                    {
                        Name = Constants.Settings_DefaultContextVerificationExpirationTime,
                        Data = TimeSpan.FromDays(2).ToString(),
                        Type = Axis.Luna.CommonDataType.TimeSpan
                    },
                    new SystemSetting
                    {
                        Name = Constants.Settings_DefaultPasswordExpirationTime,
                        Data = TimeSpan.FromDays(730).ToString(), //two years
                        Type = Axis.Luna.CommonDataType.TimeSpan
                    },
                    new SystemSetting
                    {
                        Name = Constants.Settings_MaxBitLevel,
                        Data = "3",
                        Type = Axis.Luna.CommonDataType.Integer
                    },
                    new SystemSetting
                    {
                        Name = Constants.Settings_UpgradeCostVector,
                        Data = "[0.11, 0.1826, 0.3652]",
                        Type = Axis.Luna.CommonDataType.JsonObject
                    }
                }
                .ForAll((cnt, next) => cxt.Add(next));

                cxt.CommitChanges();
            });
            #endregion

            #region 2. Roles
            #endregion

            #region 3. Resources
            #endregion

            #region 4. Permissions
            #endregion

            #region 5. Default users (root and guest)
            //dont forget to assign roles the the users
            #endregion

            #region 6. Apex User
            //dont forget to assign roles to this user
            #endregion

        }
    }
}
