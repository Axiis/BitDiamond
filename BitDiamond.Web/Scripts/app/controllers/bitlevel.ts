

module BitDiamond.Controllers.BitLevel {

    export class Home {

        donationsMissed: number = 0;
        donationsReceived: number = 0;
        cycle: Utils.Domain.BitCycle;
        bitLevel: Models.IBitLevel;
        hasBitLevel: boolean;
        hasActiveBitcoinAddress: boolean;
        upgradeFee: number = 0;
        receiver: Models.IReferralNode;
        transactionHash: string;
        isLoadingView: boolean;

        isUpgrading: boolean;
        isSavingTransactionHash: boolean;
        isVerifyingTransaction: boolean;

        __bitlevel: Services.BitLevel;
        __usercontext: Utils.Services.UserContext;
        __notify: Utils.Services.NotifyService;

        $q: ng.IQService;

        get currentLevel(): number {
            if (Object.isNullOrUndefined(this.cycle)) return 0;
            return this.cycle.level;
        }
        get currentCycle(): number {
            if (Object.isNullOrUndefined(this.cycle)) return 0;
            return this.cycle.cycle;
        }
        get nextLevel(): string {
            if (Object.isNullOrUndefined(this.cycle)) return '-';
            else {
                var next = this.cycle.nextCycle();
                return next.cycle + '/' + next.level;
            }
        }
        get hasTransactionHash(): boolean {
            return !Object.isNullOrUndefined(this.bitLevel) &&
                !Object.isNullOrUndefined(this.bitLevel.Donation) &&
                !Object.isNullOrUndefined(this.bitLevel.Donation.TransactionHash) &&
                this.bitLevel.Donation.TransactionHash.trim() != '';
        }
        get isAtMaxCycleLevel(): boolean {
            return !Object.isNullOrUndefined(this.bitLevel) &&
                this.bitLevel.Level == Utils.Constants.Settings_MaxBitLevel;
        }

        cyclePercentage(): string {
            return Math.round((this.currentLevel / Utils.Constants.Settings_MaxBitLevel) * 100) + '%';
        }
        receiverName(): string {
            if (Object.isNullOrUndefined(this.bitLevel)) return '-';
            else if (Object.isNullOrUndefined(this.bitLevel.Donation)) return '-';
            else if (Object.isNullOrUndefined(this.bitLevel.Donation.Receiver)) return '-';
            else if (Object.isNullOrUndefined(this.bitLevel.Donation.Receiver.OwnerRef)) return this.bitLevel.Donation.Receiver.OwnerId;
            else if (Object.isNullOrUndefined(this.bitLevel.Donation.Receiver.OwnerRef.UserBio)) return this.bitLevel.Donation.Receiver.OwnerId;
            else {
                var bio = this.bitLevel.Donation.Receiver.OwnerRef.UserBio;
                return bio.FirstName + ' ' + bio.LastName;
            }
        }
        receiverCode(): string {
            if (Object.isNullOrUndefined(this.bitLevel)) return '-';
            else if (Object.isNullOrUndefined(this.bitLevel.Donation)) return '-';
            else if (Object.isNullOrUndefined(this.bitLevel.Donation.Receiver)) return '-';
            else if (Object.isNullOrUndefined(this.bitLevel.Donation.Receiver.OwnerRef)) return this.bitLevel.Donation.Receiver.OwnerId;
            else return this.bitLevel.Donation.Receiver.OwnerRef.ReferenceCode;
        }
        receiverAddress(): string {
            if (Object.isNullOrUndefined(this.bitLevel)) return '-';
            else if (Object.isNullOrUndefined(this.bitLevel.Donation)) return '-';
            else if (Object.isNullOrUndefined(this.bitLevel.Donation.Receiver)) return '-';
            else return this.bitLevel.Donation.Receiver.BlockChainAddress;
        }


