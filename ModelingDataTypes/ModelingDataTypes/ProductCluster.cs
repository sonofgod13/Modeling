using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    public class CProductCluster   //wrapper-class для EntityCluster
    {
        private CEntityCluster Cluster;

        private int LIMITATION = CParams.PRODUCTS_NUMBER;

        public CProductCluster()
        {
            Cluster = new CEntityCluster(LIMITATION);
        }

        public CProductCluster(CProductCluster copy)
        {
            Cluster = new CEntityCluster(copy.Cluster, LIMITATION);
        }


        public bool AddProduct(int iProductNumber, int iAmount)
        {
            return Cluster.AddEntity(iProductNumber, iAmount);
        }

        public void AddProductCluster(CProductCluster productCluster)
        {
            Cluster.AddEntityCluster(productCluster.Cluster);
        }

        public bool CleanProduct(int iProductNumber)
        {
            return Cluster.CleanEntity(iProductNumber);
        }

        public void CleanProductsCluster()
        {
            Cluster.CleanEntitysCluster();
        }

        public bool GetProduct(int iProductNumber, out int iProductValue)
        {
            iProductValue = 0;
            return Cluster.GetEntity(iProductNumber, out iProductValue);
        }

        public bool IsProduct(int iProductNumber, int iAmount)
        {
            return Cluster.IsEntity(iProductNumber, iAmount);
        }

        public bool IsProductCluster(CProductCluster productCluster)
        {
            return Cluster.IsEntityCluster(productCluster.Cluster);
        }

        public bool TakeAwayProduct(int iProductNumber, int iAmount)
        {
            return Cluster.TakeAwayEntity(iProductNumber, iAmount);
        }

        public bool TakeAwayProductCluster(CProductCluster productCluster)
        {
            return Cluster.TakeAwayEntityCluster(productCluster.Cluster);
        }

        public bool CompareProduct(int iProductNumber, int iAmount)
        {
            return Cluster.CompareEntity(iProductNumber, iAmount);
        }

        public bool CompareNomenclatureIsMore(CProductCluster productCluster)
        {
            return Cluster.CompareNomenclatureIsMore(productCluster.Cluster);
        }
    }
}
