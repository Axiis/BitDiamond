using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services.Query
{
    public interface ISettingsQuery
    {
        SystemSetting GetSetting(string name);

        IEnumerable<SystemSetting> AllSettings();
    }
}
