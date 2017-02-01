using Axis.Luna;
using System;
using System.ComponentModel.DataAnnotations;
using static Axis.Luna.Extensions.ObjectExtensions;

namespace BitDiamond.Core.Models
{
    public class SystemSetting : BaseModel<long>, IDataItem
    {
        public virtual string Data { get; set; }

        [MaxLength(400, ErrorMessage = "Name is too long")]
        public virtual string Name { get; set; }

        public virtual CommonDataType Type { get; set; }

        public override string ToString() => $"[{Name}: {this.DisplayData()}]";


        public SystemSetting()
        {
        }
    }
}
