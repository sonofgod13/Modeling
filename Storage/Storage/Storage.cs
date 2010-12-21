using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelingDataTypes;

namespace Storage
{
    public class CStorage //временное хранилище результатов моделирования
    {
        private int[] modifyStatistic;
        // статистика утверждённых изменений в заявках первое - число принятых, второе - отклонённых 
        // надо будет как нибудь это дело облагородить, может ввести парметры по каждой заявке


        private Dictionary<int, CDemand> m_acceptedDemands; // Принятые Заявки. 

        private Dictionary<int, CDemand> m_declinedDemands; // Отклонённые Заявки

        private Dictionary<int, CDemand> m_canceledDemands; // Отменённые Заявки


        private Queue<CPlanElement> m_plan; //очередь из элементов плана на день


        public CMaterialCluster m_materials; //Склад. Кластер материалов.


        private Dictionary<DateTime, CPlanReportElement> m_planReport;
        //Статистика производства. Пара: время окончания выполнения - элемент плана

        private Dictionary<int, CDeliveryDemand> m_DeliveryDemands;
        // обработанные заявки на поставку материалов


        private Dictionary<int, List<int>> materialsPerDay; 
        // статистика - количество материалов на каждый день моделирования

        private List<double> idleTimePerDay;
        // статистика - доля времени простоя производства от рабочего времени на каждый день моделирования

        private List<double> demandAverageDelayPerDay;
        // статистика - среденее время задержки заказов на каждый день моделирования

        private List<double> finishedDemandsPerDay;
        // статистика - доля выполненных заказов на каждый день моделирования

        private List<double> canceledDemandsPerDay;
        // статистика - доля отменённых заказов на каждый день моделирования



        public CStorage()   // конструктор - инициализация значений
        {
            modifyStatistic = new int[] { 0, 0 };


            m_acceptedDemands = new Dictionary<int, CDemand>(); // Принятые Заявки.

            m_declinedDemands = new Dictionary<int, CDemand>(); // Отклонённые Заявки

            m_canceledDemands = new Dictionary<int, CDemand>(); // Оменённые Заявки


            m_plan = new Queue<CPlanElement>(); //очередь из элементов плана на день


            m_materials = new CMaterialCluster();
            //Инициализация склада нулевыми значениями

        
            m_planReport = new Dictionary<DateTime, CPlanReportElement>();
            //Статистика производства. Пара: время окончания выполнения - элемент плана


            m_DeliveryDemands = new Dictionary<int,CDeliveryDemand>();
            // обработанные заявки на поставку материалов


            materialsPerDay = new Dictionary<int, List<int>>
            // количество материалов на каждый день моделирования
            { 
            {1,new List<int>()},{2,new List<int>()},{3,new List<int>()},{4,new List<int>()},
            {5,new List<int>()},{6,new List<int>()},{7,new List<int>()},{8,new List<int>()},
            {9,new List<int>()},{10,new List<int>()},{11,new List<int>()},{12,new List<int>()}
            }; 

            idleTimePerDay = new List<double>();
            // доля времени простоя производства от рабочего времени на каждый день моделирования

            demandAverageDelayPerDay = new List<double>();
            // среденее время задержки заказов на каждый день моделирования

            finishedDemandsPerDay = new List<double>();
            // доля выполненных заказов на каждый день моделирования

            canceledDemandsPerDay = new List<double>();
            // доля отменённых заказов на каждый день моделирования

        }


       
        /*
       private Queue<CDeliveryDemand> m_newDeliveryDemands = new Queue<CDeliveryDemand>(); 
       // заявки на поставку материалов, пришедшие от back office и ещё не обработанные
       */

        public bool AddAcceptedDemand(CDemand demand) // добавить новую заявку в список принятых
        {
            if (m_acceptedDemands.ContainsKey(demand.m_iID))
                return ModelError.Error();

            m_acceptedDemands.Add(demand.m_iID, demand);
            return true;
        }

