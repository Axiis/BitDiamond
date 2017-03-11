using System;
using Axis.Luna;
using BitDiamond.Core.Models;
using Axis.Jupiter.Kore.Command;
using Axis.Pollux.RBAC.Services;

using static Axis.Luna.Extensions.ExceptionExtensions;
using BitDiamond.Core.Services.Query;
using System.Linq;
using BitDiamond.Core.Utils;
using Axis.Luna.Extensions;

namespace BitDiamond.Core.Services
{
    public class PostService : IPostService
    {
        private IPersistenceCommands _pcommand = null;
        private IUserAuthorization _auth = null;
        private IUserNotifier _notifier = null;
        private IPostQuery _query = null;

        public IUserContext UserContext { get; private set; }

        public PostService(IUserAuthorization authorizer, IUserContext userContext,
                           IPersistenceCommands pcommand, IUserNotifier notifier,
                           IPostQuery query)
        {
            ThrowNullArguments(() => userContext,
                               () => pcommand,
                               () => notifier,
                               () => query);

            _query = query;
            _pcommand = pcommand;
            _auth = authorizer;
            _notifier = notifier;

            UserContext = userContext;
        }

        public Operation<Post> ArchivePost(long id)
        => _auth.AuthorizeAccess(this.PermissionProfile<IPostService>(UserContext.CurrentUser()), () =>
        {
            var post = _query.GetPostById(id).ThrowIfNull("Invalid post");

            post.Status = PostStatus.Archived;
            return _pcommand.Update(post);
        });

        public Operation<Post> CreateNewsPost(Post post)
        => _auth.AuthorizeAccess(this.PermissionProfile<IPostService>(UserContext.CurrentUser()), () =>
        {
            post.OwnerId = UserContext.CurrentUser().UserId;
            return post.Validate()
                .Then(opr =>
                {
                    post.Id = 0;
                    post.Status = PostStatus.Draft;
                    post.OwnerId = UserContext.CurrentUser().UserId;
                    post.Context = Constants.PostContext_News;

                    return _pcommand.Add(post);
                });
        });

        public Operation<Post> GetPostById(long id)
        => _auth.AuthorizeAccess(this.PermissionProfile<IPostService>(UserContext.CurrentUser()), () =>
        {
            var currentUser = UserContext.CurrentUser();
            var post = _query.GetPostById(id);

            if (post.Status == PostStatus.Published) return post;
            else if (post.OwnerId == currentUser.UserId) return post;
            else if (UserContext.CurrentUserRoles().Contains(Constants.Roles_AdminRole) ||
                     UserContext.CurrentUserRoles().Contains(Constants.Roles_RootRole)) return post;
            else throw new Exception("invalid operation");
        });

        public Operation<SequencePage<Post>> PagedNewsPosts(int pageSize, int pageIndex)
        => _auth.AuthorizeAccess(this.PermissionProfile<IPostService>(UserContext.CurrentUser()), () =>
        {
            var userRoles = UserContext.CurrentUserRoles();
            if (userRoles.Contains(Constants.Roles_AdminRole) || userRoles.Contains(Constants.Roles_RootRole))
                return _query.GetPagedNewsPosts(null, pageSize, pageIndex);
            else return _query.GetPagedNewsPosts(PostStatus.Published, pageSize, pageIndex);
        });

        public Operation<Post> PublishPost(long id)
        => _auth.AuthorizeAccess(this.PermissionProfile<IPostService>(UserContext.CurrentUser()), () =>
        {
            var post = _query.GetPostById(id).ThrowIfNull("Invalid post");
            
            post.Status = PostStatus.Published;
            return _pcommand.Update(post);
        });

        public Operation<Post> UpdatePost(Post post)
        => _auth.AuthorizeAccess(this.PermissionProfile<IPostService>(UserContext.CurrentUser()), () =>
        {
            if (post.OwnerId != UserContext.CurrentUser().UserId) throw new Exception("Access Denied");
            else return post.Validate().Then(opr =>
            {
                post.ModifiedOn = DateTime.Now;
                return _pcommand.Update(post);
            });
        });
    }
}
