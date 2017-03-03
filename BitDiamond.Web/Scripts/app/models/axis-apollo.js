var Apollo;
(function (Apollo) {
    var Models;
    (function (Models) {
        var JsonTimeSpan = (function () {
            function JsonTimeSpan(initArg) {
                this.days = 0;
                this.hours = 0;
                this.minutes = 0;
                this.seconds = 0;
                this.milliSeconds = 0;
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
                    initArg.copyTo(this);
                }
            }
            JsonTimeSpan.prototype.add = function (value) {
                if (!value)
                    return this;
                return new JsonTimeSpan(this.totalMilliseconds() + value.totalMilliseconds());
            };
            JsonTimeSpan.prototype.subtract = function (value) {
                if (!value)
                    return this;
                return new JsonTimeSpan(this.totalMilliseconds() - value.totalMilliseconds());
            };
            JsonTimeSpan.prototype.totalMilliseconds = function () {
                return ((((((this.days * 24) + this.hours * 60) + this.minutes) * 60) + this.seconds) * 1000) + this.milliSeconds;
            };
            return JsonTimeSpan;
        }());
        Models.JsonTimeSpan = JsonTimeSpan;
        var JsonDateTime = (function () {
            function JsonDateTime(initArg) {
                this.year = 0;
                this.month = 0;
                this.day = 0;
                this.hour = 0;
                this.minute = 0;
                this.second = 0;
                this.millisecond = 0;
                if (typeof initArg === 'number') {
                    this.fromMoment(moment(initArg));
                }
                else if (initArg) {
                    initArg.copyTo(this);
                }
            }
            JsonDateTime.prototype.toMoment = function () {
                return moment.utc({
                    year: this.year,
                    month: this.month - 1,
                    day: this.day,
                    hour: this.hour,
                    minute: this.minute,
                    second: this.second,
                    millisecond: this.millisecond
                }).local();
            };
            JsonDateTime.prototype.fromMoment = function (m) {
                if (m.isValid()) {
                    m = m.utc();
                    this.year = m.year();
                    this.month = m.month() + 1;
                    this.day = m.date();
                    this.hour = m.hour();
                    this.minute = m.minute();
                    this.second = m.second();
                    this.millisecond = m.millisecond();
                    return this;
                }
                else
                    throw 'invalid moment object';
            };
            return JsonDateTime;
        }());
        Models.JsonDateTime = JsonDateTime;
    })(Models = Apollo.Models || (Apollo.Models = {}));
})(Apollo || (Apollo = {}));
//# sourceMappingURL=axis-apollo.js.map