        public bool AddCanceledDemand(int id) // добавить новую заявку в список отменённых
        {
            
            if (m_acceptedDemands.ContainsKey(id))
                 ModelError.Error();

            CDemand demand = new CDemand(m_acceptedDemands[id]);
            IEnumerable<CPlanReportElement> demandPlanElementReports = this.m_planReport.Values.Where(x => x.m_planElement.m_iDemandID == demand.m_iID);
            foreach (CPlanReportElement c in demandPlanElementReports)
            {
                c.m_planElement.m_iDemandID = -1;
            }

            m_acceptedDemands.Remove(id);
        
            if (m_canceledDemands.ContainsKey(id))
                return ModelError.Error();

            m_canceledDemands.Add(demand.m_iID, demand);
            return true;
        }

        public bool AddDeclinedDemand(CDemand demand) // добавить новую заявку в список отклонённых
        {
            if (m_declinedDemands.ContainsKey(demand.m_iID))
                return ModelError.Error();

            m_declinedDemands.Add(demand.m_iID, demand);
            return true;
        }


        public CDemand[] GetNotFinishedDemands() // получить все не завершённые заявки
        {
            List<CDemand> list = new List<CDemand>();
            foreach (CDemand d in m_acceptedDemands.Values)
            {
                if (d.m_dtFinishing.HasValue==false) 
                    list.Add(d);
            }            
            return list.ToArray();
        }

        public bool GetAcceptedDemand(int ind, out CDemand demand) // получить утверждённую заявку по id
        {
            demand = new CDemand();

            if (!m_acceptedDemands.ContainsKey(ind))
                return ModelError.Error();

            //ВАЖНО! Возможно здесь надо принудительно вызвать деструктор для demand
            demand = new CDemand(this.m_acceptedDemands[ind]);
            return true;
        }


        public int GetAcceptedDemandsNumber() // посчитать кол-во принятых заявок
        {
            return m_acceptedDemands.Count;
        }

        public int GetDeclinedDemandsNumber() // посчитать кол-во отклонённых заявок
        {
            return m_declinedDemands.Count;
        }

        public bool ModifyDemand(CDemand modifiedDemand) //изменить заявку в m_acceptedDemands
        {
            if ( !m_acceptedDemands.ContainsKey(modifiedDemand.m_iID) )
                return ModelError.Error();

            //this.m_acceptedDemands[modifiedDemand.m_iID].m_iUrgency = modifiedDemand.m_iUrgency;      срочность нельзя изменить
            this.m_acceptedDemands[modifiedDemand.m_iID].m_dtShouldBeDone = modifiedDemand.m_dtShouldBeDone;

            /* //add product cluster
            for (int iProductNumber = 1; iProductNumber < CParams.PRODUCTS_NUMBER + 1; iProductNumber++)
            {
                this.m_acceptedDemands[modifiedDemand.m_iID].m_products[iProductNumber] =
                    modifiedDemand.m_products[iProductNumber];
            }
             */

            //--->
            this.m_acceptedDemands[modifiedDemand.m_iID].m_products.CleanProductsCluster();
            this.m_acceptedDemands[modifiedDemand.m_iID].m_products.AddProductCluster(modifiedDemand.m_products);
            //<---

            return true;
        }

        public bool TranDemand(int demandInd, int prodId, int count) // Переброска произведённых продуктов другому заказу
        {
            try
            {
                IEnumerable<CPlanReportElement> demandPlanElementReports = this.m_planReport.Values.Where(
                    x => (x.m_planElement.m_iDemandID == -1) && (x.m_planElement.m_iProductID == prodId)
                    ).Take(count);
                foreach (CPlanReportElement c in demandPlanElementReports)
                {
                    c.m_planElement.m_iDemandID = demandInd;
                }
                return true;
            }
            catch
            {
                ModelError.Error();
                return false;
            }
        }

