using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    /// <summary>
    /// Wrapper-class для EntityCluster
    /// </summary>
    public class ProductCluster
    {
        private EntityCluster Cluster;

        private int LIMITATION = Params.PRODUCTS_NUMBER;

        public ProductCluster()
        {
            Cluster = new EntityCluster(LIMITATION);
        }

        public ProductCluster(ProductCluster copy)
        {
            Cluster = new EntityCluster(copy.Cluster, LIMITATION);
        }


        public bool AddProduct(int iProductNumber, int iAmount)
        {
            return Cluster.AddEntity(iProductNumber, iAmount);
        }

        public void AddProductCluster(ProductCluster productCluster)
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

        public bool IsProductCluster(ProductCluster productCluster)
        {
            return Cluster.IsEntityCluster(productCluster.Cluster);
        }

        public bool TakeAwayProduct(int iProductNumber, int iAmount)
        {
            return Cluster.TakeAwayEntity(iProductNumber, iAmount);
        }

        public bool TakeAwayProductCluster(ProductCluster productCluster)
        {
            return Cluster.TakeAwayEntityCluster(productCluster.Cluster);
        }

        public bool CompareProduct(int iProductNumber, int iAmount)
        {
            return Cluster.CompareEntity(iProductNumber, iAmount);
        }

        public bool CompareNomenclatureIsMore(ProductCluster productCluster)
        {
            return Cluster.CompareNomenclatureIsMore(productCluster.Cluster);
        }
    }
}
