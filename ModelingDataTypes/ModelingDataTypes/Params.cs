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

        public static bool useDump; // использовать дамп




        public static bool Initialization() //инициализация начальных параметров
        {
            if (m_bInitialized) //если параметры уже инициализированы, ничего не делать
                return false;

            m_bUseFakeServices = false; // НЕ использовать фиктивные внешние сервисы

            useDump = false;  // НЕ использовать дамп

            WORKDAY_MINUTES_NUMBER = 1440;  //рабочее время в минутах


            m_iModelingDayToWork = 20;      //время работы моделирования до остановки


            MATERIALS_NUMBER = 12;        //количество типов материалов


            PRODUCTS_NUMBER = 3;            //количество типов продуктов


            DELIVERY_PERIOD = 7;            //период в днях между получением заказов на поставки материалов


            m_products = new Dictionary<int, CGeneratedProduct>();

            CGeneratedProduct product1 = new CGeneratedProduct();
            product1.m_iTime = 26;
            product1.m_iIndex = 1;
            product1.m_iGeneratorType = 1;
            product1.m_fA = 5.0;
            product1.m_fB = 2.0;
            product1.m_modify.m_iGeneratorType = 1;
            product1.m_modify.m_fA = 0.0; 
            product1.m_modify.m_fB = 3.0;
            product1.m_materials.AddMaterial(1, 6);
            product1.m_materials.AddMaterial(2, 2);
            product1.m_materials.AddMaterial(3, 3);
            product1.m_materials.AddMaterial(4, /*4);*/0);
            product1.m_materials.AddMaterial(5, /*0);*/2);
            product1.m_materials.AddMaterial(6, /*0);*/4);
            product1.m_materials.AddMaterial(7, /*0);*/5);
            product1.m_materials.AddMaterial(8, /*0);*/0);
            product1.m_materials.AddMaterial(9, /*0);*/3);
            product1.m_materials.AddMaterial(10, /*0);*/8);
            product1.m_materials.AddMaterial(11, /*0);*/2);
            product1.m_materials.AddMaterial(12, /*0);*/1);

            m_products.Add(1, product1 );


            CGeneratedProduct product2 = new CGeneratedProduct();
            product2.m_iTime = 72;
            product2.m_iIndex = 2;
            product2.m_iGeneratorType = 1;
            product2.m_fA = 5.0;
            product2.m_fB = 2.0;
            product1.m_modify.m_iGeneratorType = 1;
            product1.m_modify.m_fA = 0.0;
            product1.m_modify.m_fB = 3.0;
            product2.m_materials.AddMaterial(1, 0);
            product2.m_materials.AddMaterial(2, 0);
            product2.m_materials.AddMaterial(3, /*0);*/1);
            product2.m_materials.AddMaterial(4, /*0);*/4);
            product2.m_materials.AddMaterial(5, 5);
            product2.m_materials.AddMaterial(6, 1);
            product2.m_materials.AddMaterial(7, 2);
            product2.m_materials.AddMaterial(8, 4);
            product2.m_materials.AddMaterial(9, /*0);*/7);
            product2.m_materials.AddMaterial(10, /*0);*/8);
            product2.m_materials.AddMaterial(11, /*0);*/8);
            product2.m_materials.AddMaterial(12, /*0);*/3);

            m_products.Add(2, product2 );


            CGeneratedProduct product3 = new CGeneratedProduct();
            product3.m_iTime = 49;
            product3.m_iIndex = 3;
            product3.m_iGeneratorType = 1;
            product3.m_fA = 5.0;
            product3.m_fB = 2.0;
            product1.m_modify.m_iGeneratorType = 1;
            product1.m_modify.m_fA = 0.0;
            product1.m_modify.m_fB = 3.0;
            product3.m_materials.AddMaterial(1, /*0);*/2);
            product3.m_materials.AddMaterial(2, /*0);*/6);
            product3.m_materials.AddMaterial(3, /*0);*/5);
            product3.m_materials.AddMaterial(4, /*0);*/3);
            product3.m_materials.AddMaterial(5, /*0);*/1);
            product3.m_materials.AddMaterial(6, /*0);*/6);
            product3.m_materials.AddMaterial(7, /*0);*/9);
            product3.m_materials.AddMaterial(8, /*0);*/2);
            product3.m_materials.AddMaterial(9, 1);
            product3.m_materials.AddMaterial(10, 3);
            product3.m_materials.AddMaterial(11, /*6);*/0);
            product3.m_materials.AddMaterial(12, 4);

            m_products.Add(3, product3 );


            m_materials = new Dictionary<int, CMaterial>();

            m_materials.Add(1,
                new CMaterial { m_iIndex = 1, m_iGeneratorType = 1, m_fA = 3.0, m_fB = 4.0 }
                );
            m_materials.Add(2,
               new CMaterial { m_iIndex = 2, m_iGeneratorType = 1, m_fA = 3.0, m_fB = 4.0 }
               );
            m_materials.Add(3,
               new CMaterial { m_iIndex = 3, m_iGeneratorType = 1, m_fA = 3.0, m_fB = 4.0 }
               );
            m_materials.Add(4,
               new CMaterial { m_iIndex = 4, m_iGeneratorType = 1, m_fA = 3.0, m_fB = 4.0 }
               );
            m_materials.Add(5,
               new CMaterial { m_iIndex = 5, m_iGeneratorType = 1, m_fA = 3.0, m_fB = 4.0 }
               );
            m_materials.Add(6,
               new CMaterial { m_iIndex = 6, m_iGeneratorType = 1, m_fA = 3.0, m_fB = 4.0 }
               );
            m_materials.Add(7,
               new CMaterial { m_iIndex = 7, m_iGeneratorType = 1, m_fA = 3.0, m_fB = 4.0 }
               );
            m_materials.Add(8,
               new CMaterial { m_iIndex = 8, m_iGeneratorType = 1, m_fA = 3.0, m_fB = 4.0 }
               );
            m_materials.Add(9,
               new CMaterial { m_iIndex = 9, m_iGeneratorType = 1, m_fA = 3.0, m_fB = 4.0 }
               );
            m_materials.Add(10,
               new CMaterial { m_iIndex = 10, m_iGeneratorType = 1, m_fA = 3.0, m_fB = 4.0 }
               );
            m_materials.Add(11,
               new CMaterial { m_iIndex = 11, m_iGeneratorType = 1, m_fA = 3.0, m_fB = 4.0 }
               );
            m_materials.Add(12,
               new CMaterial { m_iIndex = 12, m_iGeneratorType = 1, m_fA = 3.0, m_fB = 4.0 }
               );


            retargetTimes = new int[3] { 68, 39, 95 }; // время на перенастройку оборудования


            m_generatorDemandsTime = new CGeneratedElement()    //генератор поступления заявок
            {
                m_iGeneratorType = 4,
                m_fA = 636.0,
                m_fB = 0.0
            };


            m_fUrgencyPropabilityDemand = 0.3;      //вероятность срочности заявки

            m_fRefusePropabilityDemand = 0.07;      //вероятность отказа от заявки


            m_deliveryDelayGenerator = new CGeneratedElement()
            //генератор времени задержки поставки материалов
            {
                m_iGeneratorType = 4,
                m_fA = 12.0,
                m_fB = 0.0
            };


            m_demandModifyTime = new CGeneratedElement()
            //генератор времени изменения заявки
            {
                m_iGeneratorType = 4,
                m_fA = 1281.0,
                m_fB = 0.0
            };


            m_ugrToStandModify = new CGeneratedElement()
            //генератор ???
            {
                m_iGeneratorType = 2,
                m_fA = 3000.0,
                m_fB = 0.0
            };


            m_standToUrgModify = new CGeneratedElement()
            //генератор ??? 
            {
                m_iGeneratorType = 2,
                m_fA = 3000.0,
                m_fB = 0.0
            };


            m_articlesModify = new CGeneratedElement()
            //генератор ???
            {
                m_iGeneratorType = 4,
                m_fA = 9.0,
                m_fB = 30.0
            };


            m_bInitialized = true;  //параметры моделирования инициализированы
            return true;

        } //end Initialization


    } // end CParams
}
