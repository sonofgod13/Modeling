using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    public class CDeliveryDemand            // заявка на поставку материалов
    {
        //public static int idNext;           //счетчик создания заявок

        public int m_iID;                   // идентификатор заявки
        public DateTime? m_dtRealDelivery;
        //Реальное время поступления поставки материалов
        public DateTime m_dtFillDelivery;
        //Ожидаемое время поступления поставки материалов
        //public Dictionary<int, int> m_materialsDemand;
        public CMaterialCluster m_materialsDemand;
        //Материалы. Пара: идентификатор материала - его количество


        public CDeliveryDemand(int id, DateTime dtFillDelivery, CMaterialCluster materialsDemand)
        {
            // idNext++;

            this.m_iID = id;
            this.m_dtRealDelivery = null;
            this.m_dtFillDelivery = dtFillDelivery;

            //***this.m_materialsDemand = new Dictionary<int, int>(materialsDemand);
            m_materialsDemand = new CMaterialCluster(materialsDemand);
        }

        public CDeliveryDemand(CDeliveryDemand copy)
        {
            this.m_iID = copy.m_iID;

            this.m_dtRealDelivery = copy.m_dtRealDelivery;
            this.m_dtFillDelivery = copy.m_dtFillDelivery;
            //this.m_materialsDemand = new Dictionary<int, int>(copy.m_materialsDemand);
            this.m_materialsDemand = new CMaterialCluster(copy.m_materialsDemand); 
        }
    }
}
