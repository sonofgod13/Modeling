using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    public class CPlanElement // элемент плана
    {
        public int m_iDemandID;        // идентификатор заявки.
        //Если номер заявки нулевой – то это перенастройка оборудования
        //Если номер заявки равен -1 – это значит что произведённый товар относился к отменённой заявке
        public int m_iProductID; //!      //Идентификатор продукта, который нужно сделать
    }
}
