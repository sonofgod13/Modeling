using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelingDataTypes;

namespace GeneratorSubsystem
{
    public class GammaGen : AbstractGen
    {
        private double k;
        private double d;
        private UniformGen uGen;

        public GammaGen(double k, double d)
        {
            this.k = k;
            this.d = d;
            this.uGen = new UniformGen(0, 1);
        }

        #region Взято с Alglib

        private double lngamma(double x, ref double sgngam)
        {
            double result = 0;
            double a = 0;
            double b = 0;
            double c = 0;
            double p = 0;
            double q = 0;
            double u = 0;
            double w = 0;
            double z = 0;
            int i = 0;
            double logpi = 0;
            double ls2pi = 0;
            double tmp = 0;

            sgngam = 0;

            sgngam = 1;
            logpi = 1.14472988584940017414;
            ls2pi = 0.91893853320467274178;
            if ((double)(x) < (double)(-34.0))
            {
                q = -x;
                w = lngamma(q, ref tmp);
                p = (int)Math.Floor(q);
                i = (int)Math.Round(p);
                if (i % 2 == 0)
                {
                    sgngam = -1;
                }
                else
                {
                    sgngam = 1;
                }
                z = q - p;
                if ((double)(z) > (double)(0.5))
                {
                    p = p + 1;
                    z = p - q;
                }
                z = q * Math.Sin(Math.PI * z);
                result = logpi - Math.Log(z) - w;
                return result;
            }
            if ((double)(x) < (double)(13))
            {
                z = 1;
                p = 0;
                u = x;
                while ((double)(u) >= (double)(3))
                {
                    p = p - 1;
                    u = x + p;
                    z = z * u;
                }
                while ((double)(u) < (double)(2))
                {
                    z = z / u;
                    p = p + 1;
                    u = x + p;
                }
                if ((double)(z) < (double)(0))
                {
                    sgngam = -1;
                    z = -z;
                }
                else
                {
                    sgngam = 1;
                }
                if ((double)(u) == (double)(2))
                {
                    result = Math.Log(z);
                    return result;
                }
                p = p - 2;
                x = x + p;
                b = -1378.25152569120859100;
                b = -38801.6315134637840924 + x * b;
                b = -331612.992738871184744 + x * b;
                b = -1162370.97492762307383 + x * b;
                b = -1721737.00820839662146 + x * b;
                b = -853555.664245765465627 + x * b;
                c = 1;
                c = -351.815701436523470549 + x * c;
                c = -17064.2106651881159223 + x * c;
                c = -220528.590553854454839 + x * c;
                c = -1139334.44367982507207 + x * c;
                c = -2532523.07177582951285 + x * c;
                c = -2018891.41433532773231 + x * c;
                p = x * b / c;
                result = Math.Log(z) + p;
                return result;
            }
            q = (x - 0.5) * Math.Log(x) - x + ls2pi;
            if ((double)(x) > (double)(100000000))
            {
                result = q;
                return result;
            }
            p = 1 / (x * x);
            if ((double)(x) >= (double)(1000.0))
            {
                q = q + ((7.9365079365079365079365 * 0.0001 * p - 2.7777777777777777777778 * 0.001) * p + 0.0833333333333333333333) / x;
            }
            else
            {
                a = 8.11614167470508450300 * 0.0001;
                a = -(5.95061904284301438324 * 0.0001) + p * a;
                a = 7.93650340457716943945 * 0.0001 + p * a;
                a = -(2.77777777730099687205 * 0.001) + p * a;
                a = 8.33333333333331927722 * 0.01 + p * a;
                q = q + a / x;
            }
            result = q;
            return result;
        }

        private double incompletegamma(double a, double x)
        {
            double result = 0;
            double igammaepsilon = 0;
            double ans = 0;
            double ax = 0;
            double c = 0;
            double r = 0;
            double tmp = 0;

            igammaepsilon = 0.000000000000001;
            if ((double)(x) <= (double)(0) | (double)(a) <= (double)(0))
            {
                result = 0;
                return result;
            }
            if ((double)(x) > (double)(1) & (double)(x) > (double)(a))
            {
                result = 1 - incompletegammac(a, x);
                return result;
            }
            ax = a * Math.Log(x) - x - this.lngamma(a, ref tmp);
            if ((double)(ax) < (double)(-709.78271289338399))
            {
                result = 0;
                return result;
            }
            ax = Math.Exp(ax);
            r = a;
            c = 1;
            ans = 1;
            do
            {
                r = r + 1;
                c = c * x / r;
                ans = ans + c;
            }
            while ((double)(c / ans) > (double)(igammaepsilon));
            result = ans * ax / a;
            return result;
        }

