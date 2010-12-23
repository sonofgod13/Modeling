using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    /// <summary>
    /// элемент выполнения плана
    /// </summary>
    public class PlanReportElement
    {
        /// <summary>
        /// элемент плана
        /// </summary>
        public PlanElement PlanElement;

        /// <summary>
        /// фактическое начало его выполнения
        /// </summary>
        public DateTime StartExecuteDate;

        /// <summary>
        /// фактическое окончание его выполнения. если время нулевое то элемент не выполнен
        /// </summary>
        public DateTime EndExecuteDate;
    }
}
