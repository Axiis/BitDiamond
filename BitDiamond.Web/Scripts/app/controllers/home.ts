
module BitDiamond.Controllers.Home {

    export class Landing {

        currentYear: string;
        totalUsers: string = '-';
        totalTransactions: string = '-';
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

        refreshCounters() {
            this.isLoadingTransactions = true;
            this.isLoadingUsers = true;
            return this.__blockChain.getSystemTransactionTotal().then(opr => {
                this.totalTransactions = opr.Result.toString();
                this.__account.getUserCount().then(opr => {
                    this.totalUsers = opr.Result.toString();
                }).finally(() => {
                    this.isLoadingUsers = false;
                });
            }).finally(() => {
                this.isLoadingTransactions = false;
            });
        }

        __blockChain: Services.BlockChain;
        __account: Services.Account;
        __notification: Services.Notification;
        __notify: Utils.Services.NotifyService;
        $q: ng.IQService;
        $interval: ng.IIntervalService;


        constructor(__blockChain, __account, __notification, __notify, $q, $interval) {
            this.__blockChain = __blockChain;
            this.__account = __account;
            this.$q = $q;
            this.$interval = $interval;

            this.currentYear = moment().format('YYYY');

            //note that after the initial loading, set a 1 minute timer to refresh the transaction total, but without the boxloader
            this.refreshCounters().then(() => {
                this.$interval(() => this.refreshCounters(), 30000);
            });
        }
    }

}