        upgradeLevel() {
            if (this.isUpgrading || !this.hasActiveBitcoinAddress) return;
            else {
                this.isUpgrading = true;
                this.__bitlevel.upgrade().then(this.initState.bind(this), err => {
                    swal({
                        text: 'Your upgrade failed. Please try again later.',
                        title: 'Oops!',
                        type: 'error'
                    });
                }).finally(() => {
                    this.isUpgrading = false;
                });
            }
        }
        updateTransactionHash() {
            if (this.isSavingTransactionHash) return;
            else {
                this.isSavingTransactionHash = true;
                this.__bitlevel.updateTransactionHash(this.transactionHash).then(opr => {
                    this.bitLevel.Donation = opr.Result;
                    this.transactionHash = null;
                    this.__notify.success('Your transaction hash was verified!');
                }, err => {
                    this.__notify.error('Something went wrong - could not save the transaction hash.', 'Oops!');
                }).finally(() => {
                    this.isSavingTransactionHash = false;
                });
            }
        }
        verifyTransaction() {
            if (this.isVerifyingTransaction) return;
            else {
                this.isVerifyingTransaction = true;
                this.__bitlevel.confirmUpgradeDonationTransaction().then(opr => {
                    this.bitLevel.Donation = opr.Result;
                    this.__notify.success('Your transaction hash was verified!');
                }, err => {
                    this.__notify.error('Something went wrong - could not verify your transaction.', 'Oops!');
                }).finally(() => {
                    this.isVerifyingTransaction = false;
                });
            }
        }
        verifiedTransactionLedgerCountClass() {
            if (!Object.isNullOrUndefined(this.bitLevel) &&
                !Object.isNullOrUndefined(this.bitLevel.Donation) &&
                this.bitLevel.Donation.LedgerCount >= 3) return { 'text-success': true };
            else return {};
        }
        isTransactionVerified() {
            if (!Object.isNullOrUndefined(this.bitLevel) &&
                !Object.isNullOrUndefined(this.bitLevel.Donation) &&
                this.bitLevel.Donation.Status == Models.BlockChainTransactionStatus.Verified) return true;
            else return false;
        }

        initState(opr: Utils.Operation<Models.IBitLevel>): ng.IPromise<Utils.Operation<Models.IBitLevel>> {
            this.bitLevel = opr.Result;
            if (!Object.isNullOrUndefined(this.bitLevel)) {
                this.hasBitLevel = true;
                this.hasActiveBitcoinAddress = true;
                this.donationsMissed = this.bitLevel.SkipCount;
                this.donationsReceived = this.bitLevel.DonationCount;
                this.upgradeFee = this.bitLevel.Donation.Amount;
                this.cycle = new Utils.Domain.BitCycle({
                    level: this.bitLevel.Level,
                    cycle: this.bitLevel.Cycle
                });
            }
            else {
                this.hasBitLevel = false;
                this.__bitlevel.getActiveBitcoinAddress().then(opr => {
                    if (!Object.isNullOrUndefined(opr.Result)) this.hasActiveBitcoinAddress = true;
                });
            }

            return this.$q.resolve(<Utils.Operation<Models.IBitLevel>>{
                Message: null,
                Succeeded: true,
                Result: this.bitLevel
            });
        }

        constructor(__bitlevel, __userContext, __notify, $q) {
            this.__bitlevel = __bitlevel;
            this.__usercontext = __userContext;
            this.__notify = __notify;
            this.$q = $q;

            this.hasActiveBitcoinAddress = false;
            this.isLoadingView = true;
            this.__bitlevel.currentLevel().then(this.initState.bind(this), err => {
                this.__notify.error('Could not load Bit Level information.', 'Oops!');
            }).finally(() => {
                this.isLoadingView = false;
            });
        }
    }



    export class History {

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



    export class BitcoinAddresses {

        __bitlevel: Services.BitLevel;
        __userContext: Utils.Services.UserContext;
        __notify: Utils.Services.NotifyService;
        $q: ng.IQService;
        
        tempAddress: Models.IBitcoinAddress;
        addresses: Models.IBitcoinAddress[] = [];
        activeAddress: Models.IBitcoinAddress;
        isPersistingAddress: boolean;
        isDeactivatingActiveAddress: boolean;
        isLoadingView: boolean;

        //UI Flags
        get isEditingAddress(): boolean {

            return !Object.isNullOrUndefined(this.tempAddress);
        }

