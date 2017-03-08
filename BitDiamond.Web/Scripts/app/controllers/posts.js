var BitDiamond;
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Posts;
        (function (Posts) {
            var List = (function () {
                function List(__bitlevel, __userContext, __notify, $q) {
                    this.pageSize = 20;
                    this.__bitLevel = __bitlevel;
                    this.__userContext = __userContext;
                    this.__notify = __userContext;
                    this.$q = $q;
                    //load the initial view
                    this.loadHistory(0, this.pageSize);
                }
                List.prototype.loadHistory = function (index, size) {
                    var _this = this;
                    this.isLoadingView = true;
                    return this.__bitLevel.getPagedBitLevelHistory(index, size || this.pageSize || 30).then(function (opr) {
                        if (!Object.isNullOrUndefined(opr.Result)) {
                            opr.Result.Page = opr.Result.Page.map(function (lvl) {
                                lvl.CreatedOn = new Apollo.Models.JsonDateTime(lvl.CreatedOn);
                                lvl.ModifiedOn = new Apollo.Models.JsonDateTime(lvl.ModifiedOn);
                                if (!Object.isNullOrUndefined(lvl.Donation)) {
                                    lvl.Donation.ModifiedOn = new Apollo.Models.JsonDateTime(lvl.Donation.ModifiedOn);
                                    lvl.Donation.CreatedOn = new Apollo.Models.JsonDateTime(lvl.Donation.CreatedOn);
                                }
                                return lvl;
                            });
                            _this.levels = new BitDiamond.Utils.SequencePage(opr.Result.Page, opr.Result.SequenceLength, opr.Result.PageSize, opr.Result.PageIndex);
                        }
                        else {
                            _this.levels = new BitDiamond.Utils.SequencePage([], 0, 0, 0);
                        }
                        _this.pageLinks = _this.levels.AdjacentIndexes(2);
                        return _this.$q.resolve(opr.Result);
                    }, function (err) {
                        _this.__notify.error('Something went wrong - couldn\'t load your history.', 'Oops!');
                    }).finally(function () {
                        _this.isLoadingView = false;
                    });
                };
                List.prototype.loadLastPage = function () {
                    this.loadHistory(this.levels.PageCount - 1, this.pageSize);
                };
                List.prototype.loadFirstPage = function () {
                    this.loadHistory(0, this.pageSize);
                };
                List.prototype.loadLinkPage = function (pageIndex) {
                    this.loadHistory(pageIndex, this.pageSize);
                };
                List.prototype.linkButtonClass = function (page) {
                    return {
                        'btn-outline': page != this.levels.PageIndex,
                        'btn-default': page != this.levels.PageIndex,
                        'btn-info': page == this.levels.PageIndex,
                    };
                };
                List.prototype.displayDate = function (date) {
                    if (Object.isNullOrUndefined(date))
                        return null;
                    else
                        return date.toMoment().format('YYYY/M/D - H:m');
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
                function Edit() {
                }
                return Edit;
            }());
            Posts.Edit = Edit;
        })(Posts = Controllers.Posts || (Controllers.Posts = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
