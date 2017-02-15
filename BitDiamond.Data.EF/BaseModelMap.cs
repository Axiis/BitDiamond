using Axis.Jupiter.Europa.Mappings;
using BitDiamond.Core.Models;

namespace BitDiamond.Data.EF
{
    public class BaseModelMap<Entity, Key> : BaseMap<Entity>
    where Entity : BaseModel<Key>
    {
        protected BaseModelMap() : this(true)
        { }

        protected BaseModelMap(bool useDefaultTable) : base(useDefaultTable)
        {
            //configure the primary key
            this.HasKey(e => e.Id);
        }
    }
};
