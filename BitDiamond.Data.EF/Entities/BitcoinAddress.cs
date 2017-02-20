using Axis.Jupiter;
using Axis.Luna;
using Axis.Pollux.Identity.Principal;
using System.ComponentModel.DataAnnotations;
using static Axis.Luna.Extensions.ObjectExtensions;

namespace BitDiamond.Data.EF.Entities
{
    public class BitcoinAddress : Core.Models.BitcoinAddress
    {
        public override User Owner
        {
            get { return base.Owner; }
            set
            {
                OwnerId = value?.EntityId;
                base.Owner = value;
            }
        }
        public string OwnerId { get; set; }

        #region Conversions
        public static Core.Models.BitcoinAddress ToModel(BitcoinAddress entity, Core.Models.BitcoinAddress model, DomainConverter converter)
        {
            return model;
        }
        public static BitcoinAddress ToEntity(Core.Models.BitcoinAddress model, BitcoinAddress entity, DomainConverter converter)
        {
            entity.CreatedOn = model.CreatedOn;
            entity.Id = model.Id;
            entity.ModifiedOn = model.ModifiedOn;
            entity.OwnerId = model.Owner?.EntityId;

            return entity;
        }
        #endregion
    }
}
