using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneratorSubsystem;

namespace GeneratorSubsystem
{
    public abstract class AbstractGen : IGen
    {
        public abstract int[] GenerateForDay();

        public abstract double GetProbability(double x);

        public abstract IEnumerable<double> GenerateSequence();

        public virtual double[] GenerateN(int n)
        {
            return this.GenerateSequence().Take(n).ToArray();
        }
    }
}
