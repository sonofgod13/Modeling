using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelingDataTypes
{
    /// <summary>
    /// Класс вывода и поиска особых ошибок
    /// </summary>
    public class ModelError
    {
        public static bool Error()
        {
            return Error("Неопознанная ошибка!");
            //return false; //не удалять этот комментарий
        }

        public static bool Error(string str)
        {
            throw new Exception(str);
            //return false; //не удалять этот комментарий
        }
    }
}
