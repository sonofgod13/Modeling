using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    public class CEntityCluster
    {
        protected Dictionary<int, int> Entities;
        //Продукты. Пара: идентификатор материала - его количество

        protected int GLOBAL_LIMITATION;

        public CEntityCluster(int iLimitation)
        //конструктор по умоланию инициализирует кластер нулевыми значениями
        {
            GLOBAL_LIMITATION = iLimitation;

            this.Entities = new Dictionary<int, int>();
            for (int iEntityNumber = 1; iEntityNumber < GLOBAL_LIMITATION + 1; iEntityNumber++)
            {
                this.Entities.Add(iEntityNumber, 0);
            }
        }

        //конструктор, принимающий Dictionary или Array ЗАПРЕЩЕН!!!


        public CEntityCluster(CEntityCluster copy, int iLimitation) //конструктор копирования
        {
            GLOBAL_LIMITATION = iLimitation;

            this.Entities = new Dictionary<int, int>(copy.Entities);
        }


        public void AddEntityCluster(CEntityCluster entityCluster)
        //приплюсовать к текущим материалам перед. кластер
        {
            for (int iEntityNumber = 1; iEntityNumber < GLOBAL_LIMITATION + 1; iEntityNumber++)
            {
                this.Entities[iEntityNumber] += entityCluster.Entities[iEntityNumber];
            }
        }

        public bool IsEntityCluster(CEntityCluster entityCluster)
        //проверяет есть ли такое кол-то материалов в кластере как в перед. кластере
        //если количество материалов по каждому наименованию больше или равно - возвращает true
        {
            for (int iEntityNumber = 1; iEntityNumber < GLOBAL_LIMITATION + 1; iEntityNumber++)
            {
                if (Entities[iEntityNumber] < entityCluster.Entities[iEntityNumber])
                {
                    return false;
                }
            }
            return true;
        }

        public bool TakeAwayEntityCluster(CEntityCluster entityCluster)
        //вычесть из кластера материалы из перед. кластера
        {
            if (!IsEntityCluster(entityCluster))
            //проверка: есть ли такое кол-то материалов в кластере как в перед. кластере
            {
                return false;
            }
            for (int iEntityNumber = 1; iEntityNumber < GLOBAL_LIMITATION + 1; iEntityNumber++)
            {
                this.Entities[iEntityNumber] -= entityCluster.Entities[iEntityNumber];
            }
            return true;
        }

        public void CleanEntitysCluster()
        //обнуляет количество материалов во всем кластере
        {
            for (int iEntityNumber = 1; iEntityNumber < GLOBAL_LIMITATION + 1; iEntityNumber++)
            {
                this.Entities[iEntityNumber] = 0;
            }
        }

        public bool AddEntity(int iEntityNumber, int iAmount)
        //добавляет заданное количество к выбранному материалу
        {
            if (iEntityNumber < 1 || iEntityNumber > GLOBAL_LIMITATION || iAmount < 0)
            {
                return ModelError.Error();
            }

            this.Entities[iEntityNumber] += iAmount;
            return true;
        }

        public bool IsEntity(int iEntityNumber, int iAmount)
        //проверяет есть ли такое количество выбранного материала
        {
            if (iEntityNumber < 1 || iEntityNumber > GLOBAL_LIMITATION)
            {
                return ModelError.Error();
            }
            if (iAmount < 0)
            {
                return ModelError.Error();
            }

            return Entities[iEntityNumber] >= iAmount;
        }

        public bool TakeAwayEntity(int iEntityNumber, int iAmount)
        //вычитает из количества выбранного материала перед. количество
        {
            if (iEntityNumber < 1 || iEntityNumber > GLOBAL_LIMITATION)
            {
                return ModelError.Error();
            }
            if (iAmount < 0)
            {
                return ModelError.Error();
            }

            if (!this.IsEntity(iEntityNumber, iAmount))
            {
                return false;
            }

            this.Entities[iEntityNumber] -= iAmount;

            return true;
        }

        public bool CleanEntity(int iEntityNumber)
        //обнуляет количество выбранного материала
        {
            if (iEntityNumber < 1 || iEntityNumber > GLOBAL_LIMITATION)
            {
                return ModelError.Error();
            }

            this.Entities[iEntityNumber] = 0;
            return true;
        }

        public bool GetEntity(int iEntityNumber, out int iEntityValue)
        //возвращает количество выбранного материала
        {
            iEntityValue = 0;

            if (iEntityNumber < 1 || iEntityNumber > GLOBAL_LIMITATION)
            {
                return ModelError.Error();
            }

            iEntityValue = this.Entities[iEntityNumber];

            return true;
        }

        public bool CompareEntity(int iEntityNumber, int iAmount)
        //сравнивает количество единиц сущности iEntityNumber с числом iAmount (если равно - возвращает true)
        {
            if (iEntityNumber < 1 || iEntityNumber > GLOBAL_LIMITATION)
            {
                return ModelError.Error();
            }

            return this.Entities[iEntityNumber] == iAmount;
        }

        public bool CompareNomenclatureIsMore(CEntityCluster materialCluster)
        {
            bool isMoreFlag = false;
            for (int iEntityNumber = 1; iEntityNumber < GLOBAL_LIMITATION + 1; iEntityNumber++)
            {
                if (this.Entities[iEntityNumber] >= materialCluster.Entities[iEntityNumber])
                {
                    if ((this.Entities[iEntityNumber] > materialCluster.Entities[iEntityNumber])) 
                        isMoreFlag = true;
                }
                else 
                    return false;
            }

            return isMoreFlag;
        }
    }
}
