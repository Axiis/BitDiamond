using Axis.Luna;
using Axis.Luna.Extensions;
using BitDiamond.Core.Models;
using BitDiamond.Core.Services;
using BitDiamond.Core.Utils;
using BitDiamond.Web.Controllers.Api.PostModels;
using BitDiamond.Web.Infrastructure.Exceptions;
using BitDiamond.Web.Infrastructure.Utils;
using Newtonsoft.Json;
using System;
using System.Text;
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
        public IHttpActionResult GetPostById(string data)
        => this.Log(() => Operation.Try(() => ThrowIfFail(() => Encoding.UTF8.GetString(Convert.FromBase64String(data)), ex => new MalformedApiArgumentsException()))
            .Then(_jopr => ThrowIfFail(() => JsonConvert.DeserializeObject<PostArg>(_jopr.Result, Constants.Misc_DefaultJsonSerializerSettings), ex => new MalformedApiArgumentsException()))
            .Then(argopr => _postService.GetPostById(argopr.Result.Id))
            .OperationResult(Request));

        [HttpGet, Route("api/posts/news/paged")]
        public IHttpActionResult PagedNewsPosts(string data)
        => this.Log(() => Operation.Try(() => ThrowIfFail(() => Encoding.UTF8.GetString(Convert.FromBase64String(data)), ex => new MalformedApiArgumentsException()))
            .Then(_jopr => ThrowIfFail(() => JsonConvert.DeserializeObject<PageArg>(_jopr.Result, Constants.Misc_DefaultJsonSerializerSettings), ex => new MalformedApiArgumentsException()))
            .Then(argopr => _postService.PagedNewsPosts(argopr.Result.PageSize, argopr.Result.PageIndex))
            .OperationResult(Request));

        [HttpPost, Route("api/posts")]
        public IHttpActionResult CreateNewsPost([FromBody]Post post)
        => this.Log(() => Operation.Try(() => post.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _postService.CreateNewsPost(post))
            .OperationResult(Request));

        [HttpPut, Route("api/posts")]
        public IHttpActionResult UpdatePost([FromBody]Post post)
        => this.Log(() => Operation.Try(() => post.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _postService.UpdatePost(post))
            .OperationResult(Request));

        [HttpPut, Route("api/posts/published")]
        public IHttpActionResult PublishPost([FromBody] PostArg args)
        => this.Log(() => Operation.Try(() => args.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _postService.PublishPost(args.Id))
            .OperationResult(Request));

        [HttpPut, Route("api/posts/archived")]
        public IHttpActionResult ArchivePost([FromBody]PostArg args)
        => this.Log(() => Operation.Try(() => args.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _postService.ArchivePost(args.Id))
            .OperationResult(Request));
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