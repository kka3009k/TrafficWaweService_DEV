using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrafficWaveService.Client
{
    /// <summary>
    /// Класс подозрительных лиц
    /// работает в основном с таблицей reg
    /// </summary>
    public class ClientMain
    {
        /// <summary>
        /// Объект для передачи параметров запроса
        /// </summary>
        private ClientQuery _ClientQuery { get; set; }

        /// <summary>
        /// Информация о клиенте
        /// </summary>
        private ClientInfo _clientInfo { get; set; }

        Dictionary<string, object> _dopInfo;

       

        /// <summary>
        /// Конструктор
        /// </summary>
        public ClientMain() { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="pClientQuery">Параметры запроса</param>
        public ClientMain(ClientQuery pClientQuery)
        {
            _ClientQuery = pClientQuery;
            Init();
        }


        private void Init()
        {
            try
            {
                //Десериализация строки
                string str = _ClientQuery.RequestStringClient.Replace("None", "null").Replace("False", "false").Replace("True", "true");
                _clientInfo = JsonConvert.DeserializeObject<ClientInfo>(str);

                string strDop = _ClientQuery.RequestStringDopInfo.Replace("None", "null").Replace("False", "false").Replace("True", "true");
                _dopInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(strDop);
            }
            catch(Exception ex)
            {
                new DataBase().WriteLog(ex, "CreateClient");
            }
        }

        /// <summary>
        /// Добавление или обновление информации о клиенте
        /// </summary>
        /// <returns></returns>
        public Task<Result> Run()
        {
            return Task.Run(() =>
            {
                Result res = new Result();
                try
                {
                    res.Code = AddOrUpdateClient();
                    res.Message = "Клиент создан";
                }
                catch (Exception ex)
                {
                    new DataBase().WriteLog(ex, "Run");
                    res.Code = 500;
                    res.Message = ex.Message;
                }
                return res;
            });
        }

      

        /// <summary>
        /// Метод для добавления или обновления клиента
        /// </summary>
        /// <returns></returns>
        private int AddOrUpdateClient()
        { 
            using (bankasiaNSEntities db = new bankasiaNSEntities())
            {
                clients cl = db.clients.FirstOrDefault(x => x.kl_inn.Trim() == _clientInfo.inn.Trim());
                if (cl == null)
                {
                    ClientNew clNew = new ClientNew(_clientInfo, _dopInfo);
                    return clNew.Add();
                }
                else
                {
                    ClientUpdate clUpdate = new ClientUpdate(_clientInfo, cl.kl_kod, 0);
                    return clUpdate.Update();
                }
            }
        }
       

    }
}