        public bool IsDemandDone(int demandInd) // Выполнена ли полностью заявка
        {
            CPlanReportElement[] demandPlanElementReports = this.m_planReport.Values.Where(x=>x.m_planElement.m_iDemandID==demandInd).ToArray();
            int firstArticle=0;
            int thirdArticle=0;
            int secondArticle=0;
            for (int i=0;i<demandPlanElementReports.Length;i++) 
            {
                switch (demandPlanElementReports[i].m_planElement.m_iProductID)
                {
                    case 1:
                        firstArticle++;
                        break;
                    case 2:
                        secondArticle++;
                        break;
                    case 3:
                        thirdArticle++;
                        break;
                    default:
                        break;
                }
            }
            CDemand demand;
            this.GetAcceptedDemand(demandInd, out demand);
            /* 
            if ((demand.m_products[1] <= firstArticle) && (demand.m_products[2] <= secondArticle) && (demand.m_products[3] <= thirdArticle))
                // Здесь по идеии строгое равенство но на всякий случай в рамках заглушки
                return true;
            */ 

            //--->
            int iProductValue = 0;
            bool bToTrue = true;
            demand.m_products.GetProduct(1, out iProductValue);
            if ( iProductValue > firstArticle )
                bToTrue = false;

            demand.m_products.GetProduct(2, out iProductValue);
            if (iProductValue > secondArticle)
                bToTrue = false;

            demand.m_products.GetProduct(3, out iProductValue);
            if (iProductValue > thirdArticle)
                bToTrue = false;

            if (bToTrue)
                return true;
            //<---

            else 
                return false;
        }

        public bool FinishDemand(int demandInd, DateTime date)
        // Присвоить заявке в m_acceptedDemands время завершения (по id)
        {
            if ( !m_acceptedDemands.ContainsKey(demandInd) )
                return ModelError.Error();

            this.m_acceptedDemands[demandInd].m_dtFinishing = date;
                return true;
        }

        public bool AddModifyStatistic(bool modified)
        {
            if (modified == true) this.modifyStatistic[0]++;
            else this.modifyStatistic[1]++;
            return true;
        }



        public bool AddDailyPlan(CPlanElement[] planElements) //добавить план на день
        {
            for (int i = 0; i < planElements.Length; i++)
            {
                m_plan.Enqueue(planElements[i]);
            }
            return true;
        }

        public CPlanElement GetFirstPlanElementAndDelete() //вынуть первый элемент плана
        {
            return m_plan.Dequeue();
        }

        public CPlanElement GetFirstPlanElement() //вернуть первый элемент плана
        {
            return m_plan.Peek();
        }

        public bool ClearAllPlan() //очистить план
        {
            m_plan.Clear();
            return true;
        }

        public int GetPlanElementsToGo() //вернуть количество оставшихся для выполнения элементов плана
        {
            return m_plan.Count;
        }


        /*
        public CDeliveryDemand[] GetNewDeliveryDemandsAndDelete() // получить все новые заявки на поставку материалов и удалить их
        {
            List<CDeliveryDemand> list = new List<CDeliveryDemand>();
            for (int i=0; i < this.m_newDeliveryDemands.Count; i++)
            {
                list.Add(this.m_newDeliveryDemands.Dequeue());
            }
            this.m_newDeliveryDemands.Clear();
            return list.ToArray();
        }

        public bool AddNewDeliveryDemand(CDeliveryDemand deliveryDemand)  //добавить новую заявку на поставку материалов
        {
            this.m_newDeliveryDemands.Enqueue(deliveryDemand);
            return true;
        }
        */

        public bool AddDeliveryDemand(CDeliveryDemand deliveryDemand) 
            // добавить обработанную заявку на поставку материалов
        {
            if (m_DeliveryDemands.ContainsKey(deliveryDemand.m_iID))
                return ModelError.Error();

            this.m_DeliveryDemands.Add(deliveryDemand.m_iID, deliveryDemand);
            return true;
        }

