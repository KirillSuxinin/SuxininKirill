using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuxininKirill
{
    public static class ApplicationDB
    {
        /// <summary>
        /// Обобщённый класс для работы с подключение к базе данных
        /// </summary>
        public static System.Data.Common.DbConnection Connection;

        /// <summary>
        /// Статический класс для перевода технических названий стоблцов на русский язык.
        /// </summary>
        public static class LocalizationHelper
        {
            /// <summary>
            /// Метод по имени столбца переводит на русский из файла Json
            /// </summary>
            /// <param name="Name">Имя столбца</param>
            /// <returns>Русское представление</returns>
            public static string GetNameRusRow(string Name)
            {
                var objectLocalization = JsonConvert.DeserializeObject<Json.Localizations.Rootobject>(System.IO.File.ReadAllText("rus\\NameRows.json"));
                foreach(var v in  objectLocalization.Rus) {
                    if (Name.ToUpper() == v.Row.ToUpper())
                        return v.Text;
                }
                return null;
            }
        }
    }
}
