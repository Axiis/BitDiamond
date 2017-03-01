var BitDiamond;
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var BitLevel;
        (function (BitLevel) {
            var Home = (function () {
                function Home(__bitlevel, __userContext, __notify, $q) {
                    var _this = this;
                    this.donationsMissed = 0;
                    this.donationsReceived = 0;
                    this.upgradeFee = 0;
                    this.__bitlevel = __bitlevel;
                    this.__usercontext = __userContext;
                    this.__notify = __notify;
                    this.$q = $q;
                    this.hasActiveBitcoinAddress = false;
                    this.isLoadingView = true;
                    this.__bitlevel.currentLevel().then(this.initState.bind(this), function (err) {
                        _this.__notify.error('Could not load Bit Level information.', 'Oops!');
                    }).finally(function () {
                        _this.isLoadingView = false;
                    });
                }
                Object.defineProperty(Home.prototype, "currentLevel", {
                    get: function () {
                        if (Object.isNullOrUndefined(this.cycle))
                            return 0;
                        return this.cycle.level;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(Home.prototype, "currentCycle", {
                    get: function () {
                        if (Object.isNullOrUndefined(this.cycle))
                            return 0;
                        return this.cycle.cycle;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(Home.prototype, "nextLevel", {
                    get: function () {
                        if (Object.isNullOrUndefined(this.cycle))
                            return '-';
                        else {
                            var next = this.cycle.nextCycle();
                            return next.cycle + '/' + next.level;
                        }
                    },
                    enumerable: true,
                    configurable: true
                });
                Home.prototype.cyclePercentage = function () {
                    return Math.round((this.currentLevel / BitDiamond.Utils.Constants.Settings_MaxBitLevel) * 100) + '%';
                };
                Home.prototype.upgradeLevel = function () {
                    var _this = this;
                    if (this.isUpgrading || !this.hasActiveBitcoinAddress)
                        return;
                    else {
                        this.isUpgrading = true;
                        this.__bitlevel.upgrade().then(this.initState.bind(this), function (err) {
                            swal({
                                text: 'Your upgrade failed. Please try again later.',
                                title: 'Oops!',
                                type: 'error'
                            });
                        }).finally(function () {
                            _this.isUpgrading = false;
                        });
                    }
                };
                Home.prototype.updateTransactionHash = function () {
                    var _this = this;
                    if (this.isUpdatingHash)
                        return;
                    else {
                        this.__bitlevel.updateTransactionHash(this.transactionHash).then(function (opr) {
                            _this.__notify.success('Your transaction hash was verified!');
                        }, function (err) {
                        }).finally(function () {
                            _this.isUpdatingHash = false;
                        });
                    }
                };
                Home.prototype.initState = function (opr) {
                    var _this = this;
                    this.bitLevel = opr.Result;
                    if (!Object.isNullOrUndefined(this.bitLevel)) {
                        this.hasBitLevel = true;
                        this.hasActiveBitcoinAddress = true;
                        this.hasTransactionHash = !Object.isNullOrUndefined(this.bitLevel.Donation.TransactionHash) || this.bitLevel.Donation.TransactionHash != '';
                        this.donationsMissed = this.bitLevel.SkipCount;
                        this.donationsReceived = this.bitLevel.DonationCount;
                        this.upgradeFee = this.bitLevel.Donation.Amount;
                        this.cycle = new BitDiamond.Utils.Domain.BitCycle({
                            level: this.bitLevel.Level,
                            cycle: this.bitLevel.Cycle
                        });
                        //load the donation receiver
                        this.__bitlevel.getUpgradeDonationReceiver(this.bitLevel.Id).then(function (opr) {
                            _this.receiver = opr.Result;
                            _this.receiverCode = BitDiamond.Utils.Domain.GenerateReferenceCode(_this.receiver.OwnerId);
                        }, function (err) {
                            _this.__notify.error('Could not load upgrade donation receiver information.', 'Oops!');
                        });
                    }
                    else {
                        this.hasBitLevel = false;
                        this.__bitlevel.getActiveBitcoinAddress().then(function (opr) {
                            if (!Object.isNullOrUndefined(opr.Result))
                                _this.hasActiveBitcoinAddress = true;
                        });
                    }
                    return this.$q.resolve({
                        Message: null,
                        Succeeded: true,
                        Result: this.bitLevel
                    });
                };
                return Home;
            }());
            BitLevel.Home = Home;
            var History = (function () {
                function History(__bitlevel, __userContext, __notify) {
                }
                return History;
            }());
            BitLevel.History = History;
            var BitcoinAddresses = (function () {
                function BitcoinAddresses(__bitlevel, __userContext, __notify, $q) {
                    var _this = this;
                    this.addresses = [];
                    this.__bitlevel = __bitlevel;
                    this.__userContext = __userContext;
                    this.__notify = __notify;
                    this.$q = $q;
                    //load the addresses
                    this.isLoadingView = true;
                    this.__bitlevel.getAllBitcoinAddresses().then(function (opr) {
                        if (Object.isNullOrUndefined(opr.Result))
                            _this.addresses = [];
                        else {
                            _this.addresses = opr.Result;
                            _this.initAddressList();
                        }
                    }, function (err) {
                        _this.__notify.error('Something went wrong - could not load your bitcoin addresses', 'Oops!');
                    }).finally(function () {
                        _this.isLoadingView = false;
                    });
                }
                Object.defineProperty(BitcoinAddresses.prototype, "isEditingAddress", {
                    //UI Flags
                    get: function () {
                        return !Object.isNullOrUndefined(this.tempAddress);
                    },
                    enumerable: true,
                    configurable: true
                });
                BitcoinAddresses.prototype.isActivatingAddress = function (address) {
                    return address['$__isActivating'];
                };
                BitcoinAddresses.prototype.isVerifyingAddress = function (address) {
                    return address['$__isVerifying'];
                };
                BitcoinAddresses.prototype.canShowActiveAddress = function () {
                    return !Object.isNullOrUndefined(this.activeAddress) && !this.isEditingAddress;
                };
                BitcoinAddresses.prototype.cardStateColor = function (address) {
                    return {
                        'border-left-color': !address.IsVerified ? '#999' : (address.IsVerified && !address.IsActive) ? '#ab8ce4' : address.IsActive ? '#00c292' : '#f0ad4e'
                    };
                };
                BitcoinAddresses.prototype.canActivate = function (address) {
                    return address.IsVerified && !address.IsActive;
                };
                BitcoinAddresses.prototype.canShowActivatedFlag = function (address) {
                    return address.IsVerified && address.IsActive;
                };
                BitcoinAddresses.prototype.canShowDeactivatedFlag = function (address) {
                    return address.IsVerified && !address.IsActive;
                };
                //events
                BitcoinAddresses.prototype.editNewAddress = function () {
                    this.tempAddress = {
                        BlockChainAddress: '',
                        Id: 0,
                        IsActive: false,
                        IsVerified: false,
                        Owner: null,
                        OwnerId: null
                    };
                };
                BitcoinAddresses.prototype.persistNewAddress = function () {
                    var _this = this;
                    if (this.isPersistingAddress)
                        return;
                    else {
                        this.isPersistingAddress = true;
                        this.__bitlevel.addBitcoinAddress(this.tempAddress).then(function (opr) {
                            _this.addresses.insert(0, _this.tempAddress);
                            _this.tempAddress = null;
                        }, function (err) {
                            _this.__notify.error('Something went wrong - could not save your new Address.', 'Ooops!');
                        }).finally(function () {
                            _this.isPersistingAddress = false;
                        });
                    }
                };
                BitcoinAddresses.prototype.cancelEdit = function () {
                    if (this.isPersistingAddress)
                        return;
                    this.tempAddress = null;
                };
                BitcoinAddresses.prototype.activateAddress = function (address) {
                    var _this = this;
                    if (this.isActivatingAddress(address))
                        return;
                    else {
                        address['$__isActivating'] = true;
                        this.__bitlevel.activateBitcoinAddress(address.Id).then(function (opr) {
                            address.IsActive = true;
                            if (!Object.isNullOrUndefined(_this.activeAddress)) {
                                _this.activeAddress.IsActive = false;
                                _this.addresses.push(_this.activeAddress);
                            }
                            _this.initAddressList();
                            _this.__notify.success('Your Bitcoin Address was activated.');
                        }, function (err) {
                            _this.__notify.error('Something went wrong - could not activate the bitcoin address', 'Oops!');
                        }).finally(function () {
                            delete address['$__isActivating'];
                        });
                    }
                };
                BitcoinAddresses.prototype.deactivateActiveAddress = function () {
                    var _this = this;
                    if (this.isDeactivatingActiveAddress ||
                        Object.isNullOrUndefined(this.activeAddress))
                        return;
                    else {
                        var address = this.activeAddress;
                        this.isDeactivatingActiveAddress = true;
                        this.__bitlevel.deactivateBitcoinAddress(address.Id).then(function (opr) {
                            address.IsActive = false;
                            _this.addresses.push(_this.activeAddress);
                            _this.initAddressList();
                            _this.__notify.success('Your Bitcoin Address was deactivated.');
                        }, function (err) {
                            _this.__notify.error('Something went wrong - could not deactivate the bitcoin address', 'Oops!');
                        }).finally(function () {
                            _this.isDeactivatingActiveAddress = false;
                        });
                    }
                };
                BitcoinAddresses.prototype.verifyAddress = function (address) {
                    var _this = this;
                    if (this.isVerifyingAddress(address))
                        return;
                    else {
                        address['$__isVerifying'] = true;
                        this.__bitlevel.verifyBitcoinAddress(address.Id).then(function (opr) {
                            address.IsVerified = true;
                            _this.__notify.success('Your Bitcoin Address was Verified.');
                        }, function (err) {
                            _this.__notify.error('Something went wrong - could not verify the bitcoin address', 'Oops!');
                        }).finally(function () {
                            delete address['$__isVerifying'];
                        });
                    }
                };
                //utils
                BitcoinAddresses.prototype.initAddressList = function () {
                    this.activeAddress = this.addresses.firstOrDefault(function (_addr) { return _addr.IsActive; });
                    this.addresses = this.addresses.filter(function (_addr) { return !_addr.IsActive; });
                };
                return BitcoinAddresses;
            }());
            BitLevel.BitcoinAddresses = BitcoinAddresses;
        })(BitLevel = Controllers.BitLevel || (Controllers.BitLevel = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=bitlevel.js.map