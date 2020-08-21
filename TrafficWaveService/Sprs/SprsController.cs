using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TrafficWaveService.Sprs
{
    public class SprsController
    {
        /// <summary>
        /// Объект для передачи параметров запроса
        /// </summary>
        private SprQuery _SprQuery { get; set; }
        Dictionary<string, object> data;

        /// <summary>
        /// Конструктор
        /// </summary>
        public SprsController() { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="pClientQuery">Параметры запроса</param>
        public SprsController(SprQuery pSprQuery)
        {
            _SprQuery = pSprQuery;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> Run()
        {
            return await Task.Run(() => GetSprList());
        }

        private string GetSprList()
        {
            bankasiaNSEntities db = new bankasiaNSEntities();
            string sprs_json = "";
            try
            {
                switch(_SprQuery.sprType)
                {
                    case 1:
                        sprs_json = JsonConvert.SerializeObject(
                            db.spr.Where(x => x.s_tip == 17).ToList<object>());

                        break;
                    case 2:
                        sprs_json = JsonConvert.SerializeObject(
                            db.spr.Where(x => x.s_tip == 18).ToList<object>());
                        break;
                    case 3:
                        sprs_json = JsonConvert.SerializeObject(
                            db.spr.Where(x => x.s_tip == 5).ToList<object>());
                        break;
                    case 4:
                        sprs_json = JsonConvert.SerializeObject(
                            db.klient_p482_spr.Where(x=>(bool)x.IsActive).OrderBy(x=>x.p482.Trim()).ToList<object>());
                        break;

                }
            }
            catch (Exception ex)
            {
                new DataBase().WriteLog(ex, "Run");

            }
            return sprs_json;
        }

    }
}