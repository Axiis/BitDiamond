using Axis.Jupiter.Europa.Module;
using System;

namespace BitDiamond.Data.EF
{
    public class EuropaConfig : BaseModuleConfigProvider
    {
        public override string ModuleName => "BitDiamond.Data.EF.OAModule";

        protected override void Initialize()
        {
            //load all mappings

            //add all seed data
        }
    }
}
