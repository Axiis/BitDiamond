using Axis.Luna;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;

namespace BitDiamond.Core.Services.Query
{
    public interface IPostQuery
    {
        Post GetPostById(long id);
        Post GetLatestPost(User user);
        SequencePage<Post> GetPagedPosts(User user, PostStatus? status, int pageSize, int pageIndex = 0);
        SequencePage<Post> GetPagedNewsPosts(PostStatus? status, int pageSize, int pageIndex = 0);

        int PostCount(User user);
    }
}
