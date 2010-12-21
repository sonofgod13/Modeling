using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelingDataTypes;
using GeneratorSubsystem;
using System.Xml;

namespace WindowsFormsApplication1
{
    class BackOfficeInterface
    {
        static int ind=0; ///заглушка
        static int delInd=0; ///заглушка

        private BackOfficeFront.FrontOfficeService frontOffice;
        private BackOfficeSim.SimulationService simulation;


        public BackOfficeInterface()
        {
            frontOffice = new WindowsFormsApplication1.BackOfficeFront.FrontOfficeService();
            simulation = new WindowsFormsApplication1.BackOfficeSim.SimulationService();
        }

        private int[] approveDemandParse(object[] obs)   //кривой но работающий парсер xml
        {
            int[] lol = new int[2];
            lol[0] = Int32.Parse(((XmlLinkedNode)obs[1]).InnerText.Substring(7));
            lol[1] = Int32.Parse(((XmlLinkedNode)obs[2]).InnerText.Substring(13));
            return lol;
        }


        public bool approveDemand(ref CDemand demand)                    // утверждение в back office новой заявки
        {
            /*         Реальный код
            int date = (int)(demand.m_dtGeting-new DateTime(1970,1,1,0,0,0)).TotalSeconds;
            object[] obs = frontOffice.newOrder(date, "modeling", demand.m_products[1], demand.m_products[2], demand.m_products[3]);
            int[] lol = approveDemandParse(obs);
            demand.m_iID = lol[0];
            if (demand.m_iUrgency == 2)
            {
               return frontOffice.cancelOrder(date, demand.m_iID);
            }
            else
            {
                return frontOffice.confirmOrder(date, demand.m_iID, demand.m_iUrgency + 2);                
            }
            */

            /*         Заглушка     */
            if ((demand.m_iID == 1) || (demand.m_iID == 2) || (demand.m_iID == 3) || (demand.m_iID == 4)) return true;
                        
            ind++;
            demand.m_iID = ind;
            if (demand.m_iUrgency == 2) return false;
            demand.m_dtShouldBeDone = DateTime.Now.AddDays(new Random().Next(18));

            UniformGen ug = new UniformGen(1, 0);
            if (ug.generateN(1).ElementAt(0) < 0.8) return true;
            else return false;
        }

        public bool approveModifyDemand(DateTime date, CDemand demand)               // утверждение в back office изменения заявки
        {
            /*         Реальный код
            int backDate = (int)(date - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            if (demand.m_products[1] + demand.m_products[2] + demand.m_products[3] == 0)
            {
                return frontOffice.cancelOrder(backDate, demand.m_iID);
            }
            else 
            {
                int returnDate = frontOffice.changeOrder(backDate, demand.m_iID, 1);
                return frontOffice.confirmChange(backDate, demand.m_iID, 1);
            }
            */

            /*         Заглушка     */
            UniformGen ug = new UniformGen(1, 0);
            if (ug.generateN(1).ElementAt(0) < 0.8) return true;
            else return false;
        }

        /*
        public bool reportPlanElem(CPlanReportElement curPlanElem)       // отсылка back office сообщения о выполненном элементе плана 
        {
            return true;
        }
        */

        public bool reportDeliveryDemand(CDeliveryDemand del)
        {
            /*         Реальный код
            int backDate = (int)(del.m_dtRealDelivery.Value - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return simulation.receivingMaterials(backDate, del.m_iID, 1);
            */

            /*         Заглушка     */
            bool g = del.m_dtRealDelivery.HasValue; // заглушка
            return true;
        }

        public CDeliveryDemand getDeliveryDemands(DateTime dt)          //Получение от back office заявок на поставки материалов
        {
            /*         Реальный код
            int backDate = (int)(dt - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return (CDeliveryDemand) simulation.getShoppingList(backDate); //тут надо парсер написать            
            */

            /*         Заглушка     */
            delInd++;
            //***Dictionary<int, int> dict = new Dictionary<int, int>(){{1,2},{2,3},{3,1},{4,7},{5,4},{6,9},{7,7},
            //{8,3},{9,2},{10,7},{11,4},{12,8}};

            CMaterialCluster materialToAdd = new CMaterialCluster();
            materialToAdd.AddMaterial(1, 2);
            materialToAdd.AddMaterial(2, 3);
            materialToAdd.AddMaterial(3, 1);
            materialToAdd.AddMaterial(4, 7);
            materialToAdd.AddMaterial(5, 4);
            materialToAdd.AddMaterial(6, 9);
            materialToAdd.AddMaterial(7, 7);
            materialToAdd.AddMaterial(8, 3);
            materialToAdd.AddMaterial(9, 2);
            materialToAdd.AddMaterial(10, 7);
            materialToAdd.AddMaterial(11, 4);
            materialToAdd.AddMaterial(12, 8);
          
            CDeliveryDemand del = new CDeliveryDemand(delInd, dt, materialToAdd);            
            return del;
        }

        public CPlanElement[] getDailyPlan(DateTime date)          //Получение от back office плана на день
        {
            /*         Реальный код
            int backDate = (int)(date - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return (CPlanElement[]) simulation.getPlan(backDate); //тут надо парсер написать            
            */

            /*         Заглушка     */
            CPlanElement p1 = new CPlanElement() { m_iDemandID = 1, m_iProductID = 3 };
            CPlanElement p2 = new CPlanElement() { m_iDemandID = 1, m_iProductID = 1 };
            CPlanElement p3 = new CPlanElement() { m_iDemandID = 2, m_iProductID = 2 };
            CPlanElement p4 = new CPlanElement() { m_iDemandID = 0, m_iProductID = 3 };
            CPlanElement p5 = new CPlanElement() { m_iDemandID = 3, m_iProductID = 3 };
            CPlanElement p6 = new CPlanElement() { m_iDemandID = 2, m_iProductID = 2 };
            CPlanElement p7 = new CPlanElement() { m_iDemandID = 4, m_iProductID = 1 };
            
            return new CPlanElement[]{ p1,p2,p3,p4,p5,p6,p7};
        }
    }
}
