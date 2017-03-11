using Axis.Luna;
using Axis.Pollux.RBAC.Auth;
using BitDiamond.Core.Models;

namespace BitDiamond.Core.Services
{
    public interface IPostService
    {
        [Resource(":system/posts/@getById")]
        Operation<Post> GetPostById(long id);

        [Resource(":system/posts/@getPaged")]
        Operation<SequencePage<Post>> PagedNewsPosts(int pageSize, int pageIndex);

        [Resource(":system/posts/@createNews")]
        Operation<Post> CreateNewsPost(Post post);

        [Resource(":system/posts/@update")]
        Operation<Post> UpdatePost(Post post);

        [Resource(":system/posts/@publish")]
        Operation<Post> PublishPost(long id);

        [Resource(":system/posts/@archive")]
        Operation<Post> ArchivePost(long id);
    }
}
