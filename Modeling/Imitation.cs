﻿using System;
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
    public class CImitation
    {
        private BackOfficeInterface backOffice;
        private CStorage storage;       //временное хранилище результатов моделирования
        private Generator generator;    
        private DateTime modelTime;
        private DateTime startTime;
        private int modelingDays;
        private bool stopFlag;
        private static AutoResetEvent pauseDone;
        private int currentModellingDay;

        public CImitation()
        {
            this.backOffice = new BackOfficeInterface();
            storage = new Storage.CStorage();   //содается временное хранилище результатов моделирования
            pauseDone = new AutoResetEvent(false);

            generator = new Generator(
                Generator.CreateGenerator(CParams.GeneratorDemandsTime.GeneratorType, CParams.GeneratorDemandsTime.fA, CParams.GeneratorDemandsTime.fB),
                Generator.CreateGenerator(CParams.Products[1].GeneratorType, CParams.Products[1].fA, CParams.Products[1].fB),
                Generator.CreateGenerator(CParams.Products[2].GeneratorType, CParams.Products[2].fA, CParams.Products[2].fB),
                Generator.CreateGenerator(CParams.Products[3].GeneratorType, CParams.Products[3].fA, CParams.Products[3].fB),
                CParams.fUrgencyPropabilityDemand, CParams.fRefusePropabilityDemand,
                Generator.CreateGenerator(CParams.DemandModifyTime.GeneratorType, CParams.DemandModifyTime.fA, CParams.DemandModifyTime.fB),
                Generator.CreateGenerator(CParams.ArticlesModify.GeneratorType, CParams.ArticlesModify.fA, CParams.ArticlesModify.fB),
                Generator.CreateGenerator(CParams.Products[1].Modify.GeneratorType, CParams.Products[1].Modify.fA, CParams.Products[1].Modify.fB),
                Generator.CreateGenerator(CParams.Products[2].Modify.GeneratorType, CParams.Products[2].Modify.fA, CParams.Products[2].Modify.fB),
                Generator.CreateGenerator(CParams.Products[3].Modify.GeneratorType, CParams.Products[3].Modify.fA, CParams.Products[3].Modify.fB),
                Generator.CreateGenerator(CParams.UgrToStandModify.GeneratorType, CParams.UgrToStandModify.fA, CParams.UgrToStandModify.fB),
                Generator.CreateGenerator(CParams.StandToUrgModify.GeneratorType, CParams.StandToUrgModify.fA, CParams.StandToUrgModify.fB),
                Generator.CreateGenerator(CParams.DeliveryDelayGenerator.GeneratorType, CParams.DeliveryDelayGenerator.fA, CParams.DeliveryDelayGenerator.fB),
                new IGen[]
                {   
                    Generator.CreateGenerator(CParams.Materials[1].GeneratorType, CParams.Materials[1].fA, CParams.Materials[1].fB),
                    Generator.CreateGenerator(CParams.Materials[2].GeneratorType, CParams.Materials[2].fA, CParams.Materials[2].fB),
                    Generator.CreateGenerator(CParams.Materials[3].GeneratorType, CParams.Materials[3].fA, CParams.Materials[3].fB),
                    Generator.CreateGenerator(CParams.Materials[4].GeneratorType, CParams.Materials[4].fA, CParams.Materials[4].fB),
                    Generator.CreateGenerator(CParams.Materials[5].GeneratorType, CParams.Materials[5].fA, CParams.Materials[5].fB),
                    Generator.CreateGenerator(CParams.Materials[6].GeneratorType, CParams.Materials[6].fA, CParams.Materials[6].fB),
                    Generator.CreateGenerator(CParams.Materials[7].GeneratorType, CParams.Materials[7].fA, CParams.Materials[7].fB),
                    Generator.CreateGenerator(CParams.Materials[8].GeneratorType, CParams.Materials[8].fA, CParams.Materials[8].fB),
                    Generator.CreateGenerator(CParams.Materials[9].GeneratorType, CParams.Materials[9].fA, CParams.Materials[9].fB),
                    Generator.CreateGenerator(CParams.Materials[10].GeneratorType, CParams.Materials[10].fA, CParams.Materials[10].fB),
                    Generator.CreateGenerator(CParams.Materials[11].GeneratorType, CParams.Materials[11].fA, CParams.Materials[11].fB),
                    Generator.CreateGenerator(CParams.Materials[12].GeneratorType, CParams.Materials[12].fA, CParams.Materials[12].fB)
                }
            );
            this.modelingDays = CParams.ModelingDayToWork;
            this.modelTime = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,0,0,0);
            this.startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);


        }

        private delegate void lDelegate(Label label, string text);
        private void setLabelText(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                lDelegate deleg = new lDelegate(setLabelText);
                label.Invoke(deleg, new object[] { label, text });
            }
            else
            {
                label.Text = text;
            }

        }

        public DateTime getModelingTime()   // Тукущее модельное время
        {
            return this.modelTime;
        }

        public bool saveNewFrontDemands(Modeling.FrontOffice.Order[] newFrontOrders)
        {
            foreach(Modeling.FrontOffice.Order order in newFrontOrders)
            {
                int urg=0;
                if (order.isExpress==true) urg=1;
                /*
                CDemand demand = new CDemand(order.OrderID, this.modelTime, urg, new Dictionary<int, int> { { 1, order.ProductCount[0] }, { 2, order.ProductCount[1] }, { 3, order.ProductCount[2] } });
                */

                //--->
                CProductCluster productCluster = new CProductCluster();
                for (int iProductNumber = 1; iProductNumber <= CParams.PRODUCTS_NUMBER; iProductNumber++)
                {
                    productCluster.AddProduct(iProductNumber, order.ProductCount[iProductNumber - 1 ]);
                }

                CDemand demand = new CDemand(order.OrderID, this.modelTime, urg, productCluster);
                //<---

                demand.ShouldBeDoneDate = order.doneDate;
                bool addResult = this.storage.AddAcceptedDemand(demand);
                if (addResult==false) return false;
            }
            return true;
        }

        public bool saveChangedFrontDemands(Modeling.FrontOffice.Order[] changedFrontOrders)
        {
            foreach (Modeling.FrontOffice.Order order in changedFrontOrders)
            {
                int urg = 0;
                if (order.isExpress == true) urg = 1;                
                CProductCluster productCluster = new CProductCluster();
                for (int iProductNumber = 1; iProductNumber <= CParams.PRODUCTS_NUMBER; iProductNumber++)
                {
                    productCluster.AddProduct(iProductNumber, order.ProductCount[iProductNumber - 1]);
                }
                CDemand demand = new CDemand(order.OrderID, new DateTime(), urg, productCluster);               
                demand.ShouldBeDoneDate = order.doneDate;
                bool changeResult = this.storage.ModifyDemand(demand);
                if (changeResult == false) return false;
            }
            return true;
        }

        public double getDemandAverageDelay()   // Среднее время задержки заказов в днях
        {
            return this.storage.DemandAverageDelay();
        }

        public double getFinishedDemandsNum()   // Количество выполненных заказов
        {
            return this.storage.FinishedDemandsNum;
        }

        public double getCanceledDemandsNum()   // Количество отменённых заказов
        {
            return this.storage.CanceledDemandsNum;
        }

        public double getAcceptedDemandsNum()   // Количество полученных заказов
        {
            return this.storage.GetAcceptedDemandsNumber();
        }

        public double getActivityFactor()    // Коэффициент использования системы
        {
            return this.storage.SumWorkTime()/(CParams.WORKDAY_MINUTES_NUMBER*this.modelingDays);
        }

        public double getRetargetTimePercent()   // Доля времени перенастройки от общего времени производства
        {
            return this.storage.SumRetargetTime() / this.storage.SumWorkTime();
        }

        public int getRefusesNum()    // Количество заявок от которых отказались
        {
            return this.storage.RefuseNum;
        }

        public int[][] getMaterialsPerDayStatistic()      // Получить статистику изменения количества материалов на складе по дням
        {
            return this.storage.GetMaterialsPerDayStatistic();
        }

        public double[] getIdlePerDayStatistic()      // Получить статистику изменения доли времени простоя производства от рабочего времени по дням
        {
            return this.storage.GetIdlePerDayStatistic();
        }

        public double[] getDemandAverageDelayPerDayStatistic()   // Получить среднее время задержки заказов в днях по дням
        {
            return this.storage.GetDemandAverageDelayPerDayStatistic();
        }

         public double[] getFinishedDemandsPerDayStatistic()      // Получить статистику изменения доли выполненных заказов
        {
            return this.storage.GetFinishedDemandsPerDayStatistic();
        }

         public double[] getCanceledDemandsPerDayStatistic()       // Получить статистику изменения доли отменённых заказов
         {
             return this.storage.GetCanceledDemandsPerDayStatistic();
         }
       

        public bool Start(Label label)       // Начальные заявки и материалы, запуск итератора
        {
           // CDemand.idNext = 0; // сброс счетчика уникальности заявок
            this.currentModellingDay = 0;
            this.stopFlag = false;
            this.backOffice.startModeling(this.modelTime);
            
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

        public bool Stop()  // остановка и пауза моделирования
        {
            if (stopFlag == true) return true;
            else
            {
                if (currentModellingDay == (CParams.ModelingDayToWork - 1)) return false;
                else
                {
                    this.stopFlag = true;
                    pauseDone.WaitOne();
                    return true;
                }
            }
        }

        public bool Continue(Label label) // продолжить моделирование
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

                int[] materialsNumToday = new int[12];
                for (int j=0;j<12;j++)
                {
                    //***materialsNumToday[j]=this.storage.GetMaterialNumberFromID(j+1);
                    //возвращает со склада в materialsNumToday[j] количество материала с номером (j + 1)
                    this.storage.Materials.GetMaterial(j + 1, out materialsNumToday[j]);
                }
                this.storage.AddMaterialsStatisticDay(materialsNumToday);
                CDemand[] newDemands = this.generator.generateDemands(workdayStartTime);
                int[] modifyDemandsTime = this.generator.generateModifyTime();
                
                ////////////////////////////////////////////////// Обращение к back-office
                this.storage.ClearAllPlan();
                this.storage.AddDailyPlan(this.backOffice.getDailyPlan(modelTime,ref this.storage));

                int rem = -1;
                int span = (int)(modelTime-startTime).TotalDays;
                Math.DivRem(span,CParams.DELIVERY_PERIOD,out rem);
                if ((rem == 0)&&(span!=0))
                {
                    CDeliveryDemand delivery = this.backOffice.getDeliveryDemands(modelTime);
                    if (delivery != null)
                    {
                        CDeliveryDemand[] dlvr = new CDeliveryDemand[1] { delivery }; // это мне было лень generator.modifyDeliveries() переписывать
                        this.storage.AddDeliveryDemand(this.generator.modifyDeliveries(dlvr).ElementAt(0));
                    }
                }
                //////////////////////////////////////////////////

                
                int runtimeModifyDemandsSumTime = 0;
                int newDemandInd = 0;
                int modifyDemandInd = 0;
                int nextPlanElemEndTime = 0;
                int todayWorkTime = 0;
                CPlanReportElement curPlanElem = new CPlanReportElement();
                try
                {
                    if (this.storage.GetFirstPlanElement().DemandID != 0)
                    {
                        int prodId = this.storage.GetFirstPlanElement().ProductID;
                        nextPlanElemEndTime = CParams.Products[prodId].Time;
                        bool canDo = this.storage.Materials.TakeAwayMaterialCluster(CParams.Products[prodId].Materials);
                        if (canDo == false) throw new Exception("Не достаточно материалов для производства товара");
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
                        nextPlanElemEndTime = CParams.RetargetTimes[this.storage.GetFirstPlanElement().ProductID-1];
                    }
                    curPlanElem.StartExecuteDate=modelTime;
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
                        modifyDemandNextTime = runtimeModifyDemandsSumTime -(int)(modelTime - workdayStartTime).TotalMinutes;
                    }

                    if ((nextPlanElemEndTime == -1) && (newDemandNextTime == -1) &&
                        (modifyDemandNextTime == -1) && (nextDeliveryDemandTime == -1))
                        endOfDayFlag = true;
                 
                    else
                    {
                        List<int> nextTimes = new List<int>();
                        if (nextPlanElemEndTime != -1) nextTimes.Add(nextPlanElemEndTime);
                        if (newDemandNextTime != -1) nextTimes.Add(newDemandNextTime);
                        if (modifyDemandNextTime != -1) nextTimes.Add(modifyDemandNextTime);
                        if (nextDeliveryDemandTime != -1) nextTimes.Add(nextDeliveryDemandTime);

                        modelTime = modelTime.AddMinutes(nextTimes.Min());
                        if (modelTime.Day != workdayStartTime.Day) break;
                        setLabelText(label, "Модельное время: " + modelTime.ToString());

                        if (nextTimes.Min() == newDemandNextTime)
                        {
                            bool approved = backOffice.approveDemand(ref newDemands[newDemandInd]);
                            if (approved == true) storage.AddAcceptedDemand(newDemands[newDemandInd]);
                            else storage.AddDeclinedDemand(newDemands[newDemandInd]);
                            newDemandInd++;
                        }

                        if (nextTimes.Min() == modifyDemandNextTime)
                        {
                            var notFinishedDemands = this.storage.GetNotFinishedDemands();
                            if (notFinishedDemands.Count() > 0)
                            {
                                CDemand modifiedDemand = this.generator.modifyDemand(notFinishedDemands.ToArray(), modelTime);
                                CDemand demand;
                                this.storage.GetAcceptedDemand(modifiedDemand.ID, out demand);
                                bool approved = backOffice.approveModifyDemand(modelTime, ref modifiedDemand, demand);
                                if (approved == true) this.storage.ModifyDemand(modifiedDemand);
                                this.storage.AddModifyStatistic(approved);
                            }
                            modifyDemandInd++;
                        }

                        if ((nextTimes.Min() != nextPlanElemEndTime) && (nextPlanElemEndTime != -1))
                            nextPlanElemEndTime = nextPlanElemEndTime - nextTimes.Min();

                        if (nextTimes.Min() == nextPlanElemEndTime)
                        {
                            CPlanElement planElem = this.storage.GetFirstPlanElementAndDelete();
                            curPlanElem.PlanElement = planElem;
                            curPlanElem.EndExecuteDate = modelTime;
                            this.storage.AddPlanReportElement(curPlanElem);
                           // backOffice.reportPlanElem(curPlanElem);   отчёт в бекофис не надо
                            if ((planElem.DemandID!=0)&&(this.storage.IsDemandDone(planElem.DemandID) == true))
                                this.storage.FinishDemand(planElem.DemandID, modelTime);

                            curPlanElem = new CPlanReportElement();
                            try
                            {
                                if (this.storage.GetFirstPlanElement().DemandID != 0)
                                {
                                    int prodId = this.storage.GetFirstPlanElement().ProductID;
                                    nextPlanElemEndTime = CParams.Products[prodId].Time;
                                    bool canDo = this.storage.Materials.TakeAwayMaterialCluster(CParams.Products[prodId].Materials);
                                    if (canDo == false) throw new Exception("Не достаточно материалов для производства товара");
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
                                    nextPlanElemEndTime = CParams.RetargetTimes[this.storage.GetFirstPlanElement().ProductID-1];
                                }
                                curPlanElem.StartExecuteDate = modelTime;

                            }
                            catch
                            {
                                nextPlanElemEndTime = -1;
                            }
                            if (nextPlanElemEndTime != -1) todayWorkTime += nextPlanElemEndTime;
                            if (nextPlanElemEndTime != -1)    // костыль для реализации круглосуточной работы
                            {
                                TimeSpan nextPlanElemEndTimeSpan = new TimeSpan(0,nextPlanElemEndTime,0);
                                DateTime nextWorkDayStartTime = workdayStartTime + new TimeSpan(1,0,0,0);
                                if (modelTime + nextPlanElemEndTimeSpan > nextWorkDayStartTime)
                                {
                                    nextPlanElemEndTime = (int)(nextWorkDayStartTime - modelTime).TotalMinutes - this.storage.GetPlanElementsToGo(); 
                                }
                            }
                        }

                        if (nextTimes.Min() == nextDeliveryDemandTime)
                        {
                            CDeliveryDemand[] deliveryDemands = this.storage.GetDeliveryDemand(modelTime);
                            if (deliveryDemands.Length > 0)
                            {
                                foreach (CDeliveryDemand d in deliveryDemands)
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
                                    backOffice.reportDeliveryDemand(d);
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
            if (stopFlag!=true) setLabelText(label, "Модельное время: " + modelTime.ToString() + "\nМоделирование завершено");
            return;
        }

    }
}
