using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneratorSubsystem
{
    public interface IGen
    {
        int[] GenerateForDay();
        double[] GenerateN(int n);
        double GetProbability(double x);
    }    

}
