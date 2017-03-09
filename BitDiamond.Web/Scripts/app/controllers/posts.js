var BitDiamond;
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Posts;
        (function (Posts) {
            var List = (function () {
                function List(__posts, __userContext, __notify, $q, $state) {
                    this.pageSize = 20;
                    this.isAdmin = true;
                    this.__posts = __posts;
                    this.__userContext = __userContext;
                    this.__notify = __userContext;
                    this.$q = $q;
                    this.$state = $state;
                    //load the initial view
                    this.loadHistory(0, this.pageSize);
                    this.__userContext.userRoles.then(function (r) {
                        //this.isAdmin = r.contains(Utils.Constants.Roles_AdminRole);
                    });
                }
                List.prototype.loadHistory = function (index, size) {
                    var _this = this;
                    this.isLoadingView = true;
                    return this.__posts.getPagedNewsPosts(index, size || this.pageSize || 30).then(function (opr) {
                        if (!Object.isNullOrUndefined(opr.Result)) {
                            opr.Result.Page = opr.Result.Page.map(function (_post) {
                                _post.CreatedOn = new Apollo.Models.JsonDateTime(_post.CreatedOn);
                                _post.ModifiedOn = new Apollo.Models.JsonDateTime(_post.ModifiedOn);
                                return _post;
                            });
                            _this.posts = new BitDiamond.Utils.SequencePage(opr.Result.Page, opr.Result.SequenceLength, opr.Result.PageSize, opr.Result.PageIndex);
                        }
                        else {
                            _this.posts = new BitDiamond.Utils.SequencePage([], 0, 0, 0);
                        }
                        _this.pageLinks = _this.posts.AdjacentIndexes(2);
                        return _this.$q.resolve(opr.Result);
                    }, function (err) {
                        _this.__notify.error('Something went wrong - couldn\'t load your history.', 'Oops!');
                    }).finally(function () {
                        _this.isLoadingView = false;
                    });
                };
                List.prototype.loadLastPage = function () {
                    this.loadHistory(this.posts.PageCount - 1, this.pageSize);
                };
                List.prototype.loadFirstPage = function () {
                    this.loadHistory(0, this.pageSize);
                };
                List.prototype.loadLinkPage = function (pageIndex) {
                    this.loadHistory(pageIndex, this.pageSize);
                };
                List.prototype.linkButtonClass = function (page) {
                    return {
                        'btn-outline': page != this.posts.PageIndex,
                        'btn-default': page != this.posts.PageIndex,
                        'btn-info': page == this.posts.PageIndex,
                    };
                };
                List.prototype.displayDate = function (date) {
                    if (Object.isNullOrUndefined(date))
                        return null;
                    else
                        return date.toMoment().format('YYYY/M/D - H:m');
                };
                List.prototype.ownerImageUrl = function (post) {
                    return '/content/images/default-user.png';
                };
                List.prototype.canEdit = function (post) {
                    return this.isAdmin && post.Status != BitDiamond.Models.PostStatus.Archived;
                };
                List.prototype.postStatus = function (post) {
                    return BitDiamond.Models.PostStatus[post.Status];
                };
                List.prototype.postStatusClass = function (post) {
                    return {
                        'label-success': post.Status == BitDiamond.Models.PostStatus.Published,
                        'label-default': post.Status == BitDiamond.Models.PostStatus.Draft,
                        'label-warning': post.Status == BitDiamond.Models.PostStatus.Archived
                    };
                };
                List.prototype.postActionText = function (post) {
                    if (post.Status == BitDiamond.Models.PostStatus.Draft)
                        return 'Publish';
                    else if (post.Status == BitDiamond.Models.PostStatus.Published)
                        return 'Archive';
                    else
                        return '';
                };
                List.prototype.canAct = function (post) {
                    return post.Status != BitDiamond.Models.PostStatus.Archived;
                };
                List.prototype.showDetails = function (post) {
                    this.$state.go('details', { post: post });
                };
                List.prototype.createNews = function () {
                    this.$state.go('edit');
                };
                List.prototype.editNews = function (post) {
                    this.$state.go('edit', { post: post });
                };
                List.prototype.postAction = function (post) {
                    var _this = this;
                    var action = post.Status == BitDiamond.Models.PostStatus.Draft ? this.__posts.publishPost :
                        BitDiamond.Models.PostStatus.Published ? this.__posts.archivePost : null;
                    if (!Object.isNullOrUndefined(action)) {
                        post['$__isActing'] = true;
                        action(post.Id).then(function (opr) {
                            post.Status = opr.Result.Status;
                            _this.__notify.success('Done');
                        }, function (err) {
                            _this.__notify.error('Something happened' + (err.Messaage || ''), 'Oops!');
                        }).finally(function () {
                            delete post['$__isActing'];
                        });
                    }
                };
                return List;
            }());
            Posts.List = List;
            var Details = (function () {
                function Details() {
                }
                return Details;
            }());
            Posts.Details = Details;
            var Edit = (function () {
                function Edit(__posts, __userContext, __notify, $q, $state, $stateParams) {
                    this.__posts = __posts;
                    this.__userContext = __userContext;
                    this.__notify = __userContext;
                    this.$q = $q;
                    this.$state = $state;
                    this.$stateParams = $stateParams;
                    this.post = $stateParams['post'] || {
                        Status: BitDiamond.Models.PostStatus.Draft,
                        '$__isNascent': true
                    };
                    this.previous = $stateParams['previous'] || 'list';
                }
                Edit.prototype.persist = function () {
                    var _this = this;
                    if (this.isPersisting)
                        return;
                    else {
                        this.isPersisting = true;
                        var _persist = null;
                        if (this.post['$__isNascent'])
                            _persist = this.__posts.updatePost;
                        else
                            _persist = this.__posts.createPost;
                        _persist(this.post).then(function (opr) {
                            _this.__notify.success('Your post was saved');
                        }, function (err) {
                            _this.__notify.error('Something went wrong - ' + (err.Message || 'not sure'), 'Oops');
                        }).finally(function () {
                            _this.isPersisting = false;
                        });
                    }
                };
                Edit.prototype.back = function () {
                    this.$state.go(this.previous, { post: this.post });
                };
                return Edit;
            }());
            Posts.Edit = Edit;
        })(Posts = Controllers.Posts || (Controllers.Posts = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
