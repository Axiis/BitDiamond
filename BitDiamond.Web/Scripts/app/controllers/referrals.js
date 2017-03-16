var BitDiamond;
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Referrals;
        (function (Referrals) {
            var Downlines = (function () {
                function Downlines(__referrals, __userContext, __notify, $q) {
                    var _this = this;
                    this.__referrals = __referrals;
                    this.__usercontext = __userContext;
                    this.__notify = __notify;
                    this.$q = $q;
                    //load user info
                    this.isLoadingView = true;
                    this.__usercontext.userRef.then(function (_ref) {
                        _this.userRef = _ref;
                        _this.__referrals.getAllDownlines(_this.userRef.ReferenceCode).then(function (opr) {
                            _this.downlines = opr.Result || [];
                            _this.generateTree.bind(_this)();
                        }, function (err) {
                            _this.__notify.error('Something went wrong - could not load your downlines at this time...', 'Oops!');
                        }).finally(function () {
                            _this.isLoadingView = false;
                        });
                    }, function (err) {
                        _this.__notify.error('Something went wrong - could not load your downlines at this time...', 'Oops!');
                        _this.isLoadingView = false;
                    });
                }
                Downlines.prototype.generateTree = function () {
                    var _this = this;
                    var nodeMap = (_a = this.downlines).concat.apply(_a, [this.userRef]).group(function (_node) { return _node.UplineCode; })
                        .aggregate({}, function (acc, next) {
                        var parent = Object.isNullOrUndefined(acc[next.Key]) ? acc[next.Key] = { text: '', nodes: [], tags: [] } : acc[next.Key];
                        parent.tags.push(next.Value.length.toString());
                        next.Value.forEach(function (_node) {
                            var tnode = _this.transform(_node, acc[_node.ReferenceCode]);
                            acc[_node.ReferenceCode] = tnode;
                            parent.nodes.push(tnode);
                        });
                        return acc;
                    });
                    $('#downlines').html('').treeview({
                        data: [nodeMap[this.userRef.ReferenceCode]],
                        showTags: true
                    });
                    var _a;
                };
                Downlines.prototype.transform = function (_node, tnode) {
                    var name = null;
                    if (Object.isNullOrUndefined(_node.UserBio))
                        name = _node.UserId;
                    else
                        name = _node.UserBio.FirstName + ' ' + _node.UserBio.LastName + ' (' + _node.ReferenceCode + ')';
                    tnode = Object.isNullOrUndefined(tnode) ? {} : tnode;
                    tnode.text = name;
                    tnode.icon = 'icon-user';
                    tnode.nodes = tnode.nodes || [];
                    tnode.tags = tnode.tags || [];
                    //after doing other stuff...
                    return tnode;
                };
                return Downlines;
            }());
            Referrals.Downlines = Downlines;
        })(Referrals = Controllers.Referrals || (Controllers.Referrals = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=referrals.js.map