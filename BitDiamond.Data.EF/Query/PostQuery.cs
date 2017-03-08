using Axis.Jupiter;
using BitDiamond.Core.Services.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Luna;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using Axis.Luna.Extensions;
using BitDiamond.Core.Utils;

namespace BitDiamond.Data.EF.Query
{
    public class PostQuery : IPostQuery
    {
        private IDataContext _europa = null;

        public PostQuery(IDataContext context)
        {
            ThrowNullArguments(() => context);

            _europa = context;
        }

        public Post GetLatestPost(User user)
        => _europa.Store<Post>()
                  .QueryWith(_p => _p.Owner)
                  .Where(_p => _p.OwnerId == user.UserId)
                  .OrderByDescending(_p => _p.CreatedOn)
                  .FirstOrDefault();

        public SequencePage<Post> GetPagedNewsPosts(PostStatus? status, int pageSize, int pageIndex = 0)
        {
            var q = _europa.Store<Post>()
            .QueryWith(_p => _p.Owner)
            .Where(_p => _p.Context == Constants.PostContext_News);

            if (status.HasValue) q = q.Where(_q => _q.Status == status.Value);

            return q.OrderByDescending(_p => _p.CreatedOn)
            .Pipe(_q => new SequencePage<Post>(_q.Skip(pageSize * pageIndex).Take(pageSize).ToArray(), _q.Count(), pageSize, pageIndex));
        }

        public SequencePage<Post> GetPagedPosts(User user, PostStatus? status, int pageSize, int pageIndex = 0)
        {
            var q = _europa.Store<Post>()
                .QueryWith(_p => _p.Owner)
                .Where(_p => _p.OwnerId == user.UserId);

            if (status.HasValue) q = q.Where(_q => _q.Status == status.Value);

            return q.OrderByDescending(_p => _p.CreatedOn)
                .Pipe(_q => new SequencePage<Post>(_q.Skip(pageSize * pageIndex).Take(pageSize).ToArray(), _q.Count(), pageSize, pageIndex));
        }

        public Post GetPostById(long id)
        => _europa.Store<Post>().QueryWith(_p => _p.Owner).FirstOrDefault(_p => _p.Id == id);

        public int PostCount(User user)
        => _europa.Store<Post>()
            .QueryWith(_p => _p.Owner)
            .Where(_p => _p.OwnerId == user.UserId)
            .Count();
    }
}
