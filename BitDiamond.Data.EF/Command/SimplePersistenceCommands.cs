using Axis.Jupiter.Kore.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Axis.Luna;
using Axis.Jupiter;

using static Axis.Luna.Extensions.ExceptionExtensions;
using BitDiamond.Core.Models;
using Axis.Luna.Extensions;

namespace BitDiamond.Data.EF.Command
{
    public class SimplePersistenceCommands : IPersistenceCommands
    {
        private IDataContext _context = null;

        public SimplePersistenceCommands(IDataContext context)
        {
            ThrowNullArguments(() => context);

            this._context = context;
        }


        public Operation<Domain> Add<Domain>(Domain d)
        where Domain : class => Operation.Try(() =>
        {
            _context.Add(d);
            _context.CommitChanges();
            return d;
        });

        public AsyncOperation<IEnumerable<Domain>> AddBulk<Domain>(IEnumerable<Domain> d)
        where Domain : class => Operation.TryAsync(() =>
        {
            _context.BulkInsert(d).Wait();
            return d;
        });

        public Operation<Domain> Delete<Domain>(Domain d)
        where Domain : class => Operation.Try(() =>
        {
            _context.Delete(d);
            _context.CommitChanges();
            return d;
        });

        public Operation<Domain> Update<Domain>(Domain d)
        where Domain : class => Operation.Try(() =>
        {
            d.As<BaseModel>().ModifiedOn = DateTime.Now;

            _context.Modify(d);
            _context.CommitChanges();
            return d;
        });
    }
}
