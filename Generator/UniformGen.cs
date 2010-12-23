using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelingDataTypes;

namespace GeneratorSubsystem
{
    public class UniformGen : AbstractGen
    {
        private double a;
        private double b;

        public UniformGen(double a, double b)
        {
            this.a = a;
            this.b = b;
        }

        public override int[] GenerateForDay()
        {
            var sequence = new List<int>();

            var r = new Random(Guid.NewGuid().GetHashCode());

            int x = r.Next(int.MaxValue);
            
            double y = a + (b - a) * (x / int.MaxValue);
            int sum = (int) Math.Round(y);

            sequence.Add(sum);

            while (sum <= Params.WORKDAY_MINUTES_NUMBER)
            {
                Math.DivRem(630360016 * x, int.MaxValue, out x);

                y = a + (b - a) * (x / int.MaxValue);

                int yR = (int)Math.Round(y);

                sum = sum + yR;
                if (sum <= Params.WORKDAY_MINUTES_NUMBER) 
                    sequence.Add(yR);
            }
            
            return sequence.ToArray();

        }

        public override IEnumerable<double> GenerateSequence()
        {
            var r = new Random(Guid.NewGuid().GetHashCode());

            long x = r.Next(int.MaxValue);

            yield return a + (b - a) * ((double)x / int.MaxValue);

            while (true)
            {
                Math.DivRem((long)630360016 * x, (long)int.MaxValue, out x);
                yield return a + (b - a) * ((double)x / int.MaxValue);
            }
        }

        public override double GetProbability(double x)
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
