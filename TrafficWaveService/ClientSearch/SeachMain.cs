using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TrafficWaveService.ClientSearch
{
    public class SeachMain
    {

        /// <summary>
        /// Объект для передачи параметров запроса
        /// </summary>
        private SearchClientQuery _SearchQuery { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public SeachMain() { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="pClientQuery">Параметры запроса</param>
        public SeachMain(SearchClientQuery pSearchQuery)
        {
            _SearchQuery = pSearchQuery;
        }





        /// <summary>
        /// Формирование шаблона
        /// </summary>
        /// <returns></returns>
        public async Task<ResultData> Run()
        {
            return await Task.Run(() => GetTemplate());
        }

        private ResultData GetTemplate()
        {
            ResultData res = new ResultData();
            try
            {
                string str = JsonConvert.SerializeObject(Search());
                res.DataClient = str;
               
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                new DataBase().WriteLog(ex, "Run");

            }
            return res;
        }


        private DataClient Search ()
        {
            DataClient data = new DataClient();
            bankasiaNSEntities db = new bankasiaNSEntities();
            clients client = db.clients.FirstOrDefault(x => x.kl_inn.Trim() == _SearchQuery.INN.Trim());
            if(client == null)
            {
                data.check_result = "NONE";
            }
            else
            {
                int clCode = client.kl_kod;
                data.clients = client;
                data.client_adress = db.client_adress.Where(x => x.kl_kod == clCode).ToList().Count != 0 ? db.client_adress.Where(x => x.kl_kod == clCode).ToList(): null;
                data.client_paspdata = db.client_paspdata.FirstOrDefault(x => x.kl_kod == clCode) != null ? db.client_paspdata.FirstOrDefault(x => x.kl_kod == clCode): null;
                data.check_result = "OK";
            }
            return data;
        }

    }
}