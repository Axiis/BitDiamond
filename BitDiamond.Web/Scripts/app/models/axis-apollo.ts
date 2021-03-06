﻿
module Apollo.Models {

    export class JsonTimeSpan {
        days: number = 0;
        hours: number = 0;
        minutes: number = 0;
        seconds: number = 0;
        milliSeconds: number = 0;


        add(value: JsonTimeSpan): JsonTimeSpan {
            if (!value) return this;

            return new JsonTimeSpan(this.totalMilliseconds() + value.totalMilliseconds());
        }

        subtract(value: JsonTimeSpan): JsonTimeSpan {
            if (!value) return this;

            return new JsonTimeSpan(this.totalMilliseconds() - value.totalMilliseconds());
        }

        totalMilliseconds(): number {
            return ((((((this.days * 24) + this.hours * 60) + this.minutes) * 60) + this.seconds) * 1000) + this.milliSeconds;
        }

        constructor(initArg?: number | Object) {

            if (typeof initArg === 'number') {
                var value = initArg;

                //mili seconds
                this.milliSeconds = value % 1000;
                value = Math.ceil((value - this.milliSeconds) / 1000);

                //seconds
                this.seconds = value % 60;
                value = Math.ceil((value - this.seconds) / 60);

                //minutes
                this.minutes = value % 60;
                value = Math.ceil((value - this.minutes) / 60);

                //hours
                this.hours = value % 24;
                value = Math.ceil((value - this.hours) / 24);

                //days
                this.days = value;
            }
            else if (typeof initArg === 'object' && initArg) {
                (initArg as Object).copyTo(this);
            }
        }
    }

    //This object ALWAYS represents LOCAL time. UTC OFFSET exists to represent the number of minutes to add or subtract to/from the current
    //time to get back to UTC time. If utcOffset is 0, it means this object represents utc time.
    export class JsonDateTime {

        year: number = 0;
        month: number = 0;
        day: number = 0;
        hour: number = 0;
        minute: number = 0;
        second: number = 0;
        millisecond: number = 0;

        //Designates number of minutes to add (or subtract pending on polarity of number) to utc time to get current time.
        //a value of zero means we are in utc time, a value of 60 means we are in west-central-africa, berlin/germany, etc
        utcOffset: number = 0;

        toMoment(): moment.Moment {
            return moment({
                year: this.year,
                month: this.month - 1, //<-- moment uses a zero-indexed scale for months when initializing it this way
                day: this.day,
                hour: this.hour,
                minute: this.minute,
                second: this.second,
                millisecond: this.millisecond
            }).utcOffset(this.utcOffset);
        }


        static fromMoment(m: moment.Moment): JsonDateTime {

            if (m.isValid()) {
                var jdt = new JsonDateTime();
                jdt.year = m.year();
                jdt.month = m.month() + 1;
                jdt.day = m.date();
                jdt.hour = m.hour();
                jdt.minute = m.minute();
                jdt.second = m.second();
                jdt.millisecond = m.millisecond();
                jdt.utcOffset = m.utcOffset();

                return jdt;
            }
            else throw 'invalid moment object';
        }



        constructor(initArg?: number | Object) {

            if (typeof initArg === 'number') {
                JsonDateTime.fromMoment(moment(initArg as number)).copyTo(this);
            }
            else if (initArg) {
                initArg.copyTo(this);
            }
        }
    }
}