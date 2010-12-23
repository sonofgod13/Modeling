using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    public class CMaterialCluster   //wrapper-class для EntityCluster
    {
        private CEntityCluster cluster;

        private int LIMITATION = CParams.MATERIALS_NUMBER;

        public CMaterialCluster()
        {
            cluster = new CEntityCluster(LIMITATION);
        }

        public CMaterialCluster(CMaterialCluster copy)
        {
            cluster = new CEntityCluster(copy.cluster, LIMITATION);
        }


        public bool AddMaterial(int iMaterialNumber, int iAmount)
        {
            return cluster.AddEntity(iMaterialNumber, iAmount);
        }

        public void AddMaterialCluster(CMaterialCluster materialCluster)
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

        public bool IsMaterialCluster(CMaterialCluster materialCluster)
        {
            return cluster.IsEntityCluster(materialCluster.cluster);
        }

        public bool TakeAwayMaterial(int iMaterialNumber, int iAmount)
        {
            return cluster.TakeAwayEntity(iMaterialNumber, iAmount);
        }

        public bool TakeAwayMaterialCluster(CMaterialCluster materialCluster)
        {
            return cluster.TakeAwayEntityCluster(materialCluster.cluster);
        }

        public bool CompareMaterial(int iMaterialNumber, int iAmount)
        {
            return cluster.CompareEntity(iMaterialNumber, iAmount);
        }

        
    }
}
