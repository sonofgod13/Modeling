using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ModelingDataTypes
{
    public class CDumper
    {
        public static void Dump(string str)
        {
            if (CParams.useDump==true)
            {
                DefaultTraceListener dtl = new DefaultTraceListener();
                dtl.WriteLine("\n========= " + str + "\n");
            }
        }
    }
}
