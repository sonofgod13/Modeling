using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    public class CEntityCluster
    {
        protected Dictionary<int, int> m_entities;
        //Продукты. Пара: идентификатор материала - его количество

        protected int GLOBAL_LIMITATION;

        public CEntityCluster(int iLimitation)
        //конструктор по умоланию инициализирует кластер нулевыми значениями
        {
            GLOBAL_LIMITATION = iLimitation;

            m_entities = new Dictionary<int, int>();
            for (int iEntityNumber = 1; iEntityNumber < GLOBAL_LIMITATION + 1; iEntityNumber++)
            {
                m_entities.Add(iEntityNumber, 0);
            }
        }

        //конструктор, принимающий Dictionary или Array ЗАПРЕЩЕН!!!


        public CEntityCluster(CEntityCluster copy, int iLimitation) //конструктор копирования
        {
            GLOBAL_LIMITATION = iLimitation;

            m_entities = new Dictionary<int, int>(copy.m_entities);
        }


        public void AddEntityCluster(CEntityCluster entityCluster)
        //приплюсовать к текущим материалам перед. кластер
        {
            for (int iEntityNumber = 1; iEntityNumber < GLOBAL_LIMITATION + 1; iEntityNumber++)
            {
                m_entities[iEntityNumber] += entityCluster.m_entities[iEntityNumber];
            }
        }

        public bool IsEntityCluster(CEntityCluster entityCluster)
        //проверяет есть ли такое кол-то материалов в кластере как в перед. кластере
        //если количество материалов по каждому наименованию больше или равно - возвращает true
        {
            for (int iEntityNumber = 1; iEntityNumber < GLOBAL_LIMITATION + 1; iEntityNumber++)
            {
                if (m_entities[iEntityNumber] < entityCluster.m_entities[iEntityNumber])
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
                m_entities[iEntityNumber] -= entityCluster.m_entities[iEntityNumber];
            }
            return true;
        }

        public void CleanEntitysCluster()
        //обнуляет количество материалов во всем кластере
        {
            for (int iEntityNumber = 1; iEntityNumber < GLOBAL_LIMITATION + 1; iEntityNumber++)
            {
                m_entities[iEntityNumber] = 0;
            }
        }

        public bool AddEntity(int iEntityNumber, int iAmount)
        //добавляет заданное количество к выбранному материалу
        {
            if (iEntityNumber < 1 || iEntityNumber > GLOBAL_LIMITATION)
            {
                return ModelError.Error();
            }
            if (iAmount < 0)
            {
                return ModelError.Error();
            }

            m_entities[iEntityNumber] += iAmount;
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

            if (m_entities[iEntityNumber] < iAmount)
            {
                return false;
            }
            return true;
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

            if (!IsEntity(iEntityNumber, iAmount))
            {
                return false;
            }

            m_entities[iEntityNumber] -= iAmount;
            return true;
        }

        public bool CleanEntity(int iEntityNumber)
        //обнуляет количество выбранного материала
        {
            if (iEntityNumber < 1 || iEntityNumber > GLOBAL_LIMITATION)
            {
                return ModelError.Error();
            }

            m_entities[iEntityNumber] = 0;
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

            iEntityValue = m_entities[iEntityNumber];

            return true;
        }

        public bool CompareEntity(int iEntityNumber, int iAmount)
        //сравнивает количество единиц сущности iEntityNumber с числом iAmount (если равно - возвращает true)
        {
            if (iEntityNumber < 1 || iEntityNumber > GLOBAL_LIMITATION)
            {
                return ModelError.Error();
            }

            if (m_entities[iEntityNumber] == iAmount)
            {
                return true;
            }
            return false;
        }

        public bool CompareNomenclatureIsMore(CEntityCluster materialCluster)
        {
            bool isMoreFlag = false;
            for (int iEntityNumber = 1; iEntityNumber < GLOBAL_LIMITATION + 1; iEntityNumber++)
            {
                if (m_entities[iEntityNumber] >= materialCluster.m_entities[iEntityNumber])
                {
                    if ((m_entities[iEntityNumber] > materialCluster.m_entities[iEntityNumber])) isMoreFlag = true;
                }
                else return false;
            }
            if (isMoreFlag == true) return true;
            else return false;
        }

        public string Dump()
        {
            string str = "";
            for (int iEntityNumber = 1; iEntityNumber < GLOBAL_LIMITATION + 1; iEntityNumber++)
            {
                str = str + "сущность №: " + iEntityNumber.ToString() + " = " + m_entities[iEntityNumber].ToString() + "\n";
            }

            return str;
        }
    }
}
