using BitDiamond.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Services.Query
{
    public interface ISettingsQuery
    {
        SystemSetting GetSetting(string name);
    }
}
