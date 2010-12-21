using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    public class CPlanReportElement // элемент выполнения плана
    {
        public CPlanElement m_planElement;    // элемент плана
        public DateTime m_dtStartExecute;   // фактическое начало его выполнения
        public DateTime m_dtEndExecute;   // фактическое окончание его выполнения // если время нулевое то элемент не выполнен
    }
}
