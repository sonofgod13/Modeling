using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelingDataTypes;

namespace GeneratorSubsystem
{
    public class RayleighGen : AbstractGen
    {
        private double d;
        private UniformGen uGen;

        public RayleighGen(double d)
        {
            this.d = d;
            this.uGen = new UniformGen(0, 1);
        }

        public override int[] GenerateForDay()
        {
            List<int> sequence = new List<int>();
            int suggNum = (int)Math.Round((double)Params.WORKDAY_MINUTES_NUMBER/d);
            double[] uSeq = uGen.GenerateN(suggNum);
            int i = 0;
            int sum = 0;
            while (sum <= Params.WORKDAY_MINUTES_NUMBER)
            {
                if (i == suggNum)
                {
                    uSeq = uGen.GenerateN(suggNum);
                    i = 0;
                }
                int x = (int)Math.Round(d * Math.Sqrt((-2) * Math.Log(uSeq[i])));
                sum = sum + x;
                if (sum <= Params.WORKDAY_MINUTES_NUMBER)
                {
                    i++;
                    sequence.Add(x);
                }
            }
            
            return sequence.ToArray();

        }

        public override IEnumerable<double> GenerateSequence()
        {
            foreach (var currentUniform in uGen.GenerateSequence())
            {
                yield return d * Math.Sqrt((-2) * Math.Log(currentUniform));
            }
        }

        public override double GetProbability(double x)
        {
            return (x >= 0)
                ? (1 - Math.Exp(((-1) * x * x) / (2 * d * d)))
                : 0;
        }
    }
}
