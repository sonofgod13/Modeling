using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelingDataTypes;

namespace GeneratorSubsystem
{
    public class Generator
    {
        private IGen requestTimeGen;
        private IGen firstArticleGen;
        private IGen secondArticleGen;
        private IGen thirdArticleGen;
        private double urgencyProb;
        private double refuseProb;
        private IGen demandModifyTimeGen;
        private IGen articlesModifyGen;
        private IGen firstArticleModifyGen;
        private IGen secondArticleModifyGen;
        private IGen thirdArticleModifyGen;
        private IGen urgToStandModifyGen;
        private IGen standToUrgModifyGen;
        private UniformGen uGen;
        private IGen deliveryDelayGen;
        private IGen[] deliveryElementsModifyGens;


        public static IGen makeGenerator(int iGeneratorType, double fA, double fB)  
            //функция создания генератора заданого типа
            //возвращает интефейс этого генератора
        {
            switch (iGeneratorType)
            {
                case 0:
                    return new UniformGen(fA, fB);
                case 1:
                    return new NormalGen(fA, fB);
                case 2:
                    return new RayleighGen(fA);
                case 3:
                    return new GammaGen(fA, fB);
                case 4:
                    return new ExponentialGen(fA);
            }
            return new UniformGen(0, 1);
        }

        public Generator(IGen requestTimeGen, IGen firstArticleGen, IGen secondArticleGen, IGen thirdArticleGen, double urgencyProb, double refuseProb,
            IGen demandModifyTimeGen, IGen articlesModifyGen, IGen firstArticleModifyGen, IGen secondArticleModifyGen, IGen thirdArticleModifyGen,
            IGen urgToStandModifyGen, IGen standToUrgModifyGen, IGen deliveryDelayGen, IGen[] deliveryElementsModifyGens) 
        {
            this.uGen = new UniformGen(0, 1);  
            this.requestTimeGen = requestTimeGen;
            this.firstArticleGen = firstArticleGen;
            this.secondArticleGen = secondArticleGen;
            this.thirdArticleGen = thirdArticleGen;
            this.urgencyProb = urgencyProb;
            this.refuseProb = refuseProb;
            this.demandModifyTimeGen = demandModifyTimeGen;
            this.articlesModifyGen = articlesModifyGen;
            this.firstArticleModifyGen = firstArticleModifyGen;
            this.secondArticleModifyGen = secondArticleModifyGen;
            this.thirdArticleModifyGen = thirdArticleModifyGen;            
            this.urgToStandModifyGen = urgToStandModifyGen;
            this.standToUrgModifyGen = standToUrgModifyGen;
            this.deliveryDelayGen = deliveryDelayGen;
            this.deliveryElementsModifyGens = deliveryElementsModifyGens;
        }

        private int[] generateUrgencyRefuse(int n)
        {
            double[] standGen =  uGen.generateN(n);
            List<int> urgency = new List<int>(); 
            foreach (double s in standGen)
            {
                if (s <= this.urgencyProb) urgency.Add(1);
                else if ((s > this.urgencyProb) && (s <= (this.urgencyProb+this.refuseProb))) urgency.Add(2);
                else urgency.Add(0);
            }
            return urgency.ToArray();
        }

        
        //public CDemand[] generateDemands(DateTime dt)
        //{
        //    int[] requestTimes = this.requestTimeGen.generateForDay();
        //    double[] firstArticlesSeq = this.firstArticleGen.generateN(requestTimes.Length);
        //    double[] secondArticlesSeq = this.secondArticleGen.generateN(requestTimes.Length);
        //    double[] thirdArticlesSeq = this.thirdArticleGen.generateN(requestTimes.Length);
        //    int[] urgency = generateUrgencyRefuse(requestTimes.Length);

        //    List<int> firstAricles = new List<int>();
        //    foreach (double i in firstArticlesSeq)
        //    {
        //        firstAricles.Add((int)Math.Round(i));
        //    }
        //    List<int> secondAricles = new List<int>();
        //    foreach (double i in secondArticlesSeq)
        //    {
        //        secondAricles.Add((int)Math.Round(i));
        //    }
        //    List<int> thirdAricles = new List<int>();
        //    foreach (double i in thirdArticlesSeq)
        //    {
        //        thirdAricles.Add((int)Math.Round(i));
        //    }
        //    List<CDemand> demands = new List<CDemand>();
        //    int minutes = 0;
        //    for (int i = 0; i < requestTimes.Length; i++)
        //    {
        //        minutes = minutes + requestTimes[i];

