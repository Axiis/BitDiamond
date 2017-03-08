using Axis.Luna;
using Axis.Luna.Extensions;
using BitDiamond.Core.Models;
using BitDiamond.Core.Services;
using BitDiamond.Web.Controllers.Api.PostModels;
using BitDiamond.Web.Infrastructure.Exceptions;
using System.Web.Http;

using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Web.Controllers.Api
{
    public class PostController : ApiController
    {
        private IPostService _postService = null;

        public PostController(IPostService postService)
        {
            ThrowNullArguments(() => postService);

            this._postService = postService;
        }

        [HttpGet, Route("api/posts/single")]
        IHttpActionResult GetPostById([FromBody] PostArg args)
        => Operation.Try(() => args.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _postService.GetPostById(args.Id))
            .OperationResult(Request);

        [HttpGet, Route("api/posts/news/paged")]
        IHttpActionResult PagedNewsPosts([FromBody] PageArg args)
        => Operation.Try(() => args.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _postService.PagedNewsPosts(args.PageSize, args.PageIndex))
            .OperationResult(Request);

        [HttpPost, Route("api/posts")]
        IHttpActionResult CreateNewsPost([FromBody]Post post)
        => Operation.Try(() => post.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _postService.CreateNewsPost(post))
            .OperationResult(Request);

        [HttpPut, Route("api/posts")]
        IHttpActionResult UpdatePost([FromBody]Post post)
        => Operation.Try(() => post.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _postService.UpdatePost(post))
            .OperationResult(Request);

        [HttpPut, Route("api/posts/published")]
        IHttpActionResult PublishPost([FromBody] PostArg args)
        => Operation.Try(() => args.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _postService.PublishPost(args.Id))
            .OperationResult(Request);

        [HttpPut, Route("api/posts/archived")]
        IHttpActionResult ArchivePost([FromBody]PostArg args)
        => Operation.Try(() => args.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _postService.ArchivePost(args.Id))
            .OperationResult(Request);
    }

    namespace PostModels
    {
        public class PageArg
        {
            public int PageSize { get; set; }
            public int PageIndex { get; set; }
        }

        public class PostArg
        {
            public long Id { get; set; }
        }
    }
}