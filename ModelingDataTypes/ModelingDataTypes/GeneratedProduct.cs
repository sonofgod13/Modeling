using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    /// <summary>
    /// Класс параметров продукта
    /// </summary>
    public class GeneratedProduct : GeneratedElement
    {
        /// <summary>
        /// Индекс продукта (его номер - начиная с единицы)
        /// </summary>
        public int Index;

        /// <summary>
        /// Время производства продукта
        /// </summary>
        public int Time;

        /// <summary>
        /// Кластер материалов 
        /// </summary>
        public MaterialCluster Materials;

        /// <summary>
        /// Изменения продукта
        /// </summary>
        public GeneratedElement Modify;

        public GeneratedProduct()
        {
            GeneratorType = GeneratorType.Normal;   //CGeneratedElement
            fA = 2.0;             //CGeneratedElement
            fB = 1.0;             //CGeneratedElement

            Index = 0;
            Time = 0;
            Materials = new MaterialCluster();

            Modify = new GeneratedElement();
            Modify.GeneratorType = GeneratorType.Normal;
            Modify.fA = 2.0;
            Modify.fB = 1.0;
        }
    }
}
