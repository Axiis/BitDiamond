
module BitDiamond.Controllers.Payments {

    export class Incoming {

        payments: Utils.SequencePage<Models.IBlockChainTransaction>;
        pageLinks: number[];
        pageSize: number = 20;

        isLoadingView: boolean;
        isVerifyingTransaction: boolean;

        __blockChain: Services.BlockChain;
        __userContext: Utils.Services.UserContext;
        __notify: Utils.Services.NotifyService;
        $q: ng.IQService;


        isTransactionVerified(transaction: Models.IBlockChainTransaction) {
            if (Object.isNullOrUndefined(transaction)) return false;
            else return transaction.Status == Models.BlockChainTransactionStatus.Verified;
        }


        loadHistory(index: number, size: number): ng.IPromise<Utils.SequencePage<Models.IBlockChainTransaction>> {
            this.isLoadingView = true;
            return this.__blockChain.getPagedIncomingTransactions(index, size || this.pageSize || 30).then(opr => {

                if (!Object.isNullOrUndefined(opr.Result)) {
                    opr.Result.Page = opr.Result.Page.map(trnx => {
                        trnx.CreatedOn = new Apollo.Models.JsonDateTime(trnx.CreatedOn);
                        trnx.ModifiedOn = new Apollo.Models.JsonDateTime(trnx.ModifiedOn);
                        return trnx;
                    });
                    this.payments = new Utils.SequencePage<Models.IBlockChainTransaction>(opr.Result.Page, opr.Result.SequenceLength, opr.Result.PageSize, opr.Result.PageIndex);
                }
                else {
                    this.payments = new Utils.SequencePage<Models.IBlockChainTransaction>([], 0, 0, 0);
                }

                this.pageLinks = this.payments.AdjacentIndexes(2);

                return this.$q.resolve(opr.Result);
            }, err => {
                this.__notify.error('Something went wrong - couldn\'t load your history.', 'Oops!');
            }).finally(() => {
                this.isLoadingView = false;
            });
        }
        loadLastPage() {
            this.loadHistory(this.payments.PageCount - 1, this.pageSize);
        }
        loadFirstPage() {
            this.loadHistory(0, this.pageSize);
        }
        loadLinkPage(pageIndex: number) {
            this.loadHistory(pageIndex, this.pageSize);
        }
        linkButtonClass(page: number) {
            return {
                'btn-outline': page != this.payments.PageIndex,
                'btn-default': page != this.payments.PageIndex,
                'btn-info': page == this.payments.PageIndex,
            };
        }
        displayDate(date: Apollo.Models.JsonDateTime): string {
            if (Object.isNullOrUndefined(date)) return null;
            else return date.toMoment().format('YYYY/M/d  H:m');
        }
        getTransactionSender(trnx: Models.IBlockChainTransaction): string {
            if (Object.isNullOrUndefined(trnx.Sender)) return '-';
            else if (Object.isNullOrUndefined(trnx.Sender.OwnerRef)) return trnx.Sender.BlockChainAddress;
            else if (Object.isNullOrUndefined(trnx.Sender.OwnerRef.UserBio)) return trnx.Sender.OwnerRef.ReferenceCode;
            else {
                var bio = trnx.Sender.OwnerRef.UserBio;
                return (bio.FirstName || '') + ' ' + (bio.LastName || '');
            }
        }
        verifyManually(trnx: Models.IBlockChainTransaction) {
            if (!this.isVerifyingTransaction) {
                this.isVerifyingTransaction = true;
                this.__blockChain.verifyManually(trnx.TransactionHash).then(opr => {
                    this.__notify.success('Transaction verified successfully.');
                }, err => {
                    this.__notify.error('Something went wrong - could not verify the transaction.', 'Oops!');
                }).finally(() => {
                    this.isVerifyingTransaction = false;
                });
            }
        }


        constructor(__blockChain, __userContext, __notify, $q) {
            this.__blockChain = __blockChain;
            this.__userContext = __userContext;
            this.__notify = __userContext;
            this.$q = $q;

            //load the initial view
            this.loadHistory(0, this.pageSize);
        }
    }


    export class Outgoing {

        payments: Utils.SequencePage<Models.IBlockChainTransaction>;
        pageLinks: number[];
        pageSize: number = 20;

        isLoadingView: boolean;

        __blockChain: Services.BlockChain;
        __userContext: Utils.Services.UserContext;
        __notify: Utils.Services.NotifyService;
        $q: ng.IQService;


        loadHistory(index: number, size: number): ng.IPromise<Utils.SequencePage<Models.IBlockChainTransaction>> {
            this.isLoadingView = true;
            return this.__blockChain.getPagedOutgoingTransactions(index, size || this.pageSize || 30).then(opr => {

                if (!Object.isNullOrUndefined(opr.Result)) {
                    opr.Result.Page = opr.Result.Page.map(trnx => {
                        trnx.CreatedOn = new Apollo.Models.JsonDateTime(trnx.CreatedOn);
                        trnx.ModifiedOn = new Apollo.Models.JsonDateTime(trnx.ModifiedOn);
                        return trnx;
                    });
                    this.payments = new Utils.SequencePage<Models.IBlockChainTransaction>(opr.Result.Page, opr.Result.SequenceLength, opr.Result.PageSize, opr.Result.PageIndex);
                }
                else {
                    this.payments = new Utils.SequencePage<Models.IBlockChainTransaction>([], 0, 0, 0);
                }

                this.pageLinks = this.payments.AdjacentIndexes(2);

                return this.$q.resolve(opr.Result);
            }, err => {
                this.__notify.error('Something went wrong - couldn\'t load your history.', 'Oops!');
            }).finally(() => {
                this.isLoadingView = false;
            });
        }
        canVerify(trnx: Models.IBlockChainTransaction): boolean {
            return true;
        }

        loadLastPage() {
            this.loadHistory(this.payments.PageCount - 1, this.pageSize);
        }
        loadFirstPage() {
            this.loadHistory(0, this.pageSize);
        }
        loadLinkPage(pageIndex: number) {
            this.loadHistory(pageIndex, this.pageSize);
        }
        linkButtonClass(page: number) {
            return {
                'btn-outline': page != this.payments.PageIndex,
                'btn-default': page != this.payments.PageIndex,
                'btn-info': page == this.payments.PageIndex,
            };
        }
        displayDate(date: Apollo.Models.JsonDateTime): string {
            if (Object.isNullOrUndefined(date)) return null;
            else return date.toMoment().format('YYYY/M/d  H:m');
        }
        getTransactionSender(trnx: Models.IBlockChainTransaction): string {
            if (Object.isNullOrUndefined(trnx.Sender)) return '-';
            else if (Object.isNullOrUndefined(trnx.Sender.OwnerRef)) return trnx.Sender.BlockChainAddress;
            else if (Object.isNullOrUndefined(trnx.Sender.OwnerRef.UserBio)) return trnx.Sender.OwnerRef.ReferenceCode;
            else {
                var bio = trnx.Sender.OwnerRef.UserBio;
                return (bio.FirstName || '') + ' ' + (bio.LastName || '');
            }
        }
        transactionStatus(trnx: Models.IBlockChainTransaction): string {
            return Models.BlockChainTransactionStatus[trnx.Status];
        }

        constructor(__blockChain, __userContext, __notify, $q) {
            this.__blockChain = __blockChain;
            this.__userContext = __userContext;
            this.__notify = __userContext;
            this.$q = $q;

            //load the initial view
            this.loadHistory(0, this.pageSize);
        }
    }
}