var BitDiamond;
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Payments;
        (function (Payments) {
            var Incoming = (function () {
                function Incoming(__blockChain, __userContext, __notify, $q) {
                    this.pageSize = 20;
                    this.__blockChain = __blockChain;
                    this.__userContext = __userContext;
                    this.__notify = __userContext;
                    this.$q = $q;
                    //load the initial view
                    this.loadHistory(0, this.pageSize);
                }
                Incoming.prototype.isTransactionVerified = function (transaction) {
                    if (Object.isNullOrUndefined(transaction))
                        return false;
                    else
                        return transaction.Status == BitDiamond.Models.BlockChainTransactionStatus.Verified;
                };
                Incoming.prototype.loadHistory = function (index, size) {
                    var _this = this;
                    this.isLoadingView = true;
                    return this.__blockChain.getPagedIncomingTransactions(index, size || this.pageSize || 30).then(function (opr) {
                        if (!Object.isNullOrUndefined(opr.Result)) {
                            opr.Result.Page = opr.Result.Page.map(function (trnx) {
                                trnx.CreatedOn = new Apollo.Models.JsonDateTime(trnx.CreatedOn);
                                trnx.ModifiedOn = new Apollo.Models.JsonDateTime(trnx.ModifiedOn);
                                return trnx;
                            });
                            _this.payments = new BitDiamond.Utils.SequencePage(opr.Result.Page, opr.Result.SequenceLength, opr.Result.PageSize, opr.Result.PageIndex);
                        }
                        else {
                            _this.payments = new BitDiamond.Utils.SequencePage([], 0, 0, 0);
                        }
                        _this.pageLinks = _this.payments.AdjacentIndexes(2);
                        return _this.$q.resolve(opr.Result);
                    }, function (err) {
                        _this.__notify.error('Something went wrong - couldn\'t load your history.', 'Oops!');
                    }).finally(function () {
                        _this.isLoadingView = false;
                    });
                };
                Incoming.prototype.loadLastPage = function () {
                    this.loadHistory(this.payments.PageCount - 1, this.pageSize);
                };
                Incoming.prototype.loadFirstPage = function () {
                    this.loadHistory(0, this.pageSize);
                };
                Incoming.prototype.loadLinkPage = function (pageIndex) {
                    this.loadHistory(pageIndex, this.pageSize);
                };
                Incoming.prototype.linkButtonClass = function (page) {
                    return {
                        'btn-outline': page != this.payments.PageIndex,
                        'btn-default': page != this.payments.PageIndex,
                        'btn-info': page == this.payments.PageIndex,
                    };
                };
                Incoming.prototype.displayDate = function (date) {
                    if (Object.isNullOrUndefined(date))
                        return null;
                    else
                        return date.toMoment().format('YYYY/M/D  H:m');
                };
                Incoming.prototype.getTransactionSender = function (trnx) {
                    if (Object.isNullOrUndefined(trnx.Sender))
                        return '-';
                    else if (Object.isNullOrUndefined(trnx.Sender.OwnerRef))
                        return trnx.Sender.BlockChainAddress;
                    else if (Object.isNullOrUndefined(trnx.Sender.OwnerRef.UserBio))
                        return trnx.Sender.OwnerRef.ReferenceCode;
                    else {
                        var bio = trnx.Sender.OwnerRef.UserBio;
                        return (bio.FirstName || '') + ' ' + (bio.LastName || '');
                    }
                };
                Incoming.prototype.verifyManually = function (trnx) {
                    var _this = this;
                    if (!this.isVerifyingTransaction) {
                        this.isVerifyingTransaction = true;
                        this.__blockChain.verifyManually(trnx.TransactionHash).then(function (opr) {
                            _this.__notify.success('Transaction verified successfully.');
                        }, function (err) {
                            _this.__notify.error('Something went wrong - could not verify the transaction.', 'Oops!');
                        }).finally(function () {
                            _this.isVerifyingTransaction = false;
                        });
                    }
                };
                return Incoming;
            }());
            Payments.Incoming = Incoming;
            var Outgoing = (function () {
                function Outgoing(__blockChain, __userContext, __notify, $q) {
                    this.pageSize = 20;
                    this.__blockChain = __blockChain;
                    this.__userContext = __userContext;
                    this.__notify = __userContext;
                    this.$q = $q;
                    //load the initial view
                    this.loadHistory(0, this.pageSize);
                }
                Outgoing.prototype.loadHistory = function (index, size) {
                    var _this = this;
                    this.isLoadingView = true;
                    return this.__blockChain.getPagedOutgoingTransactions(index, size || this.pageSize || 30).then(function (opr) {
                        if (!Object.isNullOrUndefined(opr.Result)) {
                            opr.Result.Page = opr.Result.Page.map(function (trnx) {
                                trnx.CreatedOn = new Apollo.Models.JsonDateTime(trnx.CreatedOn);
                                trnx.ModifiedOn = new Apollo.Models.JsonDateTime(trnx.ModifiedOn);
                                return trnx;
                            });
                            _this.payments = new BitDiamond.Utils.SequencePage(opr.Result.Page, opr.Result.SequenceLength, opr.Result.PageSize, opr.Result.PageIndex);
                        }
                        else {
                            _this.payments = new BitDiamond.Utils.SequencePage([], 0, 0, 0);
                        }
                        _this.pageLinks = _this.payments.AdjacentIndexes(2);
                        return _this.$q.resolve(opr.Result);
                    }, function (err) {
                        _this.__notify.error('Something went wrong - couldn\'t load your history.', 'Oops!');
                    }).finally(function () {
                        _this.isLoadingView = false;
                    });
                };
                Outgoing.prototype.canVerify = function (trnx) {
                    return true;
                };
                Outgoing.prototype.loadLastPage = function () {
                    this.loadHistory(this.payments.PageCount - 1, this.pageSize);
                };
                Outgoing.prototype.loadFirstPage = function () {
                    this.loadHistory(0, this.pageSize);
                };
                Outgoing.prototype.loadLinkPage = function (pageIndex) {
                    this.loadHistory(pageIndex, this.pageSize);
                };
                Outgoing.prototype.linkButtonClass = function (page) {
                    return {
                        'btn-outline': page != this.payments.PageIndex,
                        'btn-default': page != this.payments.PageIndex,
                        'btn-info': page == this.payments.PageIndex,
                    };
                };
                Outgoing.prototype.displayDate = function (date) {
                    if (Object.isNullOrUndefined(date))
                        return null;
                    else
                        return date.toMoment().format('YYYY/M/D  H:m');
                };
                Outgoing.prototype.getTransactionReceiver = function (trnx) {
                    if (Object.isNullOrUndefined(trnx.Receiver))
                        return '-';
                    else if (Object.isNullOrUndefined(trnx.Receiver.OwnerRef))
                        return trnx.Receiver.BlockChainAddress;
                    else if (Object.isNullOrUndefined(trnx.Receiver.OwnerRef.UserBio))
                        return trnx.Receiver.OwnerRef.ReferenceCode;
                    else {
                        var bio = trnx.Receiver.OwnerRef.UserBio;
                        return (bio.FirstName || '') + ' ' + (bio.LastName || '');
                    }
                };
                Outgoing.prototype.transactionStatus = function (trnx) {
                    return BitDiamond.Models.BlockChainTransactionStatus[trnx.Status];
                };
                Outgoing.prototype.transactionStatusClass = function (trnx) {
                    return {
                        'text-warning': trnx.Status == BitDiamond.Models.BlockChainTransactionStatus.Unverified,
                        'text-success': trnx.Status == BitDiamond.Models.BlockChainTransactionStatus.Verified
                    };
                };
                return Outgoing;
            }());
            Payments.Outgoing = Outgoing;
        })(Payments = Controllers.Payments || (Controllers.Payments = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
