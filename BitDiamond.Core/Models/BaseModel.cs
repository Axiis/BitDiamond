using Axis.Luna;
using System;
using System.ComponentModel.DataAnnotations;

namespace BitDiamond.Core.Models
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
