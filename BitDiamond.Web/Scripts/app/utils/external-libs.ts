
//Sweet alert integration
declare function swal(config: ISweetAlertConfig, callback?: (in1: boolean) => void): void;
declare interface ISweetAlertConfig {    
    title: string,
    text?: string,
    type?: string,	//"warning", "error", "success", "info" and "input"
    allowEscapeKey?: boolean,
    customClass?: any,
    allowOutsideClick?: boolean,
    showCancelButton?: boolean, //If set to true, a "Cancel"- button will be shown, which the user can click on to dismiss the modal.
    showConfirmButton?: boolean, //	If set to false, the "OK/Confirm"- button will be hidden.Make sure you set a timer or set allowOutsideClick to true when using this, in order not to annoy the user.
    confirmButtonText?: string, //	Use this to change the text on the "Confirm"- button.If showCancelButton is set as true, the confirm button will automatically show "Confirm" instead of "OK".
    confirmButtonColor?: string, // Use this to change the background color of the "Confirm"- button(must be a HEX value).
    cancelButtonText?: string,	//	Use this to change the text on the "Cancel"- button.
    closeOnConfirm?: boolean,	//	Set to false if you want the modal to stay open even if the user presses the "Confirm"- button.This is especially useful if the function attached to the "Confirm"- button is another SweetAlert.
    closeOnCancel?:	boolean,	// Same as closeOnConfirm, but for the cancel button.
    imageUrl?: string, //Add a customized icon for the modal.Should contain a string with the path to the image.
    imageSize?: string,	//If imageUrl is set, you can specify imageSize to describes how big you want the icon to be in px.Pass in a string with two values separated by an "x".The first value is the width, the second is the height.
    timer?: number,	// Auto close timer of the modal.Set in ms(milliseconds).
    html?:	boolean,  //If set to true, will not escape title and text parameters. (Set to false if you're worried about XSS attacks.)
    animation?:	boolean, //	If set to false, the modal's animation will be disabled. Possible (string) values : pop (default when animation set to true), slide-from-top, slide-from-bottom
    inputType?: string,	//Change the type of the input field when using type: "input"(this can be useful if you want users to type in their password for example).
    inputPlaceholder?: string	//When using the input- type, you can specify a placeholder to help the user.
    inputValue?: string,	//Specify a default text value that you want your input to show when using type: "input"
    showLoaderOnConfirm?: boolean
}


//drop box password strength checker integration
declare interface IZxcvbnResult {
    guesses: number;
    guesses_log10: number;
    crack_times_seconds: {
        online_throttling_100_per_hour: number,
        online_no_throttling_10_per_second: number,
        offline_slow_hashing_1e4_per_second: number,
        offline_fast_hashing_1e10_per_second: number
    };
    crack_times_display: {
        online_throttling_100_per_hour: string,
        online_no_throttling_10_per_second: string,
        offline_slow_hashing_1e4_per_second: string,
        offline_fast_hashing_1e10_per_second: string
    };
    score: number; //0-4
    feedback: {
        warning: string,
        suggestions: string
    };
    sequence: string[];
    calc_time: number;
};
declare function zxcvbn(password: string, userInputs?: string[]): IZxcvbnResult;

//bearer token type
declare interface IBearerTokenResponse {
    access_token?: string,
    token_type?: string,
    expires_in?: number,
    refresh_token?: string,
    scope?: string,
    state?: number,

    error?: string,
    error_description?: string,
    error_uri?: string
};


//bootstrap treeview
declare interface ITreeViewNode {
    text: string,
    icon?: string,
    selectedIcon?: string,
    color?: string,
    backColor?: string,
    href?: string,
    selectable?: boolean,
    state?: {
        checked:  boolean,
        disabled: boolean,
        expanded: boolean,
        selected: boolean
    },
    tags?: string[],
    nodes?:  ITreeViewNode[]
}
declare interface ITreeViewOptions {
    data: ITreeViewNode[],
    showTags?: boolean
}

declare interface JQuery {
    treeview(options: ITreeViewOptions): JQuery;
} 