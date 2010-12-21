using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelingDataTypes;
using GeneratorSubsystem;
using Storage;
using System.Windows.Forms;

namespace WindowsFormsApplication1
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
        private int currentModellingDay;

        public CImitation()
        {
            this.backOffice = new BackOfficeInterface();
            storage = new Storage.CStorage();   //содается временное хранилище результатов моделирования

            generator = new Generator(
                Generator.makeGenerator(CParams.m_generatorDemandsTime.m_iGeneratorType, CParams.m_generatorDemandsTime.m_fA, CParams.m_generatorDemandsTime.m_fB),
                Generator.makeGenerator(CParams.m_products[1].m_iGeneratorType, CParams.m_products[1].m_fA, CParams.m_products[1].m_fB),
                Generator.makeGenerator(CParams.m_products[2].m_iGeneratorType, CParams.m_products[2].m_fA, CParams.m_products[2].m_fB),
                Generator.makeGenerator(CParams.m_products[3].m_iGeneratorType, CParams.m_products[3].m_fA, CParams.m_products[3].m_fB),
                CParams.m_fUrgencyPropabilityDemand, CParams.m_fRefusePropabilityDemand,
                Generator.makeGenerator(CParams.m_demandModifyTime.m_iGeneratorType, CParams.m_demandModifyTime.m_fA, CParams.m_demandModifyTime.m_fB),
                Generator.makeGenerator(CParams.m_articlesModify.m_iGeneratorType, CParams.m_articlesModify.m_fA, CParams.m_articlesModify.m_fB),
                Generator.makeGenerator(CParams.m_products[1].m_modify.m_iGeneratorType, CParams.m_products[1].m_modify.m_fA, CParams.m_products[1].m_modify.m_fB),
                Generator.makeGenerator(CParams.m_products[2].m_modify.m_iGeneratorType, CParams.m_products[2].m_modify.m_fA, CParams.m_products[2].m_modify.m_fB),
                Generator.makeGenerator(CParams.m_products[3].m_modify.m_iGeneratorType, CParams.m_products[3].m_modify.m_fA, CParams.m_products[3].m_modify.m_fB),
                Generator.makeGenerator(CParams.m_ugrToStandModify.m_iGeneratorType, CParams.m_ugrToStandModify.m_fA, CParams.m_ugrToStandModify.m_fB),
                Generator.makeGenerator(CParams.m_standToUrgModify.m_iGeneratorType, CParams.m_standToUrgModify.m_fA, CParams.m_standToUrgModify.m_fB),
                Generator.makeGenerator(CParams.m_deliveryDelayGenerator.m_iGeneratorType, CParams.m_deliveryDelayGenerator.m_fA, CParams.m_deliveryDelayGenerator.m_fB),
                new IGen[]
                {   
                    Generator.makeGenerator(CParams.m_materials[1].m_iGeneratorType, CParams.m_materials[1].m_fA, CParams.m_materials[1].m_fB),
                    Generator.makeGenerator(CParams.m_materials[2].m_iGeneratorType, CParams.m_materials[2].m_fA, CParams.m_materials[2].m_fB),
                    Generator.makeGenerator(CParams.m_materials[3].m_iGeneratorType, CParams.m_materials[3].m_fA, CParams.m_materials[3].m_fB),
                    Generator.makeGenerator(CParams.m_materials[4].m_iGeneratorType, CParams.m_materials[4].m_fA, CParams.m_materials[4].m_fB),
                    Generator.makeGenerator(CParams.m_materials[5].m_iGeneratorType, CParams.m_materials[5].m_fA, CParams.m_materials[5].m_fB),
                    Generator.makeGenerator(CParams.m_materials[6].m_iGeneratorType, CParams.m_materials[6].m_fA, CParams.m_materials[6].m_fB),
                    Generator.makeGenerator(CParams.m_materials[7].m_iGeneratorType, CParams.m_materials[7].m_fA, CParams.m_materials[7].m_fB),
                    Generator.makeGenerator(CParams.m_materials[8].m_iGeneratorType, CParams.m_materials[8].m_fA, CParams.m_materials[8].m_fB),
                    Generator.makeGenerator(CParams.m_materials[9].m_iGeneratorType, CParams.m_materials[9].m_fA, CParams.m_materials[9].m_fB),
                    Generator.makeGenerator(CParams.m_materials[10].m_iGeneratorType, CParams.m_materials[10].m_fA, CParams.m_materials[10].m_fB),
                    Generator.makeGenerator(CParams.m_materials[11].m_iGeneratorType, CParams.m_materials[11].m_fA, CParams.m_materials[11].m_fB),
                    Generator.makeGenerator(CParams.m_materials[12].m_iGeneratorType, CParams.m_materials[12].m_fA, CParams.m_materials[12].m_fB)
                }
            );
            this.modelingDays = CParams.m_iModelingDayToWork;
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

        public double getDemandAverageDelay()   // Среднее время задержки заказов в днях
        {
            return this.storage.DemandAverageDelay();
        }

        public double getFinishedDemandsNum()   // Количество выполненных заказов
        {
            return this.storage.FinishedDemandsNum();
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
            return this.storage.RefuseNum();
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

       

        public bool Start(Label label)       // Начальные заявки и материалы, запуск итератора
        {
           // CDemand.idNext = 0; // сброс счетчика уникальности заявок
            this.currentModellingDay = 0;
            this.stopFlag = false;

            ////////////////////////  Тут определены начальные заявки и материалы

            List<CDemand> demands = new List<CDemand>();
            do
            {
                CDemand[] newDemands = this.generator.generateDemands(startTime);
                int count = 0;
                if (newDemands.Length < 10) count = newDemands.Length;
                else count = 10;
                for (int i = 0; i < count; i++)
                {
                    newDemands[i].m_dtGeting = startTime;
                    if (newDemands[i].m_iUrgency == 2) newDemands[i].m_iUrgency = 0;
                    demands.Add(newDemands[i]);
                    bool approved = backOffice.approveDemand(ref newDemands[i]);
                    storage.AddAcceptedDemand(newDemands[i]);
                }
            } while (demands.Count < 10);

            
            //Dictionary<int,int> materials = new Dictionary<int,int>{
            //    {1,0},{2,0},{3,0},{4,0},{5,0},{6,0},{7,0},{8,0},{9,0},{10,0},{11,0},{12,0}
            //};


            CMaterialCluster materials = new CMaterialCluster();

            foreach (CDemand d in demands)
            {
                for(int iProductNumber = 1; iProductNumber < CParams.PRODUCTS_NUMBER + 1; iProductNumber++ ) 
                {
                    for( int iMaterialNumber = 1; iMaterialNumber < CParams.MATERIALS_NUMBER+1; iMaterialNumber++ )
                    {
                        /***
                        int iTempMaterialCount =

                        CParams.m_products[iProductNumber].m_iMaterials[iMaterialNumber - 1] * //количетсво материалов для продукта в заявке 
                        d.m_products[ iProductNumber ]; //количество продуктов в заявке
                         */

                        int iTempMaterialCount = 0;
                        CParams.m_products[iProductNumber].m_materials.GetMaterial(iMaterialNumber, out iTempMaterialCount); //количетсво материалов для продукта в заявке
                        iTempMaterialCount *= d.m_products[iProductNumber]; //количество продуктов в заявке


                        //***materials[ iMaterialNumber ] += iTempMaterialCount;
                        //Добавляет в кластер materials iTempMaterialCount материалов с номером iMaterialNumber
                        materials.AddMaterial(iMaterialNumber, iTempMaterialCount);

                        //***this.storage.AddMaterialNumber(iMaterialNumber, iTempMaterialCount);
                        //Добавляем на склад к материалу iMaterialNumber количество iTempMaterialCount
                        this.storage.m_materials.AddMaterial(iMaterialNumber, iTempMaterialCount);
                    }
                }
            }
            //***CDeliveryDemand deliv = new CDeliveryDemand(-1,startTime,materials);
            //Создание заявки на поставку материалов
            CDeliveryDemand deliv = new CDeliveryDemand(-1, startTime, materials);
            deliv.m_dtRealDelivery = startTime;
            this.storage.AddDeliveryDemand(deliv);
            backOffice.reportDeliveryDemand(deliv);


            ////////////////////////////
            
            this.Iteration(label);            //запуск основного цикла
            return !stopFlag;
        }

        public bool Stop()  // остановка и пауза моделирования
        {
            if (currentModellingDay == 19) return false;
            else
            {
                this.stopFlag = true;
                return true;
            }
        }

        public bool Continue(Label label) // продолжить моделирование
        {
            this.stopFlag = false;
            this.Iteration(label);
            return !stopFlag;
        }

        public void Iteration(Label label)
        {
            for (int i = this.currentModellingDay; i < this.modelingDays; i++)
            {
                this.currentModellingDay = i;
                if (this.stopFlag == true) break;
    
                DateTime workdayStartTime = startTime.AddDays(i);
                modelTime = workdayStartTime;

                int[] materialsNumToday = new int[12];
                for (int j=0;j<12;j++)
                {
                    //***materialsNumToday[j]=this.storage.GetMaterialNumberFromID(j+1);
                    //возвращает со склада в materialsNumToday[j] количество материала с номером (j + 1)
                    this.storage.m_materials.GetMaterial(j + 1, out materialsNumToday[j]);
                }
                this.storage.AddMaterialsStatisticDay(materialsNumToday);
                CDemand[] newDemands = this.generator.generateDemands(workdayStartTime);
                int[] modifyDemandsTime = this.generator.generateModifyTime();
                
                ////////////////////////////////////////////////// Обращение к back-office
                this.storage.ClearAllPlan();
                this.storage.AddDailyPlan(this.backOffice.getDailyPlan(modelTime));

                int rem = -1;
                int span = (int)(modelTime-startTime).TotalDays;
                Math.DivRem(span,7,out rem);
                if ((rem == 0)&&(span!=0))
                {
                    CDeliveryDemand[] dlvr = new CDeliveryDemand[1] { this.backOffice.getDeliveryDemands(modelTime) }; // это мне было лень generator.modifyDeliveries() переписывать
                    this.storage.AddDeliveryDemand(this.generator.modifyDeliveries(dlvr).ElementAt(0));                    
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
                    if (this.storage.GetFirstPlanElement().m_iDemandID != 0)
                    {
                        int prodId = this.storage.GetFirstPlanElement().m_iProductID;
                        nextPlanElemEndTime = CParams.m_products[prodId].m_iTime;
                        bool canDo = this.storage.m_materials.TakeAwayMaterialCluster(CParams.m_products[prodId].m_materials);
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
                        nextPlanElemEndTime = CParams.retargetTimes[this.storage.GetFirstPlanElement().m_iProductID-1];
                    }
                    curPlanElem.m_dtStartExecute=modelTime;
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
                        newDemandNextTime = (int)(newDemands[newDemandInd].m_dtGeting - modelTime).TotalMinutes;
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
                            CDemand[] notFinishedDemands = this.storage.GetNotFinishedDemands();
                            if (notFinishedDemands.Length > 0)
                            {
                                CDemand modifiedDemand = this.generator.modifyDemand(notFinishedDemands, modelTime);
                                bool approved = backOffice.approveModifyDemand(modelTime, modifiedDemand);
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
                            curPlanElem.m_planElement = planElem;
                            curPlanElem.m_dtEndExecute = modelTime;
                            this.storage.AddPlanReportElement(curPlanElem);
                           // backOffice.reportPlanElem(curPlanElem);   отчёт в бекофис не надо
                            if ((planElem.m_iDemandID!=0)&&(this.storage.IsDemandDone(planElem.m_iDemandID) == true))
                                this.storage.FinishDemand(planElem.m_iDemandID, modelTime);

                            curPlanElem = new CPlanReportElement();
                            try
                            {
                                if (this.storage.GetFirstPlanElement().m_iDemandID != 0)
                                {
                                    int prodId = this.storage.GetFirstPlanElement().m_iProductID;
                                    nextPlanElemEndTime = CParams.m_products[prodId].m_iTime;
                                    bool canDo = this.storage.m_materials.TakeAwayMaterialCluster(CParams.m_products[prodId].m_materials);
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
                                    nextPlanElemEndTime = CParams.retargetTimes[this.storage.GetFirstPlanElement().m_iProductID-1];
                                }
                                curPlanElem.m_dtStartExecute = modelTime;

                            }
                            catch
                            {
                                nextPlanElemEndTime = -1;
                            }
                            if (nextPlanElemEndTime != -1) todayWorkTime += nextPlanElemEndTime;
                        }

                        if (nextTimes.Min() == nextDeliveryDemandTime)
                        {
                            CDeliveryDemand[] deliveryDemands = this.storage.GetDeliveryDemand(modelTime);
                            if (deliveryDemands.Length > 0)
                            {
                                foreach (CDeliveryDemand d in deliveryDemands)
                                {
                                    this.storage.m_materials.AddMaterialCluster(d.m_materialsDemand);
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

                this.storage.SaveIdleStatistic((double)(CParams.WORKDAY_MINUTES_NUMBER - todayWorkTime) / CParams.WORKDAY_MINUTES_NUMBER);
                this.storage.SaveDemandAverageDelayStatistic();
                this.storage.SaveFinishedDemandsPerDayStatistic();
            }
            if (stopFlag!=true) setLabelText(label, "Модельное время: " + modelTime.ToString() + "\nМоделирование завершено");
            return;
        }

    }
}
