using System.Collections.Generic;
using Axis.Luna;
using static Axis.Luna.Extensions.ExceptionExtensions;

using BitDiamond.Core.Models;
using BitDiamond.Core.Services.Query;
using Axis.Pollux.RBAC.Services;
using BitDiamond.Core.Services.Command;

namespace BitDiamond.Core.Services
{
    public class SettingsManager : ISettingsManager, IUserContextAware
    {
        private ISettingsQuery _query = null;
        private IUserAuthorization _auth = null;
        private IPersistenceCommands _pcommand;

        public IUserContext UserContext { get; private set; }
        

        public SettingsManager(IUserContext userContext, 
                               ISettingsQuery query,
                               IUserAuthorization userAuthorization,
                               IPersistenceCommands persistenceCommands)
        {
            ThrowNullArguments(() => query,
                               () => userContext,
                               () => userAuthorization,
                               () => persistenceCommands);

            _query = query;
            _auth = userAuthorization;
            _pcommand = persistenceCommands;
            UserContext = userContext;
        }


        public Operation<SystemSetting> GetSetting(string settingName)
        => _auth.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _query.GetSetting(settingName);
        });

        public Operation<IEnumerable<SystemSetting>> GetSettings()
        => _auth.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _query.AllSettings();
        });

        public Operation ModifySetting(string settingName, string settingValue)
        => _auth.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var setting = _query.GetSetting(settingName).ThrowIfNull($"Setting '{settingName}' does not exist");
            setting.Data = settingValue;

            _pcommand.Update(setting).Resolve();
        });
    }
}
