
module BitDiamond.Controllers.Shared {

    export class Message {

        title: string;
        message: string;
        actionTitle: string;

        $state: ng.ui.IStateService;
        $stateParams: ng.ui.IStateParamsService;

        back() {
            this.$state.go(this.$stateParams['action']);
        }

        constructor($state, $stateParams) {
            this.$state = $state;
            this.$stateParams = $stateParams;

            this.title = this.$stateParams['title'];
            this.message = this.$stateParams['message'];
            this.actionTitle = this.$stateParams['actionTitle'];
        }
    }

}