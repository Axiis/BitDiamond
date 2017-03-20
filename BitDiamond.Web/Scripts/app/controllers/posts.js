var BitDiamond;
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Posts;
        (function (Posts) {
            var List = (function () {
                function List(__posts, __userContext, __notify, $q, $state) {
                    var _this = this;
                    this.pageSize = 20;
                    this.isAdmin = true;
                    this.__posts = __posts;
                    this.__userContext = __userContext;
                    this.__notify = __notify;
                    this.$q = $q;
                    this.$state = $state;
                    //load the initial view
                    this.loadHistory(0, this.pageSize);
                    this.__userContext.userRoles.then(function (r) {
                        _this.isAdmin = r.contains(BitDiamond.Utils.Constants.Roles_AdminRole);
                    });
                }
                List.prototype.loadHistory = function (index, size) {
                    var _this = this;
                    this.isLoadingView = true;
                    return this.__posts.getPagedNewsPosts(size || this.pageSize || 20, index).then(function (opr) {
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
                        return date.toMoment().format('MMM D, Y');
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
                        'text-success': post.Status == BitDiamond.Models.PostStatus.Published,
                        'text-muted': post.Status == BitDiamond.Models.PostStatus.Draft,
                        'text-warning': post.Status == BitDiamond.Models.PostStatus.Archived
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
                    this.$state.go('base.details', { post: post });
                };
                List.prototype.createNews = function () {
                    this.$state.go('base.edit');
                };
                List.prototype.editNews = function (post) {
                    this.$state.go('base.edit', { post: post });
                };
                List.prototype.postAction = function (post) {
                    var _this = this;
                    if (!post['$__isActing']) {
                        post['$__isActing'] = true;
                        if (post.Status == BitDiamond.Models.PostStatus.Draft) {
                            this.__posts.publishPost(post.Id).then(function (opr) {
                                post.Status = opr.Result.Status;
                                _this.__notify.success('Done');
                            }, function (err) {
                                _this.__notify.error('Something happened' + (err.Messaage || ''), 'Oops!');
                            }).finally(function () {
                                delete post['$__isActing'];
                            });
                        }
                        else if (post.Status == BitDiamond.Models.PostStatus.Published) {
                            this.__posts.archivePost(post.Id).then(function (opr) {
                                post.Status = opr.Result.Status;
                                _this.__notify.success('Done');
                            }, function (err) {
                                _this.__notify.error('Something happened' + (err.Messaage || ''), 'Oops!');
                            }).finally(function () {
                                delete post['$__isActing'];
                            });
                        }
                        else {
                            this.__notify.info('Cannot act on an archived post');
                            delete post['$__isActing'];
                        }
                    }
                };
                return List;
            }());
            Posts.List = List;
            var Details = (function () {
                function Details($state, $stateParams, __notify, __posts) {
                    var _this = this;
                    this.$state = $state;
                    this.$stateParams = $stateParams;
                    this.__notify = __notify;
                    this.__posts = __posts;
                    this.previous = this.$stateParams['previous'] || 'base.list';
                    var p = this.$stateParams['post'];
                    var id = this.$stateParams['id'];
                    if (Object.isNullOrUndefined(p) && (Object.isNullOrUndefined(id) || id <= 0)) {
                        swal({
                            title: 'Error',
                            text: 'Sorry, something went wrong while trying to display the post.',
                            type: 'error'
                        });
                        this.$state.go(this.previous);
                    }
                    else if (typeof p == 'number') {
                        this.isLoadingView = true;
                        this.__posts.getPostById(p).then(function (opr) {
                            _this.post.CreatedOn = new Apollo.Models.JsonDateTime(_this.post.CreatedOn);
                            _this.post = opr.Result;
                        }, function (err) {
                            _this.__notify.error('Something went wrong ' + (err.Message || ''), 'Oops');
                        }).finally(function () {
                            _this.isLoadingView = false;
                        });
                    }
                    else
                        this.post = p;
                }
                Details.prototype.back = function () {
                    this.$state.go(this.previous, { post: this.post });
                };
                Details.prototype.getHeadingContainerStyle = function () {
                    return {
                        height: '150px',
                        padding: '20px',
                        display: 'flex',
                        position: 'relative',
                        'align-items': 'flex-end',
                        'flex-direction': 'row',
                        'background': 'no-repeat url(/content/images/material-backgrounds/2.jpg) top left',
                        'background-size': 'cover'
                    };
                };
                Details.prototype.displayDate = function (date) {
                    if (Object.isNullOrUndefined(date))
                        return null;
                    else
                        return date.toMoment().format('MMM D, Y');
                };
                return Details;
            }());
            Posts.Details = Details;
            var Edit = (function () {
                function Edit(__posts, __userContext, __notify, $q, $state, $stateParams) {
                    this.__posts = __posts;
                    this.__userContext = __userContext;
                    this.__notify = __notify;
                    this.$q = $q;
                    this.$state = $state;
                    this.$stateParams = $stateParams;
                    this.post = $stateParams['post'] || {
                        Status: BitDiamond.Models.PostStatus.Draft,
                        '$__isNascent': true
                    };
                    this.previous = $stateParams['previous'] || 'base.list';
                }
                //events
                Edit.prototype.persist = function () {
                    var _this = this;
                    if (this.isPersisting)
                        return;
                    else {
                        this.isPersisting = true;
                        if (this.post['$__isNascent']) {
                            this.__posts.createPost(this.post).then(function (opr) {
                                _this.post.Id = opr.Result.Id;
                                delete _this.post['$__isNascent'];
                                _this.__notify.success('Your post was saved');
                            }, function (err) {
                                _this.__notify.error('Something went wrong - ' + (err.Message || 'not sure'), 'Oops');
                            }).finally(function () {
                                _this.isPersisting = false;
                            });
                        }
                        else {
                            this.__posts.updatePost(this.post).then(function (opr) {
                                _this.post.Id = opr.Result.Id;
                                delete _this.post['$__isNascent'];
                                _this.__notify.success('Your post was saved');
                            }, function (err) {
                                _this.__notify.error('Something went wrong - ' + (err.Message || 'not sure'), 'Oops');
                            }).finally(function () {
                                _this.isPersisting = false;
                            });
                        }
                    }
                };
                Edit.prototype.back = function () {
                    this.$state.go(this.previous, { post: this.post });
                };
                //ui states
                Edit.prototype.editorTitle = function () {
                    if (this.post['$__isNascent'])
                        return 'New Post';
                    else
                        return 'Edit Post';
                };
                return Edit;
            }());
            Posts.Edit = Edit;
        })(Posts = Controllers.Posts || (Controllers.Posts = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=posts.js.map