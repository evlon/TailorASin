using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Diagnostics;

namespace Asin
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 2000000;
            List<double> values = new List<double>(count);

            Random r = new Random();
           

            CodeTime.Init();
            int? iter = 0;
           

            while (true)
            {
                var vt = r.NextDouble() * 2 - 1;
                var ret = Maclaurin.Asin(vt);
                var ret2 = Math.Asin(vt);
                Console.WriteLine(ret);
                Console.WriteLine(ret2);

                iter = 0;
                CodeTime.Timer("Math.ASin", 1, () =>
                {
                    for (var i = 0; i <= count; ++i)
                    {
                        var v = r.NextDouble() * 2 - 1;
                        Math.Asin(v);
                    }
                   

                });

                iter = 0;
                CodeTime.Timer("Maclaurin.ASin", 1, () =>
                {
                    for (var i = 0; i <= count; ++i)
                    {
                        var v = r.NextDouble() * 2 - 1;
                        Maclaurin.Asin(v);
                    }
                   

                    

                });
            }
           

           
            Console.ReadLine();
        }
    }


    class ASinImpl
    {
        private readonly int _precision;
        private double[] _quotieties = null;
        private long[] _factorials =null;
        public ASinImpl(int precision = 3)
        {
            _precision = precision;
            _quotieties = new double[precision + 1];
            _factorials = new long[(precision + 2)*2 + 1];
            Factorial();
            ComputeQuotiety();
        }

        public double Calc(double v)
        {
            unchecked
            {
                double retVal = 0;

                double vVal = 1;
                double q = v * v;
                unsafe
                {
                    fixed (double* pq = _quotieties)
                    {
                        for (int i = 0; i < _precision; ++i)
                        {

                            if (i == 0)
                            {
                                vVal = v;
                                //yield return ret;
                                retVal += pq[i] * vVal;
                            }
                            else
                            {
                                vVal *= q;
                                //yield return ret;
                                retVal += pq[i] * vVal;
                            }
                        }

                        return retVal;
                    }

                   
                }
               
            }
            
        }


        private void ComputeQuotiety()
        {
            unchecked
            {
                int precision = _quotieties.Length;
                for (int i = 0; i < precision; i++)
                {

                    double up = _factorials[2*i];

                    double down = Math.Pow(Math.Pow(2, i)*_factorials[i], 2)*(2*i + 1);

                    double quotiety = up/down;

                    _quotieties[i] = quotiety;


                }
            }

        }

        private void Factorial()
        {
            unchecked
            {
                int precision = _factorials.Length ;
                long ret = 1;
                for (long v = 0; v < precision; ++v)
                {
                    if (v == 0)
                    {
                        this._factorials[v] = 1;
                    }
                    else if (v == 1){
                            this._factorials[v] = 1;
                    }
                    else
                    {
                        ret *= v;

                        this._factorials[v] = ret;
                    }

                }
            }
        }
    }

    class Maclaurin
    {
        //class AASinImpl
        //{
        //    private List<double> quotieties = new List<double>();
        //    private List<long> factorials = new List<long>();
        //    private IEnumerator<double> computeQuotieties = null;
        //    private IEnumerator<long> computeFactorials = null;
        //    public AASinImpl()
        //    {
        //        this.computeQuotieties = ComputeQuotiety();
        //        this.computeFactorials = Factorial();
        //    }

        //    public double Calc(double v, int precision = 2)
        //    {
        //        if (quotieties.Count < precision)
        //        {
        //            for (var i = quotieties.Count; i < precision; ++i)
        //            {
        //                computeQuotieties.MoveNext();
        //                quotieties.Add(computeQuotieties.Current);
        //            }
        //        }

        //        double ret = 0;
        //        var values = ComputeValues(v);
        //        for (int i = 0; i < precision; ++i)
        //        {
        //            values.MoveNext();
        //            ret += quotieties[i]*values.Current;
        //        }

        //        return ret;
        //    }

        //    private IEnumerator<double> ComputeValues(double v)
        //    {
        //        double ret = 1;
        //        double q = v*v;
        //        for(int i = 0;;++i)
        //        {
                   
        //            if (i == 0)
        //            {
        //                ret = v;
        //                yield return ret;
        //            }
        //            else
        //            {
        //                ret *= q;
        //                yield return ret;
        //            }
        //        }

        //        throw new NotImplementedException();
        //    }

        //    private IEnumerator<double> ComputeQuotiety()
        //    {
        //        for (int i = 0;; i++)
        //        {
        //            long i2 = 2*i;
        //            for (long j = factorials.Count; j <= i2; j++)
        //            {
        //                computeFactorials.MoveNext();
        //                factorials.Add(computeFactorials.Current);
        //            }
        //            double up = factorials[2 * i];

        //            double down = Math.Pow(Math.Pow(2, i) * factorials[i], 2) * (2 * i + 1);

        //            double quotiety = up/down;

        //            yield return quotiety;


        //        }

        //        throw new NotImplementedException();
        //    }

        //    private IEnumerator<long> Factorial()
        //    {
        //        long v = 0;
        //        if (v == 0)
        //            yield return 1;

        //        if (v == 1)
        //            yield return 1;

        //        long ret = 1;
        //        for (int i = 2; ; ++i)
        //        {
        //            ret *= i;

        //            yield return ret;
        //        }

        //        throw new NotImplementedException();
        //    }
        //}


        private static ASinImpl asinImpl = new ASinImpl(6);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static double Asin(double v)
        {
            if (v < -1 || v > 1)
            {
                throw new ArgumentOutOfRangeException("v");
            }

            return asinImpl.Calc(v);
        }
    }
}
