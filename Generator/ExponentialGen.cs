using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelingDataTypes;

namespace GeneratorSubsystem
{
    public class ExponentialGen : IGen
    {
        private double m;
        private UniformGen uGen;

        public ExponentialGen(double m)
        {
            this.m = m;
            this.uGen = new UniformGen(0, 1);
        }

        public int[] generateForDay()
        {
            List<int> sequence = new List<int>();
            int suggNum = (int)Math.Round((double)CParams.WORKDAY_MINUTES_NUMBER/(0.8*m));
            double[] uSeq = uGen.generateN(suggNum);
            int i = 0;
            int sum = 0;
            while (sum <= CParams.WORKDAY_MINUTES_NUMBER)
            {
                if (i == suggNum)
                {
                    uSeq = uGen.generateN(suggNum);
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

        public double[] generateN(int n)
        {            
            List<double> sequence = new List<double>();
            double[] uSeq = uGen.generateN(n);
            for (int i = 0; i < n; i++)
            {
                sequence.Add((-1)*m*Math.Log(uSeq[i]));
                
            }
            
            return sequence.ToArray();
        }

        public double getProbability(double x)
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
