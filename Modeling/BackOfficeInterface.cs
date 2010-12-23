using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelingDataTypes;
using GeneratorSubsystem;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using System.Collections;
using Storage;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.IO;

namespace Modeling
{
    public class BackOfficeInterface
    {
        /// <summary>
        /// заглушка
        /// </summary>
        static int ind = 0;
        static int delInd = 1;

        private int prevProductId;

        /// <summary>
        /// WTF?!
        /// </summary>
        private /*Modeling.MamlayBackOfficeFront.*/ BackOfficeFront.FrontOfficeService frontOffice;
        private /*Modeling.MamlayBackOfficeSim.*/ BackOfficeSim.SimulationService simulation;

        [Serializable]
        private struct BackOfficePlanElem
        {
            public int demandId { get; set; }
            public int productId { get; set; }
            public int count { get; set; }
        }

        public BackOfficeInterface()
        {
            prevProductId = 0;
            // WTF
            frontOffice = new Modeling.BackOfficeFront.FrontOfficeService(); //new Modeling.MamlayBackOfficeFront.FrontOfficeService();
            simulation = new Modeling.BackOfficeSim.SimulationService(); //new Modeling.MamlayBackOfficeSim.SimulationService();
        }

        private static XElement GetXElement(XmlNode node)
        {
            var xDoc = new XDocument();

            using (var xmlWriter = xDoc.CreateWriter())
                node.WriteTo(xmlWriter);

            return xDoc.Root;
        }

