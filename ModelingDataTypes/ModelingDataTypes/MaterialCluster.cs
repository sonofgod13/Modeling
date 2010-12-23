using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    /// <summary>
    /// wrapper-class для EntityCluster
    /// </summary>
    public class MaterialCluster
    {
        private EntityCluster cluster;

        private int MaterialsCount = Params.MATERIALS_NUMBER;

        public MaterialCluster()
        {
            cluster = new EntityCluster(MaterialsCount);
        }

        public MaterialCluster(MaterialCluster copy)
        {
            cluster = new EntityCluster(copy.cluster, MaterialsCount);
        }


        public bool AddMaterial(int materialIndex, int amount)
        {
            return cluster.AddEntity(materialIndex, amount);
        }

        public void AddMaterialCluster(MaterialCluster materialCluster)
        {
            cluster.AddEntityCluster(materialCluster.cluster);
        }

        public bool CleanMaterial(int materialIndex)
        {
            return cluster.CleanEntity(materialIndex);
        }

        public void CleanMaterialsCluster()
        {
            cluster.CleanEntitysCluster();
        }

        public bool GetMaterial(int materialIndex, out int materialValue)
        {
            materialValue = 0;
            return cluster.GetEntity(materialIndex, out materialValue);
        }

        public bool IsMaterial(int materialIndex, int amount)
        {
            return cluster.IsEntity(materialIndex, amount);
        }

        public bool IsMaterialCluster(MaterialCluster materialCluster)
        {
            return cluster.IsEntityCluster(materialCluster.cluster);
        }

        public bool TakeAwayMaterial(int materialIndex, int amount)
        {
            return cluster.TakeAwayEntity(materialIndex, amount);
        }

        public bool TakeAwayMaterialCluster(MaterialCluster materialCluster)
        {
            return cluster.TakeAwayEntityCluster(materialCluster.cluster);
        }

        public bool CompareMaterial(int materialIndex, int amount)
        {
            return cluster.CompareEntity(materialIndex, amount);
        }

        
    }
}
