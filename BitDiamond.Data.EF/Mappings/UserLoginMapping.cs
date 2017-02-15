using Axis.Jupiter.Europa.Mappings;
using BitDiamond.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Data.EF.Mappings
{
    public class UserLoginMapping: BaseModelMap<UserLogon, long>
    {
        public UserLoginMapping()
        {
            this.HasRequired(e => e.User)
                .WithMany();

            this.Property(e => e.OwinToken).IsRequired();
        }
    }

    public class UserAgentMapping: BaseComplexMap<UserAgent>
    {
        public UserAgentMapping()
        {
            //nothing to map
        }
    }
}
