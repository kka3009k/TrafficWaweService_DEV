using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Wordprocessing;

namespace TrafficWaveService.Reports.Utils
{
    public abstract class DocumentTableCommand : DocumentCommand
    {
        /// <summary>
        /// Параметры ячеек для столбцов. Если добавить только одну,
        /// то параметр применится для всех столбцов
        /// </summary>
        public List<TableCellProperties> cellProperties = new List<TableCellProperties>();

        public void AddTableCellProperty(params TableCellProperties[] tCellProp)
        {
            if (tCellProp != null && tCellProp.Count() > 0)
                cellProperties.AddRange(tCellProp);
        }

        /// <summary>
        /// Параметры ячеек для столбцов. Если добавить только одну,
        /// то параметр применится для всех столбцов
        /// </summary>
        public List<ParagraphProperties> cellPragraphProperties = new List<ParagraphProperties>();

        public void addParagraphProperties(params ParagraphProperties[] tCellPhProp)
        {
            if (tCellPhProp != null && tCellPhProp.Count() > 0)
                cellPragraphProperties.AddRange(tCellPhProp);
        }
    }
}