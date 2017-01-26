using Axis.Luna;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services.Command
{
    public interface IPersistenceCommand
    {
        Operation<Domain> Persist<Domain>(Domain d);
        Operation<Domain> Delete<Domain>(Domain d);

        Operation BulkPersist<Domain>(IEnumerable<Domain> sequence);
    }
}
