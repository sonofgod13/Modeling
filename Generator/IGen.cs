using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneratorSubsystem
{
    public interface IGen
    {
        int[] generateForDay();
        double[] generateN(int n);
        double getProbability(double x);
    }    

}
