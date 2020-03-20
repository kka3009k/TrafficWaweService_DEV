using System;
using System.Collections.Generic;
using System.Linq;
using TrafficWaveService.Factory.SearchFactory;

namespace TrafficWaveService
{
    /// <summary>
    /// Класс работает с базой данных
    /// </summary>
    public class DataBase
    {
        /// <summary>
        /// Логирование сохраняет ошибки в базу
        /// </summary>
        /// <param name="pEx">Объект</param>
        /// <param name="pMethod">Названия метода</param>
        /// <returns></returns>
        internal Result WriteLog(Exception pEx, string pMethod = "")
        {
            Result res = new Result() { Code = 0, Message = "" };
            try
            {
                using (bankasiaNSEntities db = new bankasiaNSEntities())
                {
                    string errorMsg = pEx != null && pEx.Message != null ? pEx.Message : "";
                    string innerErrorMsg = pEx != null && pEx.InnerException != null ? pEx.InnerException.Message : "";

                    LOG_APP log = new LOG_APP();
                    log.formNum = 0;
                    log.errorMsg = errorMsg;
                    log.innerMsg = innerErrorMsg;
                    log.method = "TrafficWaveService - " + pMethod;
                    log.dateLog = DateTime.Now;
                    db.LOG_APP.Add(log);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string errMsg = 
                    ex.InnerException?.Message ?? ex.Message;
                res.Code = 500;
                res.Message = "Internal error";
            }
            return res;
        }

        /// <summary>
        /// Для записи в онлайн монитор
        /// </summary>
        /// <param name="pSearchQuery">Параметры запроса</param>
        /// <param name="pSearchRegs">Результат поиска</param>
        /// <returns></returns>
        internal Result WriteInOnlineWindow(SearchQuery pSearchQuery, List<SearchReg_Result> pSearchRegs)
        {
            Result res = new Result() { Code = 0, Message = "" };
            try
            {
                using (bankasiaNSEntities db = new bankasiaNSEntities())
                {
                    string name = pSearchQuery?.FirstName ?? "";
                    name += " " + pSearchQuery?.LastName ?? "";
                    name += " " + pSearchQuery?.SecondName ?? "";
                    foreach (var reg in pSearchRegs)
                    {
                        ow ow = new ow();
                        ow.ow_modul = 1;
                        ow.ow_type = 1;
                        ow.ow_operation_id = (int)pSearchQuery.QueryId;
                        ow.ow_object_id = reg.id;
                        ow.ow_name = name.Trim();
                        ow.ow_otv_oper = -1;
                        ow.ow_check = 0;
                        ow.ow_open = 0;
                        ow.ow_edit = 1;
                        ow.ow_date_time = DateTime.Now;
                        ow.ow_s_date = DateTime.Now.Date;
                        if (ow != null)
                            db.ow.Add(ow);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                string errMsg =
                    ex.InnerException?.Message ?? ex.Message;
                res.Code = 500;
                res.Message = "Internal error";
            }
            return res;
        }

        /// <summary>
        /// Получает список данных из оналйн манитора
        /// </summary>
        /// <param name="pQueryId"></param>
        /// <returns></returns>
        internal List<ow> GetOnlineWindows(long pQueryId)
        {
            List<ow> res = new List<ow>();
            try
            {
                using (bankasiaNSEntities db = new bankasiaNSEntities())
                {
                    res = db.ow.Where(w => w.ow_operation_id == pQueryId).ToList();
                }
            }
            catch (Exception ex)
            {
                string errMsg =
                    ex.InnerException?.Message ?? ex.Message;
            }
            return res;
        }
    }
}