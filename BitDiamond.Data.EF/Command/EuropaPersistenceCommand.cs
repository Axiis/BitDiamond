using BitDiamond.Core.Services.Command;
using System.Collections.Generic;
using Axis.Luna;
using Axis.Jupiter.Europa;

using static Axis.Luna.Extensions.ExceptionExtensions;
using BitDiamond.Core.Models;
using BitDiamond.Data.EF.Utils;

namespace BitDiamond.Data.EF.Command
{
    public class EuropaPersistenceCommand : IPersistenceCommands
    {
        private EuropaContext _context = null;
        private PersistenceProvider _persistenceProvider = null;

        public EuropaPersistenceCommand(EuropaContext context, PersistenceProvider persistenceProvider)
        {
            ThrowNullArguments(() => context,
                               () => persistenceProvider);

            _context = context;
            _persistenceProvider = persistenceProvider;
        }

        public Operation BulkPersist<Domain>(IEnumerable<Domain> sequence)
        where Domain : class => Operation.Fail("Bulk additions are not supported at the moment");

        public Operation<Domain> Delete<Domain>(Domain d)
        where Domain : class => Operation.Try(() =>
        {
            if (_persistenceProvider.CanDelete<Domain>()) return _persistenceProvider.Delete(d);

            _context.Store<Domain>().Delete(d, true);
            return d;
        });

        public Operation<Domain> Add<Domain>(Domain d)
        where Domain : class => Operation.Try(() =>
        {
            if (_persistenceProvider.CanInsert<Domain>()) return _persistenceProvider.Insert(d);

            _context.Add(d).Context.CommitChanges();
            return d;
        });

        public Operation<Domain> Update<Domain>(Domain d)
        where Domain : class => Operation.Try(() =>
        {
            if (_persistenceProvider.CanUpdate<Domain>()) return _persistenceProvider.Update(d);

            _context.Store<Domain>().Modify(d, true);
            return d;
        });
    }
}
