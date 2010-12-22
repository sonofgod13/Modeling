using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelingDataTypes;

namespace GeneratorSubsystem
{
    public class UniformGen : IGen
    {
        private double a;
        private double b;

        public UniformGen(double a, double b)
        {
            this.a = a;
            this.b = b;
        }

        public int[] GenerateForDay()
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());
            int maxValue = 2 ^ 31 - 1;
            int x = r.Next(maxValue);
            List<int> sequence = new List<int>();
            double y =a + (b - a) * (x / maxValue);
            int sum = (int) Math.Round(y);
            sequence.Add(sum);
            while (sum <= CParams.WORKDAY_MINUTES_NUMBER)
            {
                Math.DivRem(630360016 * x, maxValue, out x);
                y = a + (b - a) * (x / maxValue);
                int yR = (int)Math.Round(y);
                sum = sum + yR;
                if (sum <= CParams.WORKDAY_MINUTES_NUMBER) sequence.Add(yR);
            }
            
            return sequence.ToArray();

        }

        public double[] GenerateN(int n)
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());             
            int maxValue = (int)Math.Pow(2,31) - 1;
            long x = r.Next(maxValue);
            List<double> sequence = new List<double>();
            sequence.Add(a + (b - a) * ((double)x / maxValue));
            if (n > 1)
            {
                for (int i = 0; i < n - 1; i++)
                {
                    Math.DivRem((long)630360016 * x, (long)maxValue, out x);
                    sequence.Add(a + (b - a) * ((double)x / maxValue));
                }
            }
            return sequence.ToArray();
        }

        public double GetProbability(double x)
        {
            if (x >= b)
            {
                return 1;
            }
            else
            {
                if (x < a) return 0;
                else return ((x - a)/(b - a));
            }
        }
    }
}
