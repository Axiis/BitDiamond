
module BitDiamond.Services {

    export class Account {

        registerUser(targetUser: string, referrer: string, credential: Pollux.Models.ICredential): ng.IPromise<Utils.Operation<Pollux.Models.IUser>> {
            return this.__transport.post<Utils.Operation<Pollux.Models.IUser>>('/api/accounts/users', {
                TargetUser: targetUser,
                Referrer: referrer,
                Credential: credential
            });
        }

        registerAdmin(targetUser: string, credential: Pollux.Models.ICredential): ng.IPromise<Utils.Operation<Pollux.Models.IUser>> {
            return this.__transport.post<Utils.Operation<Pollux.Models.IUser>>('/api/accounts/admins', {
                TargetUser: targetUser,
                Credential: credential
            });
        }

        deactivateUser(targetUser: string): ng.IPromise<Utils.Operation<void>> {
            return this.__transport.put<Utils.Operation<void>>('/api/accounts/users/deactivate', {
                TargetUser: targetUser
            });
        }

        blockUser(targetUser: string): ng.IPromise<Utils.Operation<void>> {
            return this.__transport.put<Utils.Operation<void>>('/api/accounts/users/block', {
                TargetUser: targetUser
            });
        }

        requestUserActivation(targetUser: string): ng.IPromise<Utils.Operation<void>> {
            return this.__transport.put<Utils.Operation<void>>('/api/accounts/users/activations', {
                TargetUser: targetUser
            });
        }

        verifyUserActivation(targetUser: string, token: string): ng.IPromise<Utils.Operation<Pollux.Models.IUser>> {
            return this.__transport.put<Utils.Operation<Pollux.Models.IUser>>('/api/accounts/users/activations/verify', {
                TargetUser: targetUser,
                Token: token
            });
        }

        requestPasswordReset(targetUser: string): ng.IPromise<Utils.Operation<void>> {
            return this.__transport.put<Utils.Operation<void>>('/api/accounts/users/credentials/reset-tokens', {
                TargetUser: targetUser,
                Metadata: {
                    Name: 'Password',
                    Access: Pollux.Models.Access.Secret
                }
            });
        }

        verifyPasswordReset(targetUser: string, token: string, $new: string): ng.IPromise<Utils.Operation<Pollux.Models.IUser>> {
            return this.__transport.put<Utils.Operation<Pollux.Models.IUser>>('/api/accounts/users/credentials/reset-tokens/verify', {
                TargetUser: targetUser,
                Token: token,
                New: <Pollux.Models.ICredential>{
                    Value: Utils.ToBase64String(Utils.ToUTF8EncodedArray($new)),
                    Metadata: {
                        Name: 'Password',
                        Access: Pollux.Models.Access.Secret
                    }
                }
            });
        }



        modifyBiodata(data: Pollux.Models.IBioData): ng.IPromise<Utils.Operation<Pollux.Models.IBioData>> {
            return this.__transport.put<Utils.Operation<Pollux.Models.IBioData>>('/api/accounts/biodata', data);
        }

        getBiodata(): ng.IPromise<Utils.Operation<Pollux.Models.IBioData>> {
            return this.__transport.get<Utils.Operation<Pollux.Models.IBioData>>('/api/accounts/biodata');
        }



        modifyContactdata(data: Pollux.Models.IContactData): ng.IPromise<Utils.Operation<Pollux.Models.IContactData>> {
            return this.__transport.put<Utils.Operation<Pollux.Models.IContactData>>('/api/accounts/contacts', data);
        }

        getContactdata(): ng.IPromise<Utils.Operation<Pollux.Models.IContactData>> {
            return this.__transport.get<Utils.Operation<Pollux.Models.IContactData>>('/api/accounts/contacts');
        }



        addData(data: Pollux.Models.IUserData[]): ng.IPromise<Utils.Operation<Pollux.Models.IUserData>> {
            return this.__transport.post<Utils.Operation<Pollux.Models.IUserData>>('/api/accounts/userdata', {
                Data: data
            });
        }

        modifyData(data: Pollux.Models.IUserData[]): ng.IPromise<Utils.Operation<Pollux.Models.IUserData>> {
            return this.__transport.put<Utils.Operation<Pollux.Models.IUserData>>('/api/accounts/userdata', {
                Data: data
            });
        }

        removeData(names: string[]): ng.IPromise<Utils.Operation<Pollux.Models.IUserData>> {
            return this.__transport.delete<Utils.Operation<Pollux.Models.IUserData>>('/api/accounts/userdata', {
                Names: names
            });
        }

        getUserData(): ng.IPromise<Utils.Operation<Pollux.Models.IUserData[]>> {
            return this.__transport.get<Utils.Operation<Pollux.Models.IUserData[]>>('/api/accounts/userdata');
        }

