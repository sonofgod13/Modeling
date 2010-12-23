using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    /// <summary>
    /// Элемент для которого требуется полноценный генератор
    /// </summary>
    public class GeneratedElement
    {
        /// <summary>
        /// Тип генератора
        /// </summary>
        public GeneratorType GeneratorType;

        /// <summary>
        /// Первый параметр генератора
        /// </summary>
        public double fA;

        /// <summary>
        /// второй параметр генератора
        /// </summary>
        public double fB;
    }
}
