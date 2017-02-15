var BitDiamond;
(function (BitDiamond) {
    var Utils;
    (function (Utils) {
        var TimeSpan = (function () {
            function TimeSpan(milisecs) {
                this.Hours = 0;
                this.Minutes = 0;
                this.Seconds = 0;
                this.Milliseconds = 0;
                if (milisecs) {
                    var value = milisecs;
                    //mili seconds
                    this.Milliseconds = value % 1000;
                    value = Math.ceil((value - this.Milliseconds) / 1000);
                    //seconds
                    this.Seconds = value % 60;
                    value = Math.ceil((value - this.Seconds) / 60);
                    //minutes
                    this.Minutes = value % 60;
                    value = Math.ceil((value - this.Minutes) / 60);
                    //hours
                    this.Hours = value % 60;
                }
            }
            TimeSpan.prototype.add = function (value) {
                if (!value)
                    return this;
                return new TimeSpan(this.totalMilliseconds() + value.totalMilliseconds());
            };
            TimeSpan.prototype.subtract = function (value) {
                if (!value)
                    return this;
                return new TimeSpan(this.totalMilliseconds() - value.totalMilliseconds());
            };
            TimeSpan.prototype.totalMilliseconds = function () {
                return (((((this.Hours * 60) + this.Minutes) * 60) + this.Seconds) * 1000) + this.Milliseconds;
            };
            return TimeSpan;
        }());
        Utils.TimeSpan = TimeSpan;
    })(Utils = BitDiamond.Utils || (BitDiamond.Utils = {}));
})(BitDiamond || (BitDiamond = {}));
