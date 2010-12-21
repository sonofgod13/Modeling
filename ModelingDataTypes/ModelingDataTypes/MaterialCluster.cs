using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    public class CMaterialCluster   //wrapper-class для EntityCluster
    {
        private CEntityCluster m_cluster;

        private int LIMITATION = CParams.MATERIALS_NUMBER;

        public CMaterialCluster()
        {
            m_cluster = new CEntityCluster(LIMITATION);
        }

        public CMaterialCluster(CMaterialCluster copy)
        {
            m_cluster = new CEntityCluster(copy.m_cluster, LIMITATION);
        }


        public bool AddMaterial(int iMaterialNumber, int iAmount)
        {
            return m_cluster.AddEntity(iMaterialNumber, iAmount);
        }

        public void AddMaterialCluster(CMaterialCluster materialCluster)
        {
            m_cluster.AddEntityCluster(materialCluster.m_cluster);
        }

        public bool CleanMaterial(int iMaterialNumber)
        {
            return m_cluster.CleanEntity(iMaterialNumber);
        }

        public void CleanMaterialsCluster()
        {
            m_cluster.CleanEntitysCluster();
        }

        public bool GetMaterial(int iMaterialNumber, out int iMaterialValue)
        {
            iMaterialValue = 0;
            return m_cluster.GetEntity(iMaterialNumber, out iMaterialValue);
        }

        public bool IsMaterial(int iMaterialNumber, int iAmount)
        {
            return m_cluster.IsEntity(iMaterialNumber, iAmount);
        }

        public bool IsMaterialCluster(CMaterialCluster materialCluster)
        {
            return m_cluster.IsEntityCluster(materialCluster.m_cluster);
        }

        public bool TakeAwayMaterial(int iMaterialNumber, int iAmount)
        {
            return m_cluster.TakeAwayEntity(iMaterialNumber, iAmount);
        }

        public bool TakeAwayMaterialCluster(CMaterialCluster materialCluster)
        {
            return m_cluster.TakeAwayEntityCluster(materialCluster.m_cluster);
        }

        public bool CompareMaterial(int iMaterialNumber, int iAmount)
        {
            return m_cluster.CompareEntity(iMaterialNumber, iAmount);
        }

        
    }
}
