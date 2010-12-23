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

        private int ClusterSizeLimit = Params.PRODUCTS_NUMBER;

        public ProductCluster()
        {
            Cluster = new EntityCluster(ClusterSizeLimit);
        }

        public ProductCluster(ProductCluster copy)
        {
            Cluster = new EntityCluster(copy.Cluster, ClusterSizeLimit);
        }


        public bool AddProduct(int productIndex, int amount)
        {
            return Cluster.AddEntity(productIndex, amount);
        }

        public void AddProductCluster(ProductCluster productCluster)
        {
            Cluster.AddEntityCluster(productCluster.Cluster);
        }

        public bool CleanProduct(int productIndex)
        {
            return Cluster.CleanEntity(productIndex);
        }

        public void CleanProductsCluster()
        {
            Cluster.CleanEntitysCluster();
        }

        public bool GetProduct(int productIndex, out int productValue)
        {
            return Cluster.GetEntity(productIndex, out productValue);
        }

        public bool IsProduct(int productIndex, int amount)
        {
            return Cluster.IsEntity(productIndex, amount);
        }

        public bool IsProductCluster(ProductCluster productCluster)
        {
            return Cluster.IsEntityCluster(productCluster.Cluster);
        }

        public bool TakeAwayProduct(int productIndex, int iAmount)
        {
            return Cluster.TakeAwayEntity(productIndex, iAmount);
        }

        public bool TakeAwayProductCluster(ProductCluster productCluster)
        {
            return Cluster.TakeAwayEntityCluster(productCluster.Cluster);
        }

        public bool CompareProduct(int productIndex, int amount)
        {
            return Cluster.CompareEntity(productIndex, amount);
        }

        public bool CompareNomenclatureIsMore(ProductCluster productCluster)
        {
            return Cluster.CompareNomenclatureIsMore(productCluster.Cluster);
        }
    }
}
