using System.ComponentModel.DataAnnotations;
using Axis.Luna;
using System;

namespace BitDiamond.Core.Domain
{
    public abstract class BaseModel<IdType>
    {
        public IdType Id { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        


        public Operation Validate()
            => Operation.Try(() => Validator.ValidateObject(this, new ValidationContext(this, null, null)));


        public BaseModel()
        {
            CreatedOn = DateTime.Now;
        }
    }
}
