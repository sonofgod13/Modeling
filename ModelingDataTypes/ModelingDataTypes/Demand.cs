using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    public class CDemand                        //заявка
    {
        //  public static int idNext;               //счетчик создания заявок

        public int m_iID;                       // идентификатор заявки (уникальный)
        public DateTime m_dtGeting;             // время подачи заявки
        public DateTime? m_dtShouldBeDone;      // заявленное время окончания делания заявки
        public DateTime? m_dtFinishing;         // РЕАЛЬНОЕ время окончания делания заявки (может быть NULL) 
        public int m_iUrgency;                 //срочность (1) или отказ (2)
        //****public Dictionary<int, int> m_products;
        //Продукты. Пара: идентификатор продукта - его количество
        public CProductCluster m_products;   //кластер продуктов

        public CDemand()
        {
            m_products = new CProductCluster();
        }

        public CDemand(int id, DateTime dtGeting, int iUrgency, CProductCluster productCluster/*Dictionary<int, int> products*/)
        {
            // idNext++;

            this.m_iID = id;
            this.m_dtGeting = dtGeting;
            this.m_dtFinishing = null;
            this.m_dtShouldBeDone = null;
            this.m_iUrgency = iUrgency;
            //****this.m_products = new Dictionary<int, int>(products);
            this.m_products = new CProductCluster(productCluster);
        }

        public CDemand(CDemand copy)
        {
            this.m_iID = copy.m_iID;
            this.m_dtGeting = copy.m_dtGeting;
            this.m_dtFinishing = copy.m_dtFinishing;
            this.m_dtShouldBeDone = copy.m_dtShouldBeDone;
            this.m_iUrgency = copy.m_iUrgency;
            //****this.m_products = new Dictionary<int, int>(copy.m_products);
            this.m_products = new CProductCluster(copy.m_products);
        }
    }
}