        getUserDataByName(name: string): ng.IPromise<Utils.Operation<Pollux.Models.IUserData>> {
            return this.__transport.get<Utils.Operation<Pollux.Models.IUserData>>('/api/accounts/userdata/filter', {
                Name: name
            });
        }

        updateProfileImage(data: Utils.EncodedBinaryData, oldUrl: string): ng.IPromise<Utils.Operation<string>> {
            return this.__transport.put<Utils.Operation<string>>('/api/accounts/profile-images', {
                Image: data.RawObjectForm(),
                OldImageUrl: oldUrl
            });
        }


        getUserRoles(): ng.IPromise<Utils.Operation<string[]>>{
            return this.__transport.get<Utils.Operation<string[]>>('/api/accounts/users/roles');
        }

        getUser(): ng.IPromise<Utils.Operation<Pollux.Models.IUser>>{
            return this.__transport.get<Utils.Operation<Pollux.Models.IUser>>('/api/accounts/users/current');
        }


        signin(email: string, password: string): ng.IPromise<Utils.Operation<void>> {
            var oldToken = JSON.parse(window.localStorage.getItem(Utils.Constants.Misc_OAuthTokenKey)) as IBearerTokenResponse;
            var config: ng.IRequestShortcutConfig = { headers: {}};
            if (!Object.isNullOrUndefined(oldToken)) {
                window.localStorage.removeItem(Utils.Constants.Misc_OAuthTokenKey);
                config.headers['OAuthOldToken'] = oldToken.access_token;
            }

            return this.__transport.postUrlEncoded<IBearerTokenResponse>('/tokens', {
                grant_type: 'password',
                username: email,
                password: password
            }, config).then(opr => {
                window.localStorage.setItem(Utils.Constants.Misc_OAuthTokenKey, JSON.stringify(opr));
                return <Utils.Operation<void>>{
                    Succeeded: true
                };
            });
        }
        signout(): ng.IPromise<void> {
            var tokenObj: IBearerTokenResponse = JSON.parse(window.localStorage.getItem(Utils.Constants.Misc_OAuthTokenKey));
            if (!Object.isNullOrUndefined(tokenObj)) {
                return this.__transport.post<void>('/api/accounts/users/logons/invalidate', {
                    Token: tokenObj.access_token
                }).finally( () => {
                    window.localStorage.removeItem(Utils.Constants.Misc_OAuthTokenKey);
                    window.location.href = '/account/index';
                });
            }
        }


        __transport: Utils.Services.DomainTransport;
        $q: ng.IQService;
        constructor(__transport, $q) {
            this.__transport = __transport;
            this.$q = $q;
        }
    }
    

    export class Dashboard {


        __transport: Utils.Services.DomainTransport;
        $q: ng.IQService;
        constructor(__transport, $q) {
            this.__transport = __transport;
            this.$q = $q;
        }
    }


    export class BitLevel {

        upgrade(): ng.IPromise<Utils.Operation<Models.IBitLevel>> {
            return this.__transport.post<Utils.Operation<Models.IBitLevel>>('/api/bit-levels/cycles', null);
        }

        updateTransactionHash(hash: string): ng.IPromise<Utils.Operation<Models.IBlockChainTransaction>> {
            return this.__transport.put<Utils.Operation<Models.IBlockChainTransaction>>('/api/bit-levels/transactions/current', {
                Hash: hash
            });
        }

        confirmUpgradeDonationTransaction(): ng.IPromise<Utils.Operation<Models.IBlockChainTransaction>> {
            return this.__transport.put<Utils.Operation<Models.IBlockChainTransaction>>('/api/bit-levels/transactions/current/confirm', null);
        }

        getCurrentUpgradeTransaction(): ng.IPromise<Utils.Operation<Models.IBlockChainTransaction>> {
            return this.__transport.get<Utils.Operation<Models.IBlockChainTransaction>>('/api/bit-levels/transactions/current');
        }

        currentLevel(): ng.IPromise<Utils.Operation<Models.IBitLevel>> {
            return this.__transport.get<Utils.Operation<Models.IBitLevel>>('/api/bit-levels/cycles/current');
        }

        getBitLevelById(id: number): ng.IPromise<Utils.Operation<Models.IBitLevel>> {
            return this.__transport.get<Utils.Operation<Models.IBitLevel>>('/api/bit-levels/cycles');
        }

        getBitLevelHistory(): ng.IPromise<Utils.Operation<Models.IBitLevel[]>> {
            return this.__transport.get<Utils.Operation<Models.IBitLevel[]>>('/api/bit-levels/cycles/history');
        }

