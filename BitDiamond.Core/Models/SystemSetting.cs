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

        public override string ToString() => $"[{Name}: {DisplayData()}]";

        private string DisplayData()
        {
            switch (Type)
            {
                case CommonDataType.Boolean:
                case CommonDataType.Real:
                case CommonDataType.Integer:
                case CommonDataType.String:
                case CommonDataType.Url:
                case CommonDataType.TimeSpan:
                case CommonDataType.Email:
                case CommonDataType.Location:
                case CommonDataType.Phone:
                case CommonDataType.IPV4:
                case CommonDataType.IPV6:
                case CommonDataType.JsonObject: return Data;
                case CommonDataType.DateTime: return Eval(() => DateTime.Parse(Data).ToString(), ex => "");

                case CommonDataType.Binary: return "Binary-Data";

                case CommonDataType.UnknownType:
                default: return "Unknown-Type";
            }
        }


        public SystemSetting()
        {
        }
    }
}
