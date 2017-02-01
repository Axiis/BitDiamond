using Axis.Luna;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services.Command
{
    public interface IPersistenceCommands
    {
        Operation<Domain> Add<Domain>(Domain d) where Domain: class;
        Operation<Domain> Update<Domain>(Domain d) where Domain: class;
        Operation<Domain> Delete<Domain>(Domain d) where Domain : class;

        Operation BulkPersist<Domain>(IEnumerable<Domain> sequence) where Domain : class;
    }
}
