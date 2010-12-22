using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelingDataTypes;

namespace GeneratorSubsystem
{
    public class ExponentialGen : AbstractGen
    {
        private double m;
        private UniformGen uGen;

        public ExponentialGen(double m)
        {
            this.m = m;
            this.uGen = new UniformGen(0, 1);
        }

        public override int[] GenerateForDay()
        {
            List<int> sequence = new List<int>();
            int suggNum = (int)Math.Round((double)CParams.WORKDAY_MINUTES_NUMBER / (0.8 * m));
            double[] uSeq = uGen.GenerateN(suggNum);
            int i = 0;
            int sum = 0;
            while (sum <= CParams.WORKDAY_MINUTES_NUMBER)
            {
                if (i == suggNum)
                {
                    uSeq = uGen.GenerateN(suggNum);
                    i = 0;
                }
                int x = (int)Math.Round((-1) * m * Math.Log(uSeq[i]));
                sum = sum + x;
                if (sum <= CParams.WORKDAY_MINUTES_NUMBER)
                {
                    i++;
                    sequence.Add(x);
                }

            }

            return sequence.ToArray();

        }

        public override IEnumerable<double> GenerateSequence()
        {
            foreach (var uniform in uGen.GenerateSequence())
            {
                yield return (-1) * m * Math.Log(uniform);
            }
        }

        public override double GetProbability(double x)
        {
            if (x >= 0)
            {
                return (1 - Math.Exp((-1) * m * x));
            }
            else
            {
                return 0;
            }
        }
    }
}
