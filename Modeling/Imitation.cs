using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelingDataTypes;
using GeneratorSubsystem;
using Storage;
using System.Windows.Forms;
using System.Threading;

namespace Modeling
{
    public class Imitation
    {
        private BackOfficeInterface backOffice;

        /// <summary>
        /// Временное хранилище результатов моделирования
        /// </summary>
        private Storage.Storage storage;
        private Generator generator;
        private DateTime modelTime;
        private DateTime startTime;
        private int modelingDays;
        private bool stopFlag;
        private static AutoResetEvent pauseDone;
        private int currentModellingDay;

        public Imitation()
        {
            this.backOffice = new BackOfficeInterface();
            storage = new Storage.Storage();   //содается временное хранилище результатов моделирования
            pauseDone = new AutoResetEvent(false);

            this.generator = new Generator(
                Generator.CreateGenerator(Params.GeneratorDemandsTime),
                this.CreateGenerators(Params.Products),
                Params.fUrgencyPropabilityDemand, Params.fRefusePropabilityDemand,
                Generator.CreateGenerator(Params.DemandModifyTime),
                Generator.CreateGenerator(Params.ArticlesModify),
                this.CreateGenerators(Params.Products, p => p.Modify),
                Generator.CreateGenerator(Params.UgrToStandModify),
                Generator.CreateGenerator(Params.StandToUrgModify),
                Generator.CreateGenerator(Params.DeliveryDelayGenerator),
                this.CreateGenerators(Params.Materials)
            );

            this.modelingDays = Params.ModelingDayToWork;
            this.modelTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            this.startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        }

        private IGen[] CreateGenerators<T>(Dictionary<int, T> parameters)
            where T : GeneratedElement
        {
            return this.CreateGenerators(parameters, p => p);
        }

        private IGen[] CreateGenerators<T>(Dictionary<int, T> parameters, Func<T, GeneratedElement> getParam)
        {
            return (from generatorInfo in parameters
                    orderby generatorInfo.Key
                    select Generator.CreateGenerator(getParam(generatorInfo.Value))).ToArray();
        }

        private void setLabelText(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                label.Invoke(
                    new Action<Label, String>(setLabelText),
                    new object[] { label, text }
                );
            }
            else
            {
                label.Text = text;
            }

        }

        /// <summary>
        /// Текущее модельное время
        /// </summary>
        /// <returns></returns>
        public DateTime ModelingTime
        {
            get { return this.modelTime; }
        }

        public bool SaveNewFrontDemands(Modeling.FrontOffice.Order[] newFrontOrders)
        {
            foreach (var order in newFrontOrders)
            {
                int urg = 0;
                if (order.isExpress == true) urg = 1;
                /*
                CDemand demand = new CDemand(order.OrderID, this.modelTime, urg, new Dictionary<int, int> { { 1, order.ProductCount[0] }, { 2, order.ProductCount[1] }, { 3, order.ProductCount[2] } });
                */

                //--->
                ProductCluster productCluster = new ProductCluster();
                for (int iProductNumber = 1; iProductNumber <= Params.PRODUCTS_NUMBER; iProductNumber++)
                {
                    productCluster.AddProduct(iProductNumber, order.ProductCount[iProductNumber - 1]);
                }

                Demand demand = new Demand(order.OrderID, this.modelTime, urg, productCluster);
                //<---

                demand.ShouldBeDoneDate = order.doneDate;

                if (!this.storage.AddAcceptedDemand(demand))
                    return false;
            }

            return true;
        }

