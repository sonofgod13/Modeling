using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelingDataTypes;

namespace GeneratorSubsystem
{
    public class Generator
    {
        private double urgencyProb;
        private double refuseProb;

        private IGen requestTimeGen;
        private IGen firstArticleGen;
        private IGen secondArticleGen;
        private IGen thirdArticleGen;
        private IGen demandModifyTimeGen;
        private IGen articlesModifyGen;
        private IGen firstArticleModifyGen;
        private IGen secondArticleModifyGen;
        private IGen thirdArticleModifyGen;
        private IGen urgToStandModifyGen;
        private IGen standToUrgModifyGen;
        private IGen uGen;
        private IGen deliveryDelayGen;

        private IGen[] deliveryElementsModifyGens;


        /// <summary>
        /// Функция создания генератора заданого типа
        /// возвращает интефейс этого генератора
        /// </summary>
        /// <param name="generatorType"></param>
        /// <param name="fA"></param>
        /// <param name="fB"></param>
        /// <returns></returns>
        public static IGen CreateGenerator(GeneratorType generatorType, double fA, double fB)
        {
            switch (generatorType)
            {
                case GeneratorType.Uniform:
                    return new UniformGen(fA, fB);

                case GeneratorType.Normal:
                    return new NormalGen(fA, fB);

                case GeneratorType.Rayleigh:
                    return new RayleighGen(fA);

                case GeneratorType.Gamma:
                    return new GammaGen(fA, fB);

                case GeneratorType.Exponential:
                    return new ExponentialGen(fA);

                default:
                    return new UniformGen(0, 1);
            }
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

        private int[] GenerateUrgencyRefuse(int n)
        {
            var standGen = uGen.GenerateN(n);
            var urgency = new List<int>();
            foreach (double s in standGen)
            {
                if (s <= this.urgencyProb)
                    urgency.Add(1);
                else if ((s > this.urgencyProb) && (s <= (this.urgencyProb + this.refuseProb)))
                    urgency.Add(2);
                else urgency.Add(0);
            }
            return urgency.ToArray();
        }

        /// <summary>
        /// генерация массива заявок
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public Demand[] GenerateDemands(DateTime dt)
        {
            int[] requestTimes = this.requestTimeGen.GenerateForDay();
            double[][] articlesModifyGen = new double[3][];

            //собственно генерация
            articlesModifyGen[0] = this.firstArticleGen.GenerateN(requestTimes.Length);
            articlesModifyGen[1] = this.secondArticleGen.GenerateN(requestTimes.Length);
            articlesModifyGen[2] = this.thirdArticleGen.GenerateN(requestTimes.Length);

            int[] urgency = GenerateUrgencyRefuse(requestTimes.Length);

            Demand[] demands = new Demand[requestTimes.Length];
            int minutes = 0;
            //цикл по заявкам
            for (int iDemandNumber = 0; iDemandNumber < requestTimes.Length; iDemandNumber++)
            {
                minutes = minutes + requestTimes[iDemandNumber];

                ProductCluster productCluster = new ProductCluster();

                //заполнение кластера продуктов
                for (int iProductNumber = 1; iProductNumber <= Params.PRODUCTS_NUMBER; iProductNumber++)
                {
                    if (articlesModifyGen[iProductNumber - 1][iDemandNumber] > 0)
                    {
                        productCluster.AddProduct(iProductNumber, (int)Math.Round(articlesModifyGen[iProductNumber - 1][iDemandNumber]));
                    }
                }

                demands[iDemandNumber] = new Demand(0, dt.AddMinutes(minutes), urgency[iDemandNumber], productCluster);
            }
            return demands;
        }


        public int[] GenerateModifyTime()
        {
            return this.demandModifyTimeGen.GenerateForDay();
        }


        public Demand ModifyDemand(Demand[] demands, DateTime currentDate)
        {
            var arctProbabilities = uGen.GenerateSequence().GetEnumerator();
            var urgProbabilities = uGen.GenerateSequence().GetEnumerator();

            Func<IEnumerator<double>, double> getNext = (sequence) =>
            {
                sequence.MoveNext();
                return sequence.Current;
            };

            var probabilitiesByProduct = new Dictionary<int, IEnumerator<double>>()
            {
                {1, this.firstArticleModifyGen.GenerateSequence().GetEnumerator()},
                {2, this.secondArticleModifyGen.GenerateSequence().GetEnumerator()},
                {3, this.thirdArticleModifyGen.GenerateSequence().GetEnumerator()},
            };

            var rand = new Random(Guid.NewGuid().GetHashCode());

            var returnDemand = new Demand();
            bool modifyFlag = false;

            while (!modifyFlag)
            {
                int i = rand.Next(demands.Length);
                var modifiedDemand = new Demand(demands[i]);
                if (modifiedDemand.Urgency == 2)
                    throw new Exception("Заявка от которой отказались не может быть изменена!");

                TimeSpan dt = currentDate.Subtract(demands[i].GettingDate);

                var demand = demands[i];

                if (this.articlesModifyGen.GetProbability(dt.TotalMinutes) >= getNext(arctProbabilities))
                {
                    bool changeFlag = false;
                    while (changeFlag == false)
                    {
                        var productClaster = new ProductCluster();

                        for (var productIndex = 1; productIndex <= 3; productIndex++)
                        {
                            int modifiedArticleNum = 0;

                            demand.Products.GetProduct(productIndex, out modifiedArticleNum);

                            modifiedArticleNum += (int)Math.Round(getNext(probabilitiesByProduct[productIndex]));

                            if (modifiedArticleNum < 0) 
                                modifiedArticleNum = 0;

                            if (!demand.Products.CompareProduct(productIndex, modifiedArticleNum))
                                changeFlag = true;

                            productClaster.AddProduct(productIndex, modifiedArticleNum);
                        }

                        if (changeFlag && productClaster.CompareNomenclatureIsMore(demand.Products))
                        {
                            modifiedDemand.Products.CleanProductsCluster();
                            modifiedDemand.Products.AddProductCluster(productClaster);
                            modifyFlag = true;
                        }
                    }
                }

                if (demand.Urgency == 1)
                {
                    if (this.urgToStandModifyGen.GetProbability(dt.TotalMinutes) >= getNext(urgProbabilities))
                    {
                        modifiedDemand.Urgency = 0;
                        //modifyFlag = true;  срочность изменяется но если не изменились продукты изменение не произошло
                    }
                }
                else
                {
                    if (this.standToUrgModifyGen.GetProbability(dt.TotalMinutes) >= getNext(urgProbabilities))
                    {
                        modifiedDemand.Urgency = 1;
                        //modifyFlag = true; срочность изменяется но если не изменились продукты изменение не произошло
                    }
                }

                if (modifyFlag)
                    returnDemand = modifiedDemand;
            }
            return returnDemand;
        }

        public DeliveryDemand[] ModifyDeliveries(DeliveryDemand[] deliveries)
        {
            List<DeliveryDemand> modifiedDeliveries = new List<DeliveryDemand>();
            double[] deliveryDelaySeq = this.deliveryDelayGen.GenerateN(deliveries.Length);
            double[][] deliveryElementsModifySeq = new double[12][];
            for (int i = 0; i < 12; i++)
            {
                deliveryElementsModifySeq[i] = this.deliveryElementsModifyGens[i].GenerateN(deliveries.Length);
            }
            for (int i = 0; i < deliveries.Length; i++)
            {
                DeliveryDemand modifiedDelivery = new DeliveryDemand(deliveries[i]);

                if (deliveryDelaySeq[i] > 0) modifiedDelivery.RealDeliveryDate = modifiedDelivery.FillDeliveryDate.AddMinutes((int)Math.Round(deliveryDelaySeq[i]));

                for (int j = 0; j < Params.MATERIALS_NUMBER; j++)
                {
                    //***if (deliveries[i].m_materialsDemand[j + 1] != 0)
                    if (deliveries[i].MaterialsDemand.IsMaterial(j + 1, 1))
                    //проверка есть ли в данной заявке на поставку хотя бы 1 материал номера (j + 1)
                    {
                        int mod = (int)Math.Round(deliveryElementsModifySeq[j][i]);
                        if (mod > 0)
                        {
                            //***modifiedDelivery.m_materialsDemand[j + 1] = deliveries[i].m_materialsDemand[j + 1] - mod;
                            modifiedDelivery.MaterialsDemand.CleanMaterial(j + 1);

                            int iTemp = 0;
                            deliveries[i].MaterialsDemand.GetMaterial(j + 1, out iTemp);
                            iTemp -= mod;
                            if (iTemp > 0)
                            {
                                modifiedDelivery.MaterialsDemand.AddMaterial(j + 1, iTemp);
                            }
                        }
                    }
                }
               
                modifiedDeliveries.Add(modifiedDelivery);
            }
            return modifiedDeliveries.ToArray();
        }
    }
}
