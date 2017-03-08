
module BitDiamond.Controllers.Posts {

    export class List {

        levels: Utils.SequencePage<Models.IBitLevel>;
        pageLinks: number[];
        pageSize: number = 20;

        isLoadingView: boolean;

        __bitLevel: Services.BitLevel;
        __userContext: Utils.Services.UserContext;
        __notify: Utils.Services.NotifyService;
        $q: ng.IQService;

        loadHistory(index: number, size: number): ng.IPromise<Utils.SequencePage<Models.IBitLevel>> {
            this.isLoadingView = true;
            return this.__bitLevel.getPagedBitLevelHistory(index, size || this.pageSize || 30).then(opr => {

                if (!Object.isNullOrUndefined(opr.Result)) {
                    opr.Result.Page = opr.Result.Page.map(lvl => {
                        lvl.CreatedOn = new Apollo.Models.JsonDateTime(lvl.CreatedOn);
                        lvl.ModifiedOn = new Apollo.Models.JsonDateTime(lvl.ModifiedOn);
                        if (!Object.isNullOrUndefined(lvl.Donation)) {
                            lvl.Donation.ModifiedOn = new Apollo.Models.JsonDateTime(lvl.Donation.ModifiedOn);
                            lvl.Donation.CreatedOn = new Apollo.Models.JsonDateTime(lvl.Donation.CreatedOn);
                        }
                        return lvl;
                    });
                    this.levels = new Utils.SequencePage<Models.IBitLevel>(opr.Result.Page, opr.Result.SequenceLength, opr.Result.PageSize, opr.Result.PageIndex);
                }
                else {
                    this.levels = new Utils.SequencePage<Models.IBitLevel>([], 0, 0, 0);
                }

                this.pageLinks = this.levels.AdjacentIndexes(2);

                return this.$q.resolve(opr.Result);
            }, err => {
                this.__notify.error('Something went wrong - couldn\'t load your history.', 'Oops!');
            }).finally(() => {
                this.isLoadingView = false;
            });
        }

        loadLastPage() {
            this.loadHistory(this.levels.PageCount - 1, this.pageSize);
        }
        loadFirstPage() {
            this.loadHistory(0, this.pageSize);
        }
        loadLinkPage(pageIndex: number) {
            this.loadHistory(pageIndex, this.pageSize);
        }
        linkButtonClass(page: number) {
            return {
                'btn-outline': page != this.levels.PageIndex,
                'btn-default': page != this.levels.PageIndex,
                'btn-info': page == this.levels.PageIndex,
            };
        }
        displayDate(date: Apollo.Models.JsonDateTime): string {
            if (Object.isNullOrUndefined(date)) return null;
            else return date.toMoment().format('YYYY/M/D - H:m');
        }


        constructor(__bitlevel, __userContext, __notify, $q) {
            this.__bitLevel = __bitlevel;
            this.__userContext = __userContext;
            this.__notify = __userContext;
            this.$q = $q;

            //load the initial view
            this.loadHistory(0, this.pageSize);
        }
    }

    export class Details {

    }

    export class Edit {

    }

}