        private double incompletegammac(double a, double x)
        {
            double result = 0;
            double igammaepsilon = 0;
            double igammabignumber = 0;
            double igammabignumberinv = 0;
            double ans = 0;
            double ax = 0;
            double c = 0;
            double yc = 0;
            double r = 0;
            double t = 0;
            double y = 0;
            double z = 0;
            double pk = 0;
            double pkm1 = 0;
            double pkm2 = 0;
            double qk = 0;
            double qkm1 = 0;
            double qkm2 = 0;
            double tmp = 0;

            igammaepsilon = 0.000000000000001;
            igammabignumber = 4503599627370496.0;
            igammabignumberinv = 2.22044604925031308085 * 0.0000000000000001;
            if ((double)(x) <= (double)(0) | (double)(a) <= (double)(0))
            {
                result = 1;
                return result;
            }
            if ((double)(x) < (double)(1) | (double)(x) < (double)(a))
            {
                result = 1 - incompletegamma(a, x);
                return result;
            }
            ax = a * Math.Log(x) - x - this.lngamma(a, ref tmp);
            if ((double)(ax) < (double)(-709.78271289338399))
            {
                result = 0;
                return result;
            }
            ax = Math.Exp(ax);
            y = 1 - a;
            z = x + y + 1;
            c = 0;
            pkm2 = 1;
            qkm2 = x;
            pkm1 = x + 1;
            qkm1 = z * x;
            ans = pkm1 / qkm1;
            do
            {
                c = c + 1;
                y = y + 1;
                z = z + 2;
                yc = y * c;
                pk = pkm1 * z - pkm2 * yc;
                qk = qkm1 * z - qkm2 * yc;
                if ((double)(qk) != (double)(0))
                {
                    r = pk / qk;
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                {
                    t = 1;
                }
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if ((double)(Math.Abs(pk)) > (double)(igammabignumber))
                {
                    pkm2 = pkm2 * igammabignumberinv;
                    pkm1 = pkm1 * igammabignumberinv;
                    qkm2 = qkm2 * igammabignumberinv;
                    qkm1 = qkm1 * igammabignumberinv;
                }
            }
            while ((double)(t) > (double)(igammaepsilon));
            result = ans * ax;
            return result;
        }
        
        #endregion

        public override int[] GenerateForDay()
        {
            List<int> sequence = new List<int>();
            int suggNum = 100;
            double[] uSeq1 = uGen.GenerateN(suggNum);
            double[] uSeq2 = uGen.GenerateN(suggNum);
            int j = 0;
            int sum = 0;

                            
            if (k < 1)
            {
                double b = (Math.E + k) / Math.E;
                double p;
                double y;

                while (sum <= Params.WORKDAY_MINUTES_NUMBER)
                {
                    if (j == suggNum)
                    {
                        uSeq1 = uGen.GenerateN(suggNum);
                        uSeq2 = uGen.GenerateN(suggNum);
                        j = 0;
                    }
                    p = b * uSeq1[j];
                    if (p > 1)
                    {
                        y = Math.Pow(p, (1 / k));
                        if (uSeq2[j] <= Math.Exp(-y))
                        {
                            int x = (int)Math.Round(d * y);
                            sum = sum + x;
                            if (sum <= Params.WORKDAY_MINUTES_NUMBER) sequence.Add(x);
                        }                        
                    }
                    else
                    {
                        y = (-1) * Math.Log((b - p) / k);
                        if (uSeq2[j] <= Math.Pow(y, (k - 1)))
                        {
                            int x = (int)Math.Round(d * y);
                            sum = sum + x;
                            if (sum <= Params.WORKDAY_MINUTES_NUMBER) sequence.Add(x);
                        }                        
                    }
                    j++;

                }
            }
            else
            {
                double a = Math.Sqrt(2 * k - 1);
                double b = k - Math.Log(4);
                double q = (k + 1) / a;
                double bt = 4.5;
                double r = 1 + Math.Log(bt);
                double v;
                double y;
                double z;
                double w;

                while (sum <= Params.WORKDAY_MINUTES_NUMBER)
                {
                    if (j == suggNum)
                    {
                        uSeq1 = uGen.GenerateN(suggNum);
                        uSeq2 = uGen.GenerateN(suggNum);
                        j = 0;
                    }
                    v = a * Math.Log(uSeq1[j] / (1 - uSeq1[j]));
                    y = k * Math.Exp(v);
                    z = uSeq1[j] * uSeq1[j] * uSeq2[j];
                    w = b + q * v - y;

                    if ((w + r - bt * z) >= 0)
                    {
                        int x = (int)Math.Round(d * y);
                        sum = sum + x;
                        if (sum <= Params.WORKDAY_MINUTES_NUMBER) sequence.Add(x);
                    }
                    else
                    {
                        if (w >= Math.Log(z))
                        {
                            int x = (int)Math.Round(d * y);
                            sum = sum + x;
                            if (sum <= Params.WORKDAY_MINUTES_NUMBER) sequence.Add(x);
                        }
                    }
                    j++;
                }

            }
            return sequence.ToArray();

        }

        public override IEnumerable<double> GenerateSequence()
        {
            var uniform = uGen.GenerateSequence().GetEnumerator();

            Func<double> getNextUniform = () =>
            {
                uniform.MoveNext();
                return uniform.Current;
            };

            if (k < 1)
            {
                double b = (Math.E + k) / Math.E;
                double p;
                double y;

                while (true)
                {
                    p = b * getNextUniform();
                    if (p > 1)
                    {
                        y = Math.Pow(p, (1 / k));
                        if (getNextUniform() <= Math.Exp(-y))
                        {
                            yield return d * y;
                        }
                    }
                    else
                    {
                        y = (-1) * Math.Log((b - p) / k);
                        if (getNextUniform() <= Math.Pow(y, (k - 1)))
                        {
                            yield return d * y;
                        }
                    }
                }
            }
            else
            {
                double a = Math.Sqrt(2 * k - 1);
                double b = k - Math.Log(4);
                double q = (k + 1) / a;
                double bt = 4.5;
                double r = 1 + Math.Log(bt);
                double v;
                double y;
                double z;
                double w;

                while (true)
                {
                    var u1 = getNextUniform();
                    var u2 = getNextUniform();
                    v = a * Math.Log(u1 / (1 - u1));
                    y = k * Math.Exp(v);
                    z = u1 * u1 * u2;
                    w = b + q * v - y;

                    if ((w + r - bt * z) >= 0)
                    {
                        yield return d * y;
                    }
                    else
                    {
                        if (w >= Math.Log(z))
                        {
                            yield return d * y;
                        }
                    }
                }

            }
        }

        public override double GetProbability(double x)
        {
            return this.incompletegamma(k,x/d);
        }
    }
}
