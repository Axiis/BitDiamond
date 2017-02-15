
module BitDiamond.Utils {

    export interface Operation<R> {

        Result: R;
        Succeeded: boolean;
        Message: string;

        //constructor(initArg?: Object) {
        //    if (initArg) initArg.copyTo(this);
        //}
    }
}