        private static T GetNodeValueByKey<T>(XmlNode[] nodes, string key)
        {
            var elements = from node in nodes
                           where node.NodeType == XmlNodeType.Element
                           select GetXElement(node);

            foreach (var node in elements)
            {
                if (node.Elements().Where(n => n.Name == "key" & n.Value == key).Count() != 0)
                {
                    XElement value = node.Elements().Where(n => n.Name == "value").FirstOrDefault();
                    try
                    {
                        if (typeof(T) == typeof(String))
                        {
                            return (T)Convert.ChangeType(value.Value, typeof(String));
                        }

                        // wtf! reflection calls must be cached! Invoke is slooooooooowwwwwwww
                        // spowpoke wtf!
                        // rewrite to expression construction + compilation + caching
                        Type t = typeof(T);
                        MethodInfo parseMethod = t.GetMethod("Parse", new Type[] { typeof(String) });
                        if (parseMethod != null)
                        {
                            return (T)parseMethod.Invoke(null, new object[] { value.Value });
                        }
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }
            }
            return default(T);
        }

        private static IEnumerable<BackOfficePlanElem> PlanElemParse(XmlNode[] nodes)
        {
            int count = nodes.Count() - 1;

            var xElems = new List<XElement>(
                from node in nodes
                where node.NodeType == XmlNodeType.Element
                select GetXElement(node)
            );

            var planElems = new List<BackOfficePlanElem>();

            var elements = from element in xElems
                           let key = int.Parse(element.Element("key").Value)
                           where key >= 1 && key <= count
                           orderby key ascending
                           select element;

            foreach (var node in elements)
            {
                var newPlanElement = new BackOfficePlanElem();

                foreach (var nodd in node.Element("value").Elements())
                {
                    if (nodd.Elements().Where(n => n.Name == "key" & n.Value == "demandId").Count() != 0)
                    {
                        var value = nodd.Elements().Where(n => n.Name == "value").FirstOrDefault();
                        try
                        {
                            newPlanElement.demandId = int.Parse(value.Value);
                        }
                        catch (Exception e)
                        {
                            throw;
                        }
                    }

                    if (nodd.Elements().Where(n => n.Name == "key" & n.Value == "productId").Count() != 0)
                    {
                        var value = nodd.Elements().Where(n => n.Name == "value").FirstOrDefault();
                        try
                        {
                            newPlanElement.productId = int.Parse(value.Value);
                        }
                        catch (Exception e)
                        {
                            throw;
                        }
                    }

                    if (nodd.Elements().Where(n => n.Name == "key" & n.Value == "count").Count() != 0)
                    {
                        var value = nodd.Elements().Where(n => n.Name == "value").FirstOrDefault();
                        try
                        {
                            newPlanElement.count = int.Parse(value.Value);
                        }
                        catch (Exception e)
                        {
                            throw;
                        }
                    }
                }
                planElems.Add(newPlanElement);
            }

            return planElems;
        }


        /// <summary>
        /// Передача в back office время начала нового моделирования
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool StartModeling(DateTime date)
        {
            if (!CParams.UseFakeServices)
            {
                //         Реальный код    
                string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");

                bool startResult = simulation.niceStart(dateStr);

                if (!simulation.niceStart(dateStr))
                    return ModelError.Error("Не удалось начать новое моделирование");
            }
            return true;
        }

        /// <summary>
        /// Утверждение в back office новой заявки
        /// </summary>
        /// <param name="demand"></param>
        /// <returns></returns>
        public bool ApproveDemand(ref CDemand demand)
        {
            if (!CParams.UseFakeServices)
            {
                //         Реальный код            
                string date = demand.GettingDate.ToString("yyyy-MM-dd HH:mm:ss");
                var executionDate = new DateTime();

                XmlNode[] result;
                try
                {
                    int iProduct1 = 0;
                    int iProduct2 = 0;
                    int iProduct3 = 0;

                    demand.Products.GetProduct(1, out iProduct1);
                    demand.Products.GetProduct(2, out iProduct2);
                    demand.Products.GetProduct(3, out iProduct3);

                    //result = frontOffice.newOrder(date, "modeling", demand.m_products[1], demand.m_products[2], demand.m_products[3]) as XmlNode[];
                    result = frontOffice.newOrder(date, "modeling", iProduct1, iProduct2, iProduct3) as XmlNode[];
                }
                catch (Exception e)
                {
                    throw;
                }
                if (result != null)
                {
                    demand.ID = GetNodeValueByKey<int>(result, "OrderId");
                    executionDate = DateTime.Parse(GetNodeValueByKey<string>(result, "ExecutionTime"));
                }

                if (demand.Urgency == 2)
                {
                    //bool cancelResult = frontOffice.cancelOrder(date, demand.m_iID);
                    //if (cancelResult==false) throw new Exception("Не удалось отменить заявку");
                    return false;
                }
                else
                {
                    demand.ShouldBeDoneDate = executionDate.AddDays(7 * (demand.Urgency + 1));

                    if (!frontOffice.confirmOrder(date, demand.ID, demand.Urgency + 2))
                        throw new Exception("Не удалось подтвердить заявку");

                    return true;
                }
            }
            else
            {
                //         Заглушка     
                if ((demand.ID >= 1) && (demand.ID <= 4))
                    return true;

                ind++;
                demand.ID = ind;
                if (demand.Urgency == 2)
                    return false;

                demand.ShouldBeDoneDate = DateTime.Now.AddDays(new Random().Next(18));

                UniformGen ug = new UniformGen(1, 0);

                return ug.GenerateN(1).ElementAt(0) < 0.8;
            }
        }

        /// <summary>
        /// Утверждение в back office изменения заявки
        /// </summary>
        /// <param name="date"></param>
        /// <param name="modifiedDemand"></param>
        /// <param name="demand"></param>
        /// <returns></returns>
        public bool ApproveModifyDemand(DateTime date, ref CDemand modifiedDemand, CDemand demand)
        {
            if (!CParams.UseFakeServices)
            {
                //         Реальный код
                string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");

                //int firstProdDelta=modifiedDemand.m_products[1]-demand.m_products[1];
                //int secondProdDelta=modifiedDemand.m_products[2]-demand.m_products[2];
                //int thirdProdDelta=modifiedDemand.m_products[3]-demand.m_products[3];

                //--->
                int firstProdDelta = 0;
                int secondProdDelta = 0;
                int thirdProdDelta = 0;
                int iTemp = 0;

                modifiedDemand.Products.GetProduct(1, out firstProdDelta);
                demand.Products.GetProduct(1, out iTemp);
                firstProdDelta -= iTemp;

                modifiedDemand.Products.GetProduct(2, out secondProdDelta);
                demand.Products.GetProduct(2, out iTemp);
                secondProdDelta -= iTemp;

                modifiedDemand.Products.GetProduct(3, out thirdProdDelta);
                demand.Products.GetProduct(3, out iTemp);
                thirdProdDelta -= iTemp;
                //<---

                //if (modifiedDemand.m_products[1] + modifiedDemand.m_products[2] + modifiedDemand.m_products[3] == 0)
                if (
                    modifiedDemand.Products.CompareProduct(1, 0)
                    && modifiedDemand.Products.CompareProduct(2, 0)
                    && modifiedDemand.Products.CompareProduct(3, 0)
                    )
                {
                    return frontOffice.cancelOrder(dateStr, modifiedDemand.ID);
                }
                else
                {
                    if (firstProdDelta + secondProdDelta + thirdProdDelta == 0)
                        ModelError.Error("Номенклатура изменяемой завки не изменена");

                    string returnDateStr = frontOffice.changeOrder(
                        dateStr,
                        modifiedDemand.ID,
                        new int[] { 
                            firstProdDelta, 
                            secondProdDelta, 
                            thirdProdDelta 
                        }
                    );

                    DateTime returnDate = DateTime.Parse(returnDateStr);

                    bool confirmResult = frontOffice.confirmChange(
                        dateStr,
                        modifiedDemand.ID,
                        new int[] { 
                            firstProdDelta, 
                            secondProdDelta, 
                            thirdProdDelta 
                        }
                    );

                    if (confirmResult)
                        modifiedDemand.ShouldBeDoneDate = returnDate;

                    return confirmResult;
                }
            }
            else
            {
                //         Заглушка     
                UniformGen ug = new UniformGen(1, 0);
                return ug.GenerateN(1).ElementAt(0) < 0.8;
            }
        }

        public bool ReportDeliveryDemand(CDeliveryDemand del)
        {
            if (!CParams.UseFakeServices)
            {
                //         Реальный код
                string date = del.RealDeliveryDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                var materials = new int[12];

                for (int i = 0; i < 12; i++)
                {
                    int curMatCount = 0;
                    del.MaterialsDemand.GetMaterial(i + 1, out curMatCount);
                    materials[i] = curMatCount;
                }

                if (!simulation.receivingMaterials(date, del.ID, materials))
                    throw new Exception("Не удалось отослать пришедшие материалы");

                return true;
            }
            else
            {
                //         Заглушка     
                bool g = del.RealDeliveryDate.HasValue; // заглушка
                return true;
            }
        }

        public CDeliveryDemand GetDeliveryDemands(DateTime date)          //Получение от back office заявок на поставки материалов
        {
            if (!CParams.UseFakeServices)
            {
                //         Реальный код
                string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");
                CDeliveryDemand delivery = null;

                XmlNode[] result = new XmlNode[0];
                try
                {
                    result = simulation.getShoppingList(dateStr) as XmlNode[];
                }
                catch (Exception e)
                {
                    throw;
                }
                if (result != null)
                {
                    CMaterialCluster materials = new CMaterialCluster();
                    for (int i = 1; i < CParams.MATERIALS_NUMBER + 1; i++)
                    {
                        materials.AddMaterial(i, GetNodeValueByKey<int>(result, i.ToString()));
                    }
                    delivery = new CDeliveryDemand(delInd, date, materials);
                }
                delInd++;
                return delivery;
            }
            else
            {
                //         Заглушка     
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

                CDeliveryDemand del = new CDeliveryDemand(delInd, date, materialToAdd);
                return del;
            }
        }

        public CPlanElement[] GetDailyPlan(DateTime date, ref CStorage storage)          //Получение от back office плана на день
        {
            if (!CParams.UseFakeServices)
            {
                //      Реальный код
                string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");
                var dailyPlan = new List<CPlanElement>();

                var result = new XmlNode[0];
                try
                {
                    result = simulation.getDayPlan(dateStr) as XmlNode[];
                }
                catch (Exception e)
                {
                    throw;
                }

                if (result != null)
                {
                    //int[] canc = cancelElemParse(result);
                    var plan = PlanElemParse(result);
                    //BackOfficePlanElem[] tran = planElemParse(result, "transArray");

                    /*for (int i = 0; i < canc.Length; i++)
                    {
                        storage.AddCanceledDemand(canc[i]);
                    }

                    List<int> tranDemandIds = new List<int>();
                    foreach (BackOfficePlanElem elem in tran)
                    {
                        storage.TranDemand(elem.demandId, elem.productId, elem.count);
                        if (tranDemandIds.Contains(elem.demandId)==false) tranDemandIds.Add(elem.demandId);
                    }
                    foreach(int id in tranDemandIds)
                    {
                        if(storage.IsDemandDone(id) == true) storage.FinishDemand(id, date);
                    }                
                    */
                    int idleTime = 0;
                    if (plan.Where(p => p.demandId == 1 && p.count == 0).Count() == 2)
                    {
                        storage.SaveIdleStatistic(1);
                    }
                    else
                    {
                        foreach (var planElement in plan)
                        {
                            if (planElement.productId != 4)
                            {
                                if ((prevProductId != 0) && (planElement.productId != prevProductId))
                                {
                                    dailyPlan.Add(new CPlanElement
                                    {
                                        DemandID = 0,
                                        ProductID = planElement.productId
                                    });
                                }
                                for (int j = 0; j < planElement.count; j++)
                                {
                                    dailyPlan.Add(new CPlanElement
                                    {
                                        DemandID = planElement.demandId,
                                        ProductID = planElement.productId
                                    });
                                    prevProductId = planElement.productId;
                                }
                            }
                            else
                            {
                                int ticks = planElement.count;
                                if (ticks != -99.0)
                                    idleTime = idleTime + ticks / 60;    // Простой = (n/100)*24*60 - минуты
                            }
                        }
                        storage.SaveIdleStatistic((double)idleTime / CParams.WORKDAY_MINUTES_NUMBER);
                    }
                }
                else
                {
                    storage.SaveIdleStatistic(1);
                }
                return dailyPlan.ToArray();
            }
            else
            {
                return new[] {
                    new CPlanElement() { DemandID = 1, ProductID = 2 },
                    new CPlanElement() { DemandID = 1, ProductID = 1 },
                    new CPlanElement() { DemandID = 2, ProductID = 2 },
                    new CPlanElement() { DemandID = 0, ProductID = 3 },
                    new CPlanElement() { DemandID = 3, ProductID = 3 },
                    new CPlanElement() { DemandID = 2, ProductID = 2 },
                    new CPlanElement() { DemandID = 4, ProductID = 1 }
                };
            }
        }

    }
}
