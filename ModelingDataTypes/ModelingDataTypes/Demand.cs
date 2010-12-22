using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    /// <summary>
    /// заявка
    /// </summary>
    public class CDemand
    {
        //  public static int idNext;               //счетчик создания заявок

        /// <summary>
        /// идентификатор заявки (уникальный)
        /// </summary>
        public int ID;

        /// <summary>
        /// время подачи заявки
        /// </summary>
        public DateTime GettingDate;

        /// <summary>
        /// заявленное время окончания делания заявки
        /// </summary>
        public DateTime? ShouldBeDoneDate;

        /// <summary>
        /// РЕАЛЬНОЕ время окончания делания заявки (может быть NULL) 
        /// </summary>
        public DateTime? FinishingDate;

        /// <summary>
        /// срочность (1) или отказ (2)
        /// </summary>
        public int Urgency;

        /// <summary>
        /// кластер продуктов
        /// </summary>
        public CProductCluster Products;

        public CDemand()
        {
            Products = new CProductCluster();
        }

        public CDemand(int id, DateTime geting, int urgency, CProductCluster productCluster)
        {
            this.ID = id;

            this.GettingDate = geting;
            this.FinishingDate = null;
            this.ShouldBeDoneDate = null;
            this.Urgency = urgency;
            this.Products = new CProductCluster(productCluster);
        }

        public CDemand(CDemand copy)
        {
            this.ID = copy.ID;

            this.GettingDate = copy.GettingDate;
            this.FinishingDate = copy.FinishingDate;
            this.ShouldBeDoneDate = copy.ShouldBeDoneDate;
            this.Urgency = copy.Urgency;
            this.Products = new CProductCluster(copy.Products);
        }
    }
}
