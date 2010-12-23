using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectionPerformanceTests
{
    public static class RandomSequence
    {
        static Random random = new Random();

        public static IEnumerable<int> GetSequenceOfInts()
        {
            while (true)
            {
                yield return random.Next(int.MaxValue);
            }
        }

        public static IEnumerable<double> GetSequenceOfDoubles()
        {
            while (true)
            {
                yield return random.NextDouble() * int.MaxValue;
            }
        }
    }
}
