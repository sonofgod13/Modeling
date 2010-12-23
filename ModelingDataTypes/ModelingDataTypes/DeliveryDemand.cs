using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    /// <summary>
    /// заявка на поставку материалов
    /// </summary>
    public class DeliveryDemand
    {
        ///// <summary>
        ///// счетчик создания заявок
        ///// </summary>
        //public static int idNext;

        /// <summary>
        /// идентификатор заявки
        /// </summary>
        public int ID;

        /// <summary>
        /// Реальное время поступления поставки материалов
        /// </summary>
        public DateTime? RealDeliveryDate;

        /// <summary>
        /// Ожидаемое время поступления поставки материалов
        /// </summary>
        public DateTime FillDeliveryDate;

        /// <summary>
        /// Материалы. Пара: идентификатор материала - его количество
        /// </summary>
        public MaterialCluster MaterialsDemand;

        /// <summary>
        /// Заявка отправлена
        /// </summary>
        public bool IsDone;

        public DeliveryDemand(int id, DateTime fillDeliveryDate, MaterialCluster materialsDemand)
        {
            this.ID = id;
            this.RealDeliveryDate = null;
            this.FillDeliveryDate = fillDeliveryDate;
            this.IsDone = false;

            MaterialsDemand = new MaterialCluster(materialsDemand);
        }

        public DeliveryDemand(DeliveryDemand copy)
        {
            this.ID = copy.ID;
            this.IsDone = copy.IsDone;
            this.RealDeliveryDate = copy.RealDeliveryDate;
            this.FillDeliveryDate = copy.FillDeliveryDate;

            this.MaterialsDemand = new MaterialCluster(copy.MaterialsDemand); 
        }
    }
}
