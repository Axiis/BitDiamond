using Axis.Luna;
using System;
using System.ComponentModel.DataAnnotations;

namespace BitDiamond.Core.Models
{
    public class BaseModel
    {
        public virtual Operation Validate()
            => Operation.Try(() => Validator.ValidateObject(this, new ValidationContext(this, null, null)));

        protected BaseModel()
        { }
    }

    public class BaseModel<IdType>: BaseModel
    {
        public IdType Id { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }    


        protected BaseModel()
        {
            CreatedOn = DateTime.Now;
        }
    }
}