        //        /*
        //        demands.Add(new CDemand(0,dt.AddMinutes(minutes), urgency[i], new Dictionary<int, int> { 
        //        { 1, firstAricles.ElementAt(i) }, 
        //        { 2, secondAricles.ElementAt(i) }, 
        //        { 3, thirdAricles.ElementAt(i) } 
        //        }));
        //         */ 

        //        //--->
        //        CProductCluster productCluster = new CProductCluster();

        //        //Дима: очень странно, но thirdAricles.ElementAt(i) иногда выдает ОТРИЦАТЕЛЬНОЕ число!!!
        //        //вставил проверки на неотрицательность.
        //        if (firstAricles.ElementAt(i)>0)
        //            productCluster.AddProduct(1, firstAricles.ElementAt(i));

        //        if (secondAricles.ElementAt(i) > 0)
        //            productCluster.AddProduct(2, secondAricles.ElementAt(i));

        //        if (thirdAricles.ElementAt(i) > 0)
        //            productCluster.AddProduct(3, thirdAricles.ElementAt(i));

        //        demands.Add(new CDemand(0,dt.AddMinutes(minutes), urgency[i], productCluster));
        //        //<---
        //    }
        //    return demands.ToArray();
        //}


        //генерация массива заявок
        public CDemand[] generateDemands(DateTime dt)
        {
            int[] requestTimes = this.requestTimeGen.generateForDay();
            double[][] articlesModifyGen = new double[3][];

            //собственно генерация
            articlesModifyGen[0] = this.firstArticleGen.generateN(requestTimes.Length);
            articlesModifyGen[1] = this.firstArticleGen.generateN(requestTimes.Length);
            articlesModifyGen[2] = this.firstArticleGen.generateN(requestTimes.Length);

            int[] urgency = generateUrgencyRefuse(requestTimes.Length);

            CDemand[] demands = new CDemand[requestTimes.Length];
            int minutes = 0;
            //цикл по заявкам
            for (int iDemandNumber = 0; iDemandNumber < requestTimes.Length; iDemandNumber++)
            {
                minutes = minutes + requestTimes[iDemandNumber];

                CProductCluster productCluster = new CProductCluster();

                //заполнение кластера продуктов
                for (int iProductNumber = 1; iProductNumber <= CParams.PRODUCTS_NUMBER; iProductNumber++)
                {
                    if (articlesModifyGen[iProductNumber - 1][iDemandNumber] > 0)
                    {
                        productCluster.AddProduct(iProductNumber, (int)Math.Round(articlesModifyGen[iProductNumber - 1][iDemandNumber]));
                    }
                }

                demands[iDemandNumber] = new CDemand(0, dt.AddMinutes(minutes), urgency[iDemandNumber], productCluster);
            }
            return demands;
        }

        
        public int[] generateModifyTime()
        {
            return this.demandModifyTimeGen.generateForDay();
        }
        

