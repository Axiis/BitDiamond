using System;

namespace BitDiamond.Core.Utils
{
    public class BitCycle
    {
        public int Cycle { get; set; }
        public int Level { get; set; }

        public BitCycle Increment(int unit)
        {
            var scalar = (Cycle * 4) + Level;
            scalar += unit;
            return FromUnits(scalar);
        }

        public BitCycle Decrement(int unit)
        {
            var scalar = (Cycle * 4) + Level;
            scalar -= unit;
            return FromUnits(scalar);
        }

        public static BitCycle Create(int cycle, int level) => FromUnits((cycle * 4) + level);

        public static BitCycle FromUnits(int units) => new BitCycle
        {
            Level = units % 4,
            Cycle = units / 4
        };

        public override string ToString()
        {
            return $"{Cycle}/{Level}";
        }
    }
}