        getPagedBitLevelHistory(pageIndex: number, pageSize: number): ng.IPromise<Utils.Operation<Utils.SequencePage<Models.IBitLevel>>> {
            return this.__transport.get<Utils.Operation<Utils.SequencePage<Models.IBitLevel>>>('/api/bit-levels/cycles/history/pages', {
                PageSize: pageSize,
                PageIndex: pageIndex
            });
        }

        getUpgradeFee(level: number): ng.IPromise<Utils.Operation<number>> {
            return this.__transport.get<Utils.Operation<number>>('/api/bit-levels/upgrade-fees/' + level);
        }

        getUpgradeDonationReceiverRef(levelId: number): ng.IPromise<Utils.Operation<Models.IReferralNode>> {
            return this.__transport.get<Utils.Operation<Models.IReferralNode>>('/api/bit-levels/transactions/receivers', {
                Id: levelId
            });
        }

        receiverConfirmation(hash: string): ng.IPromise<Utils.Operation<void>> {
            return this.__transport.put<Utils.Operation<void>>('/api/bit-levels/transactions/receiver-confirmation', {
                Hash: hash
            });
        }

        getAllBitcoinAddresses(): ng.IPromise<Utils.Operation<Models.IBitcoinAddress[]>> {
            return this.__transport.get<Utils.Operation<Models.IBitcoinAddress[]>>('/api/bit-levels/bitcoin-addresses');
        }

        getActiveBitcoinAddress(): ng.IPromise<Utils.Operation<Models.IBitcoinAddress>> {
            return this.__transport.get<Utils.Operation<Models.IBitcoinAddress>>('/api/bit-levels/bitcoin-addresses/active');
        }

        addBitcoinAddress(address: Models.IBitcoinAddress): ng.IPromise<Utils.Operation<Models.IBitcoinAddress>> {
            return this.__transport.post<Utils.Operation<Models.IBitcoinAddress>>('/api/bit-levels/bitcoin-addresses', address);
        }

        activateBitcoinAddress(id: number): ng.IPromise<Utils.Operation<Models.IBitcoinAddress>> {
            return this.__transport.put<Utils.Operation<Models.IBitcoinAddress>>('/api/bit-levels/bitcoin-addresses/activate', {
                Id: id
            });
        }

        deactivateBitcoinAddress(id: number): ng.IPromise<Utils.Operation<Models.IBitcoinAddress>> {
            return this.__transport.put<Utils.Operation<Models.IBitcoinAddress>>('/api/bit-levels/bitcoin-addresses/deactivate', {
                Id: id
            });
        }

        verifyBitcoinAddress(id: number): ng.IPromise<Utils.Operation<Models.IBitcoinAddress>> {
            return this.__transport.put<Utils.Operation<Models.IBitcoinAddress>>('/api/bit-levels/bitcoin-addresses/verify', {
                Id: id
            });
        }


        private _maxBitLevel: ng.IPromise<Utils.Operation<number>>;
        __transport: Utils.Services.DomainTransport;
        $q: ng.IQService;

        constructor(__transport, $q) {
            this.__transport = __transport;
            this.$q = $q;

        }
    }


    export class BlockChain {