        public CDemand modifyDemand(CDemand[] demands, DateTime currentDate)
        {
            //List<CDemand> modifiedDemands = new List<CDemand>();
            double[] arctProbs = uGen.generateN(demands.Length);
            int arctProbsInd = 0;
            double[] urgProbs = uGen.generateN(demands.Length);
            int urgProbsInd = 0;
            double[] firstArticleModifySeq = this.firstArticleModifyGen.generateN(demands.Length);
            int firstArctInd = 0;
            double[] secondArticleModifySeq = this.secondArticleModifyGen.generateN(demands.Length);
            int secondArctInd = 0;
            double[] thirdArticleModifySeq = this.secondArticleModifyGen.generateN(demands.Length);
            int thirdArctInd = 0;
            Random rand = new Random(Guid.NewGuid().GetHashCode());           
            /*
            for (int i = 0; i < demands.Length; i++)
            {
            */
            CDemand returnDemand = new CDemand();
            bool modifyFlag = false;
            while (modifyFlag == false)
            {
                int i = rand.Next(demands.Length);
                CDemand modifiedDemand = new CDemand(demands[i]);
                if (modifiedDemand.m_iUrgency == 2) throw new Exception("Заявка от которой отказались не может быть изменена!");
                TimeSpan dt = currentDate.Subtract(demands[i].m_dtGeting);
                
                if (this.articlesModifyGen.getProbability(dt.TotalMinutes) >= arctProbs[arctProbsInd])
                {
                    bool changeFlag = false;
                    while (changeFlag == false)
                    {
                        /*
                        int modifiedFirstArticleNum = demands[i].m_products[1] + (int)Math.Round(firstArticleModifySeq[firstArctInd]);
                        */

                        //--->
                        int modifiedFirstArticleNum = 0;
                        demands[i].m_products.GetProduct(1, out modifiedFirstArticleNum);
                        modifiedFirstArticleNum += (int)Math.Round(firstArticleModifySeq[firstArctInd]);
                        //<---

                        if (firstArctInd < (demands.Length - 1)) firstArctInd++;
                        else
                        {
                            firstArctInd = 0;
                            firstArticleModifySeq = this.firstArticleModifyGen.generateN(demands.Length);
                        }
                        /*
                        int modifiedSecondArticleNum = demands[i].m_products[2] + (int)Math.Round(secondArticleModifySeq[secondArctInd]);
                         */

                        //--->
                        int modifiedSecondArticleNum = 0;
                        demands[i].m_products.GetProduct(2, out modifiedSecondArticleNum);
                        modifiedSecondArticleNum += (int)Math.Round(secondArticleModifySeq[secondArctInd]);
                        //<---

                        if (secondArctInd < (demands.Length - 1)) secondArctInd++;
                        else
                        {
                            secondArctInd = 0;
                            secondArticleModifySeq = this.secondArticleModifyGen.generateN(demands.Length);
                        }
                        /*
                        int modifiedThirdArticleNum = demands[i].m_products[3] + (int)Math.Round(thirdArticleModifySeq[thirdArctInd]);
                         */

                        //--->
                        int modifiedThirdArticleNum = 0;
                        demands[i].m_products.GetProduct(3, out modifiedThirdArticleNum);
                        modifiedThirdArticleNum += (int)Math.Round(thirdArticleModifySeq[thirdArctInd]);
                        //<---

                        if (thirdArctInd < (demands.Length - 1)) thirdArctInd++;
                        else
                        {
                            thirdArctInd = 0;
                            thirdArticleModifySeq = this.thirdArticleModifyGen.generateN(demands.Length);
                        }
                        if (modifiedFirstArticleNum < 0) modifiedFirstArticleNum = 0;
                        if (modifiedSecondArticleNum < 0) modifiedSecondArticleNum = 0;
                        if (modifiedThirdArticleNum < 0) modifiedThirdArticleNum = 0;
                        /*
                        if (modifiedFirstArticleNum != demands[i].m_products[1]) changeFlag = true;
                        if (modifiedSecondArticleNum != demands[i].m_products[2]) changeFlag = true;
                        if (modifiedThirdArticleNum != demands[i].m_products[3]) changeFlag = true;
                        
                        if (changeFlag == true)
                        {
                            modifiedDemand.m_products[1]=modifiedFirstArticleNum;
                            modifiedDemand.m_products[2]=modifiedSecondArticleNum;
                            modifiedDemand.m_products[3]=modifiedThirdArticleNum;
                            modifyFlag = true;
                        }  
                        */

                        //--->
                        if (!demands[i].m_products.CompareProduct(1, modifiedFirstArticleNum))
                            changeFlag = true;

                        if (!demands[i].m_products.CompareProduct(2, modifiedSecondArticleNum))
                            changeFlag = true;

                        if (!demands[i].m_products.CompareProduct(3, modifiedThirdArticleNum))
                            changeFlag = true;

                        CProductCluster productClaster = new CProductCluster();
                        productClaster.AddProduct(1, modifiedFirstArticleNum);
                        productClaster.AddProduct(2, modifiedSecondArticleNum);
                        productClaster.AddProduct(3, modifiedThirdArticleNum);

                        if ((changeFlag == true)&&(productClaster.CompareNomenclatureIsMore(demands[i].m_products) == true))
                        {
                            modifiedDemand.m_products.CleanProductsCluster();
                            modifiedDemand.m_products.AddProductCluster(productClaster);
                            modifyFlag = true;
                        }
                        
                                               
                    }                    
                }

                if (arctProbsInd < (demands.Length - 1)) arctProbsInd++;
                else
                {
                    arctProbsInd = 0;
                    arctProbs = uGen.generateN(demands.Length);
                }

                if (demands[i].m_iUrgency == 1)
                {
                    if (this.urgToStandModifyGen.getProbability(dt.TotalMinutes) >= urgProbs[urgProbsInd])
                    {
                        modifiedDemand.m_iUrgency = 0;
                        //modifyFlag = true;  срочность изменяется но если не изменились продукты изменение не произошло
                    }
                }
                else
                {
                    if (this.standToUrgModifyGen.getProbability(dt.TotalMinutes) >= urgProbs[urgProbsInd])
                    {
                        modifiedDemand.m_iUrgency = 1;
                        //modifyFlag = true; срочность изменяется но если не изменились продукты изменение не произошло
                    }
                }
                if (urgProbsInd < (demands.Length - 1)) urgProbsInd++;
                else
                {
                    urgProbsInd = 0;
                    urgProbs = uGen.generateN(demands.Length);
                }

                if (modifyFlag == true) returnDemand = modifiedDemand; 
            }
            return returnDemand;
        }

