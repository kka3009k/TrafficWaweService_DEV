using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrafficWaveService.Reports.Utils
{
    public abstract class TableData
    {
        public abstract Type type { get; set; }
        public string caption { get; set; }
        public string[] head { get; set; }
        /// <summary>
        /// Имена полей которые Entity объекта,
        /// которые должны быть вставлены в Word таблицу
        ///  - Если есть '@' в начале строки то
        ///    полученные данные данного столбца будут приведены в читабельный вид
        ///   <see cref="DocumentUtils.fillTable{T}
        ///   (DocumentFormat.OpenXml.Wordprocessing.Table, TableData{T}, int, 
        ///   DocumentFormat.OpenXml.Wordprocessing.TableCellProperties[], 
        ///   DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties[])"/>
        /// </summary>
        public string[] fields { get; set; }
        public string[] bottom { get; set; }
        public bool isNumbered { get; set; }
    }

    /// <summary>
    /// Класс для данных таблицы
    /// </summary>
    /// <typeparam name="T">Тип с которого копируются данные таблицы</typeparam>
    public class TableData<T> : TableData
    {
        public override Type type
        {
            get
            {
                return typeof(T);
            }
            set
            {
                type = value;
            }
        }
        public List<T> data { get; set; }
    }
}