        public bool SaveChangedFrontDemands(Modeling.FrontOffice.Order[] changedFrontOrders)
        {
            foreach (var order in changedFrontOrders)
            {
                int urg = 0;
                if (order.isExpress == true) urg = 1;

                ProductCluster productCluster = new ProductCluster();

                for (int iProductNumber = 1; iProductNumber <= Params.PRODUCTS_NUMBER; iProductNumber++)
                {
                    productCluster.AddProduct(iProductNumber, order.ProductCount[iProductNumber - 1]);
                }

                Demand demand = new Demand(order.OrderID, new DateTime(), urg, productCluster);
                demand.ShouldBeDoneDate = order.doneDate;

                if (!this.storage.ModifyDemand(demand))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Среднее время задержки заказов в днях
        /// </summary>
        /// <returns></returns>
        public double GetDemandAverageDelay()
        {
            return this.storage.DemandAverageDelay();
        }

        /// <summary>
        /// Количество выполненных заказов
        /// </summary>
        /// <returns></returns>
        public double GetFinishedDemandsNum()
        {
            return this.storage.FinishedDemandsNum;
        }

        /// <summary>
        /// Количество отменённых заказов
        /// </summary>
        /// <returns></returns>
        public double GetCanceledDemandsNum()
        {
            return this.storage.CanceledDemandsNum;
        }

        /// <summary>
        /// Количество полученных заказов
        /// </summary>
        /// <returns></returns>
        public double GetAcceptedDemandsNum()
        {
            return this.storage.GetAcceptedDemandsNumber();
        }

        /// <summary>
        /// Коэффициент использования системы
        /// </summary>
        /// <returns></returns>
        public double GetActivityFactor()
        {
            return this.storage.SumWorkTime() / (Params.WORKDAY_MINUTES_NUMBER * this.modelingDays);
        }

        /// <summary>
        /// Доля времени перенастройки от общего времени производства
        /// </summary>
        /// <returns></returns>
        public double GetRetargetTimePercent()
        {
            return this.storage.SumRetargetTime() / this.storage.SumWorkTime();
        }

        /// <summary>
        /// Количество заявок от которых отказались
        /// </summary>
        /// <returns></returns>
        public int GetRefusesNum()
        {
            return this.storage.RefuseNum;
        }

        /// <summary>
        /// Получить статистику изменения количества материалов на складе по дням
        /// </summary>
        /// <returns></returns>
        public int[][] GetMaterialsPerDayStatistic()
        {
            return this.storage.GetMaterialsPerDayStatistic();
        }

        /// <summary>
        /// Получить статистику изменения доли времени простоя производства от рабочего времени по дням
        /// </summary>
        /// <returns></returns>
        public double[] GetIdlePerDayStatistic()
        {
            return this.storage.GetIdlePerDayStatistic();
        }

        /// <summary>
        /// Получить среднее время задержки заказов в днях по дням
        /// </summary>
        /// <returns></returns>
        public double[] GetDemandAverageDelayPerDayStatistic()
        {
            return this.storage.GetDemandAverageDelayPerDayStatistic();
        }

        /// <summary>
        /// Получить статистику изменения доли выполненных заказов
        /// </summary>
        /// <returns></returns>
        public double[] GetFinishedDemandsPerDayStatistic()
        {
            return this.storage.GetFinishedDemandsPerDayStatistic();
        }

        /// <summary>
        /// Получить статистику изменения доли отменённых заказов
        /// </summary>
        /// <returns></returns>
        public double[] GetCanceledDemandsPerDayStatistic()
        {
            return this.storage.GetCanceledDemandsPerDayStatistic();
        }


        /// <summary>
        /// Начальные заявки и материалы, запуск итератора
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public bool Start(Label label)
        {
            // CDemand.idNext = 0; // сброс счетчика уникальности заявок
            this.currentModellingDay = 0;
            this.stopFlag = false;
            this.backOffice.StartModeling(this.modelTime);

            this.Iteration(label);            //запуск основного цикла
            return !stopFlag;
        }

        /// <summary>
        /// Остановка и пауза моделирования
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            if (stopFlag == true)
                return true;

            if (currentModellingDay == (Params.ModelingDayToWork - 1))
                return false;

            this.stopFlag = true;
            pauseDone.WaitOne();

            return true;
        }

        /// <summary>
        /// Продолжить моделирование
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public bool Continue(Label label)
        {
            this.stopFlag = false;
            this.Iteration(label);

            return !stopFlag;
        }

        private void Iteration(Label label)
        {
            for (int i = this.currentModellingDay; i < this.modelingDays; i++)
            {
                this.currentModellingDay = i;
                if (this.stopFlag == true)
                {
                    pauseDone.Set();
                    break;
                }

                DateTime workdayStartTime = startTime.AddDays(i);
                modelTime = workdayStartTime;

                var materialsNumToday = new int[12];
                for (int j = 0; j < 12; j++)
                {
                    //***materialsNumToday[j]=this.storage.GetMaterialNumberFromID(j+1);
                    //возвращает со склада в materialsNumToday[j] количество материала с номером (j + 1)
                    this.storage.Materials.GetMaterial(j + 1, out materialsNumToday[j]);
                }
                this.storage.AddMaterialsStatisticDay(materialsNumToday);
                var newDemands = this.generator.GenerateDemands(workdayStartTime);
                int[] modifyDemandsTime = this.generator.GenerateModifyTime();

                ////////////////////////////////////////////////// Обращение к back-office
                this.storage.ClearAllPlan();
                this.storage.AddDailyPlan(this.backOffice.GetDailyPlan(modelTime, ref this.storage));

                int rem = -1;
                int span = (int)(modelTime - startTime).TotalDays;
                Math.DivRem(span, Params.DELIVERY_PERIOD, out rem);
                if ((rem == 0) && (span != 0))
                {
                    var delivery = this.backOffice.GetDeliveryDemands(modelTime);
                    if (delivery != null)
                    {
                        var dlvr = new DeliveryDemand[1] { delivery }; // это мне было лень generator.modifyDeliveries() переписывать
                        this.storage.AddDeliveryDemand(this.generator.ModifyDeliveries(dlvr).ElementAt(0));
                    }
                }
                //////////////////////////////////////////////////


                int runtimeModifyDemandsSumTime = 0;
                int newDemandInd = 0;
                int modifyDemandInd = 0;
                int nextPlanElemEndTime = 0;
                int todayWorkTime = 0;
                var curPlanElem = new PlanReportElement();
                try
                {
                    if (this.storage.GetFirstPlanElement().DemandID != 0)
                    {
                        int prodId = this.storage.GetFirstPlanElement().ProductID;
                        nextPlanElemEndTime = Params.Products[prodId].Time;
                        bool canDo = this.storage.Materials.TakeAwayMaterialCluster(Params.Products[prodId].Materials);

                        if (!canDo)
                            throw new Exception("Не достаточно материалов для производства товара");
                    }
                    else
                    {
                        nextPlanElemEndTime = Params.RetargetTimes[this.storage.GetFirstPlanElement().ProductID - 1];
                    }
                    curPlanElem.StartExecuteDate = modelTime;
                }
                catch
                {
                    nextPlanElemEndTime = -1;
                }
                if (nextPlanElemEndTime != -1) todayWorkTime += nextPlanElemEndTime;

                bool endOfDayFlag = false;

                int k = 0;
                while (endOfDayFlag == false)
                {
                    k++;

                    int newDemandNextTime = -1;
                    int modifyDemandNextTime = -1;
                    int nextDeliveryDemandTime = this.storage.GetNextDeliveryDemandTime(modelTime);

                    if (newDemandInd < newDemands.Length)
                    {
                        newDemandNextTime = (int)(newDemands[newDemandInd].GettingDate - modelTime).TotalMinutes;
                    }
                    if (modifyDemandInd < modifyDemandsTime.Length)
                    {
                        runtimeModifyDemandsSumTime = modifyDemandsTime.Take(modifyDemandInd).Sum() + modifyDemandsTime[modifyDemandInd];
                        modifyDemandNextTime = runtimeModifyDemandsSumTime - (int)(modelTime - workdayStartTime).TotalMinutes;
                    }

                    if ((nextPlanElemEndTime == -1) && (newDemandNextTime == -1) &&
                        (modifyDemandNextTime == -1) && (nextDeliveryDemandTime == -1))
                    {
                        endOfDayFlag = true;
                    }
                    else
                    {
                        var nextTimes = new List<int>();
                        if (nextPlanElemEndTime != -1) nextTimes.Add(nextPlanElemEndTime);
                        if (newDemandNextTime != -1) nextTimes.Add(newDemandNextTime);
                        if (modifyDemandNextTime != -1) nextTimes.Add(modifyDemandNextTime);
                        if (nextDeliveryDemandTime != -1) nextTimes.Add(nextDeliveryDemandTime);

                        modelTime = modelTime.AddMinutes(nextTimes.Min());

                        if (modelTime.Day != workdayStartTime.Day)
                            break;

                        setLabelText(label, "Модельное время: " + modelTime.ToString());

                        if (nextTimes.Min() == newDemandNextTime)
                        {
                            bool approved = backOffice.ApproveDemand(ref newDemands[newDemandInd]);

                            if (approved)
                                storage.AddAcceptedDemand(newDemands[newDemandInd]);
                            else
                                storage.AddDeclinedDemand(newDemands[newDemandInd]);

                            newDemandInd++;
                        }

                        if (nextTimes.Min() == modifyDemandNextTime)
                        {
                            var notFinishedDemands = this.storage.GetNotFinishedDemands();

                            if (notFinishedDemands.Count() > 0)
                            {
                                Demand modifiedDemand = this.generator.ModifyDemand(notFinishedDemands.ToArray(), modelTime);
                                Demand demand;
                                this.storage.GetAcceptedDemand(modifiedDemand.ID, out demand);
                                bool approved = backOffice.ApproveModifyDemand(modelTime, ref modifiedDemand, demand);

                                if (approved)
                                    this.storage.ModifyDemand(modifiedDemand);

                                this.storage.AddModifyStatistic(approved);
                            }
                            modifyDemandInd++;
                        }

                        if ((nextTimes.Min() != nextPlanElemEndTime) && (nextPlanElemEndTime != -1))
                            nextPlanElemEndTime = nextPlanElemEndTime - nextTimes.Min();

                        if (nextTimes.Min() == nextPlanElemEndTime)
                        {
                            PlanElement planElem = this.storage.GetFirstPlanElementAndDelete();
                            curPlanElem.PlanElement = planElem;
                            curPlanElem.EndExecuteDate = modelTime;
                            this.storage.AddPlanReportElement(curPlanElem);
                            // backOffice.reportPlanElem(curPlanElem);   отчёт в бекофис не надо
                            if ((planElem.DemandID != 0) && (this.storage.IsDemandDone(planElem.DemandID) == true))
                                this.storage.FinishDemand(planElem.DemandID, modelTime);

                            curPlanElem = new PlanReportElement();
                            try
                            {
                                if (this.storage.GetFirstPlanElement().DemandID != 0)
                                {
                                    int prodId = this.storage.GetFirstPlanElement().ProductID;

                                    nextPlanElemEndTime = Params.Products[prodId].Time;

                                    bool canDo = this.storage.Materials.TakeAwayMaterialCluster(Params.Products[prodId].Materials);

                                    if (!canDo)
                                        throw new Exception("Недостаточно материалов для производства товара");

                                }
                                else
                                {
                                    nextPlanElemEndTime = Params.RetargetTimes[this.storage.GetFirstPlanElement().ProductID - 1];
                                }
                                curPlanElem.StartExecuteDate = modelTime;

                            }
                            catch
                            {
                                nextPlanElemEndTime = -1;
                            }
                            if (nextPlanElemEndTime != -1)
                                todayWorkTime += nextPlanElemEndTime;

                            if (nextPlanElemEndTime != -1)    // костыль для реализации круглосуточной работы
                            {
                                TimeSpan nextPlanElemEndTimeSpan = new TimeSpan(0, nextPlanElemEndTime, 0);
                                DateTime nextWorkDayStartTime = workdayStartTime + new TimeSpan(1, 0, 0, 0);
                                if (modelTime + nextPlanElemEndTimeSpan > nextWorkDayStartTime)
                                {
                                    nextPlanElemEndTime = (int)(nextWorkDayStartTime - modelTime).TotalMinutes - this.storage.GetPlanElementsToGo();
                                }
                            }
                        }

                        if (nextTimes.Min() == nextDeliveryDemandTime)
                        {
                            var deliveryDemands = this.storage.GetDeliveryDemand(modelTime);

                            if (deliveryDemands.Length > 0)
                            {
                                foreach (DeliveryDemand d in deliveryDemands)
                                {
                                    this.storage.Materials.AddMaterialCluster(d.MaterialsDemand);

                                    backOffice.ReportDeliveryDemand(d);
                                    d.IsDone = true;
                                }
                            }
                        }
                    }

                }

                //this.storage.SaveIdleStatistic((double)(CParams.WORKDAY_MINUTES_NUMBER - todayWorkTime) / CParams.WORKDAY_MINUTES_NUMBER);
                this.storage.SaveDemandAverageDelayStatistic();
                this.storage.SaveFinishedDemandsPerDayStatistic();
                this.storage.SaveCanceledDemandsPerDayStatistic();
            }

            if (stopFlag != true)
                setLabelText(label, "Модельное время: " + modelTime.ToString() + "\nМоделирование завершено");
        }
    }
}
