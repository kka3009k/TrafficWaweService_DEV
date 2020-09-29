using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Web.Hosting;
using DocumentFormat.OpenXml;

namespace TrafficWaveService.Reports.Utils
{
    /// <summary>
    /// Абстрактный класс команда
    /// </summary>
    public abstract class DocumentCommand
    {
        public WordprocessingDocument document { set; get; }
        public abstract void Execute();
    }

    /// <summary>
    /// Класс комнда копирует содержание одного документа в второй
    /// </summary>
    public class CopyReplaceCommand : DocumentCommand
    {
        /// <summary>
        /// Данные для перезаписи в документе источник
        /// </summary>
        public Dictionary<string, object> replaceData { set; get; }
        /// <summary>
        /// Имя документа источника
        /// </summary>
        public string sourceFileName { set; get; }
        /// <summary>
        /// Имя закладки после которого будет записано содержание документа источника
        /// </summary>
        public string afterBookMark { set; get; }
        /// <summary>
        /// Удалить закладку после записи
        /// </summary>
        public bool removeBookMark { set; get; }
        /// <summary>
        /// Команды для документа источника
        /// </summary>
        private List<DocumentCommand> innerCommands = new List<DocumentCommand>();

        public CopyReplaceCommand() { }

        public CopyReplaceCommand(string sourceFileName, string afterBookMark)
        {
            this.sourceFileName = sourceFileName;
            this.afterBookMark = afterBookMark;
        }
        /// <summary>
        /// Добавить команду для выполнения к документу источнику
        /// </summary>
        /// <param name="innerCommand"></param>
        public void addInnerCoomand(DocumentCommand innerCommand)
        {
            innerCommands.Add(innerCommand);
        }

        /// <summary>
        /// Признак договоров КЗ
        /// </summary>
        /// <param name="pIsKr"></param>
        public override void Execute()
        {
            if (document != null && document.MainDocumentPart.Document.Body != null)
            {
                string sourceFilePath = HostingEnvironment.MapPath("~/Reports/Templates/") + sourceFileName;
                string sourceTempFilePath = Util.createTempWorkFilePath(sourceFileName);

                File.Copy(sourceFilePath, sourceTempFilePath, true);

                using (var sourceDocument = WordprocessingDocument.Open(sourceTempFilePath, true))
                {
                    foreach (string key in replaceData.Keys)
                    {
                        SearchAndReplacer.SearchAndReplace(sourceDocument, key, DocumentUtils.castTo<string>(replaceData[key]), false);
                    }
                    if (innerCommands.Count() > 0)
                        foreach (DocumentCommand innerCommand in innerCommands)
                        {
                            innerCommand.document = sourceDocument;
                            innerCommand.Execute();
                        }
                    sourceDocument.MainDocumentPart.Document.Save();
                    sourceDocument.Close();
                }

                string altChunkId = DocumentUtils.randomString(15);
                AlternativeFormatImportPart chunk =
                    document.MainDocumentPart.AddAlternativeFormatImportPart(
                    AlternativeFormatImportPartType.WordprocessingML, altChunkId);

                using (FileStream fileStream = File.Open(sourceTempFilePath, FileMode.Open))
                    chunk.FeedData(fileStream);
                AltChunk altChunk = new AltChunk();
                altChunk.Id = altChunkId;

                Paragraph ph = DocumentUtils.findParagraphByBookmark(afterBookMark, document.MainDocumentPart.Document.Body);
                if (ph != null)
                {
                    ph.InsertAfterSelf(altChunk);
                    if (removeBookMark) ph.Remove();
                }

                File.Delete(sourceTempFilePath);
                document.MainDocumentPart.Document.Save();
            }
        }
    }

