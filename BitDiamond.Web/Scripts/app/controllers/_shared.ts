
module BitDiamond.Controllers.Shared {


    export class Message {
        message: string;
        title: string;
        actionState: string;
        actionTitle: string;
        hasNoActionTitle: boolean;

        $state: ng.ui.IStateService;

        action() {
            if (Object.isNullOrUndefined(this.actionState)) {
                window.location.href = "/index.html";
            }
            else {
                this.$state.go(this.actionState);
            }
        }

        constructor($state, $stateParams) {
            this.$state = $state;

            this.message = $stateParams['message'];
            this.title = $stateParams['title'];
            this.actionState = $stateParams['actionState'];
            this.actionTitle = $stateParams['actionTitle'];
            this.hasNoActionTitle = Object.isNullOrUndefined(this.actionTitle);
        }
    }

    export class NavBar {

    }
}