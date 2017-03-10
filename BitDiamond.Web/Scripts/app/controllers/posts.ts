
module BitDiamond.Controllers.Posts {

    export class List {

        loadHistory(index: number, size: number): ng.IPromise<Utils.SequencePage<Models.IPost>> {
            this.isLoadingView = true;
            return this.__posts.getPagedNewsPosts(index, size || this.pageSize || 30).then(opr => {

                if (!Object.isNullOrUndefined(opr.Result)) {
                    opr.Result.Page = opr.Result.Page.map(_post => {
                        _post.CreatedOn = new Apollo.Models.JsonDateTime(_post.CreatedOn);
                        _post.ModifiedOn = new Apollo.Models.JsonDateTime(_post.ModifiedOn);
                        return _post;
                    });
                    this.posts = new Utils.SequencePage<Models.IPost>(opr.Result.Page, opr.Result.SequenceLength, opr.Result.PageSize, opr.Result.PageIndex);
                }
                else {
                    this.posts = new Utils.SequencePage<Models.IPost>([], 0, 0, 0);
                }

                this.pageLinks = this.posts.AdjacentIndexes(2);

                return this.$q.resolve(opr.Result);
            }, err => {
                this.__notify.error('Something went wrong - couldn\'t load your history.', 'Oops!');
            }).finally(() => {
                this.isLoadingView = false;
            });
        }

        loadLastPage() {
            this.loadHistory(this.posts.PageCount - 1, this.pageSize);
        }
        loadFirstPage() {
            this.loadHistory(0, this.pageSize);
        }
        loadLinkPage(pageIndex: number) {
            this.loadHistory(pageIndex, this.pageSize);
        }
        linkButtonClass(page: number) {
            return {
                'btn-outline': page != this.posts.PageIndex,
                'btn-default': page != this.posts.PageIndex,
                'btn-info': page == this.posts.PageIndex,
            };
        }
        displayDate(date: Apollo.Models.JsonDateTime): string {
            if (Object.isNullOrUndefined(date)) return null;
            else return date.toMoment().format('MMM D, Y');
        }
        ownerImageUrl(post: Models.IPost) {
            return '/content/images/default-user.png';
        }
        canEdit(post: Models.IPost) {
            return this.isAdmin && post.Status != Models.PostStatus.Archived;
        }
        postStatus(post: Models.IPost) {
            return Models.PostStatus[post.Status];
        }
        postStatusClass(post: Models.IPost) {
            return {
                'text-success': post.Status == Models.PostStatus.Published,
                'text-muted': post.Status == Models.PostStatus.Draft,
                'text-warning': post.Status == Models.PostStatus.Archived
            };
        }
        postActionText(post: Models.IPost) {
            if (post.Status == Models.PostStatus.Draft) return 'Publish';
            else if (post.Status == Models.PostStatus.Published) return 'Archive';
            else return '';
        }
        canAct(post: Models.IPost) {
            return post.Status != Models.PostStatus.Archived;
        }

        showDetails(post: Models.IPost) {
            this.$state.go('details', { post: post });
        }
        createNews() {
            this.$state.go('edit');
        }
        editNews(post: Models.IPost) {
            this.$state.go('edit', { post: post });
        }
        postAction(post: Models.IPost) {
            var action = post.Status == Models.PostStatus.Draft? this.__posts.publishPost:
                Models.PostStatus.Published ? this.__posts.archivePost : null;

            if (!Object.isNullOrUndefined(action)) {
                post['$__isActing'] = true;
                action(post.Id).then(opr => {
                    post.Status = opr.Result.Status;
                    this.__notify.success('Done');
                }, err => {
                    this.__notify.error('Something happened' + (err.Messaage || ''), 'Oops!');
                }).finally(() => {
                    delete post['$__isActing'];
                });
            }     
        }


        posts: Utils.SequencePage<Models.IPost>;
        pageLinks: number[];
        pageSize: number = 20;

        isLoadingView: boolean;
        isAdmin: boolean = true;

        __posts: Services.Posts;
        __userContext: Utils.Services.UserContext;
        __notify: Utils.Services.NotifyService;
        $q: ng.IQService;
        $state: ng.ui.IStateService;

        constructor(__posts, __userContext, __notify, $q, $state) {
            this.__posts = __posts;
            this.__userContext = __userContext;
            this.__notify = __userContext;
            this.$q = $q;
            this.$state = $state;

            //load the initial view
            this.loadHistory(0, this.pageSize);

            this.__userContext.userRoles.then(r => {
                //this.isAdmin = r.contains(Utils.Constants.Roles_AdminRole);
            });
        }
    }


    export class Details {

    }


    export class Edit {

        persist() {
            if (this.isPersisting) return;
            else {
                this.isPersisting = true;
                var _persist: (_p: Models.IPost) => ng.IPromise<Utils.Operation<Models.IPost>> = null;
                if (this.post['$__isNascent']) _persist = this.__posts.updatePost;
                else _persist = this.__posts.createPost;

                _persist(this.post).then(opr => {
                    this.__notify.success('Your post was saved');
                }, err => {
                    this.__notify.error('Something went wrong - ' + (err.Message || 'not sure'), 'Oops');
                }).finally(() => {
                    this.isPersisting = false;
                });
            }
        }

        back() {
            this.$state.go(this.previous, { post: this.post });
        }


        post: Models.IPost;
        previous: string;

        isPersisting: boolean;

        __posts: Services.Posts;
        __userContext: Utils.Services.UserContext;
        __notify: Utils.Services.NotifyService;
        $q: ng.IQService;
        $state: ng.ui.IStateService;
        $stateParams: ng.ui.IStateParamsService;

        constructor(__posts, __userContext, __notify, $q, $state, $stateParams) {
            this.__posts = __posts;
            this.__userContext = __userContext;
            this.__notify = __userContext;
            this.$q = $q;
            this.$state = $state;
            this.$stateParams = $stateParams;

            this.post = $stateParams['post'] || {
                Status: Models.PostStatus.Draft,
                '$__isNascent': true
            };
            this.previous = $stateParams['previous'] || 'list';
        }
    }

}