    public class DocumentUtils
    {
        // INVOKED
        /// <summary>
        /// Заполняет таблицу данными из TabData.data
        /// </summary>
        /// <typeparam name="T">Тип обектов источников</typeparam>
        /// <param name="table">Имя таблицы в документе</param>
        /// <param name="tabData">Данные для таблицы</param>
        /// <param name="afterRow">Данные для таблицы</param>
        public static void fillTable<T>(Table table, TableData<T> tabData, int afterRow, TableCellProperties[] tCellProperties = null, ParagraphProperties[] paragraphProperties = null)
        {
            if (table != null && tabData != null && tabData.data != null && tabData.data.Count() > 0)
            {
                List<TableRow> leftRows = new List<DocumentFormat.OpenXml.Wordprocessing.TableRow>();
                List<TableRow> rows = table.Elements<TableRow>().ToList();
                if (afterRow > 0)
                {
                    for (int i = afterRow; i < rows.Count(); i++)
                    {
                        leftRows.Add((TableRow)rows.ElementAt(i).Clone());
                        rows.ElementAt(i).Remove();
                    }
                }
                appendRow(table, tabData.head, tCellProperties, paragraphProperties);
                for (int i = 0; i < tabData.data.Count(); i++)
                {
                    string[] cells = new string[tabData.fields.Count()];
                    int start = 0;
                    if (tabData.isNumbered)
                    {
                        cells = new string[tabData.fields.Count() + 1];
                        cells[0] = i.ToString();
                        start = 1;
                    }
                    for (int j = start; j < (tabData.isNumbered ? tabData.fields.Count() + 1 : tabData.fields.Count()); j++)
                    {
                        string fieldName = tabData.fields[tabData.isNumbered ? j - 1 : j];
                        bool isNeedBeautify = fieldName != null && fieldName[0].Equals('@');

                        fieldName = isNeedBeautify ? fieldName.Replace("@", "") : fieldName;

                        string fieldData = findField<string, T>(tabData.data.ElementAt(i), fieldName);

                        if (isNeedBeautify)
                        {
                            fieldData = Util.toBeautyString(fieldData);
                            fieldData = "0".Equals(fieldData) ? "" : fieldData.Equals("0,00") ? "" : fieldData;
                        }

                        cells[j] = fieldData;
                    }
                    appendRow(table, cells, tCellProperties, paragraphProperties);
                }
                foreach (TableRow tr in leftRows)
                    table.Append(tr);
                appendRow(table, tabData.bottom, tCellProperties, paragraphProperties);
            }
        }
        /// <summary>
        /// Добавление новой строки в таблицу
        /// </summary>
        /// <param name="table">Обект таблицы DocumentFormat.OpenXml.Wordprocessing.Table</param>
        /// <param name="values"></param>
        /// <param name="tCellProperties"></param>
        public static void appendRow(Table table, string[] values, TableCellProperties[] tCellProperties, ParagraphProperties[] parahraphProperties)
        {
            if (table == null || values == null) return;
            TableRow tr = new TableRow();
            for (int i = 0; i < values.Count(); i++)
            {
                TableCell tc = new TableCell();
                if (tCellProperties != null)
                {
                    if (tCellProperties.Length == 1 && tCellProperties[0] != null)
                        tc.Append((TableCellProperties)tCellProperties[0].Clone());
                    else if (tCellProperties.Length > i && tCellProperties[i] != null)
                        tc.Append((TableCellProperties)tCellProperties[i].Clone());
                }
                var run = new Run();
                var runProp = new RunProperties {
                    RunFonts = new RunFonts { Ascii = "Times New Roman" },
                    FontSize = new FontSize { Val = new StringValue("24") }
                };
                run.Append(runProp);
                run.Append(new Text(values[i]));
                Paragraph paragraph = new Paragraph(run);
                if (parahraphProperties != null)
                {
                    if (parahraphProperties.Length == 1 && parahraphProperties[0] != null)
                        tc.AppendChild<ParagraphProperties>((ParagraphProperties)parahraphProperties[0].Clone());
                    else if (parahraphProperties.Length > i && parahraphProperties[i] != null)
                        tc.AppendChild<ParagraphProperties>((ParagraphProperties)parahraphProperties[i].Clone());
                }
                tc.Append(paragraph);
                tr.Append(tc);
            }
            table.Append(tr);
        }

