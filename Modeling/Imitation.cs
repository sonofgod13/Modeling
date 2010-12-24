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
        private Storage.Storage storage;       //временное хранилище результатов моделирования
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

            generator = new Generator(
                Generator.CreateGenerator(Params.GeneratorDemandsTime.GeneratorType, Params.GeneratorDemandsTime.fA, Params.GeneratorDemandsTime.fB),
                Generator.CreateGenerator(Params.Products[1].GeneratorType, Params.Products[1].fA, Params.Products[1].fB),
                Generator.CreateGenerator(Params.Products[2].GeneratorType, Params.Products[2].fA, Params.Products[2].fB),
                Generator.CreateGenerator(Params.Products[3].GeneratorType, Params.Products[3].fA, Params.Products[3].fB),
                Params.fUrgencyPropabilityDemand, Params.fRefusePropabilityDemand,
                Generator.CreateGenerator(Params.DemandModifyTime.GeneratorType, Params.DemandModifyTime.fA, Params.DemandModifyTime.fB),
                Generator.CreateGenerator(Params.ArticlesModify.GeneratorType, Params.ArticlesModify.fA, Params.ArticlesModify.fB),
                Generator.CreateGenerator(Params.Products[1].Modify.GeneratorType, Params.Products[1].Modify.fA, Params.Products[1].Modify.fB),
                Generator.CreateGenerator(Params.Products[2].Modify.GeneratorType, Params.Products[2].Modify.fA, Params.Products[2].Modify.fB),
                Generator.CreateGenerator(Params.Products[3].Modify.GeneratorType, Params.Products[3].Modify.fA, Params.Products[3].Modify.fB),
                Generator.CreateGenerator(Params.UgrToStandModify.GeneratorType, Params.UgrToStandModify.fA, Params.UgrToStandModify.fB),
                Generator.CreateGenerator(Params.StandToUrgModify.GeneratorType, Params.StandToUrgModify.fA, Params.StandToUrgModify.fB),
                Generator.CreateGenerator(Params.DeliveryDelayGenerator.GeneratorType, Params.DeliveryDelayGenerator.fA, Params.DeliveryDelayGenerator.fB),
                new IGen[]
                {   
                    Generator.CreateGenerator(Params.Materials[1].GeneratorType, Params.Materials[1].fA, Params.Materials[1].fB),
                    Generator.CreateGenerator(Params.Materials[2].GeneratorType, Params.Materials[2].fA, Params.Materials[2].fB),
                    Generator.CreateGenerator(Params.Materials[3].GeneratorType, Params.Materials[3].fA, Params.Materials[3].fB),
                    Generator.CreateGenerator(Params.Materials[4].GeneratorType, Params.Materials[4].fA, Params.Materials[4].fB),
                    Generator.CreateGenerator(Params.Materials[5].GeneratorType, Params.Materials[5].fA, Params.Materials[5].fB),
                    Generator.CreateGenerator(Params.Materials[6].GeneratorType, Params.Materials[6].fA, Params.Materials[6].fB),
                    Generator.CreateGenerator(Params.Materials[7].GeneratorType, Params.Materials[7].fA, Params.Materials[7].fB),
                    Generator.CreateGenerator(Params.Materials[8].GeneratorType, Params.Materials[8].fA, Params.Materials[8].fB),
                    Generator.CreateGenerator(Params.Materials[9].GeneratorType, Params.Materials[9].fA, Params.Materials[9].fB),
                    Generator.CreateGenerator(Params.Materials[10].GeneratorType, Params.Materials[10].fA, Params.Materials[10].fB),
                    Generator.CreateGenerator(Params.Materials[11].GeneratorType, Params.Materials[11].fA, Params.Materials[11].fB),
                    Generator.CreateGenerator(Params.Materials[12].GeneratorType, Params.Materials[12].fA, Params.Materials[12].fB)
                }
            );
            this.modelingDays = Params.ModelingDayToWork;
            this.modelTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            this.startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);


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
        public DateTime GetModelingTime()
        {
            return this.modelTime;
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

            ////////////////////////////  Тут определены начальные заявки и материалы - это как оказалось не нужно((((

            //List<CDemand> demands = new List<CDemand>();
            //do
            //{
            //    CDemand[] newDemands = this.generator.generateDemands(startTime);
            //    int count = 0;
            //    if (newDemands.Length < 10) count = newDemands.Length;
            //    else count = 10;
            //    for (int i = 0; i < count; i++)
            //    {
            //        newDemands[i].m_dtGeting = startTime;
            //        if (newDemands[i].m_iUrgency == 2) newDemands[i].m_iUrgency = 0;
            //        demands.Add(newDemands[i]);
            //        bool approved = backOffice.approveDemand(ref newDemands[i]);
            //        storage.AddAcceptedDemand(newDemands[i]);
            //    }
            //} while (demands.Count < 10);

            ////Dictionary<int,int> materials = new Dictionary<int,int>{
            ////    {1,0},{2,0},{3,0},{4,0},{5,0},{6,0},{7,0},{8,0},{9,0},{10,0},{11,0},{12,0}
            ////};

            //CMaterialCluster materials = new CMaterialCluster();

            //foreach (CDemand d in demands)
            //{
            //    for(int iProductNumber = 1; iProductNumber < CParams.PRODUCTS_NUMBER + 1; iProductNumber++ ) 
            //    {
            //        for( int iMaterialNumber = 1; iMaterialNumber < CParams.MATERIALS_NUMBER+1; iMaterialNumber++ )
            //        {
            //            /***
            //            int iTempMaterialCount =
            //                        //            CParams.m_products[iProductNumber].m_iMaterials[iMaterialNumber - 1] * //количетсво материалов для продукта в заявке 
            //            d.m_products[ iProductNumber ]; //количество продуктов в заявке
            //             */

            //            //количетсво материалов для продукта в заявке
            //            int iTempMaterialCount = 0;
            //            CParams.m_products[iProductNumber].m_materials.GetMaterial(iMaterialNumber, out iTempMaterialCount); 

            //            //количество продуктов в заявке
            //            int iTempProductsCount = 0;
            //            d.m_products.GetProduct(iProductNumber, out iTempProductsCount);

            //            //***materials[ iMaterialNumber ] += iTempMaterialCount;
            //            //Добавляет в кластер materials iTempMaterialCount материалов с номером iMaterialNumber
            //            materials.AddMaterial(iMaterialNumber, iTempMaterialCount * iTempProductsCount);

            //            //***this.storage.AddMaterialNumber(iMaterialNumber, iTempMaterialCount);
            //            //Добавляем на склад к материалу iMaterialNumber количество iTempMaterialCount
            //            this.storage.m_materials.AddMaterial(iMaterialNumber, iTempMaterialCount * iTempProductsCount);
            //        }
            //    }
            //}
            ////***CDeliveryDemand deliv = new CDeliveryDemand(-1,startTime,materials);
            ////Создание заявки на поставку материалов
            //CDeliveryDemand deliv = new CDeliveryDemand(-1, startTime, materials);
            //deliv.m_dtRealDelivery = startTime;
            //this.storage.AddDeliveryDemand(deliv);
            //backOffice.reportDeliveryDemand(deliv);

            ////////////////////////////////

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
                        /*                        
                        for (int j = 1; j <= CParams.MATERIALS_NUMBER; j++)
                        {
                            //***int materialNum = CParams.m_products[this.storage.GetFirstPlanElement().m_iProductID].m_iMaterials[j - 1];
                            int materialNum = 0;
                            //получает в materialNum количество материала № j нужное для производства продукта
                            CParams.m_products[this.storage.GetFirstPlanElement().m_iProductID].m_materials.GetMaterial(j, out materialNum);
       
                            if (materialNum > 0)
                            {
                                //***this.storage.DeleteMaterialNumber(j, materialNum);
                                //удаление со склада от материала с номером j количества materialNum
                                this.storage.m_materials.TakeAwayMaterial(j, materialNum); 
                            }
                        }
                        */
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
                                        throw new Exception("Не достаточно материалов для производства товара");
                                    /*
                                    for (int j = 1; j <= CParams.MATERIALS_NUMBER; j++)
                                    {   
                                        //***int materialNum = CParams.m_products[this.storage.GetFirstPlanElement().m_iProductID + 1].m_iMaterials[j - 1];
                                        int materialNum = 0;
                                        //получает в materialNum количество материала № j нужное для производства продукта
                                        CParams.m_products[this.storage.GetFirstPlanElement().m_iProductID].m_materials.GetMaterial(j, out materialNum);

                                        if (materialNum > 0)
                                        {
                                            //***this.storage.DeleteMaterialNumber(j, materialNum);
                                            //удаление со склада от материала с номером j количества materialNum
                                            this.storage.m_materials.TakeAwayMaterial(j, materialNum); 
                                        }
                                    }
                                    */
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
                                    /*
                                    for (int j = 1; j <= CParams.MATERIALS_NUMBER; j++)
                                    {
                                        //***int materialNum = d.m_materialsDemand[j];
                                        int materialNum = 0;
                                        d.m_materialsDemand.GetMaterial(j, out materialNum);

                                        //if (materialNum > 0)
                                        //{
                                            //***this.storage.AddMaterialNumber(j, materialNum);
                                            //добавление на склад количества materialNum материала номером j
                                            this.storage.m_materials.AddMaterial(j, materialNum);
                                        //}
                                    }
                                    */
                                    backOffice.ReportDeliveryDemand(d);
                                    d.IsDone = true;
                                }
                            }
                        }


                        //// Не нужно если план не меняется в течение дня

                        //if (nextPlanElemEndTime == -1)
                        //{
                        //    curPlanElem = new CPlanReportElement();
                        //    try
                        //    {
                        //        int prodId = this.storage.GetFirstPlanElement().m_iProductID;
                        //        nextPlanElemEndTime = CParams.m_products[prodId].m_iTime;
                        //        this.storage.m_materials.TakeAwayMaterialCluster(CParams.m_products[prodId].m_materials);
                        //        /*
                        //        for (int j = 1; j <= CParams.MATERIALS_NUMBER; j++)
                        //        {
                        //            //***int materialNum = CParams.m_products[this.storage.GetFirstPlanElement().m_iProductID + 1].m_iMaterials[j - 1];
                        //            int materialNum = 0;
                        //            //получает в materialNum количество материала № j нужное для производства продукта
                        //            CParams.m_products[this.storage.GetFirstPlanElement().m_iProductID].m_materials.GetMaterial(j, out materialNum);

                        //            if (materialNum > 0)
                        //            {
                        //                //***this.storage.DeleteMaterialNumber(j, materialNum);
                        //                //удаление со склада от материала с номером j количества materialNum
                        //                this.storage.m_materials.TakeAwayMaterial(j, materialNum); 
                        //            }
                        //        }
                        //        */
                        //        curPlanElem.m_dtStartExecute = modelTime;
                        //    }
                        //    catch
                        //    {
                        //        nextPlanElemEndTime = -1;
                        //    }
                        //}


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