        public int GetNextDeliveryDemandTime(DateTime date, ref bool prevDeliveryDemandTimeEqualsZero)  
            //получить время ближайшей поставки материалов
        {
            int timeSpan=-1;
            foreach (CDeliveryDemand d in this.m_DeliveryDemands.Values)
            {
                int curTimeSpan=(int)(d.m_dtRealDelivery.Value-date).TotalMinutes;
                if ((curTimeSpan == 0) && (prevDeliveryDemandTimeEqualsZero == false))
                {
                    prevDeliveryDemandTimeEqualsZero = true;
                    return curTimeSpan;
                }
                else
                {
                    DateTime checkDate = date.AddMinutes(curTimeSpan);
                    if ((curTimeSpan > 0) && (date.Year == checkDate.Year) && (date.Month == checkDate.Month) && (date.Day == checkDate.Day))
                    {
                        if (timeSpan == -1) timeSpan = curTimeSpan;
                        else
                        {
                            if (curTimeSpan < timeSpan) timeSpan = curTimeSpan;
                        }
                    }
                }
            }
            prevDeliveryDemandTimeEqualsZero = false;
            return timeSpan;
        }

        public CDeliveryDemand[] GetDeliveryDemand(DateTime date)  
            //получить пришедшие в данное время поставки материалов 
        {
            List<CDeliveryDemand> list = new List<CDeliveryDemand>();
            foreach(CDeliveryDemand d in this.m_DeliveryDemands.Values)
            {
                if (d.m_dtRealDelivery == date) list.Add(d);
            }
        
            return list.ToArray();
        }


        public bool AddPlanReportElement(CPlanReportElement planReportElement) 
            //Добавить элемент выполнения плана
        {
            m_planReport.Add(planReportElement.m_dtEndExecute, planReportElement);
            //!!! Здесь при добавлении еще нужно упорядочивать элементы
            return true;
        }

        public double DemandAverageDelay()  // Среденее время задержки заказов в днях
        {
            int demandsNum=0;
            double demandsDelaySum = 0;
            foreach (CDemand d in this.m_acceptedDemands.Values)
            {
                if ((d.m_dtFinishing.HasValue == true)&&(d.m_dtShouldBeDone.HasValue == true))
                {
                    demandsNum++;
                    if (d.m_dtFinishing.Value > d.m_dtShouldBeDone.Value)
                    {
                        double span = (d.m_dtFinishing.Value - d.m_dtShouldBeDone.Value).TotalDays;
                        demandsDelaySum = demandsDelaySum + span;
                    }
                    else
                    {
                        double span = (d.m_dtShouldBeDone.Value - d.m_dtFinishing.Value).TotalDays;
                        demandsDelaySum = demandsDelaySum - span;
                    }
                }
            }
            if (demandsNum > 0) return Math.Round(demandsDelaySum / demandsNum);
            else return -1;
        }

        public double FinishedDemandsShare()  // Доля выполненных заказов
        {
            double notFinishedDemands = this.GetNotFinishedDemands().Length;
            double allAcceptedDemands = this.m_acceptedDemands.Count();
            double allCanceledDemands = this.m_canceledDemands.Count();
            return ((allAcceptedDemands - notFinishedDemands) / (allAcceptedDemands + allCanceledDemands));
        }

        public double CanceledDemandsShare()  // Доля отменённых заказов
        {
            double allAcceptedDemands = this.m_acceptedDemands.Count();
            double allCanceledDemands = this.m_canceledDemands.Count();
            return (allCanceledDemands / (allAcceptedDemands + allCanceledDemands));
        }

        public double SumWorkTime()  // Общееее время работы в минутах
        {
            double workTime=0;
            foreach (CPlanReportElement p in this.m_planReport.Values)
            {
                //workTime = workTime + (p.m_dtEndExecute - p.m_dtStartExecute).TotalMinutes; 
                //   из-за круглосуточной работы, а именно из-за костыля которым я это здесь реализовал, такой метод будет давать ошибки
                if (p.m_planElement.m_iDemandID == 0)
                {                    
                    workTime = workTime + CParams.retargetTimes[p.m_planElement.m_iProductID - 1];
                }
                else
                {
                    workTime = workTime + CParams.m_products[p.m_planElement.m_iProductID].m_iTime; ;
                }
            }
            return workTime;
        }