        isActivatingAddress(address: Models.IBitcoinAddress): boolean {
            return address['$__isActivating'];
        }
        isVerifyingAddress(address: Models.IBitcoinAddress): boolean {
            return address['$__isVerifying'];
        }
        canShowActiveAddress(): boolean {
            return !Object.isNullOrUndefined(this.activeAddress) && !this.isEditingAddress;
        }
        cardStateColor(address: Models.IBitcoinAddress): any {
            return {
                'border-left-color': !address.IsVerified ? '#999' : (address.IsVerified && !address.IsActive) ? '#ab8ce4' : address.IsActive ? '#00c292' : '#f0ad4e'
            };
        }
        canActivate(address: Models.IBitcoinAddress): boolean {
            return address.IsVerified && !address.IsActive;
        }
        canShowActivatedFlag(address: Models.IBitcoinAddress): boolean {
            return address.IsVerified && address.IsActive;
        }
        canShowDeactivatedFlag(address: Models.IBitcoinAddress): boolean {
            return address.IsVerified && !address.IsActive;
        }

        //events
        editNewAddress() {
            this.tempAddress = {
                BlockChainAddress: '',
                Id: 0,
                IsActive: false,
                IsVerified: false,
                Owner: null,
                OwnerId: null
            } as Models.IBitcoinAddress;
        }
        persistNewAddress() {
            if (this.isPersistingAddress) return;
            else {
                this.isPersistingAddress = true;
                this.__bitlevel.addBitcoinAddress(this.tempAddress).then(opr => {
                    this.addresses.insert(0, opr.Result);
                    this.tempAddress = null;
                }, err => {
                    this.__notify.error('Something went wrong - could not save your new Address.', 'Ooops!');
                }).finally(() => {
                    this.isPersistingAddress = false;
                });
            }
        }
        cancelEdit() {
            if (this.isPersistingAddress) return;

            this.tempAddress = null;
        }
        activateAddress(address: Models.IBitcoinAddress) {
            if (this.isActivatingAddress(address)) return;
            else {
                address['$__isActivating'] = true;
                this.__bitlevel.activateBitcoinAddress(address.Id).then(opr => {
                    address.IsActive = true;

                    if (!Object.isNullOrUndefined(this.activeAddress)) {
                        this.activeAddress.IsActive = false;
                        this.addresses.push(this.activeAddress);
                    }
                    this.initAddressList();

                    this.__notify.success('Your Bitcoin Address was activated.');
                }, err => {
                    this.__notify.error('Something went wrong - could not activate the bitcoin address', 'Oops!');
                }).finally(() => {
                    delete address['$__isActivating'];
                });
            }
        }
        deactivateActiveAddress() {
            if (this.isDeactivatingActiveAddress ||
                Object.isNullOrUndefined(this.activeAddress)) return;

            else {
                var address = this.activeAddress;
                this.isDeactivatingActiveAddress = true;
                this.__bitlevel.deactivateBitcoinAddress(address.Id).then(opr => {
                    address.IsActive = false;

                    this.addresses.push(this.activeAddress);
                    this.initAddressList();

                    this.__notify.success('Your Bitcoin Address was deactivated.');
                }, err => {
                    this.__notify.error('Something went wrong - could not deactivate the bitcoin address', 'Oops!');
                    }).finally(() => {
                        this.isDeactivatingActiveAddress = false;
                });
            }
        }
        verifyAddress(address: Models.IBitcoinAddress) {

            if (this.isVerifyingAddress(address)) return;

            else {
                address['$__isVerifying'] = true;
                this.__bitlevel.verifyBitcoinAddress(address.Id).then(opr => {
                    address.IsVerified = true;
                    this.__notify.success('Your Bitcoin Address was Verified.');
                }, err => {
                    this.__notify.error('Something went wrong - could not verify the bitcoin address', 'Oops!');
                }).finally(() => {
                    delete address['$__isVerifying'];
                });
            }

        }

        //utils
        initAddressList() {
            this.activeAddress = this.addresses.firstOrDefault(_addr => _addr.IsActive);
            this.addresses = this.addresses.filter(_addr => !_addr.IsActive);
        }

        constructor(__bitlevel, __userContext, __notify, $q) {
            this.__bitlevel = __bitlevel;
            this.__userContext = __userContext;
            this.__notify = __notify;
            this.$q = $q;

            //load the addresses
            this.isLoadingView = true;
            this.__bitlevel.getAllBitcoinAddresses().then(opr => {
                if (Object.isNullOrUndefined(opr.Result)) this.addresses = [];
                else {
                    this.addresses = opr.Result;
                    this.initAddressList();
                }
            }, err => {
                this.__notify.error('Something went wrong - could not load your bitcoin addresses', 'Oops!');
            }).finally(() => {
                this.isLoadingView = false;
            });
        }
    }
}