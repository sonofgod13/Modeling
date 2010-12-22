using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelingDataTypes;

namespace GeneratorSubsystem
{
    public class NormalGen : AbstractGen
    {
        private double m;
        private double d;
        private UniformGen uGen;

        public NormalGen(double m, double d)
        {
            this.m = m;
            this.d = d;
            this.uGen = new UniformGen(0, 1);
        }

        private double erf(double x)
        {
            double a = (8 * (Math.PI - 3)) / (3 * Math.PI * (4 - Math.PI));
            double erf = (x/Math.Abs(x))* Math.Sqrt(1 - Math.Exp((-1)*x*x*((4/Math.PI + a*x*x)/(1 + a*x*x))));  
            return erf;
        }

        public override int[] GenerateForDay()
        {
            List<int> sequence = new List<int>();
            int sum = 0;
            while (sum <= CParams.WORKDAY_MINUTES_NUMBER)
            {
                int x = (int) Math.Round(((uGen.GenerateN(12)).Sum() - 6) * d + m);
                sum = sum + x;
                if (sum <= CParams.WORKDAY_MINUTES_NUMBER) sequence.Add(x);

            }
            
            return sequence.ToArray();

        }

        public override IEnumerable<double> GenerateSequence()
        {
            while (true)
            {
                yield return ((uGen.GenerateN(12)).Sum() - 6) * d + m;
            }
        }

        public override double GetProbability(double x)
        {
            return (0.5-this.erf((x-m)/d));            
        }
    }
}
