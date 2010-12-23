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

        private int LIMITATION = Params.MATERIALS_NUMBER;

        public MaterialCluster()
        {
            cluster = new EntityCluster(LIMITATION);
        }

        public MaterialCluster(MaterialCluster copy)
        {
            cluster = new EntityCluster(copy.cluster, LIMITATION);
        }


        public bool AddMaterial(int iMaterialNumber, int iAmount)
        {
            return cluster.AddEntity(iMaterialNumber, iAmount);
        }

        public void AddMaterialCluster(MaterialCluster materialCluster)
        {
            cluster.AddEntityCluster(materialCluster.cluster);
        }

        public bool CleanMaterial(int iMaterialNumber)
        {
            return cluster.CleanEntity(iMaterialNumber);
        }

        public void CleanMaterialsCluster()
        {
            cluster.CleanEntitysCluster();
        }

        public bool GetMaterial(int iMaterialNumber, out int iMaterialValue)
        {
            iMaterialValue = 0;
            return cluster.GetEntity(iMaterialNumber, out iMaterialValue);
        }

        public bool IsMaterial(int iMaterialNumber, int iAmount)
        {
            return cluster.IsEntity(iMaterialNumber, iAmount);
        }

        public bool IsMaterialCluster(MaterialCluster materialCluster)
        {
            return cluster.IsEntityCluster(materialCluster.cluster);
        }

        public bool TakeAwayMaterial(int iMaterialNumber, int iAmount)
        {
            return cluster.TakeAwayEntity(iMaterialNumber, iAmount);
        }

        public bool TakeAwayMaterialCluster(MaterialCluster materialCluster)
        {
            return cluster.TakeAwayEntityCluster(materialCluster.cluster);
        }

        public bool CompareMaterial(int iMaterialNumber, int iAmount)
        {
            return cluster.CompareEntity(iMaterialNumber, iAmount);
        }

        
    }
}
