using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace TrafficWaveService.Reports.TempController
{
    public class MainController
    {
        public string createReportDocx(Dictionary<string, object> data)
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
    }
}