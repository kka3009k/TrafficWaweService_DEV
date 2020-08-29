using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using TrafficWaveService.Reports.Data;
using TrafficWaveService.Reports.Utils;

namespace TrafficWaveService.Reports.TempController
{
    public class MainController
    {
        public string createReportDocx(Dictionary<string, object> data, List<DocumentCommand> commands)
        {
            string tmplname = data["TEMPLATE_FILE_NAME"].ToString();
            string tempWorkPath = copyTemplate(tmplname, true);

            using (var document = WordprocessingDocument.Open(tempWorkPath, true))
            {
                foreach (string key in data.Keys)
                {
                    try
                    {
                        SearchAndReplacer.SearchAndReplace(document, key, castTo<string>(data[key]), false);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                if (commands != null && commands.Count() > 0)
                {
                    try
                    {
                        foreach (DocumentCommand command in commands)
                        {
                            command.document = document;
                            command.Execute();
                        }
                    }
                    catch (Exception ex)
                    {
                        new DataBase().WriteLog(ex, "Run");
                    }
                }

                document.MainDocumentPart.Document.InnerXml = document.MainDocumentPart.Document.InnerXml.Replace("#BR", "<w:br/>");

                document.MainDocumentPart.Document.Save();
                document.Close();
            }
            Converter con = new Converter(createTempWorkPath(),tmplname);
            byte[] outputBytes = con.ConvertToPdf();
            File.Delete(tempWorkPath);
            File.Delete(createTempWorkPath() + "output.pdf");
            return Convert.ToBase64String(outputBytes);
        }

        public static string copyTemplate(string fileName, bool overwrite)
        {
            string templateFilePath = HostingEnvironment.MapPath("~/Reports/Templates/") + fileName;
            string tempWorkPath = createTempWorkFilePath(fileName);

            File.Copy(templateFilePath, tempWorkPath, overwrite);
            return tempWorkPath;
        }

        public static string createTempWorkPath()
        {
            return HostingEnvironment.MapPath("~/Reports/Templates/Temp/");
        }

        public static string createTempWorkFilePath(string fileName)
        {
            return HostingEnvironment.MapPath("~/Reports/Templates/Temp/") + fileName;
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

        public List<DocumentCommand> createComands(Dictionary<string, object> data)
        {
            List<DocumentCommand> commands = new List<DocumentCommand>();
            JArray array = (JArray)data["products"];
            List<Product> products = array.ToObject<List<Product>>();

                TableData<Product> tabData1 = new TableData<Product>
                {
                    caption = "Таблица 1",
                    data = products,
                    fields = new string[] { "name_product", "description_product", "date_production", "price_product"},
                    isNumbered = true
                };

                CopyReplaceCommand cCommand2 = new CopyReplaceCommand();
                cCommand2.replaceData = data;
                cCommand2.afterBookMark = "GrPg";
                cCommand2.sourceFileName = "act_products_tab.docx";

                FillTablesCommand fCommand = new FillTablesCommand();
                fCommand.afterRow = 1;
                fCommand.addTableData(tabData1);
                fCommand.addParagraphProperties(new ParagraphProperties
                {
                    Justification = new Justification { Val = JustificationValues.Center }
                });

                cCommand2.addInnerCoomand(fCommand);
                commands.Add(cCommand2);

            return commands;
        }

       // public void createObjects
    }
}