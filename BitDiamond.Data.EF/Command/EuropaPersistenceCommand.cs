using BitDiamond.Core.Services.Command;
using System;
using System.Linq;
using System.Collections.Generic;
using Axis.Luna;
using Axis.Jupiter.Europa;

using static Axis.Luna.Extensions.ExceptionExtensions;
using BitDiamond.Core.Models;
using BitDiamond.Data.EF.Utils;
using Axis.Luna.Extensions;

namespace BitDiamond.Data.EF.Command
{
    public class EuropaPersistenceCommand : IPersistenceCommand
    {
        private EuropaContext _context = null;
        private DomainConverter _converter = null;

        public EuropaPersistenceCommand(EuropaContext context, DomainConverter converter)
        {
            ThrowNullArguments(() => context,
                               () => converter);

            _context = context;
            _converter = converter;
        }

        public Operation BulkPersist<Domain>(IEnumerable<Domain> sequence)
        where Domain : BaseModel  => Operation.Try(() => sequence.Select(_d => _converter.Convert(_d))
                                                                 .Pipe(_es => _context.BulkInsert(_es).Wait()));

        public Operation<Domain> Delete<Domain>(Domain d)
        where Domain : BaseModel => Operation.Try(() =>
         {
             _context.Store<Domain>().Delete(d, true);
             return d;
         });

        public Operation<Domain> Persist<Domain>(Domain d)
        where Domain: BaseModel
        {
            throw new NotImplementedException();
        }
    }
}
