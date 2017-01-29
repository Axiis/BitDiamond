using Axis.Luna;
using BitDiamond.Core.Models;
using System;
using System.Collections.Generic;

namespace BitDiamond.Core.Services.Command
{
    public interface IPersistenceCommand
    {
        Operation<Domain> Persist<Domain>(Domain d) where Domain: BaseModel;
        Operation<Domain> Delete<Domain>(Domain d) where Domain : BaseModel;

        Operation BulkPersist<Domain>(IEnumerable<Domain> sequence) where Domain : BaseModel;
    }
}
