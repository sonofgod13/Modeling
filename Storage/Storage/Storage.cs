using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelingDataTypes;

namespace Storage
{
    public class CStorage //временное хранилище результатов моделирования
    {
        /// <summary>
        /// Статистика утверждённых изменений в заявках первое - число принятых, второе - отклонённых.
        /// Надо будет как нибудь это дело облагородить, может ввести парметры по каждой заявке
        /// </summary>
        private int[] modifyStatistic;

        /// <summary>
        /// Принятые Заявки. 
        /// </summary>
        private Dictionary<int, CDemand> acceptedDemands;

        /// <summary>
        /// Отклонённые Заявки
        /// </summary>
        private Dictionary<int, CDemand> declinedDemands;

        /// <summary>
        /// Отменённые Заявки
        /// </summary>
        private Dictionary<int, CDemand> canceledDemands;

        /// <summary>
        /// Очередь из элементов плана на день
        /// </summary>
        private Queue<CPlanElement> plan;

        /// <summary>
        /// Склад. Кластер материалов.
        /// </summary>
        public CMaterialCluster Materials;

        /// <summary>
        /// Статистика производства. Пара: время окончания выполнения - элемент плана
        /// </summary>
        private Dictionary<DateTime, CPlanReportElement> planReport;

        /// <summary>
        /// Обработанные заявки на поставку материалов
        /// </summary>
        private Dictionary<int, CDeliveryDemand> DeliveryDemands;

        /// <summary>
        /// Cтатистика - количество материалов на каждый день моделирования 
        /// </summary>
        private Dictionary<int, List<int>> materialsPerDay;

        /// <summary>
        /// Cтатистика - доля времени простоя производства от рабочего времени на каждый день моделирования
        /// </summary>
        private List<double> idleTimePerDay;

        /// <summary>
        /// Cтатистика - среденее время задержки заказов на каждый день моделирования
        /// </summary>
        private List<double> demandAverageDelayPerDay;

        /// <summary>
        /// Cтатистика - доля выполненных заказов на каждый день моделирования
        /// </summary>
        private List<double> finishedDemandsPerDay;

        /// <summary>
        /// Cтатистика - доля отменённых заказов на каждый день моделирования
        /// </summary>
        private List<double> canceledDemandsPerDay;


