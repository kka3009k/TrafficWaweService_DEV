using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
namespace TrafficWaveService.Reports.TempController
{
    /// <summary>
    /// Контроллер для создания шаблона согласия на сбор персональных данных
    /// </summary>
    public class PdfFormController
    { 
        MainController _mainController;
        Dictionary<string, object> data;
        /// <summary>
        /// Объект для передачи параметров запроса
        /// </summary>
        private string _requestString { get; set; }

        public PdfFormController(string pReqStr)
        {
            _requestString = pReqStr;
            Init();
        }

        private void Init()
        {
            data = new Dictionary<string, object>();
            //Десериализация строки
            string str = _requestString.Replace("None", "null").Replace("False", "false").Replace("True", "true");
            data = JsonConvert.DeserializeObject<Dictionary<string, object>>(str);
        }

        public string GetTemplate()
        {
            _mainController = new MainController();

            return _mainController.createReportDocx(data);

        }

    }
}