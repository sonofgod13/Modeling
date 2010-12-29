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
        static int ind = 0; ///заглушка
        static int delInd = 1;

        private int prevProductId;

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
            frontOffice = new Modeling.BackOfficeFront.FrontOfficeService(); //new Modeling.MamlayBackOfficeFront.FrontOfficeService();
            simulation = new Modeling.BackOfficeSim.SimulationService(); //new Modeling.MamlayBackOfficeSim.SimulationService();
        }

        private static XElement GetXElement(XmlNode node)
        {
            XDocument xDoc = new XDocument();
            using (XmlWriter xmlWriter = xDoc.CreateWriter())
                node.WriteTo(xmlWriter);
            return xDoc.Root;
        }

        private static T GetNodeValueByKey<T>(XmlNode[] nodes, string key)
        {
            //CDumper.Dump("+Получение ключа " + key + "  из XML:\n" + nodes.ToString()); 
            //не думаю что это нужно - это не событие в системе, и если оно упадёт, то это и так будет видно

            foreach (XElement node in nodes.Where(n => n.NodeType == XmlNodeType.Element).Select(n => GetXElement(n)))
            {
                if (node.Elements().Where(n => n.Name == "key" & n.Value == key).Count() != 0)
                {
                    XElement value = node.Elements().Where(n => n.Name == "value").FirstOrDefault();
                    try
                    {
                        if (typeof(T) == typeof(String))
                        {
                            return (T)Convert.ChangeType(value.Value, typeof(T));
                        }
                        Type t = typeof(T);
                        MethodInfo parseMethod = t.GetMethod("Parse", new Type[] { typeof(String) });
                        if (parseMethod != null)
                        {
                            return (T)parseMethod.Invoke(null, new object[] { value.Value });
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            return default(T);
        }

        private static BackOfficePlanElem[] planElemParse(XmlNode[] nodes)
        {
            //CDumper.Dump("+Парсинг плана XML:\n" + nodes.ToString());
            //не думаю что это нужно - это не событие в системе, и если оно упадёт, то это и так будет видно

            int count = nodes.Count() - 1;
            List<XElement> xElems = new List<XElement>();
            foreach (XmlNode xN in nodes)
            {
                if (xN.NodeType == XmlNodeType.Element) xElems.Add(GetXElement(xN));
            }
            List<BackOfficePlanElem> planElems = new List<BackOfficePlanElem>();
            for (int i = 1; i < count + 1; i++)
            {
                XElement node = xElems.Where(n => n.Element("key").Value == i.ToString()).Single();
                BackOfficePlanElem temp = new BackOfficePlanElem();
                foreach (XElement nodd in node.Element("value").Elements().ToArray())
                {
                    if (nodd.Elements().Where(n => n.Name == "key" & n.Value == "demandId").Count() != 0)
                    {
                        XElement value = nodd.Elements().Where(n => n.Name == "value").FirstOrDefault();
                        try
                        {
                            Type t = typeof(int);
                            MethodInfo parseMethod = t.GetMethod("Parse", new Type[] { typeof(String) });
                            if (parseMethod != null)
                            {
                                temp.demandId = (int)parseMethod.Invoke(null, new object[] { value.Value });
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                    if (nodd.Elements().Where(n => n.Name == "key" & n.Value == "productId").Count() != 0)
                    {
                        XElement value = nodd.Elements().Where(n => n.Name == "value").FirstOrDefault();
                        try
                        {
                            Type t = typeof(int);
                            MethodInfo parseMethod = t.GetMethod("Parse", new Type[] { typeof(String) });
                            if (parseMethod != null)
                            {
                                temp.productId = (int)parseMethod.Invoke(null, new object[] { value.Value });
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                    if (nodd.Elements().Where(n => n.Name == "key" & n.Value == "count").Count() != 0)
                    {
                        XElement value = nodd.Elements().Where(n => n.Name == "value").FirstOrDefault();
                        try
                        {
                            Type t = typeof(int);
                            MethodInfo parseMethod = t.GetMethod("Parse", new Type[] { typeof(String) });
                            if (parseMethod != null)
                            {
                                temp.count = (int)parseMethod.Invoke(null, new object[] { value.Value });
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
                planElems.Add(temp);
            }
            return planElems.ToArray();
        }


        public bool startModeling(DateTime date)                       // передача в back office время начала нового моделирования
        {
            CDumper.Dump("+Попытка передачи в back office время начала нового моделирования - " + date.ToString());

            if (!CParams.m_bUseFakeServices)
            {
                //         Реальный код    
                string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");
                bool startResult = simulation.niceStart(dateStr);
                if (startResult == false) return ModelError.Error("Не удалось начать новое моделирование");
                else CDumper.Dump("+передача в back office время начала нового моделирования. УСПЕШНО.");
            }
            else CDumper.Dump("+Фиктивный сервис. передача в back office время начала нового моделирования. УСПЕШНО.");
            return true;
        }

        public bool approveDemand(ref CDemand demand)                    // утверждение в back office новой заявки
        {
            CDumper.Dump("+Попытка утверждения в back office новой заявки: " + demand.Dump());

            if (!CParams.m_bUseFakeServices)
            {
                //         Реальный код            
                string date = demand.m_dtGeting.ToString("yyyy-MM-dd HH:mm:ss");
                DateTime executionDate = new DateTime();

                XmlNode[] result = new XmlNode[0];
                try
                {
                    int iProduct1 = 0;
                    int iProduct2 = 0;
                    int iProduct3 = 0;

                    demand.m_products.GetProduct(1, out iProduct1);
                    demand.m_products.GetProduct(2, out iProduct2);
                    demand.m_products.GetProduct(3, out iProduct3);

                    //result = frontOffice.newOrder(date, "modeling", demand.m_products[1], demand.m_products[2], demand.m_products[3]) as XmlNode[];
                    result = frontOffice.newOrder(date, "modeling", iProduct1, iProduct2, iProduct3) as XmlNode[];
                }
                catch (Exception e)
                {
                    throw e;
                }
                if (result != null)
                {
                    demand.m_iID = GetNodeValueByKey<int>(result, "OrderId");
                    executionDate = DateTime.Parse(GetNodeValueByKey<string>(result, "ExecutionTime"));
                }

                if (demand.m_iUrgency == 2)
                {
                    //bool cancelResult = frontOffice.cancelOrder(date, demand.m_iID);
                    //if (cancelResult==false) ModelError.Error("Не удалось отменить заявку");
                    return false;
                }
                else
                {
                    demand.m_dtShouldBeDone = executionDate.AddDays(7 * (demand.m_iUrgency + 1));
                    bool confirmResult = frontOffice.confirmOrder(date, demand.m_iID, demand.m_iUrgency + 2);
                    if (confirmResult == false) ModelError.Error("Не удалось подтвердить заявку");
                    return true;
                }
            }
            else
            {
                //         Заглушка     
                if ((demand.m_iID == 1) || (demand.m_iID == 2) || (demand.m_iID == 3) || (demand.m_iID == 4)) return true;
                            
                ind++;
                demand.m_iID = ind;
                if (demand.m_iUrgency == 2) return false;
                demand.m_dtShouldBeDone = DateTime.Now.AddDays(new Random().Next(18));

                UniformGen ug = new UniformGen(1, 0);
                if (ug.generateN(1).ElementAt(0) < 0.8) return true;
                else return false;
            }
        }

        public bool approveModifyDemand(DateTime date, ref CDemand modifiedDemand, CDemand demand)               // утверждение в back office изменения заявки
        {
            CDumper.Dump("+Попытка утверждения в back office изменения заявки:\nдата: " +
                date.ToString() +
                "\nзаявка: " + demand.Dump()
                + "\nизмененная: " + modifiedDemand.Dump());

            if (!CParams.m_bUseFakeServices)
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

                modifiedDemand.m_products.GetProduct(1, out firstProdDelta);
                demand.m_products.GetProduct(1, out iTemp);
                firstProdDelta -= iTemp;

                modifiedDemand.m_products.GetProduct(2, out secondProdDelta);
                demand.m_products.GetProduct(2, out iTemp);
                secondProdDelta -= iTemp;

                modifiedDemand.m_products.GetProduct(3, out thirdProdDelta);
                demand.m_products.GetProduct(3, out iTemp);
                thirdProdDelta -= iTemp;
                //<---

                //if (modifiedDemand.m_products[1] + modifiedDemand.m_products[2] + modifiedDemand.m_products[3] == 0)
                if (
                    modifiedDemand.m_products.CompareProduct(1, 0)
                    && modifiedDemand.m_products.CompareProduct(2, 0)
                    && modifiedDemand.m_products.CompareProduct(3, 0)
                    )
                {
                    return frontOffice.cancelOrder(dateStr, modifiedDemand.m_iID);
                }
                else
                {
                    if (firstProdDelta + secondProdDelta + thirdProdDelta == 0) ModelError.Error("Номенклатура изменяемой завки не изменена");
                    string returnDateStr = frontOffice.changeOrder(dateStr, modifiedDemand.m_iID, new int[] { firstProdDelta, secondProdDelta, thirdProdDelta });
                    DateTime returnDate = DateTime.Parse(returnDateStr);
                    bool confirmResult = frontOffice.confirmChange(dateStr, modifiedDemand.m_iID, new int[] { firstProdDelta, secondProdDelta, thirdProdDelta });
                    if (confirmResult == false) return false;
                    else
                    {
                        modifiedDemand.m_dtShouldBeDone = returnDate;
                        return true;
                    }
                }
            }
            else
            {
                //         Заглушка     
                UniformGen ug = new UniformGen(1, 0);
                if (ug.generateN(1).ElementAt(0) < 0.8) return true;
                else return false;
            }
        }

        /*
        public bool reportPlanElem(CPlanReportElement curPlanElem)       // отсылка back office сообщения о выполненном элементе плана 
        {
            return true;
        }
        */



        public bool reportDeliveryDemand(CDeliveryDemand del)
        {
            CDumper.Dump("+reportDeliveryDemand: " + del.Dump());

            if (!CParams.m_bUseFakeServices)
            {
                //         Реальный код
                string date = del.m_dtRealDelivery.Value.ToString("yyyy-MM-dd HH:mm:ss");
                int[] materials = new int[12];
                for (int i = 0; i < 12; i++)
                {
                    int curMatCount = 0;
                    del.m_materialsDemand.GetMaterial(i + 1, out curMatCount);
                    materials[i] = curMatCount;
                }

                bool receivingResult = simulation.receivingMaterials(date, del.m_iID, materials);
                if (receivingResult == false) ModelError.Error("Не удалось отослать пришедшие материалы");
                return true;
            }
            else
            {
                //         Заглушка     
                bool g = del.m_dtRealDelivery.HasValue; // заглушка
                return true;            
            }
        }

        public CDeliveryDemand getDeliveryDemands(DateTime date)          //Получение от back office заявок на поставки материалов
        {
            CDumper.Dump("+Попытка получения от back office заявок на поставки материалов\nдата: " + date.ToString());

            if (!CParams.m_bUseFakeServices)
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
                    throw e;
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

        public CPlanElement[] getDailyPlan(DateTime date, ref CStorage storage)          //Получение от back office плана на день
        {
            CDumper.Dump("+Попытка получения от back office плана на день\nдата: " + date.ToString());

            if (!CParams.m_bUseFakeServices)
            {
                //      Реальный код
                string dateStr = date.ToString("yyyy-MM-dd HH:mm:ss");
                List<CPlanElement> dailyPlan = new List<CPlanElement>();

                XmlNode[] result = new XmlNode[0];
                try
                {
                    result = simulation.getDayPlan(dateStr) as XmlNode[];
                }
                catch (Exception e)
                {
                    throw e;
                }
                if (result != null)
                {
                    //int[] canc = cancelElemParse(result);
                    BackOfficePlanElem[] plan = planElemParse(result);
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
                    
                    for (int i = 0; i < plan.Length; i++)
                    {
                        if (plan[i].productId != 4)
                        {
                            if ((prevProductId != 0) && (plan[i].productId != prevProductId))
                            {
                                dailyPlan.Add(new CPlanElement { m_iDemandID = 0, m_iProductID = plan[i].productId });
                            }
                            for (int j = 0; j < plan[i].count; j++)
                            {
                                dailyPlan.Add(new CPlanElement { m_iDemandID = plan[i].demandId, m_iProductID = plan[i].productId });
                                prevProductId = plan[i].productId;
                            }
                        }
                        else
                        {
                            int ticks = plan[i].count;
                            if (ticks!=-99.0)
                            idleTime = idleTime + ticks / 60;    // Простой = (n/100)*24*60 - минуты
                        }
                    }
                    storage.SaveIdleStatistic((double)idleTime / CParams.WORKDAY_MINUTES_NUMBER);
                    
                }
                else
                {
                    storage.SaveIdleStatistic(1);
                }
                return dailyPlan.ToArray();  
            }
            else
            {
                //         Заглушка     
                CPlanElement p1 = new CPlanElement() { m_iDemandID = 1, m_iProductID = 2 };
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
}
