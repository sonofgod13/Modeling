using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    public class CGeneratedProduct : CGeneratedElement
    //класс параметров продукта
    {
        public int Index;            //индекс продукта (его номер - начиная с единицы)
        public int Time;             //время производства продукта
        //public int[] m_iMaterials;      //количество материалов 
        public CMaterialCluster Materials; //кластер материалов 

        public CGeneratedElement Modify; // изменения продукта

        public CGeneratedProduct()
        {
            GeneratorType = GeneratorType.Normal;   //CGeneratedElement
            fA = 2.0;             //CGeneratedElement
            fB = 1.0;             //CGeneratedElement

            Index = 0;
            Time = 0;
            Materials = new CMaterialCluster();

            Modify = new CGeneratedElement();
            Modify.GeneratorType = GeneratorType.Normal;
            Modify.fA = 2.0;
            Modify.fB = 1.0;
        }
    }
}
