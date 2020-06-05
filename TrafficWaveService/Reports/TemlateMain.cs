using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Threading.Tasks;

using TrafficWaveService.Reports.TempController;

namespace TrafficWaveService.Reports
{
    public class TemlateMain
    {

        /// <summary>
        /// Объект для передачи параметров запроса
        /// </summary>
        private TemplateQuery _TemplateQuery { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public TemlateMain() { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="pClientQuery">Параметры запроса</param>
        public TemlateMain(TemplateQuery pTemlateMain)
        {
            _TemplateQuery = pTemlateMain;
        }


       


        /// <summary>
        /// Формирование шаблона
        /// </summary>
        /// <returns></returns>
        public async Task<ResultBase64> Run()
        {
            return await Task.Run(() => GetTemplate());
        }

        private ResultBase64 GetTemplate()
        {
            ResultBase64 res = new ResultBase64();
            try
            {
                PdfFormController pdfFormController = new PdfFormController(_TemplateQuery.RequestStringMetaData);
                res.ResultPdf = pdfFormController.GetTemplate();
            }
            catch (Exception ex)
            {
                res.ResultPdf = ex.Message;
                new DataBase().WriteLog(ex, "Run");

            }
            return res;
           

        }
               
        

    }
}