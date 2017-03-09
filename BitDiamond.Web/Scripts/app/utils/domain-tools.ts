
module BitDiamond.Utils.Domain {

    export class BitCycle {
        level: number;
        cycle: number;

        previousCycle(): BitCycle {
            var lastLevel = (this.level - 1) % (Constants.Settings_MaxBitLevel + 1);
            if (this.cycle == 1 && lastLevel == Constants.Settings_MaxBitLevel) return null;
            else return new BitCycle({
                level: lastLevel,
                cycle: lastLevel == 3 ? this.cycle - 1 : this.cycle
            });
        }
        nextCycle(): BitCycle {
            var nextLevel = (this.level + 1) % (Constants.Settings_MaxBitLevel + 1);
            return new BitCycle({
                level: nextLevel,
                cycle: nextLevel == 0 ? this.cycle + 1 : this.cycle
            });
        }

        constructor(init?: any) {
            if (!Object.isNullOrUndefined(init)) {
                (<Object>init).copyTo(this);
            }
        }
    }

    export function GenerateReferenceCode(userId: string) {

        var isValidEmail = function (email: string): boolean {
            return Utils.EmailRegex.test(userId);
        };

        if (userId == Constants.SystemUsers_Apex) return userId + "-001";
        else if (!isValidEmail(userId)) throw "invalid userid";
        else {
            var parts = userId.split('@').filter(_p => _p != '');
            var hash = parts[1].asChars()
                .aggregate(17, (hash, next) => hash * 283 + next.charCodeAt(0));
            return '@' + parts[0] + '-' + hash;
        }
    }
}