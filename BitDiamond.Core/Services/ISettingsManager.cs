using Axis.Luna;
using Axis.Pollux.RBAC.Auth;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services
{
    public interface ISettingsManager
    {
        [Resource(":system/settings/@update")]
        Operation ModifySetting(string settingName, string settingValue);


        [Resource(":system/settings/all/@get")]
        Operation<IEnumerable<SystemSetting>> GetSettings();


        [Resource(":system/settings/@get")]
        Operation<SystemSetting> GetSetting(string settingName);
    }
}
