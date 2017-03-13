using System;

namespace BitDiamond.Core.Utils
{
    public class BitCycle
    {
        public int Cycle { get; set; }
        public int Level { get; set; }

        public static BitCycle operator +(BitCycle cycle, int unit)
        {
            var scalar = (cycle.Cycle * 4) + cycle.Level;
            scalar += unit;
            return FromUnits(scalar);
        }

        public static BitCycle operator -(BitCycle cycle, int unit)
        {
            var scalar = (cycle.Cycle * 4) + cycle.Level;
            scalar -= unit;
            return FromUnits(scalar);
        }

        public static BitCycle Create(int cycle, int level) => FromUnits(cycle * level);

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
