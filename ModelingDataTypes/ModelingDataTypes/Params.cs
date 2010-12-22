using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    public class CParams                                //класс параметров моделирования
    {
        public static int PRODUCTS_NUMBER;              //количество типов продуктов
        public static int MATERIALS_NUMBER;             //количество типов материалов
        public static int WORKDAY_MINUTES_NUMBER;       //рабочее время в минутах
        public static int DELIVERY_PERIOD;              //период в днях между получением заказов на поставки материалов

        public static int m_iModelingDayToWork;         //время работы моделирования до остановки

        private static bool m_bInitialized = false;     //параметры моделирования инициализированы (да/нет)


        public static Dictionary<int, CGeneratedProduct> m_products;
        //описание всех типов производимых продуктов
        //пара: номер продукта - продукт (номера начинаются с единицы)


        public static Dictionary<int, CMaterial> m_materials;
        //описание всех используемых материалов
        //пара: номер материала - материал (номера начинаются с единицы)


        public static int[] retargetTimes;  // время на перенастройку оборудования


        public static CGeneratedElement m_generatorDemandsTime; //генератор поступления заявок


        public static double m_fUrgencyPropabilityDemand;       //вероятность срочности заявки

        public static double m_fRefusePropabilityDemand;        //вероятность отказа от заявки


        public static CGeneratedElement m_deliveryDelayGenerator;
        //генератор времени задержки поставки материалов


        public static CGeneratedElement m_demandModifyTime;
        //генератор времени изменения заявки


        public static CGeneratedElement m_ugrToStandModify; //генератор ???


        public static CGeneratedElement m_standToUrgModify; //генератор ???


        public static CGeneratedElement m_articlesModify;   //генератор ???


        public static bool m_bUseFakeServices; // использовать фиктивные внешние сервисы




        public static bool Initialization() //инициализация начальных параметров
        {
            if (m_bInitialized) //если параметры уже инициализированы, ничего не делать
                return false;

            m_bUseFakeServices = false; // НЕ использовать фиктивные внешние сервисы

            WORKDAY_MINUTES_NUMBER = 1440;  //рабочее время в минутах


            m_iModelingDayToWork = 20;      //время работы моделирования до остановки


            MATERIALS_NUMBER = 12;        //количество типов материалов


            PRODUCTS_NUMBER = 3;            //количество типов продуктов


            DELIVERY_PERIOD = 1;            //период в днях между получением заказов на поставки материалов


            Action<CGeneratedProduct, int[]> assignMaterialsCount = (product, materialsCount) =>
            {
                for (var materialIndex = 0; materialIndex < 12; materialIndex++)
                {
                    product.m_materials.AddMaterial(
                        materialIndex + 1,
                        materialsCount[materialIndex]
                    );
                }
            };

            m_products = new Dictionary<int, CGeneratedProduct>();

            CGeneratedProduct product1 = new CGeneratedProduct()
            {
                m_iTime = 26,
                m_iIndex = 1,
                m_iGeneratorType = GeneratorType.Normal,
                m_fA = 5.0,
                m_fB = 2.0,
                m_modify = new CGeneratedElement
                {
                    m_iGeneratorType = GeneratorType.Normal,
                    m_fA = 0.0,
                    m_fB = 3.0
                }
            };

            // Denis Bykov: да, это тоже плохо, посмотрим, что можно будет сделать впоследствии
            assignMaterialsCount(product1, new[] { 6, 2, 3, 0, 2, 4, 5, 0, 3, 8, 2, 1 });

            m_products.Add(1, product1);


            CGeneratedProduct product2 = new CGeneratedProduct()
            {
                m_iTime = 72,
                m_iIndex = 2,
                m_iGeneratorType = GeneratorType.Normal,
                m_fA = 5.0,
                m_fB = 2.0,
                m_modify = new CGeneratedElement
                {
                    m_iGeneratorType = GeneratorType.Normal,
                    m_fA = 0.0,
                    m_fB = 3.0
                }
            };

            assignMaterialsCount(product2, new[] { 0, 0, 1, 4, 5, 1, 2, 4, 7, 8, 8, 3 });

            m_products.Add(2, product2);


            CGeneratedProduct product3 = new CGeneratedProduct()
            {
                m_iTime = 49,
                m_iIndex = 3,
                m_iGeneratorType = GeneratorType.Normal,
                m_fA = 5.0,
                m_fB = 2.0,
                m_modify = new CGeneratedElement
                {
                    m_iGeneratorType = GeneratorType.Normal,
                    m_fA = 0.0,
                    m_fB = 3.0
                }
            };

            assignMaterialsCount(product3, new[] { 2, 6, 5, 3, 1, 6, 9, 2, 1, 3, 0, 4 });

            m_products.Add(3, product3);


            m_materials = new Dictionary<int, CMaterial>();
            for (var materialIndex = 1; materialIndex <= 12; materialIndex++)
            {
                m_materials.Add(
                    materialIndex,
                    new CMaterial { m_iIndex = materialIndex, m_iGeneratorType = GeneratorType.Normal, m_fA = 3.0, m_fB = 4.0 }
                );
            }

            retargetTimes = new int[] { 68, 39, 95 }; // время на перенастройку оборудования


            m_generatorDemandsTime = new CGeneratedElement()    //генератор поступления заявок
            {
                m_iGeneratorType = GeneratorType.Exponential,
                m_fA = 636.0,
                m_fB = 0.0
            };


            m_fUrgencyPropabilityDemand = 0.3;      //вероятность срочности заявки

            m_fRefusePropabilityDemand = 0.07;      //вероятность отказа от заявки


            m_deliveryDelayGenerator = new CGeneratedElement()
            //генератор времени задержки поставки материалов
            {
                m_iGeneratorType = GeneratorType.Normal,
                m_fA = 12.0,
                m_fB = 0.0
            };


            m_demandModifyTime = new CGeneratedElement()
            //генератор времени изменения заявки
            {
                m_iGeneratorType = GeneratorType.Normal,
                m_fA = 1281.0,
                m_fB = 0.0
            };


            m_ugrToStandModify = new CGeneratedElement()
            //генератор ???
            {
                m_iGeneratorType = GeneratorType.Rayleigh,
                m_fA = 3000.0,
                m_fB = 0.0
            };


            m_standToUrgModify = new CGeneratedElement()
            //генератор ??? 
            {
                m_iGeneratorType = GeneratorType.Rayleigh,
                m_fA = 3000.0,
                m_fB = 0.0
            };


            m_articlesModify = new CGeneratedElement()
            //генератор ???
            {
                m_iGeneratorType = GeneratorType.Exponential,
                m_fA = 9.0,
                m_fB = 30.0
            };


            m_bInitialized = true;  //параметры моделирования инициализированы
            return true;

        } //end Initialization


    } // end CParams
}
