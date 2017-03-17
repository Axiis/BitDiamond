
module BitDiamond.Controllers.Home {

    export class Landing {

        currentYear: string;
        totalUsers: string;
        totalTransactions: string;
        isLoadingTransactions: boolean;
        isLoadingUsers: boolean;

        firstName: string;
        lastName: string;
        subject: string;
        email: string;
        message: string;

        sendMessage() {
            //validate message...
            this.__notification.notifySupport(this.firstName, this.lastName, this.email, this.subject, this.message).then(opr => {
            }, err => {
            });
        }

        __blockChain: Services.BlockChain;
        __account: Services.Account;
        __notification: Services.Notification;
        $q: ng.IQService;


        constructor(__blockChain, __account, __notification, $q) {
            this.__blockChain = __blockChain;
            this.__account = __account;
            this.$q = $q;

            this.currentYear = moment().format('YYYY');

            //note that after the initial loading, set a 1minute timer to refresh the user count, but without the boxloader
            this.isLoadingUsers = true;
            this.__account.getUserCount().then(opr => {
                this.totalUsers = opr.Result.toString();
            }, err => {
                this.totalUsers = '-';
            }).finally(() => this.isLoadingUsers = false);

            //note that after the initial loading, set a 1 minute timer to refresh the transaction total, but without the boxloader
            this.isLoadingTransactions = true;
            this.__blockChain.getSystemTransactionTotal().then(opr => {
                this.totalTransactions = opr.Result.toString();
            }, err => {
                this.totalTransactions = '-';
            }).finally(() => this.isLoadingTransactions = false);
        }

    }

}