        public double SumRetargetTime()  // Общееее время перенастройки в минутах
        {
            double retargetTime = 0;
            foreach (CPlanReportElement p in this.m_planReport.Values)
            {
                if (p.m_planElement.m_iDemandID == 0)
                {
                    //retargetTime = retargetTime + (p.m_dtEndExecute - p.m_dtStartExecute).TotalMinutes;
                    //   из-за круглосуточной работы, а именно из-за костыля которым я это здесь реализовал, такой метод будет давать ошибки
                    retargetTime = retargetTime + CParams.retargetTimes[p.m_planElement.m_iProductID - 1];
                }
            }
            return retargetTime;
        }

        public int RefuseNum()  // Количество заявок от которых отказались
        {
            return m_declinedDemands.Count;
        }

        public int FinishedDemandsNum()  // Количество выполненных заявок
        {
            return (this.m_acceptedDemands.Count() - this.GetNotFinishedDemands().Length); 
        }

        public int CanceledDemandsNum()  // Количество отменённых заявок
        {
            return m_canceledDemands.Count(); 
        }

        public bool AddMaterialsStatisticDay(int[] materials)  
            // Добавить в статистику количество материалов в текущий день
        {
            for (int i=0; i<12 ; i++)
            {
                this.materialsPerDay[i+1].Add(materials[i]);
            }
            return true;
        }

        public int[][] GetMaterialsPerDayStatistic()      
            // Получить статистику изменения количества материалов на складе по дням
        {
            int[][] materials = new int[12][];
            for (int i=0; i<12 ; i++)
            {
                materials[i] = this.materialsPerDay[i+1].ToArray();
            }
            return materials;
        }

        public double[] GetIdlePerDayStatistic()
        // Получить статистику изменения доли простоя от времени производства по дням
        {
            return this.idleTimePerDay.ToArray();
        }

        public bool SaveIdleStatistic(double idleTime)
            // Сохранить время простоя на текущий день в статистику простоя
        {
            this.idleTimePerDay.Add(idleTime);
            return true;
        }

        public double[] GetDemandAverageDelayPerDayStatistic()
        // Получить статистику изменения среденего времени задержки заказов в днях по дням
        {
            return this.demandAverageDelayPerDay.ToArray();
        }

        public bool SaveDemandAverageDelayStatistic()
        // Сохранить среденее время задержки заказов на текущий день в статистику 
        {
            this.demandAverageDelayPerDay.Add(this.DemandAverageDelay());
            return true;
        }

        public bool SaveFinishedDemandsPerDayStatistic()
        // Сохранить долю выполненных заказов на текущий день в статистику 
        {
            this.finishedDemandsPerDay.Add(this.FinishedDemandsShare());
            return true;
        }

        public bool SaveCanceledDemandsPerDayStatistic()
        // Сохранить долю отменённых заказов на текущий день в статистику 
        {
            this.canceledDemandsPerDay.Add(this.CanceledDemandsShare());
            return true;
        }


        public double[] GetFinishedDemandsPerDayStatistic()
        // Получить статистику изменения доли выполненных заказов по дням
        {
            return this.finishedDemandsPerDay.ToArray();
        }
        
        public double[] GetCanceledDemandsPerDayStatistic()
        // Получить статистику изменения доли отменённых заказов по дням
        {
            return this.canceledDemandsPerDay.ToArray();
        }
      
        /*
        public CPlanReportElement GetFirstPlanReportElement()
        //Вернуть первый элемент выполнения плана
        {
            return m_planReport.First().Value;
        }

        public CPlanReportElement GetFirstPlanReportElementAndDeleteIT()
        //Вернуть и удалить первый элемент выполнения плана
        {
            CPlanReportElement res = m_planReport.First().Value;
            m_planReport.Remove(res.m_dtEndExecute);
            return res;
        }
         */
    }
}
