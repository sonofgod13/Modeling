using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    public class EntityCluster
    {
        /// <summary>
        /// Продукты. Пара: идентификатор материала - его количество
        /// </summary>
        protected Dictionary<int, int> Entities;

        /// <summary>
        /// Конструктор по умоланию инициализирует кластер нулевыми значениями
        /// </summary>
        /// <param name="iLimitation"></param>
        public EntityCluster(int clusterSize)
        {
            this.Entities = new Dictionary<int, int>(clusterSize);
            for (int entityIndex = 1; entityIndex < clusterSize + 1; entityIndex++)
            {
                this.Entities.Add(entityIndex, 0);
            }
        }

        //конструктор, принимающий Dictionary или Array ЗАПРЕЩЕН!!!

        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="copy"></param>
        /// <param name="iLimitation"></param>
        public EntityCluster(EntityCluster copy, int clusterSize)
        {
            this.Entities = new Dictionary<int, int>(copy.Entities);
        }

        /// <summary>
        /// Приплюсовать к текущим материалам перед. кластер
        /// </summary>
        /// <param name="entityCluster"></param>
        public void AddEntityCluster(EntityCluster entityCluster)
        {
            foreach (var entityIndex in this.Entities.Keys)
            {
                this.Entities[entityIndex] += entityCluster.Entities[entityIndex];
            }
        }

        /// <summary>
        ///Проверяет есть ли такое кол-то материалов в кластере как в перед. кластере. 
        ///Если количество материалов по каждому наименованию больше или равно - возвращает true
        /// </summary>
        /// <param name="entityCluster"></param>
        /// <returns></returns>
        public bool IsEntityCluster(EntityCluster entityCluster)
        {
            for (int entityIndex = 1; entityIndex < this.Entities.Count + 1; entityIndex++)
            {
                if (this.Entities[entityIndex] < entityCluster.Entities[entityIndex])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Вычесть из кластера материалы из перед. кластера
        /// </summary>
        /// <param name="entityCluster"></param>
        /// <returns></returns>
        public bool TakeAwayEntityCluster(EntityCluster entityCluster)
        {
            if (!IsEntityCluster(entityCluster))
            //проверка: есть ли такое кол-то материалов в кластере как в перед. кластере
            {
                return false;
            }
            for (int iEntityNumber = 1; iEntityNumber < this.Entities.Count + 1; iEntityNumber++)
            {
                this.Entities[iEntityNumber] -= entityCluster.Entities[iEntityNumber];
            }
            return true;
        }

        /// <summary>
        /// Обнуляет количество материалов во всем кластере
        /// </summary>
        public void CleanEntitysCluster()
        {
            foreach (var entityIndex in this.Entities.Keys)
            {
                this.Entities[entityIndex] = 0;
            }
        }

        /// <summary>
        /// Добавляет заданное количество к выбранному материалу
        /// </summary>
        /// <param name="iEntityNumber"></param>
        /// <param name="iAmount"></param>
        /// <returns></returns>
        public bool AddEntity(int iEntityNumber, int iAmount)
        {
            if (iEntityNumber < 1 || iEntityNumber > this.Entities.Count || iAmount < 0)
            {
                return ModelError.Error();
            }

            this.Entities[iEntityNumber] += iAmount;
            return true;
        }

        /// <summary>
        /// Проверяет есть ли такое количество выбранного материала
        /// </summary>
        /// <param name="iEntityNumber"></param>
        /// <param name="iAmount"></param>
        /// <returns></returns>
        public bool IsEntity(int iEntityNumber, int iAmount)
        {
            if (iEntityNumber < 1 || iEntityNumber > this.Entities.Count)
            {
                return ModelError.Error();
            }
            if (iAmount < 0)
            {
                return ModelError.Error();
            }

            return Entities[iEntityNumber] >= iAmount;
        }

        /// <summary>
        /// Вычитает из количества выбранного материала перед. количество
        /// </summary>
        /// <param name="iEntityNumber"></param>
        /// <param name="iAmount"></param>
        /// <returns></returns>
        public bool TakeAwayEntity(int iEntityNumber, int iAmount)
        {
            if (iEntityNumber < 1 || iEntityNumber > this.Entities.Count)
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

        /// <summary>
        /// Обнуляет количество выбранного материала
        /// </summary>
        /// <param name="iEntityNumber"></param>
        /// <returns></returns>
        public bool CleanEntity(int iEntityNumber)
        {
            if (iEntityNumber < 1 || iEntityNumber > this.Entities.Count)
            {
                return ModelError.Error();
            }

            this.Entities[iEntityNumber] = 0;
            return true;
        }

        /// <summary>
        /// Возвращает количество выбранного материала
        /// </summary>
        /// <param name="iEntityNumber"></param>
        /// <param name="iEntityValue"></param>
        /// <returns></returns>
        public bool GetEntity(int iEntityNumber, out int iEntityValue)
        {
            iEntityValue = 0;

            if (iEntityNumber < 1 || iEntityNumber > this.Entities.Count)
            {
                return ModelError.Error();
            }

            iEntityValue = this.Entities[iEntityNumber];

            return true;
        }

        /// <summary>
        /// Сравнивает количество единиц сущности iEntityNumber с числом iAmount (если равно - возвращает true)
        /// </summary>
        /// <param name="iEntityNumber"></param>
        /// <param name="iAmount"></param>
        /// <returns></returns>
        public bool CompareEntity(int iEntityNumber, int iAmount)
        {
            if (iEntityNumber < 1 || iEntityNumber > this.Entities.Count)
            {
                return ModelError.Error();
            }

            return this.Entities[iEntityNumber] == iAmount;
        }

        public bool CompareNomenclatureIsMore(EntityCluster materialCluster)
        {
            bool isMoreFlag = false;

            for (int iEntityNumber = 1; iEntityNumber < this.Entities.Count + 1; iEntityNumber++)
            {
                if (this.Entities[iEntityNumber] < materialCluster.Entities[iEntityNumber])
                    return false;

                if ((this.Entities[iEntityNumber] > materialCluster.Entities[iEntityNumber]))
                    isMoreFlag = true;
            }

            return isMoreFlag;
        }
    }
}
