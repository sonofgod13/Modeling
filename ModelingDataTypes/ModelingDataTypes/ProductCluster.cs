using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    public class CProductCluster   //wrapper-class для EntityCluster
    {
        private CEntityCluster m_cluster;

        private int LIMITATION = CParams.PRODUCTS_NUMBER;

        public CProductCluster()
        {
            m_cluster = new CEntityCluster(LIMITATION);
        }

        public CProductCluster(CProductCluster copy)
        {
            m_cluster = new CEntityCluster(copy.m_cluster, LIMITATION);
        }


        public bool AddProduct(int iProductNumber, int iAmount)
        {
            return m_cluster.AddEntity(iProductNumber, iAmount);
        }

        public void AddProductCluster(CProductCluster productCluster)
        {
            m_cluster.AddEntityCluster(productCluster.m_cluster);
        }

        public bool CleanProduct(int iProductNumber)
        {
            return m_cluster.CleanEntity(iProductNumber);
        }

        public void CleanProductsCluster()
        {
            m_cluster.CleanEntitysCluster();
        }

        public bool GetProduct(int iProductNumber, out int iProductValue)
        {
            iProductValue = 0;
            return m_cluster.GetEntity(iProductNumber, out iProductValue);
        }

        public bool IsProduct(int iProductNumber, int iAmount)
        {
            return m_cluster.IsEntity(iProductNumber, iAmount);
        }

        public bool IsProductCluster(CProductCluster productCluster)
        {
            return m_cluster.IsEntityCluster(productCluster.m_cluster);
        }

        public bool TakeAwayProduct(int iProductNumber, int iAmount)
        {
            return m_cluster.TakeAwayEntity(iProductNumber, iAmount);
        }

        public bool TakeAwayProductCluster(CProductCluster productCluster)
        {
            return m_cluster.TakeAwayEntityCluster(productCluster.m_cluster);
        }

        public bool CompareProduct(int iProductNumber, int iAmount)
        {
            return m_cluster.CompareEntity(iProductNumber, iAmount);
        }

        public bool CompareNomenclatureIsMore(CProductCluster productCluster)
        {
            return m_cluster.CompareNomenclatureIsMore(productCluster.m_cluster);
        }

        public string Dump()
        {
            return ("ПРОДУКТЫ.\n" + m_cluster.Dump());
        }
    }
}
