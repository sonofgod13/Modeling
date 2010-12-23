using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    /// <summary>
    /// класс параметров моделирования
    /// </summary>
    public class Params                                
    {
        /// <summary>
        /// количество типов продуктов
        /// </summary>
        public static int PRODUCTS_NUMBER;

        /// <summary>
        /// количество типов материалов
        /// </summary>
        public static int MATERIALS_NUMBER;

        /// <summary>
        /// рабочее время в минутах
        /// </summary>
        public static int WORKDAY_MINUTES_NUMBER;

        /// <summary>
        /// период в днях между получением заказов на поставки материалов
        /// </summary>
        public static int DELIVERY_PERIOD;

        /// <summary>
        /// время работы моделирования до остановки
        /// </summary>
        public static int ModelingDayToWork;

        /// <summary>
        /// параметры моделирования инициализированы (да/нет)
        /// </summary>
        private static bool Initialized = false;

        /// <summary>
        /// описание всех типов производимых продуктов. пара: номер продукта - продукт (номера начинаются с единицы)
        /// </summary>
        public static Dictionary<int, GeneratedProduct> Products;

        /// <summary>
        /// описание всех используемых материалов. пара: номер материала - материал (номера начинаются с единицы)
        /// </summary>
        public static Dictionary<int, Material> Materials;

        /// <summary>
        /// время на перенастройку оборудования
        /// </summary>
        public static int[] RetargetTimes;

        /// <summary>
        /// генератор поступления заявок
        /// </summary>
        public static GeneratedElement GeneratorDemandsTime;

        /// <summary>
        /// вероятность срочности заявки
        /// </summary>
        public static double fUrgencyPropabilityDemand;

        /// <summary>
        /// вероятность отказа от заявки
        /// </summary>
        public static double fRefusePropabilityDemand;

        /// <summary>
        /// генератор времени задержки поставки материалов
        /// </summary>
        public static GeneratedElement DeliveryDelayGenerator;

        /// <summary>
        /// генератор времени изменения заявки
        /// </summary>
        public static GeneratedElement DemandModifyTime;

        /// <summary>
        /// генератор ???
        /// </summary>
        public static GeneratedElement UgrToStandModify;

        /// <summary>
        /// генератор ???
        /// </summary>
        public static GeneratedElement StandToUrgModify;

        /// <summary>
        /// генератор ???
        /// </summary>
        public static GeneratedElement ArticlesModify;

        /// <summary>
        /// использовать фиктивные внешние сервисы
        /// </summary>
        public static bool UseFakeServices;
        
        /// <summary>
        /// инициализация начальных параметров
        /// </summary>
        /// <returns></returns>
        public static bool Initialization()
        {
            if (Initialized) //если параметры уже инициализированы, ничего не делать
                return false;

            UseFakeServices = false; // НЕ использовать фиктивные внешние сервисы

            WORKDAY_MINUTES_NUMBER = 1440;  //рабочее время в минутах


            ModelingDayToWork = 20;      //время работы моделирования до остановки


            MATERIALS_NUMBER = 12;        //количество типов материалов


            PRODUCTS_NUMBER = 3;            //количество типов продуктов


            DELIVERY_PERIOD = 1;            //период в днях между получением заказов на поставки материалов


            Action<GeneratedProduct, int[]> assignMaterialsCount = (product, materialsCount) =>
            {
                for (var materialIndex = 0; materialIndex < 12; materialIndex++)
                {
                    product.Materials.AddMaterial(
                        materialIndex + 1,
                        materialsCount[materialIndex]
                    );
                }
            };

            Products = new Dictionary<int, GeneratedProduct>();

            GeneratedProduct product1 = new GeneratedProduct()
            {
                Time = 26,
                Index = 1,
                GeneratorType = GeneratorType.Normal,
                fA = 5.0,
                fB = 2.0,
                Modify = new GeneratedElement
                {
                    GeneratorType = GeneratorType.Normal,
                    fA = 0.0,
                    fB = 3.0
                }
            };

            // Denis Bykov: да, это тоже плохо, посмотрим, что можно будет сделать впоследствии
            assignMaterialsCount(product1, new[] { 6, 2, 3, 0, 2, 4, 5, 0, 3, 8, 2, 1 });

            Products.Add(1, product1);


            GeneratedProduct product2 = new GeneratedProduct()
            {
                Time = 72,
                Index = 2,
                GeneratorType = GeneratorType.Normal,
                fA = 5.0,
                fB = 2.0,
                Modify = new GeneratedElement
                {
                    GeneratorType = GeneratorType.Normal,
                    fA = 0.0,
                    fB = 3.0
                }
            };

            assignMaterialsCount(product2, new[] { 0, 0, 1, 4, 5, 1, 2, 4, 7, 8, 8, 3 });

            Products.Add(2, product2);


            GeneratedProduct product3 = new GeneratedProduct()
            {
                Time = 49,
                Index = 3,
                GeneratorType = GeneratorType.Normal,
                fA = 5.0,
                fB = 2.0,
                Modify = new GeneratedElement
                {
                    GeneratorType = GeneratorType.Normal,
                    fA = 0.0,
                    fB = 3.0
                }
            };

            assignMaterialsCount(product3, new[] { 2, 6, 5, 3, 1, 6, 9, 2, 1, 3, 0, 4 });

            Products.Add(3, product3);


            Materials = new Dictionary<int, Material>();
            for (var materialIndex = 1; materialIndex <= 12; materialIndex++)
            {
                Materials.Add(
                    materialIndex,
                    new Material { Index = materialIndex, GeneratorType = GeneratorType.Normal, fA = 3.0, fB = 4.0 }
                );
            }

            RetargetTimes = new int[] { 68, 39, 95 }; // время на перенастройку оборудования


            GeneratorDemandsTime = new GeneratedElement()    //генератор поступления заявок
            {
                GeneratorType = GeneratorType.Exponential,
                fA = 636.0,
                fB = 0.0
            };


            fUrgencyPropabilityDemand = 0.3;      //вероятность срочности заявки

            fRefusePropabilityDemand = 0.07;      //вероятность отказа от заявки


            DeliveryDelayGenerator = new GeneratedElement()
            //генератор времени задержки поставки материалов
            {
                GeneratorType = GeneratorType.Normal,
                fA = 12.0,
                fB = 0.0
            };


            DemandModifyTime = new GeneratedElement()
            //генератор времени изменения заявки
            {
                GeneratorType = GeneratorType.Normal,
                fA = 1281.0,
                fB = 0.0
            };


            UgrToStandModify = new GeneratedElement()
            //генератор ???
            {
                GeneratorType = GeneratorType.Rayleigh,
                fA = 3000.0,
                fB = 0.0
            };


            StandToUrgModify = new GeneratedElement()
            //генератор ??? 
            {
                GeneratorType = GeneratorType.Rayleigh,
                fA = 3000.0,
                fB = 0.0
            };


            ArticlesModify = new GeneratedElement()
            //генератор ???
            {
                GeneratorType = GeneratorType.Exponential,
                fA = 9.0,
                fB = 30.0
            };


            Initialized = true;  //параметры моделирования инициализированы
            return true;

        } //end Initialization


    } // end CParams
}
