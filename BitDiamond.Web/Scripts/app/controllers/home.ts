
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
            var hasError = false;
            if (Object.isNullOrUndefined(this.email) && (hasError = true)) this.__notify.error('Your must provide your email addres', 'Oops!');
            if (Object.isNullOrUndefined(this.subject) && (hasError = true)) this.__notify.error('Your must provide a subject', 'Oops!');
            if (Object.isNullOrUndefined(this.message) && (hasError = true)) this.__notify.error('Your must provide a message', 'Oops!');

            if (hasError) return;

            this.__notification.notifySupport(this.firstName, this.lastName, this.email, this.subject, this.message).then(opr => {
                this.__notify.success('Your message was sent to Support.');
            }, err => {
                this.__notify.error('Something happened', 'Oops');
            });
        }

        __blockChain: Services.BlockChain;
        __account: Services.Account;
        __notification: Services.Notification;
        __notify: Utils.Services.NotifyService;
        $q: ng.IQService;


        constructor(__blockChain, __account, __notification, __notify, $q) {
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