        getPagedIncomingTransactions(pageIndex: number, pageSize: number): ng.IPromise<Utils.Operation<Utils.SequencePage<Models.IBlockChainTransaction>>> {
            return this.$q.resolve(<Utils.Operation<Utils.SequencePage<Models.IBlockChainTransaction>>>{
                Message: null,
                Succeeded: true,
                Result: new Utils.SequencePage<Models.IBlockChainTransaction>([
                    <Models.IBlockChainTransaction>{
                        Amount: Math.round(Math.random() * 1000),
                        CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                        LedgerCount: Math.round(Math.random() * 10),
                        Receiver: {
                            BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@dev.bankai-554'
                            }
                        },
                        Sender: {
                            BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@apex-001'
                            }
                        },
                        Status: BitDiamond.Models.BlockChainTransactionStatus.Verified,
                        TransactionHash: Math.random() + ''
                    },
                    <Models.IBlockChainTransaction>{
                        Amount: Math.round(Math.random() * 1000),
                        CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                        LedgerCount: Math.round(Math.random() * 10),
                        Receiver: {
                            BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@dev.bankai-554'
                            }
                        },
                        Sender: {
                            BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@apex-001'
                            }
                        },
                        Status: BitDiamond.Models.BlockChainTransactionStatus.Unverified,
                        TransactionHash: Math.random() + ''
                    },
                    <Models.IBlockChainTransaction>{
                        Amount: Math.round(Math.random() * 1000),
                        CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                        LedgerCount: Math.round(Math.random() * 10),
                        Receiver: {
                            BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@dev.bankai-554'
                            }
                        },
                        Sender: {
                            BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@apex-001'
                            }
                        },
                        Status: BitDiamond.Models.BlockChainTransactionStatus.Unverified,
                        TransactionHash: Math.random() + ''
                    },
                    <Models.IBlockChainTransaction>{
                        Amount: Math.round(Math.random() * 1000),
                        CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                        LedgerCount: Math.round(Math.random() * 10),
                        Receiver: {
                            BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@dev.bankai-554'
                            }
                        },
                        Sender: {
                            BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@apex-001'
                            }
                        },
                        Status: BitDiamond.Models.BlockChainTransactionStatus.Verified,
                        TransactionHash: Math.random() + ''
                    },
                    <Models.IBlockChainTransaction>{
                        Amount: Math.round(Math.random() * 1000),
                        CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                        LedgerCount: Math.round(Math.random() * 10),
                        Receiver: {
                            BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@dev.bankai-554'
                            }
                        },
                        Sender: {
                            BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@apex-001'
                            }
                        },
                        Status: BitDiamond.Models.BlockChainTransactionStatus.Verified,
                        TransactionHash: Math.random() + ''
                    }
                    ],10, 5, 0)
            });
        }
        getPagedOutgoingTransactions(pageIndex: number, pageSize: number): ng.IPromise<Utils.Operation<Utils.SequencePage<Models.IBlockChainTransaction>>> {
            return this.$q.resolve(<Utils.Operation<Utils.SequencePage<Models.IBlockChainTransaction>>>{
                Message: null,
                Succeeded: true,
                Result: new Utils.SequencePage<Models.IBlockChainTransaction>([
                    <Models.IBlockChainTransaction>{
                        Amount: Math.round(Math.random() * 1000),
                        CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                        LedgerCount: Math.round(Math.random() * 10),
                        Receiver: {
                            BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@dev.bankai-554'
                            }
                        },
                        Sender: {
                            BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@apex-001'
                            }
                        },
                        Status: BitDiamond.Models.BlockChainTransactionStatus.Verified,
                        TransactionHash: Math.random() + ''
                    },
                    <Models.IBlockChainTransaction>{
                        Amount: Math.round(Math.random() * 1000),
                        CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                        LedgerCount: Math.round(Math.random() * 10),
                        Receiver: {
                            BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@dev.bankai-554'
                            }
                        },
                        Sender: {
                            BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@apex-001'
                            }
                        },
                        Status: BitDiamond.Models.BlockChainTransactionStatus.Unverified,
                        TransactionHash: Math.random() + ''
                    },
                    <Models.IBlockChainTransaction>{
                        Amount: Math.round(Math.random() * 1000),
                        CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                        LedgerCount: Math.round(Math.random() * 10),
                        Receiver: {
                            BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@dev.bankai-554'
                            }
                        },
                        Sender: {
                            BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@apex-001'
                            }
                        },
                        Status: BitDiamond.Models.BlockChainTransactionStatus.Unverified,
                        TransactionHash: Math.random() + ''
                    },
                    <Models.IBlockChainTransaction>{
                        Amount: Math.round(Math.random() * 1000),
                        CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                        LedgerCount: Math.round(Math.random() * 10),
                        Receiver: {
                            BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@dev.bankai-554'
                            }
                        },
                        Sender: {
                            BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@apex-001'
                            }
                        },
                        Status: BitDiamond.Models.BlockChainTransactionStatus.Verified,
                        TransactionHash: Math.random() + ''
                    },
                    <Models.IBlockChainTransaction>{
                        Amount: Math.round(Math.random() * 1000),
                        CreatedOn: new Apollo.Models.JsonDateTime(Date.now()),
                        LedgerCount: Math.round(Math.random() * 10),
                        Receiver: {
                            BlockChainAddress: 'me-43t-gfgt5-54354refd-543e',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@dev.bankai-554'
                            }
                        },
                        Sender: {
                            BlockChainAddress: 'apex-654356-ujy56-56yt543-564ed',
                            IsActive: true,
                            IsVerified: true,
                            OwnerRef: {
                                ReferenceCode: '@apex-001'
                            }
                        },
                        Status: BitDiamond.Models.BlockChainTransactionStatus.Verified,
                        TransactionHash: Math.random() + ''
                    }
                ], 10, 5, 0)
            });
        }

        verifyManually(transactionHash: string): ng.IPromise<Utils.Operation<void>> {
            return this.$q.resolve(<Utils.Operation<void>>{
                Succeeded: true,
                Result: null,
                Message: null
            });
        }


        __transport: Utils.Services.DomainTransport;
        $q: ng.IQService;
        constructor(__transport, $q) {
            this.__transport = __transport;
            this.$q = $q;
        }
    }
}