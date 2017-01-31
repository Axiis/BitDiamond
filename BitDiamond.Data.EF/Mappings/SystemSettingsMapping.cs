using Axis.Jupiter.Europa;
using BitDiamond.Core.Models;

namespace BitDiamond.Data.EF.Mappings
{
    public class SystemSettingsMapping: BaseModelMap<SystemSetting, long>
    {
        public SystemSettingsMapping()
        {
            this.Property(e => e.Data).IsMaxLength();
            this.Property(e => e.Name)
                .HasMaxLength(400)
                .IsIndex("SystemSettingName", true);
        }
    }
}
