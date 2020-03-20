using System;
using System.Linq;
using System.Threading.Tasks;

namespace TrafficWaveService.Factory.SearchFactory
{
    /// <summary>
    /// Класс подозрительных лиц
    /// работает в основном с таблицей reg
    /// </summary>
    public class SearchCriminals : ISearch
    {
        /// <summary>
        /// Объект для передачи параметров поиска
        /// </summary>
        private SearchQuery _SearchQuery { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public SearchCriminals() { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="pSearchQuery">Параметры запроса</param>
        public SearchCriminals(SearchQuery pSearchQuery)
        {
            _SearchQuery = pSearchQuery;
        }

        /// <summary>
        /// Запуск поиска
        /// </summary>
        /// <returns></returns>
        public Task<SearchQuery> Run()
        {
            return Task.Run(() =>
            {
                SearchQuery res = new SearchQuery();
                res.Error = new Result() { Code = 0, Message = "OK" };
                try
                {
                    string firstName = _SearchQuery.FirstName;
                    string lastName = _SearchQuery.LastName;
                    string secondName = _SearchQuery.SecondName;
                    using (bankasiaNSEntities db = new bankasiaNSEntities())
                    {
                        var criminals = db.SearchReg(firstName, lastName, secondName, 2).ToList();
                        if (criminals.Count() > 0)
                        {
                            res = _SearchQuery;
                            res.Error.Code = 1;
                            res.Error.Message = "Client has in criminals list";
                            new DataBase().WriteInOnlineWindow(_SearchQuery, criminals);
                        }
                    }
                }
                catch (Exception ex)
                {
                    new DataBase().WriteLog(ex, "Run");
                    res.Error.Code = 500;
                    res.Error.Message = "Inner error";
                }
                return res;
            });
        }

        /// <summary>
        /// Проверка статуса принят или отказано
        /// </summary>
        /// <returns></returns>
        public Task<Result> Check(long pQueryId)
        {
            return Task.Run(() =>
            {
                Result res = new Result() { Code = 0, Message = "OK" };
                try
                {
                    var owRes = new DataBase().GetOnlineWindows(pQueryId);
                    if(owRes.Where(w=>w.ow_check == null).Count() > 0)
                    {
                        res.Code = 2;
                        res.Message = "Processed";
                    }
                    if (owRes.Where(w => w.ow_check == 1).Count() > 0)
                    {
                        res.Code = 3;
                        res.Message = "Accepted";
                    }
                    if (owRes.Where(w => w.ow_check == 2).Count() > 0)
                    {
                        res.Code = 4;
                        res.Message = "Not Accepted";
                    }
                }
                catch (Exception ex)
                {
                    new DataBase().WriteLog(ex, "Check");
                    res.Code = 500;
                    res.Message = "Inner error";
                }
                return res;
            });
        }
    }
}