        /// <summary>
        /// Найти таблицу в теле документа по названию
        /// </summary>
        /// <param name="caption">Название таблицы</param>
        /// <param name="bd">Тело документа</param>
        /// <returns>Таблица по заданному названию</returns>
        public static Table findTableByCaption(string caption, Body bd)
        {
            IEnumerable<TableProperties> tableProperties = bd.Descendants<TableProperties>().Where(tp => tp.TableCaption != null);
            foreach (TableProperties tProp in tableProperties)
            {
                if (tProp.TableCaption.Val.ToString().Equals(caption))
                {
                    return (Table)tProp.Parent;
                }
            }
            return null;
        }

        /// <summary>
        /// Найти Pharagraph в теле документа по имени закладки
        /// </summary>
        /// <param name="bookMarkName">Имя закладки</param>
        /// <param name="bd"></param>
        /// <returns></returns>
        public static Paragraph findParagraphByBookmark(string bookMarkName, Body bd)
        {
            if (bd == null) return null;
            IEnumerable<Paragraph> parahgraphs = bd.Elements<Paragraph>();
            foreach (Paragraph ph in parahgraphs)
            {
                var bookMarkStarts = ph.Elements<BookmarkStart>();
                var bookMarkEnds = ph.Elements<BookmarkEnd>();
                foreach (BookmarkStart bStart in bookMarkStarts)
                {
                    if (bStart.Name == bookMarkName)
                    {
                        return ph;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Найти поле в объекте 
        /// </summary>
        /// <typeparam name="E">Тип поля</typeparam>
        /// <typeparam name="T">Тип обекта</typeparam>
        /// <param name="t"></param>
        /// <param name="fieldName"></param>
        /// <returns>Значение поля в указанном типе</returns>
        public static E findField<E, T>(T t, string fieldName)
        {
            try
            {
                object prop = typeof(T).GetProperty(fieldName).GetValue(t, null);
                if (prop is DateTime?)
                {
                    string ch = parseDateTime("dd.MM.yyyy", (DateTime?)prop);
                    return castTo<E>(ch);
                }
                if (prop is DateTime)
                {
                    string ch = parseDateTime("dd.MM.yyyy", (DateTime)prop);
                    return castTo<E>(ch);
                }

                return castTo<E>(prop);
            }
            catch
            {
                return default(E);
            }
        }

        public static string parseDateTime(string pattern, DateTime? dTime)
        {
            string txtDate = dTime == null || dTime.Value == null ? "" : dTime.Value.ToString(pattern);
            return txtDate;
        }

        public static T castTo<T>(object obj)
        {
            if (obj is T)
            {
                return (T)obj;
            }
            else
            {
                try
                {
                    return (T)Convert.ChangeType(obj, typeof(T));
                }
                catch
                {
                    return default(T);
                }
            }
        }

        private static Random random = new Random();
        /// <summary>
        /// Генерирует рандосную строку в заданном количестве
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string randomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    /// <summary>
    /// Команда заполняющая таблицу добавляя новый столбец
    /// </summary>
    public class AddTableCellCommand : DocumentTableCommand
    {
        private int cellsRestPercent = 55;

        public int CellsRestPercent
        {
            set { cellsRestPercent = value; }
            get { return cellsRestPercent; }
        }

        /// <summary>
        /// Количество возможных столбцов
        /// </summary>
        public int CellsCount { set; get; }

        /// <summary>
        /// Расчитывать ширину столбцов
        /// </summary>
        public bool CalculateCellWidth { set; get; }

        /// <summary>
        /// Данные столбца
        /// </summary>
        public List<object> cellsData { set; get; }
        /// <summary>
        /// Имя таблицы
        /// </summary>
        public string tableCaption { set; get; }
        /// <summary>
        /// Номер стартовой строки (начать добавлять после строки startFrom)
        /// </summary>
        public uint startFrom { set; get; }

        /// <summary>
        /// Ширина ячейки по умолчанию
        /// </summary>
        public string tableCellDefaultWidth = "14%";
        public override void Execute()
        {
            tableCellDefaultWidth = CalculateCellWidth ? (CellsRestPercent / CellsCount) + "%" : "14%";
            if (document != null)
            {
                Table table = DocumentUtils.findTableByCaption(tableCaption, document.MainDocumentPart.Document.Body);
                IEnumerable<TableRow> rows = table.Elements<TableRow>();
                int rowCount = cellsData.Count;
                if (startFrom <= rowCount)
                {
                    int cellDataIndex = 0;
                    for (int i = 0; i < rowCount; i++)
                    {
                        if (i >= startFrom)
                        {
                            TableCell tc = new TableCell();

                            if (cellProperties != null && cellProperties.Count() > 0)
                            {
                                if (cellProperties.Count() == 1 && cellProperties.ElementAtOrDefault(0) != null)
                                    tc.Append((TableCellProperties)cellProperties.ElementAtOrDefault(i).Clone());
                                else if (cellProperties.Count() > 1 && cellProperties.ElementAtOrDefault(i) != null)
                                    tc.Append((TableCellProperties)cellProperties.ElementAtOrDefault(0).Clone());
                            }
                            else
                            {
                                tc.Append(new TableCellProperties(new TableCellWidth()
                                {
                                    Width = tableCellDefaultWidth
                                }));
                            }

                            //object o = cellsData[cellDataIndex];
                            object o = cellDataIndex < cellsData.Count ? cellsData[cellDataIndex] : null;
                            Text text = o != null ? new Text(DocumentUtils.castTo<string>(o)) : new Text();
                            var run = new Run(text);
                            var runProp = new RunProperties();
                            var runFont = new RunFonts { Ascii = "Times New Roman" };
                            var size = new FontSize { Val = new StringValue("12") };
                            runProp.Append(runFont);
                            runProp.Append(size);
                            run.Append(runProp);
                            Paragraph paragraph = new Paragraph(run);
                            if (cellPragraphProperties != null && cellPragraphProperties.Count() > 0)
                            {
                                if (cellPragraphProperties.Count() > 1 && cellPragraphProperties.ElementAtOrDefault(i) != null)
                                    tc.AppendChild((ParagraphProperties)cellPragraphProperties.ElementAtOrDefault(i).Clone());
                                else if (cellPragraphProperties.Count() == 1 && cellPragraphProperties.ElementAtOrDefault(0) != null)
                                    tc.AppendChild((ParagraphProperties)cellPragraphProperties.ElementAtOrDefault(0).Clone());
                            }
                            tc.AppendChild(paragraph);
                            if (rows.ElementAt(i) != null)
                            {
                                rows.ElementAt(i).AppendChild(tc);
                            }
                            cellDataIndex++;
                        }
                    }
                }
            }

            document.MainDocumentPart.Document.Save();
        }
    }

    /// <summary>
    /// Команда заполняющая таблицы данными добавляя новую строку
    /// </summary>
    public class FillTablesCommand : DocumentTableCommand
    {
        /// <summary>
        /// Данные таблиц
        /// </summary>
        List<TableData> tableDatas = new List<TableData>();

        public int afterRow { set; get; }

        public void addTableData(TableData tData)
        {
            tableDatas.Add(tData);
        }

        public override void Execute()
        {
            if (document != null)
            {
                foreach (TableData tData in tableDatas)
                {
                    Table table = DocumentUtils.findTableByCaption(tData.caption, document.MainDocumentPart.Document.Body);

                    MethodInfo mInf = typeof(DocumentUtils).GetMethod("fillTable", BindingFlags.Public | BindingFlags.Static)
                        .MakeGenericMethod(new Type[] { tData.type });

                    mInf.Invoke(null, new object[] { table, tData, afterRow, cellProperties.ToArray(), cellPragraphProperties.ToArray() });
                }
                document.MainDocumentPart.Document.Save();
            }
        }
    }
}