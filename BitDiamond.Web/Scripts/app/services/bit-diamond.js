var BitDiamond;
(function (BitDiamond) {
    var Services;
    (function (Services) {
        var Account = (function () {
            function Account(__transport, $q) {
                this.__transport = __transport;
                this.$q = $q;
            }
            Account.prototype.getCurrentUserRef = function () {
                return this.__transport.get('/api/referrals/current');
            };
            Account.prototype.registerUser = function (targetUser, referrer, credential) {
                return this.__transport.post('/api/accounts/users', {
                    TargetUser: targetUser,
                    Referrer: referrer,
                    Credential: credential
                });
            };
            Account.prototype.registerAdmin = function (targetUser, credential) {
                return this.__transport.post('/api/accounts/admins', {
                    TargetUser: targetUser,
                    Credential: credential
                });
            };
            Account.prototype.deactivateUser = function (targetUser) {
                return this.__transport.put('/api/accounts/users/deactivate', {
                    TargetUser: targetUser
                });
            };
            Account.prototype.blockUser = function (targetUser) {
                return this.__transport.put('/api/accounts/users/block', {
                    TargetUser: targetUser
                });
            };
            Account.prototype.requestUserActivation = function (targetUser) {
                return this.__transport.put('/api/accounts/users/activations', {
                    TargetUser: targetUser
                });
            };
            Account.prototype.verifyUserActivation = function (targetUser, token) {
                return this.__transport.put('/api/accounts/users/activations/verify', {
                    TargetUser: targetUser,
                    Token: token
                });
            };
            Account.prototype.requestPasswordReset = function (targetUser) {
                return this.__transport.put('/api/accounts/users/credentials/reset-tokens', {
                    TargetUser: targetUser,
                    Metadata: {
                        Name: 'Password',
                        Access: Pollux.Models.Access.Secret
                    }
                });
            };
            Account.prototype.verifyPasswordReset = function (targetUser, token, $new) {
                return this.__transport.put('/api/accounts/users/credentials/reset-tokens/verify', {
                    TargetUser: targetUser,
                    Token: token,
                    New: {
                        Value: BitDiamond.Utils.ToBase64String(BitDiamond.Utils.ToUTF8EncodedArray($new)),
                        Metadata: {
                            Name: 'Password',
                            Access: Pollux.Models.Access.Secret
                        }
                    }
                });
            };
            Account.prototype.modifyBiodata = function (data) {
                return this.__transport.put('/api/accounts/biodata', data);
            };
            Account.prototype.getBiodata = function () {
                return this.__transport.get('/api/accounts/biodata');
            };
            Account.prototype.modifyContactdata = function (data) {
                return this.__transport.put('/api/accounts/contacts', data);
            };
            Account.prototype.getContactdata = function () {
                return this.__transport.get('/api/accounts/contacts');
            };
            Account.prototype.addData = function (data) {
                return this.__transport.post('/api/accounts/userdata', {
                    Data: data
                });
            };
            Account.prototype.modifyData = function (data) {
                return this.__transport.put('/api/accounts/userdata', {
                    Data: data
                });
            };
            Account.prototype.removeData = function (names) {
                return this.__transport.delete('/api/accounts/userdata', {
                    Names: names
                });
            };
            Account.prototype.getUserData = function () {
                return this.__transport.get('/api/accounts/userdata');
            };
            Account.prototype.getUserDataByName = function (name) {
                return this.__transport.get('/api/accounts/userdata/filter', {
                    Name: name
                });
            };
            Account.prototype.updateProfileImage = function (data, oldUrl) {
                return this.__transport.put('/api/accounts/profile-images', {
                    Image: data.RawObjectForm(),
                    OldImageUrl: oldUrl
                });
            };
            Account.prototype.getUserRoles = function () {
                return this.__transport.get('/api/accounts/users/roles');
            };
            Account.prototype.getUser = function () {
                return this.__transport.get('/api/accounts/users/current');
            };
            Account.prototype.signin = function (email, password) {
                var oldToken = JSON.parse(window.localStorage.getItem(BitDiamond.Utils.Constants.Misc_OAuthTokenKey));
                var config = { headers: {} };
                if (!Object.isNullOrUndefined(oldToken)) {
                    window.localStorage.removeItem(BitDiamond.Utils.Constants.Misc_OAuthTokenKey);
                    config.headers['OAuthOldToken'] = oldToken.access_token;
                }
                return this.__transport.postUrlEncoded('/tokens', {
                    grant_type: 'password',
                    username: email,
                    password: password
                }, config).then(function (opr) {
                    window.localStorage.setItem(BitDiamond.Utils.Constants.Misc_OAuthTokenKey, JSON.stringify(opr));
                    return {
                        Succeeded: true
                    };
                });
            };
            Account.prototype.signout = function () {
                var tokenObj = JSON.parse(window.localStorage.getItem(BitDiamond.Utils.Constants.Misc_OAuthTokenKey));
                if (!Object.isNullOrUndefined(tokenObj)) {
                    return this.__transport.post('/api/accounts/users/logons/invalidate', {
                        Token: tokenObj.access_token
                    }).finally(function () {
                        window.localStorage.removeItem(BitDiamond.Utils.Constants.Misc_OAuthTokenKey);
                        window.location.href = '/account/index';
                    });
                }
            };
            return Account;
        }());
        Services.Account = Account;
        var Dashboard = (function () {
            function Dashboard(__transport, $q) {
                this.__transport = __transport;
                this.$q = $q;
            }
            return Dashboard;
        }());
        Services.Dashboard = Dashboard;
        var BitLevel = (function () {
            function BitLevel(__transport, $q) {
                this.__transport = __transport;
                this.$q = $q;
            }
            BitLevel.prototype.upgrade = function () {
                return this.__transport.post('/api/bit-levels/cycles', null);
            };
            BitLevel.prototype.updateTransactionHash = function (hash) {
                return this.__transport.put('/api/bit-levels/transactions/current', {
                    Hash: hash
                });
            };
            BitLevel.prototype.confirmUpgradeDonationTransaction = function () {
                return this.__transport.put('/api/bit-levels/transactions/current/confirm', null);
            };
            BitLevel.prototype.getCurrentUpgradeTransaction = function () {
                return this.__transport.get('/api/bit-levels/transactions/current');
            };
            BitLevel.prototype.currentLevel = function () {
                return this.__transport.get('/api/bit-levels/cycles/current');
            };
            BitLevel.prototype.getBitLevelById = function (id) {
                return this.__transport.get('/api/bit-levels/cycles');
            };
            BitLevel.prototype.getBitLevelHistory = function () {
                return this.__transport.get('/api/bit-levels/cycles/history');
            };
            BitLevel.prototype.getPagedBitLevelHistory = function (pageIndex, pageSize) {
                return this.__transport.get('/api/bit-levels/cycles/history/pages', {
                    PageSize: pageSize,
                    PageIndex: pageIndex
                });
            };
            BitLevel.prototype.getUpgradeFee = function (level) {
                return this.__transport.get('/api/bit-levels/upgrade-fees/' + level);
            };
            BitLevel.prototype.getAllBitcoinAddresses = function () {
                return this.__transport.get('/api/bit-levels/bitcoin-addresses');
            };
            BitLevel.prototype.getActiveBitcoinAddress = function () {
                return this.__transport.get('/api/bit-levels/bitcoin-addresses/active');
            };
            BitLevel.prototype.addBitcoinAddress = function (address) {
                return this.__transport.post('/api/bit-levels/bitcoin-addresses', address);
            };
            BitLevel.prototype.activateBitcoinAddress = function (id) {
                return this.__transport.put('/api/bit-levels/bitcoin-addresses/activate', {
                    Id: id
                });
            };
            BitLevel.prototype.deactivateBitcoinAddress = function (id) {
                return this.__transport.put('/api/bit-levels/bitcoin-addresses/deactivate', {
                    Id: id
                });
            };
            BitLevel.prototype.verifyBitcoinAddress = function (id) {
                return this.__transport.put('/api/bit-levels/bitcoin-addresses/verify', {
                    Id: id
                });
            };
            return BitLevel;
        }());
        Services.BitLevel = BitLevel;
        var BlockChain = (function () {
            function BlockChain(__transport, $q) {
                this.__transport = __transport;
                this.$q = $q;
            }
            BlockChain.prototype.getPagedIncomingTransactions = function (pageIndex, pageSize) {
                return this.__transport.get('/api/block-chain/transactions/incoming', {
                    PageSize: pageSize,
                    PageIndex: pageIndex
                });
                //return this.$q.resolve(<Utils.Operation<Utils.SequencePage<Models.IBlockChainTransaction>>>{
                //    Message: null,
                //    Succeeded: true,
                //    Result: new Utils.SequencePage<Models.IBlockChainTransaction>([
                //        <Models.IBlockChainTransaction>{
                //            Amount: Math.round(Math.random() * 1000),
                //            CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                //            LedgerCount: Math.round(Math.random() * 10),
                //            Receiver: {
                //                BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@dev.bankai-554'
                //                }
                //            },
                //            Sender: {
                //                BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@apex-001'
                //                }
                //            },
                //            Status: BitDiamond.Models.BlockChainTransactionStatus.Verified,
                //            TransactionHash: Math.random() + ''
                //        },
                //        <Models.IBlockChainTransaction>{
                //            Amount: Math.round(Math.random() * 1000),
                //            CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                //            LedgerCount: Math.round(Math.random() * 10),
                //            Receiver: {
                //                BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@dev.bankai-554'
                //                }
                //            },
                //            Sender: {
                //                BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@apex-001'
                //                }
                //            },
                //            Status: BitDiamond.Models.BlockChainTransactionStatus.Unverified,
                //            TransactionHash: Math.random() + ''
                //        },
                //        <Models.IBlockChainTransaction>{
                //            Amount: Math.round(Math.random() * 1000),
                //            CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                //            LedgerCount: Math.round(Math.random() * 10),
                //            Receiver: {
                //                BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@dev.bankai-554'
                //                }
                //            },
                //            Sender: {
                //                BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@apex-001'
                //                }
                //            },
                //            Status: BitDiamond.Models.BlockChainTransactionStatus.Unverified,
                //            TransactionHash: Math.random() + ''
                //        },
                //        <Models.IBlockChainTransaction>{
                //            Amount: Math.round(Math.random() * 1000),
                //            CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                //            LedgerCount: Math.round(Math.random() * 10),
                //            Receiver: {
                //                BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@dev.bankai-554'
                //                }
                //            },
                //            Sender: {
                //                BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@apex-001'
                //                }
                //            },
                //            Status: BitDiamond.Models.BlockChainTransactionStatus.Verified,
                //            TransactionHash: Math.random() + ''
                //        },
                //        <Models.IBlockChainTransaction>{
                //            Amount: Math.round(Math.random() * 1000),
                //            CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                //            LedgerCount: Math.round(Math.random() * 10),
                //            Receiver: {
                //                BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@dev.bankai-554'
                //                }
                //            },
                //            Sender: {
                //                BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@apex-001'
                //                }
                //            },
                //            Status: BitDiamond.Models.BlockChainTransactionStatus.Verified,
                //            TransactionHash: Math.random() + ''
                //        }
                //        ],10, 5, 0)
                //});
            };
            BlockChain.prototype.getPagedOutgoingTransactions = function (pageIndex, pageSize) {
                return this.__transport.get('/api/block-chain/transactions/outgoing', {
                    PageSize: pageSize,
                    PageIndex: pageIndex
                });
                //return this.$q.resolve(<Utils.Operation<Utils.SequencePage<Models.IBlockChainTransaction>>>{
                //    Message: null,
                //    Succeeded: true,
                //    Result: new Utils.SequencePage<Models.IBlockChainTransaction>([
                //        <Models.IBlockChainTransaction>{
                //            Amount: Math.round(Math.random() * 1000),
                //            CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                //            LedgerCount: Math.round(Math.random() * 10),
                //            Receiver: {
                //                BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@dev.bankai-554'
                //                }
                //            },
                //            Sender: {
                //                BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@apex-001'
                //                }
                //            },
                //            Status: BitDiamond.Models.BlockChainTransactionStatus.Verified,
                //            TransactionHash: Math.random() + ''
                //        },
                //        <Models.IBlockChainTransaction>{
                //            Amount: Math.round(Math.random() * 1000),
                //            CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                //            LedgerCount: Math.round(Math.random() * 10),
                //            Receiver: {
                //                BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@dev.bankai-554'
                //                }
                //            },
                //            Sender: {
                //                BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@apex-001'
                //                }
                //            },
                //            Status: BitDiamond.Models.BlockChainTransactionStatus.Unverified,
                //            TransactionHash: Math.random() + ''
                //        },
                //        <Models.IBlockChainTransaction>{
                //            Amount: Math.round(Math.random() * 1000),
                //            CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                //            LedgerCount: Math.round(Math.random() * 10),
                //            Receiver: {
                //                BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@dev.bankai-554'
                //                }
                //            },
                //            Sender: {
                //                BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@apex-001'
                //                }
                //            },
                //            Status: BitDiamond.Models.BlockChainTransactionStatus.Unverified,
                //            TransactionHash: Math.random() + ''
                //        },
                //        <Models.IBlockChainTransaction>{
                //            Amount: Math.round(Math.random() * 1000),
                //            CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                //            LedgerCount: Math.round(Math.random() * 10),
                //            Receiver: {
                //                BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@dev.bankai-554'
                //                }
                //            },
                //            Sender: {
                //                BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@apex-001'
                //                }
                //            },
                //            Status: BitDiamond.Models.BlockChainTransactionStatus.Verified,
                //            TransactionHash: Math.random() + ''
                //        },
                //        <Models.IBlockChainTransaction>{
                //            Amount: Math.round(Math.random() * 1000),
                //            CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                //            LedgerCount: Math.round(Math.random() * 10),
                //            Receiver: {
                //                BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@dev.bankai-554'
                //                }
                //            },
                //            Sender: {
                //                BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                //                IsActive: true,
                //                IsVerified: true,
                //                OwnerRef: {
                //                    ReferenceCode: '@apex-001'
                //                }
                //            },
                //            Status: BitDiamond.Models.BlockChainTransactionStatus.Verified,
                //            TransactionHash: Math.random() + ''
                //        }
                //    ], 10, 5, 0)
                //});
            };
            BlockChain.prototype.verifyManually = function (transactionHash) {
                return this.__transport.put('/api/block-chain/transactions/incoming', {
                    TransactionHash: transactionHash
                });
            };
            return BlockChain;
        }());
        Services.BlockChain = BlockChain;
        var Referrals = (function () {
            function Referrals(__transport, $q) {
                this.__transport = __transport;
                this.$q = $q;
            }
            Referrals.prototype.getDirectDownlines = function (refCode) {
                return this.__transport.get('/api/referrals/downlines/direct', {
                    ReferenceCode: refCode
                });
            };
            Referrals.prototype.getAllDownlines = function (refCode) {
                return this.__transport.get('/api/referrals/downlines', {
                    ReferenceCode: refCode
                });
            };
            Referrals.prototype.getUplines = function (refCode) {
                return this.__transport.get('/api/referrals/uplines', {
                    ReferenceCode: refCode
                });
            };
            return Referrals;
        }());
        Services.Referrals = Referrals;
        var Notification = (function () {
            function Notification(__transport, $q) {
                this.__transport = __transport;
                this.$q = $q;
            }
            Notification.prototype.clearNotification = function (id) {
                return this.__transport.put('/api/notifications/single', {
                    Id: id
                });
            };
            Notification.prototype.clearAll = function () {
                return this.__transport.put('/api/notifications', null);
            };
            Notification.prototype.getNotificationHistory = function () {
                return this.__transport.get('/api/notifications');
            };
            Notification.prototype.getUnseenNotificaftions = function () {
                return this.__transport.get('/api/notifications/unseen');
            };
            Notification.prototype.getPagedNotificationHistory = function (pageIndex, pageSize) {
                return this.__transport.get('/api/notifications/paged', {
                    PageIndex: pageIndex,
                    PageSize: pageSize
                });
                //return this.$q.resolve(<Utils.Operation<Utils.SequencePage<Models.INotification>>>{
                //    Succeeded: true,
                //    Message: null,
                //    Result: new Utils.SequencePage<Models.INotification>([
                //        {
                //            Id: 0,
                //            Message: 'some message',
                //            Seen: true,
                //            Type: Models.NotificationType.Error,
                //            CreatedOn: new Apollo.Models.JsonDateTime(new Date().getTime())
                //        } as Models.INotification,
                //        {
                //            Id: 0,
                //            Message: 'some message2',
                //            Seen: false,
                //            Type: Models.NotificationType.Info,
                //            CreatedOn: new Apollo.Models.JsonDateTime(new Date().getTime())
                //        } as Models.INotification,
                //        {
                //            Id: 0,
                //            Message: 'some message3',
                //            Seen: false,
                //            Type: Models.NotificationType.Success,
                //            CreatedOn: new Apollo.Models.JsonDateTime(new Date().getTime())
                //        } as Models.INotification,
                //        {
                //            Id: 0,
                //            Message: 'some message4',
                //            Seen: false,
                //            Type: Models.NotificationType.Warning,
                //            CreatedOn: new Apollo.Models.JsonDateTime(new Date().getTime())
                //        } as Models.INotification,
                //        {
                //            Id: 0,
                //            Message: 'some message5',
                //            Seen: true,
                //            Type: Models.NotificationType.Error,
                //            CreatedOn: new Apollo.Models.JsonDateTime(new Date().getTime())
                //        } as Models.INotification
                //    ], 10, 5, 0)
                //});
            };
            return Notification;
        }());
        Services.Notification = Notification;
    })(Services = BitDiamond.Services || (BitDiamond.Services = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=bit-diamond.js.map