using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using TrafficWaveService.Models;

namespace TrafficWaveService.Client
{
    public class ClientReport
    {
        /// <summary>
        /// id клиента в ОДБ
        /// </summary>
        private int _ClientId = 0;

        /// <summary>
        /// id исполнителя в ОДБ
        /// </summary>
        private int _OtvId = 0;

        /// <summary>
        /// Конструктор
        /// </summary>
        public ClientReport() { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="pClientQuery">Параметры запроса</param>
        public ClientReport(int pClientId, int pOtvId)
        {
            _ClientId = pClientId;
            _OtvId = pOtvId;
            Init();
        }


        private void Init()
        {
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback((s, ce, ch, ssl) => true);
        }

        /// <summary>
        /// Метод получения анкеты клиента
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string,object>> GetClientProfile()
        {
            return await Task.Run(() => FormClientProfile());
        }

        private Dictionary<string, object> FormClientProfile()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("status", false);
            try
            {
                ReportServiceRef.ReportServiceClient client = new ReportServiceRef.ReportServiceClient();
                client.ClientCredentials.UserName.UserName = AuthData.UserNameService;
                client.ClientCredentials.UserName.Password = AuthData.PasswordService;
                ReportServiceRef.XLSDownload xls = client.od320_anketa_fiz(_ClientId, _OtvId, 320);
                byte[] bytes = xls.XLSFile;
                dict.Add("data", Convert.ToBase64String(bytes));
                dict["status"] = true;
            }
            catch (Exception ex)
            {
                new DataBase().WriteLog(ex, $"GetAnketa");
            }
            return dict;
        }
    }
}