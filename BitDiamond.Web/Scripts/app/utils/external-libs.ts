
//Sweet alert integration
declare function swal(config: any, callback?: (in1: boolean) => void): void;


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