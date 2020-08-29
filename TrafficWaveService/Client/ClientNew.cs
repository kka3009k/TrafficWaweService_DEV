using System;
using System.Linq;
using System.Threading.Tasks;
using TrafficWaveService.Dictionaries;
namespace TrafficWaveService.Client
{
    /// <summary>
    /// Класс подозрительных лиц
    /// работает в основном с таблицей reg
    /// </summary>
    public class ClientNew
    {
        public ClientInfo _clientInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private RuToLatin _Translate { get; set; }
        /// <summary>
        /// Конструктор
        /// </summary>
        public ClientNew() { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="pClientInfo">Информация о клиенте</param>
        public ClientNew(ClientInfo pClientInfo)
        {
            _clientInfo = pClientInfo;
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
            try
            {
                using (bankasiaNSEntities db = new bankasiaNSEntities())
                {
                    int cl_kod = GetClientId();
                    clients cl = new clients
                    {
                        kl_kod = cl_kod,
                        kl_inn = pCl.inn,
                        kl_datecreate = DateTime.Now,
                        kl_nam = pCl.first_name + " " + pCl.last_name + " " + pCl.patronymic,
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
                        kl_wdoljn = "Payer",
                        kl_wfio = "Payer",
                        kl_wname = "Прочее",
                        kl_otvcreate = 3009,
                        kl_ins = false,
                        kl_pzl = false,
                        kl_dopvd = "",
                        kl_fatca = false,
                        kl_isys ="OD",
                        kl_otr = (short)pCl.odb_industry,
                        kl_osnvd = (short)pCl.odb_primary_occupation,
                        kl_keyw = "",

                    };
                    db.clients.Add(cl);
                    db.SaveChanges();
                    AddNewClientPaspData(pCl, cl.kl_kod);

                    return cl.kl_kod;
                }
            }
            catch(Exception ex)
            {
                new DataBase().WriteLog(ex, "Run");
                return -1;
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
            using (bankasiaNSEntities db = new bankasiaNSEntities())
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
                    p_viddok = pCl.passport_info.passport_type,
                    p_srdok = pCl.passport_info.series,
                    p_ndok = pCl.passport_info.number,
                    p_mvd = pCl.passport_info.issued_by,
                    p_enddok = Convert.ToDateTime(pCl.passport_info.deadline),
                    p_datev = Convert.ToDateTime(pCl.passport_info.issued_date),
                    p_noend = false,
                    p_strb = "417",
                    p_db = Convert.ToDateTime(pCl.birthday),
                    p_pol = pCl.gender == null ? (byte)0 : (byte)1,
                    p_nation = pCl.nationality,
                    p_family_status = 1
                };
                db.client_paspdata.Add(cl_pasp);
                db.SaveChanges();
                AddNewClienDl(pCl, pClCode, dlCode);
            }
        }



        /// <summary>
        /// Добавление в таблицу client_dl
        /// </summary>
        /// <param name="pCl"></param>
        /// <param name="pClCode"></param>
        void AddNewClienDl(ClientInfo pCl, int pClCode, byte pDlCode)
        {
            using (bankasiaNSEntities db = new bankasiaNSEntities())
            {
                client_dl cl_dl = new client_dl
                {
                    kl_kod = pClCode,
                    dl_kod = pDlCode,
                    dl_relig = false,
                    dl_offsh = false,
                    dl_rzd = 1,
                    dl_socst =(byte)pCl.odb_social_status,
                    dl_datecreate = DateTime.Now,
                    dl_sortindx = 0
                };
                db.client_dl.Add(cl_dl);
                db.SaveChanges();
                AddNewClienAdress(pCl, pClCode, pDlCode);
            }
        }


        /// <summary>
        /// Добавление информации в таблицу client_adress
        /// </summary>
        /// <param name="pCl"></param>
        /// <param name="pClCode"></param>
        void AddNewClienAdress(ClientInfo pCl, int pClCode, byte pDlCode)
        {
            using (bankasiaNSEntities db = new bankasiaNSEntities())
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
                    a_np = "",
                    a_obl = "",
                    a_raion = "",
                    a_to = "",


                };
                db.client_adress.Add(cl_addr);

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
                    a_korpuc="",
                    a_np = "",
                    a_obl="",
                    a_raion="",
                    a_to = "",
                };
                db.client_adress.Add(cl_addr);

                //Добавление адреса работы
                cl_addr = new client_adress
                {
                    kl_kod = pClCode,
                    dl_kod = pDlCode,
                    a_typeadress = "W",
                    a_topfx = "",
                    a_ulpfx = "ул.",

                };
                db.client_adress.Add(cl_addr);

                //Добавление адреса бизнеса 
                cl_addr = new client_adress
                {
                    kl_kod = pClCode,
                    dl_kod = pDlCode,
                    a_typeadress = "B",
                    a_topfx = "",
                    a_ulpfx = "ул.",
                };
                db.client_adress.Add(cl_addr);
                db.SaveChanges();
                AddNewClienRisk(pCl, pClCode, pDlCode);
            }
        }


        /// <summary>
        /// Добавление в таблицу client_risk
        /// </summary>
        /// <param name="pCl"></param>
        /// <param name="pClCode"></param>
        void AddNewClienRisk(ClientInfo pCl, int pClCode, byte pDlCode)
        {
            using (bankasiaNSEntities db = new bankasiaNSEntities())
            {
                client_risk cl_risk = new client_risk
                {
                   CR_KL_KOD = pClCode,
                   CR_DL_KOD = pDlCode,
                   CR_DATE = DateTime.Now,
                   CR_KOM = "Низкий",
                   CR_RISK_LEVEL = 1,                   
                };
                db.client_risk.Add(cl_risk);
                db.SaveChanges();
            }
        }


        /// <summary>
        /// Метод получение кода клиента
        /// </summary>
        /// <returns></returns>
        public int GetClientId()
        {
            try
            {
                using (bankasiaNSEntities db = new bankasiaNSEntities())
                {
                    int id = 0;
                    ENUMERATOR_SPR dataClientCode = db.ENUMERATOR_SPR.FirstOrDefault();
                    id = dataClientCode.client_kod + 1;
                    dataClientCode.client_kod = id;
                    db.SaveChanges();
                    return id;
                }
            }
            catch (Exception ex)
            {
                return -90;
            }
        }

    }
}