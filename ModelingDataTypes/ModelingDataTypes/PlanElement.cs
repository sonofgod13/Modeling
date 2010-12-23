using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    /// <summary>
    /// элемент плана
    /// </summary>
    public class PlanElement
    {
        /// <summary>
        /// идентификатор заявки.
        /// </summary>
        public int DemandID;
        //Если номер заявки нулевой – то это перенастройка оборудования
        //Если номер заявки равен -1 – это значит что произведённый товар относился к отменённой заявке

        /// <summary>
        /// Идентификатор продукта, который нужно сделать
        /// </summary>
        public int ProductID; //!
    }
}