        /// <summary>
        /// Конструктор - инициализация значений
        /// </summary>
        public CStorage()
        {
            modifyStatistic = new int[] { 0, 0 };


            acceptedDemands = new Dictionary<int, CDemand>(); // Принятые Заявки.

            declinedDemands = new Dictionary<int, CDemand>(); // Отклонённые Заявки

            canceledDemands = new Dictionary<int, CDemand>(); // Оменённые Заявки


            plan = new Queue<CPlanElement>(); //очередь из элементов плана на день


            Materials = new CMaterialCluster();
            //Инициализация склада нулевыми значениями


            planReport = new Dictionary<DateTime, CPlanReportElement>();
            //Статистика производства. Пара: время окончания выполнения - элемент плана


            DeliveryDemands = new Dictionary<int, CDeliveryDemand>();
            // обработанные заявки на поставку материалов


            materialsPerDay = new Dictionary<int, List<int>>(); // количество материалов на каждый день моделирования
            for (var dayId = 1; dayId <= 12; dayId++)
                materialsPerDay.Add(dayId, new List<int>());

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

        /// <summary>
        /// Добавление новой заявки в список принятых
        /// </summary>
        /// <param name="demand"></param>
        /// <returns></returns>
        public bool AddAcceptedDemand(CDemand demand)
        {
            if (acceptedDemands.ContainsKey(demand.m_iID))
                return ModelError.Error();

            acceptedDemands.Add(demand.m_iID, demand);
            return true;
        }

        /// <summary>
        /// Добавление новой заявки в список отменённых
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AddCanceledDemand(int id)
        {

            if (acceptedDemands.ContainsKey(id))
                ModelError.Error();

            var demand = new CDemand(acceptedDemands[id]);
            var demandPlanElementReports = this.planReport.Values.Where(x => x.m_planElement.m_iDemandID == demand.m_iID);

            foreach (CPlanReportElement c in demandPlanElementReports)
            {
                c.m_planElement.m_iDemandID = -1;
            }

            acceptedDemands.Remove(id);

            if (canceledDemands.ContainsKey(id))
                return ModelError.Error();

            canceledDemands.Add(demand.m_iID, demand);
            return true;
        }

        /// <summary>
        /// Добавление новой заявки в список отклонённых
        /// </summary>
        /// <param name="demand"></param>
        /// <returns></returns>
        public bool AddDeclinedDemand(CDemand demand)
        {
            if (declinedDemands.ContainsKey(demand.m_iID))
                return ModelError.Error();

            declinedDemands.Add(demand.m_iID, demand);
            return true;
        }

        /// <summary>
        /// посчитать кол-во принятых заявок
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CDemand> GetNotFinishedDemands()
        {
            // TODO Denis Bykov: в исходном варианте массив копировался, нужно проверить, нет ли необходимости в этом
            var demands = acceptedDemands.Values;

            return demands.Where(d => !d.m_dtFinishing.HasValue);
        }

        /// <summary>
        /// Получить утверждённую заявку по id
        /// </summary>
        /// <param name="ind"></param>
        /// <param name="demand"></param>
        /// <returns></returns>
        public bool GetAcceptedDemand(int ind, out CDemand demand)
        {
            demand = new CDemand();

            if (!acceptedDemands.ContainsKey(ind))
                return ModelError.Error();

            //ВАЖНО! Возможно здесь надо принудительно вызвать деструктор для demand
            // 
            demand = new CDemand(this.acceptedDemands[ind]);
            return true;
        }

        /// <summary>
        /// Посчитать кол-во принятых заявок
        /// </summary>
        /// <returns></returns>
        public int GetAcceptedDemandsNumber()
        {
            return acceptedDemands.Count;
        }

        /// <summary>
        /// посчитать кол-во отклонённых заявок
        /// </summary>
        /// <returns></returns>
        public int GetDeclinedDemandsNumber()
        {
            return declinedDemands.Count;
        }

        /// <summary>
        /// изменить заявку в <see cref="acceptedDemands"/>
        /// </summary>
        /// <param name="modifiedDemand"></param>
        /// <returns></returns>
        public bool ModifyDemand(CDemand modifiedDemand)
        {
            if (!acceptedDemands.ContainsKey(modifiedDemand.m_iID))
                return ModelError.Error();

            var acceptedDemand = this.acceptedDemands[modifiedDemand.m_iID];

            //acceptedDemand.m_iUrgency = modifiedDemand.m_iUrgency;      срочность нельзя изменить
            acceptedDemand.m_dtShouldBeDone = modifiedDemand.m_dtShouldBeDone;

            /* //add product cluster
            for (int iProductNumber = 1; iProductNumber < CParams.PRODUCTS_NUMBER + 1; iProductNumber++)
            {
                acceptedDemand.m_products[iProductNumber] = modifiedDemand.m_products[iProductNumber];
            }
             */

            //--->
            acceptedDemand.m_products.CleanProductsCluster();
            acceptedDemand.m_products.AddProductCluster(modifiedDemand.m_products);
            //<---

            return true;
        }

        /// <summary>
        /// Переброска произведённых продуктов другому заказу
        /// </summary>
        /// <param name="demandInd"></param>
        /// <param name="prodId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool TranDemand(int demandInd, int prodId, int count)
        {
            try
            {
                var demandPlanElementReports = this.planReport.Values.Where(
                    x => (x.m_planElement.m_iDemandID == -1) &&
                         (x.m_planElement.m_iProductID == prodId)
                ).Take(count);

                foreach (var c in demandPlanElementReports)
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

        /// <summary>
        /// Выполнена ли полностью заявка
        /// </summary>
        /// <param name="demandInd"></param>
        /// <returns></returns>
        public bool IsDemandDone(int demandInd)
        {
            var demandPlanElementReports = this.planReport.Values.Where(x => x.m_planElement.m_iDemandID == demandInd).ToArray();
            int firstArticle = 0;
            int thirdArticle = 0;
            int secondArticle = 0;
            for (int i = 0; i < demandPlanElementReports.Length; i++)
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
            if (iProductValue > firstArticle)
                bToTrue = false;

            demand.m_products.GetProduct(2, out iProductValue);
            if (iProductValue > secondArticle)
                bToTrue = false;

            demand.m_products.GetProduct(3, out iProductValue);
            if (iProductValue > thirdArticle)
                bToTrue = false;

            return bToTrue;
        }

        /// <summary>
        /// Присвоить заявке в <see cref="acceptedDemands"/> время завершения (по id)
        /// </summary>
        /// <param name="demandInd"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool FinishDemand(int demandInd, DateTime date)
        {
            if (!acceptedDemands.ContainsKey(demandInd))
                return ModelError.Error();

            this.acceptedDemands[demandInd].m_dtFinishing = date;
            return true;
        }

        public bool AddModifyStatistic(bool modified)
        {
            if (modified == true)
                this.modifyStatistic[0]++;
            else
                this.modifyStatistic[1]++;

            return true;
        }

        /// <summary>
        /// добавить план на день
        /// </summary>
        /// <param name="planElements"></param>
        /// <returns></returns>
        public bool AddDailyPlan(CPlanElement[] planElements)
        {
            for (int i = 0; i < planElements.Length; i++)
            {
                plan.Enqueue(planElements[i]);
            }

            return true;
        }

        /// <summary>
        /// вынуть первый элемент плана
        /// </summary>
        /// <returns></returns>
        public CPlanElement GetFirstPlanElementAndDelete()
        {
            return plan.Dequeue();
        }

        /// <summary>
        /// вернуть первый элемент плана
        /// </summary>
        /// <returns></returns>
        public CPlanElement GetFirstPlanElement()
        {
            return plan.Peek();
        }

        /// <summary>
        /// очистить план
        /// </summary>
        /// <returns></returns>
        public bool ClearAllPlan()
        {
            plan.Clear();
            return true;
        }

        /// <summary>
        /// вернуть количество оставшихся для выполнения элементов плана
        /// </summary>
        /// <returns></returns>
        public int GetPlanElementsToGo()
        {
            return plan.Count;
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

        /// <summary>
        /// добавить обработанную заявку на поставку материалов
        /// </summary>
        /// <param name="deliveryDemand"></param>
        /// <returns></returns>
        public bool AddDeliveryDemand(CDeliveryDemand deliveryDemand)
        {
            if (DeliveryDemands.ContainsKey(deliveryDemand.m_iID))
                return ModelError.Error();

            this.DeliveryDemands.Add(deliveryDemand.m_iID, deliveryDemand);

            return true;
        }

        /// <summary>
        /// получить время ближайшей поставки материалов
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int GetNextDeliveryDemandTime(DateTime date)
        {
            int timeSpan = -1;
            foreach (var demand in this.DeliveryDemands.Values)
            {
                if (demand.isDone)
                    continue;

                int curTimeSpan = (int)(demand.m_dtRealDelivery.Value - date).TotalMinutes;

                DateTime checkDate = date.AddMinutes(curTimeSpan);

                if ((curTimeSpan >= 0) && (date.Year == checkDate.Year) && (date.Month == checkDate.Month) && (date.Day == checkDate.Day))
                {
                    if (timeSpan == -1)
                    {
                        timeSpan = curTimeSpan;
                    }
                    else if (curTimeSpan < timeSpan)
                    {
                        timeSpan = curTimeSpan;
                    }
                }
            }

            return timeSpan;
        }

        /// <summary>
        /// получить пришедшие в данное время поставки материалов 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public CDeliveryDemand[] GetDeliveryDemand(DateTime date)
        {
            var list = this.DeliveryDemands.Values.Where(
                d => (d.m_dtRealDelivery == date) && !d.isDone
            );

            return list.ToArray();
        }

        /// <summary>
        /// Добавить элемент выполнения плана
        /// </summary>
        /// <param name="planReportElement"></param>
        /// <returns></returns>
        public bool AddPlanReportElement(CPlanReportElement planReportElement)
        {
            planReport.Add(planReportElement.m_dtEndExecute, planReportElement);
            //!!! Здесь при добавлении еще нужно упорядочивать элементы
            return true;
        }

        /// <summary>
        /// Среденее время задержки заказов в днях
        /// </summary>
        /// <returns></returns>
        public double DemandAverageDelay()
        {
            int demandsNum = 0;
            double demandsDelaySum = 0;
            foreach (var demand in this.acceptedDemands.Values)
            {
                if ((demand.m_dtFinishing.HasValue == true) && (demand.m_dtShouldBeDone.HasValue == true))
                {
                    demandsNum++;
                    if (demand.m_dtFinishing.Value > demand.m_dtShouldBeDone.Value)
                    {
                        double span = (demand.m_dtFinishing.Value - demand.m_dtShouldBeDone.Value).TotalDays;
                        demandsDelaySum = demandsDelaySum + span;
                    }
                    else
                    {
                        double span = (demand.m_dtShouldBeDone.Value - demand.m_dtFinishing.Value).TotalDays;
                        demandsDelaySum = demandsDelaySum - span;
                    }
                }
            }

            return (demandsNum > 0)
                ? Math.Round(demandsDelaySum / demandsNum)
                : -1 ;
        }

        /// <summary>
        /// Доля выполненных заказов
        /// </summary>
        /// <returns></returns>
        public double FinishedDemandsShare()
        {
            double notFinishedDemands = this.GetNotFinishedDemands().Count();
            double allAcceptedDemands = this.acceptedDemands.Count();
            double allCanceledDemands = this.canceledDemands.Count();

            return ((allAcceptedDemands - notFinishedDemands) / (allAcceptedDemands + allCanceledDemands));
        }

        /// <summary>
        /// Доля отменённых заказов
        /// </summary>
        /// <returns></returns>
        public double CanceledDemandsShare()
        {
            double allAcceptedDemands = this.acceptedDemands.Count();
            double allCanceledDemands = this.canceledDemands.Count();

            return (allCanceledDemands / (allAcceptedDemands + allCanceledDemands));
        }

        /// <summary>
        /// Общееее время работы в минутах
        /// </summary>
        /// <returns></returns>
        public double SumWorkTime()
        {
            double workTime = 0;
            foreach (var reportElement in this.planReport.Values)
            {
                var planElement = reportElement.m_planElement;

                //workTime = workTime + (p.m_dtEndExecute - p.m_dtStartExecute).TotalMinutes; 
                //   из-за круглосуточной работы, а именно из-за костыля которым я это здесь реализовал, такой метод будет давать ошибки
                if (planElement.m_iDemandID == 0)
                {
                    workTime += CParams.retargetTimes[planElement.m_iProductID - 1];
                }
                else
                {
                    workTime += CParams.m_products[planElement.m_iProductID].m_iTime; ;
                }
            }

            return workTime;
        }

        /// <summary>
        /// Общее время перенастройки в минутах
        /// </summary>
        /// <returns></returns>
        public double SumRetargetTime()
        {
            double retargetTime = 0;
            foreach (var reportElement in this.planReport.Values)
            {
                var planElement = reportElement.m_planElement;
                if (planElement.m_iDemandID == 0)
                {
                    //retargetTime = retargetTime + (p.m_dtEndExecute - p.m_dtStartExecute).TotalMinutes;
                    //   из-за круглосуточной работы, а именно из-за костыля которым я это здесь реализовал, такой метод будет давать ошибки
                    retargetTime += CParams.retargetTimes[planElement.m_iProductID - 1];
                }
            }
            return retargetTime;
        }

        /// <summary>
        /// Количество заявок от которых отказались
        /// </summary>
        /// <returns></returns>
        public int RefuseNum
        {
            get { return declinedDemands.Count; }
        }

        /// <summary>
        /// Количество выполненных заявок
        /// </summary>
        /// <returns></returns>
        public int FinishedDemandsNum
        {
            get { return this.acceptedDemands.Count() - this.GetNotFinishedDemands().Count(); }
        }

        /// <summary>
        /// Количество отменённых заявок
        /// </summary>
        /// <returns></returns>
        public int CanceledDemandsNum
        {
            get { return canceledDemands.Count(); }
        }

        /// <summary>
        /// Добавить в статистику количество материалов в текущий день
        /// </summary>
        /// <param name="materials"></param>
        /// <returns></returns>
        public bool AddMaterialsStatisticDay(int[] materials)
        {
            for (int i = 0; i < 12; i++)
            {
                this.materialsPerDay[i + 1].Add(materials[i]);
            }
            return true;
        }

        /// <summary>
        /// Получить статистику изменения количества материалов на складе по дням
        /// </summary>
        /// <returns></returns>
        public int[][] GetMaterialsPerDayStatistic()
        {
            int[][] materials = new int[12][];
            for (int i = 0; i < 12; i++)
            {
                materials[i] = this.materialsPerDay[i + 1].ToArray();
            }
            return materials;
        }

        /// <summary>
        /// Получить статистику изменения доли простоя от времени производства по дням
        /// </summary>
        /// <returns></returns>
        public double[] GetIdlePerDayStatistic()
        {
            return this.idleTimePerDay.ToArray();
        }

        /// <summary>
        /// Сохранить время простоя на текущий день в статистику простоя
        /// </summary>
        /// <param name="idleTime"></param>
        /// <returns></returns>
        public bool SaveIdleStatistic(double idleTime)
        {
            this.idleTimePerDay.Add(idleTime);
            return true;
        }

        /// <summary>
        /// Получить статистику изменения среденего времени задержки заказов в днях по дням
        /// </summary>
        /// <returns></returns>
        public double[] GetDemandAverageDelayPerDayStatistic()
        {
            return this.demandAverageDelayPerDay.ToArray();
        }

        /// <summary>
        /// Сохранить среденее время задержки заказов на текущий день в статистику 
        /// </summary>
        /// <returns></returns>
        public bool SaveDemandAverageDelayStatistic()
        {
            this.demandAverageDelayPerDay.Add(this.DemandAverageDelay());
            return true;
        }

        /// <summary>
        /// Сохранить долю выполненных заказов на текущий день в статистику 
        /// </summary>
        /// <returns></returns>
        public bool SaveFinishedDemandsPerDayStatistic()
        {
            this.finishedDemandsPerDay.Add(this.FinishedDemandsShare());
            return true;
        }

        /// <summary>
        /// Сохранить долю отменённых заказов на текущий день в статистику 
        /// </summary>
        /// <returns></returns>
        public bool SaveCanceledDemandsPerDayStatistic()
        {
            this.canceledDemandsPerDay.Add(this.CanceledDemandsShare());
            return true;
        }

        /// <summary>
        /// Получить статистику изменения доли выполненных заказов по дням
        /// </summary>
        /// <returns></returns>
        public double[] GetFinishedDemandsPerDayStatistic()
        {
            return this.finishedDemandsPerDay.ToArray();
        }

        /// <summary>
        /// Получить статистику изменения доли отменённых заказов по дням
        /// </summary>
        /// <returns></returns>
        public double[] GetCanceledDemandsPerDayStatistic()
        {
            return this.canceledDemandsPerDay.ToArray();
        }

        /*
        /// <summary>
        /// Вернуть первый элемент выполнения плана
        /// </summary>
        /// <returns></returns>
        public CPlanReportElement GetFirstPlanReportElement()
        {
            return m_planReport.First().Value;
        }

        /// <summary>
        /// Вернуть и удалить первый элемент выполнения плана
        /// </summary>
        /// <returns></returns>
        public CPlanReportElement GetFirstPlanReportElementAndDeleteIT()
        {
            CPlanReportElement res = m_planReport.First().Value;
            m_planReport.Remove(res.m_dtEndExecute);
            return res;
        }
         */
    }
}
