using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace TrafficWaveService.Reports.Utils
{
    public class Util
    {
        /// <summary>
        /// Форматирует ставляя пробел после каждого третьего символа с конца
        /// и округляя цисло после запятой в два знака.
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public static string toBeautyString(object source)
        {
            string res = "0";
            decimal sourceDec = 0;
            try
            {
                if (source == null) return res;
                sourceDec = Convert.ToDecimal(source);
            }
            catch
            {
                return res;
            }

            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            res = sourceDec.ToString("#,0.00", nfi);

            if (res.Contains("."))
            {
                //if (res.Split('.')[1] == "00")
                //res = res.Split('.')[0];
                res = res.Replace(".", ",");
            }
            return res;
        }

        public static string parseDateTime(string pattern, DateTime? dTime)
        {
            string txtDate = dTime == null || dTime.Value == null ? "" : dTime.Value.ToString(pattern);
            return txtDate;
        }

        /// <summary>
        /// Возвращает сумму заданных полей если
        /// поле числовой тип
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="cellName"></param>
        /// <returns></returns>
        public static decimal getSummOfCell<T>(IEnumerable<T> data, string cellName)
        {
            decimal res = 0;
            foreach (T t in data)
            {
                res += findField<decimal, T>(t, cellName);
            }
            return res;
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

        public static string createTempWorkFilePath(string fileName)
        {
            return HostingEnvironment.MapPath("~/Reports/Templates/Temp/") + fileName;
        }

        public class ExelCellHeader
        {
            public string Name { set; get; }
            public ExelCellBoolConverter Converter { set; get; }
            public Type ExpectedType { set; get; }
        }

        /// <summary>
        /// Convert boolean to string
        /// </summary>
        public class ExelCellBoolConverter
        {
            public string TrueValue { set; get; }
            public string FalseValue { set; get; }
            public object GetString(object bVal)
            {
                if (bVal is bool)
                    return (bool)bVal ? TrueValue : FalseValue;
                return bVal;
            }
        }

        #region Класс содержащий методы формирования отчета по защите дипоза 3-7 XML
        /// <summary>
        /// Класс с набором методов для формирования отчета по приложению 3-7
        /// </summary>
        public class od205260XmlMethApp
        {
            /// <summary>
            /// Значения тегов для Application 5
            /// </summary>
            public static DetailSettings[] dSettings = new DetailSettings[]
            {
            new DetailSettings("ROW_TOTAL", 1, "sum"),
            new DetailSettings("ROW_YUR_TOTAL", 3, "sum"),
            new DetailSettings("ROW_YUR_DEMAND_DEP", 3, "sum1"),
            new DetailSettings("ROW_YUR_TERM_DEP", 3, "sum2"),
            new DetailSettings("ROW_FIZ_TOTAL", 2, "sum"),
            new DetailSettings("ROW_FIZ_DEMAND_DEP", 2, "sum1"),
            new DetailSettings("ROW_FIZ_TERM_DEP", 2, "sum2"),
            new DetailSettings("ROW_IP_TOTAL", 4, "sum"),
            new DetailSettings("ROW_IP_DEMAND_DEP", 4, "sum1"),
            new DetailSettings("ROW_IP_TERM_DEP", 4, "sum2"),
            new DetailSettings("ROW_ORG_TOTAL", 5, "sum"),
            new DetailSettings("ROW_ORG_DEMAND_DEP", 5, "sum1"),
            new DetailSettings("ROW_ORG_TERM_DEP", 5, "sum2"),
            new DetailSettings("ROW_BANK_TOTAL", 6, "sum"),
            new DetailSettings("ROW_BANK_DEMAND_DEP", 6, "sum1"),
            new DetailSettings("ROW_BANK_TERM_DEP", 6, "sum2"),
            new DetailSettings("ROW_FKU_TOTAL", 7, "sum"),
            new DetailSettings("ROW_FKU_DEMAND_DEP", 7, "sum1"),
            new DetailSettings("ROW_FKU_TERM_DEP", 7, "sum2")
            };

            /// <summary>
            /// Значения тегов для Application 7
            /// </summary>
            public static DetailSettings[] dSetting =
            {
            new DetailSettings("ROW_YUR_DEMAND_KGS", 1, "sum"),
            new DetailSettings("ROW_YUR_DEMAND_NOT_KGS", 1, "sumi"),
            new DetailSettings("ROW_YUR_TERM_KGS", 2, "sum"),
            new DetailSettings("ROW_YUR_TERM_NOT_KGS", 2, "sumi"),
            new DetailSettings("ROW_FIZ_DEMAND_KGS", 3, "sum"),
            new DetailSettings("ROW_FIZ_DEMAND_NOT_KGS", 3, "sumi"),
            new DetailSettings("ROW_FIZ_TERM_KGS", 4, "sum"),
            new DetailSettings("ROW_FIZ_TERM_NOT_KGS", 4, "sumi"),
            new DetailSettings("ROW_IP_DEMAND_KGS", 5, "sum"),
            new DetailSettings("ROW_IP_DEMAND_NOT_KGS", 5, "sumi"),
            new DetailSettings("ROW_IP_TERM_KGS", 6, "sum"),
            new DetailSettings("ROW_IP_TERM_NOT_KGS", 6, "sumi"),
            new DetailSettings("ROW_ORG_DEMAND_KGS", 7, "sum"),
            new DetailSettings("ROW_ORG_DEMAND_NOT_KGS", 7, "sumi"),
        };

            /// <summary>
            /// Балансовые счета
            /// </summary>
            public static short[] AccBall = {
            20002,20003,20004,20005,
            20006,20011,20012,20013,
            20014,20015,20016,20201,
            20202,20203,20204,20205,
            20301,20302,20303,20309,
            20213,20214,20502,20512,
            20513
        };

            /// <summary>
            /// Метод формирующий теги
            /// </summary>
            /// <param name="pStringBuilder">Оъект класса StringBuilder </param>
            /// <param name="pDetailName"> Имя тега</param>
            /// <param name="pValue"> Значение </param>
            /// <param name="pRound">Степень округления </param>
            public static void appendDetail(StringBuilder pStringBuilder, string pDetailName, decimal? pValue, int pRound)
            {
                if (pStringBuilder != null)
                {
                    pStringBuilder.AppendLine("<Detail>");
                    pStringBuilder.AppendLine($"<DetailName>{pDetailName}</DetailName>");
                    pStringBuilder.AppendLine(String.Format("<Value >{0}</Value>", formatRoundWithDot(pValue, pRound)));
                    pStringBuilder.AppendLine("</Detail>");
                }
            }

            /// <summary>
            ///Проверка на null
            /// </summary>
            /// <param name="pValue">Значение </param>
            /// <param name="pRound"> на сколько округлить</param>
            /// <returns></returns>
            public static string formatRoundWithDot(decimal? pValue, int pRound = 2)
            {
                if (pValue != null)
                {
                    return pRound == 0 ? ((double)pValue).ToString().Replace(',', '.') : Math.Round((double)pValue, 2).ToString().Replace(',', '.');
                }
                else
                {
                    return "0";
                }
            }

            /// <summary>
            /// Метод для суммирования столбцов
            /// </summary>
            /// <param name="pBallanceData"> Массив коллекции </param>
            /// <param name="pAtribyte"> имя атрибута</param>
            /// <returns></returns>
            public static decimal GetSum(object[] pBallanceData, string pAtribyte)
            {
                decimal res = 0;
                foreach (object Search in pBallanceData)
                {
                    if (Search != null)
                    {
                        res += GetDecimalValue(Search, pAtribyte);
                    }
                }

                return res;
            }

            /// <summary>
            /// Метод осуществляющий суммирование столбцов по заданному атрибуту
            /// </summary>
            /// <param name="pDataBallanc"> Объект коллекции</param>
            /// <param name="pAtributeName">Имя атрибута</param>
            /// <returns></returns>
            public static decimal GetDecimalValue(object pDataBallanc, string pAtributeName)
            {
                decimal res = 0;
                PropertyInfo Propertys = pDataBallanc.GetType().GetProperty(pAtributeName);
                if (Propertys == null)
                {
                    throw new ArgumentNullException($"Property by name \"{pAtributeName}\" not found");
                }
                try
                {
                    res = (decimal)Propertys.GetValue(pDataBallanc, null);
                }
                catch
                {
                }

                return res;
            }


            /// <summary>
            ///  Метод для получения значения определенной ячейки столбца
            /// </summary>
            /// <param name="pDataBallanc"> объект таблица с номером</param>
            /// <param name="pAtributeName"> Имя атрибута</param>
            /// <returns>Возвращаем значение атрибута </returns>
            private static int GetInt32Value(object pDataBallanc, string pAtributeName)
            {
                int res = 0;
                PropertyInfo Propertys = pDataBallanc.GetType().GetProperty(pAtributeName);
                if (Propertys == null)
                {
                    throw new ArgumentNullException($"Property by name \"{pAtributeName}\" not found");
                }

                try
                {
                    res = (int)Propertys.GetValue(pDataBallanc, null);
                }
                catch
                {
                }

                return res;
            }



            /// <summary>
            /// Класс с данными о  Application5 и Application 7 
            /// </summary>
            public class DetailSettings
            {
                /// <summary>
                /// Имена тегов
                /// </summary>
                public string Name { set; get; }

                /// <summary>
                /// Индекс 
                /// </summary>
                public int Index { set; get; }

                /// <summary>
                /// Имя Атрибута
                /// </summary>
                public string Arg { set; get; }

                public DetailSettings(string name, int index, string argName)
                {
                    this.Name = name;
                    this.Index = index;
                    this.Arg = argName;
                }
            }
            #endregion

            /// <summary>
            /// Фрмат строки для матричного принтера
            /// </summary>
            /// <param name="komment">Назначение проводки Номер параметра в string.Format=0</param>
            /// <param name="lineFormat">Формат строки</param>
            /// <param name="twoLineFormat">Формат строки для пустых строк кроме назначения</param>
            /// <param name="lineBreak">Длина строки назначения для перехода строки на новую строки</param>
            /// <param name="arg">Массив строк до 10 параметров</param>
            /// <returns></returns>
            public static string GetBreakLine(string komment, string lineFormat, string twoLineFormat, int lineBreak, string[] arg)
            {
                string res = "";
                int itm = 0;
                if (komment.Length > lineBreak)
                {
                    while (komment.Length > lineBreak)
                    {
                        if (itm == 0)
                            res += string.Format("" + lineFormat + "\n", komment.Substring(0, lineBreak), arg.Length >= 1 ? arg[0] : "", arg.Length >= 2 ? arg[1] : "", arg.Length >= 3 ? arg[2] : "",
                                arg.Length >= 4 ? arg[3] : "", arg.Length >= 5 ? arg[4] : "",
                                arg.Length >= 6 ? arg[5] : "", arg.Length >= 7 ? arg[6] : "", arg.Length >= 8 ? arg[7] : "", arg.Length >= 9 ? arg[8] : "", arg.Length >= 10 ? arg[9] : "");
                        else
                            res += string.Format("" + twoLineFormat + "\n", "", komment.Substring(0, lineBreak));
                        komment = komment.Remove(0, lineBreak);
                        itm++;
                    }
                    if (!string.IsNullOrEmpty(komment) || string.IsNullOrWhiteSpace(komment))
                        res += (string.Format("" + twoLineFormat + "\n", "", komment));
                }
                else
                {
                    res += string.Format("" + lineFormat + "\n", komment, arg.Length >= 1 ? arg[0] : "", arg.Length >= 2 ? arg[1] : "", arg.Length >= 3 ? arg[2] : "",
                                arg.Length >= 4 ? arg[3] : "", arg.Length >= 5 ? arg[4] : "",
                                arg.Length >= 6 ? arg[5] : "", arg.Length >= 7 ? arg[6] : "", arg.Length >= 8 ? arg[7] : "", arg.Length >= 9 ? arg[8] : "", arg.Length >= 10 ? arg[9] : "");
                }
                return res;
            }

            public static string copyTemplate(string fileName, bool overwrite)
            {
                string templateFilePath = HostingEnvironment.MapPath("~/Reports/Templates/") + fileName;
                string tempWorkPath = createTempWorkFilePath(fileName);

                File.Copy(templateFilePath, tempWorkPath, overwrite);
                return tempWorkPath;
            }

            /// <summary>
            /// Возврать байтов файла
            /// </summary>
            /// <param name="filePath">путь к файлу</param>
            /// <returns>байты файла</returns>
            public static byte[] getBytes(string filePath)
            {
                MemoryStream OutStream = new MemoryStream();
                byte[] outputBytes = new byte[1];
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    {
                        fs.CopyTo(OutStream);
                        BinaryReader reader = null;
                        outputBytes = new byte[(int)OutStream.Length];
                        reader = new BinaryReader(OutStream, Encoding.Unicode);
                        reader.BaseStream.Position = 0;
                        outputBytes = reader.ReadBytes((int)OutStream.Length);
                        reader.Close();
                        fs.Close();
                    }
                }
                catch (Exception ex)
                {

                }
                OutStream.Close();
                return outputBytes;
            }
        }
    }
}