        public CDeliveryDemand[] modifyDeliveries(CDeliveryDemand[] deliveries)
        {
            List<CDeliveryDemand> modifiedDeliveries = new List<CDeliveryDemand>();
            double[] deliveryDelaySeq = this.deliveryDelayGen.generateN(deliveries.Length);
            double[][] deliveryElementsModifySeq = new double[12][];
            for (int i=0;i<12;i++)
            {
                deliveryElementsModifySeq[i] = this.deliveryElementsModifyGens[i].generateN(deliveries.Length);
            }
            for (int i = 0; i < deliveries.Length; i++)
            {
                CDeliveryDemand modifiedDelivery = new CDeliveryDemand(deliveries[i]);

                if (deliveryDelaySeq[i] > 0) modifiedDelivery.m_dtRealDelivery = modifiedDelivery.m_dtFillDelivery.AddMinutes((int)Math.Round(deliveryDelaySeq[i]));

                for (int j = 0; j < CParams.MATERIALS_NUMBER; j++)
                {
                    //***if (deliveries[i].m_materialsDemand[j + 1] != 0)
                    if (deliveries[i].m_materialsDemand.IsMaterial(j + 1, 1))
                    //проверка есть ли в данной заявке на поставку хотя бы 1 материал номера (j + 1)
                    {
                        int mod = (int)Math.Round(deliveryElementsModifySeq[j][i]);
                        if (mod > 0)
                        {
                            //***modifiedDelivery.m_materialsDemand[j + 1] = deliveries[i].m_materialsDemand[j + 1] - mod;
                            modifiedDelivery.m_materialsDemand.CleanMaterial(j + 1);

                            int iTemp = 0;
                            deliveries[i].m_materialsDemand.GetMaterial(j + 1, out iTemp);
                            iTemp-=mod;
                            if (iTemp > 0)
                            {
                                modifiedDelivery.m_materialsDemand.AddMaterial(j + 1, iTemp); 
                            }   
                        }
                        /***
                        if (modifiedDelivery.m_materialsDemand[j + 1] < 0)
                        {
                            modifiedDelivery.m_materialsDemand[j + 1] = 0;
                        }
                        */ 
                    }
                        
                }                                      
                /*
                List<DeliveryElement> delivElems = new List<DeliveryElement>(modifiedDelivery.deliveryElements);
                bool nullFlag = false;
                foreach (DeliveryElement dElem in delivElems)
                {
                    if (dElem.num <= 0)
                    {
                        delivElems.Remove(dElem);
                        nullFlag = true;
                    }
                }
                if (nullFlag == true) modifiedDelivery.deliveryElements = delivElems.ToArray();
                */
                modifiedDeliveries.Add(modifiedDelivery);
            }
            return modifiedDeliveries.ToArray();
        }
    }
}
