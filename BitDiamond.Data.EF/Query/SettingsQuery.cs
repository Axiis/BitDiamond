using BitDiamond.Core.Services.Query;
using System.Collections.Generic;
using System.Linq;
using BitDiamond.Core.Models;
using Axis.Jupiter;
using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Data.EF.Query
{
    public class SettingsQuery: ISettingsQuery
    {
        private IDataContext _europa = null;

        public SettingsQuery(IDataContext context)
        {
            ThrowNullArguments(() => context);

            _europa = context;
        }

        public IEnumerable<SystemSetting> AllSettings()
        => _europa.Store<SystemSetting>().Query.ToArray();

        public SystemSetting GetSetting(string name)
        => _europa.Store<SystemSetting>().Query.FirstOrDefault(_ss => _ss.Name == name);
    }
}
