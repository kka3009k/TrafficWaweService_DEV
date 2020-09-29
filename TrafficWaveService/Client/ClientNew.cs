using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrafficWaveService.Dictionaries;
namespace TrafficWaveService.Client
{
    /// <summary>
    /// Класс создания нового клиента
    /// </summary>
    public class ClientNew
    {
        public ClientInfo _clientInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private RuToLatin _Translate { get; set; }

        private Dictionary<string, object> _dopInfo;

        bankasiaNSEntities _db;
        /// <summary>
        /// Конструктор
        /// </summary>
        public ClientNew() { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="pClientInfo">Информация о клиенте</param>
        public ClientNew(ClientInfo pClientInfo, Dictionary<string, object> pDopInfo)
        {
            _clientInfo = pClientInfo;
            _dopInfo = pDopInfo;
            _Translate = new RuToLatin();
        }

        /// <summary>
        /// Добавление нового клиента
        /// </summary>
        /// <returns></returns>
        public int Add()
        {
            string res = "Error - Add";
            int ID = -1;
            try
            {
                ID = AddNewClient(_clientInfo);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                new DataBase().WriteLog(ex, "Run");
            }
            return ID;
        }


        /// <summary>
        /// Метод для добавления нового клиента - таблица clients
        /// </summary>
        /// <param name="pCl"></param>
        public int AddNewClient(ClientInfo pCl)
        {
            _db = new bankasiaNSEntities();
                using (var _dbContextTransaction = _db.Database.BeginTransaction())
                {
                try
                {
                    int cl_kod = GetClientId();
                    clients cl = new clients
                    {
                        kl_kod = cl_kod,
                        kl_inn = pCl.inn,
                        kl_datecreate = DateTime.Now,
                        kl_nam = pCl.last_name + " " + pCl.first_name + " " + pCl.patronymic,
                        kl_nam_eng = _Translate.Front(pCl.first_name + " " + pCl.last_name + " " + pCl.patronymic),
                        kl_tel1 = ParsPhone(pCl.contact_phone),
                        kl_tel2 = pCl.home_phone == null || pCl.home_phone == "" ? "" : pCl.home_phone,
                        kl_kodter = "01",
                        kl_relig = false,
                        kl_offsh = false,
                        KODB = 60,
                        kl_rchp = false,
                        kl_cfr = 1,
                        kl_stat = 2,
                        kl_rzd = 1,
                        kl_rpat = false,
                        kl_rip = false,
                        kl_vidsob = (byte)pCl.odb_property_type,
                        kl_benefs = true,
                        kl_benefsname = "",
                        kl_benefs_paspdata = "",
                        kl_tel3 = "",
                        kl_wdoljn = pCl.position != null ? pCl.position:"",
                        kl_wfio = "",
                        kl_wname = _dopInfo["work_adress"] != null && _dopInfo["work_adress"].ToString() != ""? _dopInfo["work_adress"].ToString():"",
                        kl_otvcreate = short.Parse(_dopInfo["otv_id"].ToString()),
                        kl_ins = false,
                        kl_pzl = false,
                        kl_dopvd = "",
                        kl_fatca = false,
                        kl_isys = "OD",
                        kl_otr = (short)pCl.odb_industry,
                        kl_osnvd = (short)pCl.odb_primary_occupation,
                        kl_keyw = "",

                    };
                    _db.clients.Add(cl);
                    _db.SaveChanges();
                    AddNewClientPaspData(pCl, cl.kl_kod);
                    _dbContextTransaction.Commit();
                    return cl.kl_kod;
                }
                catch (Exception ex)
                {
                    new DataBase().WriteLog(ex, "Run");
                    _dbContextTransaction.Rollback();
                    return -1;
                }
            }
        }

        /// <summary>
        /// Изменение формата телефона для ОДБ
        /// </summary>
        /// <param name="pNum"></param>
        /// <returns></returns>
        private string ParsPhone(string pNum)
        {
            string phone = "(996)" + pNum.Remove(0, 5).Remove(4);
            string num_1 = pNum.Remove(0, 9).Remove(2);
            string num_2 = "-" + pNum.Remove(0, 11).Remove(2) + "-";
            string num_3 = pNum.Remove(0, 13);
            return phone + num_1 + num_2 + num_3;
        }

        /// <summary>
        /// Добавление паспортных данных - таблица client_paspdata
        /// </summary>
        /// <param name="pCl"></param>
        /// <param name="pClCode"></param>
        void AddNewClientPaspData(ClientInfo pCl, int pClCode)
        {
            byte dlCode = 0;
            client_paspdata cl_pasp = new client_paspdata
            {
                kl_kod = pClCode,
                dl_kod = dlCode,
                p_inn = pCl.inn,
                p_fam = pCl.last_name,
                p_name = pCl.first_name,
                p_otch = pCl.patronymic,
                p_grajd = "417",
                p_viddoksfr = Convert.ToDateTime(pCl.passport_info.issued_date) >= DateTime.Parse("2017-01-01") ? "15" : "05",
                p_viddok = Convert.ToDateTime(pCl.passport_info.issued_date) >= DateTime.Parse("2017-01-01") ? "Идентификационная карта" : "Паспорт",
                p_srdok = pCl.passport_info.series,
                p_ndok = pCl.passport_info.number,
                p_mvd = pCl.passport_info.issued_by,
                p_enddok = Convert.ToDateTime(pCl.passport_info.deadline),
                p_datev = Convert.ToDateTime(pCl.passport_info.issued_date),
                p_noend = false,
                p_strb = "417",
                p_db = Convert.ToDateTime(pCl.birthday),
                p_pol = pCl.gender == null ? (byte)1 : int.Parse(pCl.gender.ToString()) == 0 ? (byte)1:(byte)2,
                p_nation = pCl.nationality,
                p_family_status = pCl.marital_status_grs == null ? (short)1: short.Parse(pCl.marital_status_grs.ToString()) == 1 ?
                short.Parse(pCl.marital_status_grs.ToString()):(short)4
            };
            _db.client_paspdata.Add(cl_pasp);
            _db.SaveChanges();
            AddNewClienDl(pCl, pClCode, dlCode);
        }



        /// <summary>
        /// Добавление в таблицу client_dl
        /// </summary>
        /// <param name="pCl"></param>
        /// <param name="pClCode"></param>
        void AddNewClienDl(ClientInfo pCl, int pClCode, byte pDlCode)
        {
            client_dl cl_dl = new client_dl
            {
                kl_kod = pClCode,
                dl_kod = pDlCode,
                dl_relig = false,
                dl_offsh = false,
                dl_rzd = 1,
                dl_socst = (byte)pCl.odb_social_status,
                dl_datecreate = DateTime.Now,
                dl_sortindx = 0
            };
            _db.client_dl.Add(cl_dl);
            _db.SaveChanges();
            AddNewClienAdress(pCl, pClCode, pDlCode);
        }


        /// <summary>
        /// Добавление информации в таблицу client_adress
        /// </summary>
        /// <param name="pCl"></param>
        /// <param name="pClCode"></param>
        void AddNewClienAdress(ClientInfo pCl, int pClCode, byte pDlCode)
        {
            //Добавление фактического адреса
            client_adress cl_addr = new client_adress
            {
                kl_kod = pClCode,
                dl_kod = pDlCode,
                a_typeadress = "F",
                a_kstr = "417",
                a_ulpfx = "ул.",
                a_dom = pCl.real_address.house,
                a_kvart = pCl.real_address.apartment,
                a_ul = pCl.real_address.street,
                a_knp = "41711000000000",
                a_topfx = "",
                a_ind = "",
                a_ctroen = "",
                a_korpuc = "",
                a_np = pCl.real_address.locality,
                a_obl = pCl.real_address.region,
                a_raion = "",
                a_to = "",


            };
            _db.client_adress.Add(cl_addr);

            //Добавление адреса по прописке
            cl_addr = new client_adress
            {
                kl_kod = pClCode,
                dl_kod = pDlCode,
                a_typeadress = "U",
                a_kstr = "417",
                a_ulpfx = "ул.",
                a_dom = pCl.jur_address.house,
                a_kvart = pCl.jur_address.apartment,
                a_ul = pCl.jur_address.street,
                a_knp = "41711000000000",
                a_topfx = "",
                a_ind = "",
                a_ctroen = "",
                a_korpuc = "",
                a_np = pCl.jur_address.locality,
                a_obl = pCl.jur_address.region,
                a_raion = "",
                a_to = "",
            };
            _db.client_adress.Add(cl_addr);

            //Добавление адреса работы
            cl_addr = new client_adress
            {
                kl_kod = pClCode,
                dl_kod = pDlCode,
                a_typeadress = "W",
                a_topfx = "",
                a_ulpfx = "ул.",

            };
            _db.client_adress.Add(cl_addr);

            //Добавление адреса бизнеса 
            cl_addr = new client_adress
            {
                kl_kod = pClCode,
                dl_kod = pDlCode,
                a_typeadress = "B",
                a_topfx = "",
                a_ulpfx = "ул.",
            };
            _db.client_adress.Add(cl_addr);
            _db.SaveChanges();
            AddNewClienRisk(pCl, pClCode, pDlCode);
        }


        /// <summary>
        /// Добавление в таблицу client_risk
        /// </summary>
        /// <param name="pCl"></param>
        /// <param name="pClCode"></param>
        void AddNewClienRisk(ClientInfo pCl, int pClCode, byte pDlCode)
        {
            client_risk cl_risk = new client_risk
            {
                CR_KL_KOD = pClCode,
                CR_DL_KOD = pDlCode,
                CR_DATE = DateTime.Now,
                CR_KOM = "Низкий",
                CR_RISK_LEVEL = 1,
            };
            _db.client_risk.Add(cl_risk);
            _db.SaveChanges();
        }


        /// <summary>
        /// Метод получение кода для нового клиента
        /// </summary>
        /// <returns></returns>
        public int GetClientId()
        {
            try
            {
                using (bankasiaNSEntities _db = new bankasiaNSEntities())
                {
                    int id = 0;
                    ENUMERATOR_SPR dataClientCode = _db.ENUMERATOR_SPR.FirstOrDefault();
                    id = dataClientCode.client_kod + 1;
                    dataClientCode.client_kod = id;
                    _db.SaveChanges();
                    return id;
                }
            }
            catch (Exception ex)
            {
                new DataBase().WriteLog(ex, "GetClientId");
                return -90;
            }
        }

    }
}