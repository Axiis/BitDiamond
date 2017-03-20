

module BitDiamond.Controllers.Referrals {

    export class Downlines {

        isLoadingView: boolean;
        userRef: Models.IReferralNode;
        downlines: Models.IReferralNode[];

        generateTree() {

            var nodeMap = this.downlines
                .concat(...[this.userRef])
                .group(_node => _node.UplineCode)
                .aggregate({}, (acc, next) => {
                    var parent = Object.isNullOrUndefined(acc[next.Key]) ? acc[next.Key] = <ITreeViewNode>{ text: '', nodes: [], tags: [] } : <ITreeViewNode>acc[next.Key];
                    parent.tags.push(next.Value.length.toString());
                    next.Value.forEach(_node => {
                        var tnode = this.transform(_node, <ITreeViewNode>acc[_node.ReferenceCode]);
                        acc[_node.ReferenceCode] = tnode;

                        parent.nodes.push(tnode);
                    });
                    return acc;
                });

            $('#downlines').html('').treeview({
                data: [nodeMap[this.userRef.ReferenceCode]],
                showTags: true
            });
        }
        transform(_node: Models.IReferralNode, tnode: ITreeViewNode): ITreeViewNode {
            
            var name: string = null;
            if (Object.isNullOrUndefined(_node.UserBio)) name = _node.UserId;
            else name = _node.UserBio.FirstName + ' ' + _node.UserBio.LastName;

            tnode = Object.isNullOrUndefined(tnode) ? <ITreeViewNode>{} : tnode;

            tnode.text = name;
            tnode.icon = 'icon-user';
            tnode.nodes = tnode.nodes || [];
            tnode.tags = tnode.tags || [];

            //after doing other stuff...

            return tnode;
        }


        __referrals: Services.Referrals;
        __usercontext: Utils.Services.UserContext;
        __notify: Utils.Services.NotifyService;
        $q: ng.IQService;

        constructor(__referrals, __userContext, __notify, $q) {
            this.__referrals = __referrals;
            this.__usercontext = __userContext;
            this.__notify = __notify;
            this.$q = $q;

            //load user info
            this.isLoadingView = true;
            this.__usercontext.userRef.then(_ref => {
                this.userRef = _ref;

                this.__referrals.getAllDownlines(this.userRef.ReferenceCode).then(opr => {
                    this.downlines = opr.Result || [];

                    this.generateTree.bind(this)();
                }, err => {
                    this.__notify.error('Something went wrong - could not load your downlines at this time...', 'Oops!');
                }).finally(() => {
                    this.isLoadingView = false;
                });
            }, err => {
                this.__notify.error('Something went wrong - could not load your downlines at this time...', 'Oops!');
                this.isLoadingView = false;
            });
        }
    }
}