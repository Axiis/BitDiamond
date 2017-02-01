using Axis.Luna;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services
{
    public interface ISettingsManager
    {
        
        Operation ModifySetting(string settingName, string settingValue);


        Operation<IEnumerable<SystemSetting>> GetSettings();


        Operation<SystemSetting> GetSetting(string settingName);
    }
}
