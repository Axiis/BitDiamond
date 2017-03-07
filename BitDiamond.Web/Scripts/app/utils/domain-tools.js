var BitDiamond;
(function (BitDiamond) {
    var Utils;
    (function (Utils) {
        var Domain;
        (function (Domain) {
            var BitCycle = (function () {
                function BitCycle(init) {
                    if (!Object.isNullOrUndefined(init)) {
                        init.copyTo(this);
                    }
                }
                BitCycle.prototype.previousCycle = function () {
                    var lastLevel = (this.level - 1) % (Utils.Constants.Settings_MaxBitLevel + 1);
                    if (this.cycle == 1 && lastLevel == Utils.Constants.Settings_MaxBitLevel)
                        return null;
                    else
                        return new BitCycle({
                            level: lastLevel,
                            cycle: lastLevel == 3 ? this.cycle - 1 : this.cycle
                        });
                };
                BitCycle.prototype.nextCycle = function () {
                    var nextLevel = (this.level + 1) % (Utils.Constants.Settings_MaxBitLevel + 1);
                    return new BitCycle({
                        level: nextLevel,
                        cycle: nextLevel == 0 ? this.cycle + 1 : this.cycle
                    });
                };
                return BitCycle;
            }());
            Domain.BitCycle = BitCycle;
            function GenerateReferenceCode(userId) {
                var isValidEmail = function (email) {
                    return Utils.EmailRegex.test(userId);
                };
                if (userId == Utils.Constants.SystemUsers_Apex)
                    return userId + "-001";
                else if (!isValidEmail(userId))
                    throw "invalid userid";
                else {
                    var parts = userId.split('@').filter(function (_p) { return _p != ''; });
                    var hash = parts[1].asChars()
                        .reduce(17, function (hash, next) { return hash * 283 + next.charCodeAt(0); });
                    return '@' + parts[0] + '-' + hash;
                }
            }
            Domain.GenerateReferenceCode = GenerateReferenceCode;
        })(Domain = Utils.Domain || (Utils.Domain = {}));
    })(Utils = BitDiamond.Utils || (BitDiamond.Utils = {}));
})(BitDiamond || (BitDiamond = {}));
