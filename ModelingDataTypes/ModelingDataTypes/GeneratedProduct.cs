using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    public class CGeneratedProduct : CGeneratedElement
    //класс параметров продукта
    {
        public int m_iIndex;            //индекс продукта (его номер - начиная с единицы)
        public int m_iTime;             //время производства продукта
        //public int[] m_iMaterials;      //количество материалов 
        public CMaterialCluster m_materials; //кластер материалов 

        public CGeneratedElement m_modify; // изменения продукта

        public CGeneratedProduct()
        {
            m_iGeneratorType = GeneratorType.Normal;   //CGeneratedElement
            m_fA = 2.0;             //CGeneratedElement
            m_fB = 1.0;             //CGeneratedElement

            m_iIndex = 0;
            m_iTime = 0;
            m_materials = new CMaterialCluster();

            m_modify = new CGeneratedElement();
            m_modify.m_iGeneratorType = GeneratorType.Normal;
            m_modify.m_fA = 2.0;
            m_modify.m_fB = 1.0;
        }
    }
}
