using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TrafficWaveService.Client
{
    /// <summary>
    /// Класс подозрительных лиц
    /// работает в основном с таблицей reg
    /// </summary>
    public class ClientUpdate
    {
        /// <summary>
        /// Объект для передачи информациии о клиенте
        /// </summary>
        private ClientInfo _ClientInfo { get; set; }

        /// <summary>
        /// Код клиента 
        /// </summary>
        private int _ClCode { get; set; }

        /// <summary>
        /// Доп код клиента
        /// </summary>
        private byte _DlCode { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ClientUpdate() { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="pClientInfo">Информация о клиенте</param>
        public ClientUpdate(ClientInfo pClientInfo, int pClCode, byte pDlCode)
        {
            _ClientInfo = pClientInfo;
            _ClCode = pClCode;
            _DlCode = pDlCode;
        }

        /// <summary>
        /// Обновление данных о клиенте
        /// </summary>
        /// <returns></returns>
        public int Update()
        {
            string res = "Error-Update";
            int ID=-1;
            try
            {
                ID = UpdateClient(_ClientInfo);
            }
            catch (Exception ex)
            {
                res = ex.Message;
                new DataBase().WriteLog(ex, "Run");
            }
            return ID;
        }


        /// <summary>
        /// Метод для обновления информации о клиенте - таблица clients
        /// </summary>
        /// <param name="pCl"></param>
        public int UpdateClient(ClientInfo pCl)
        {
            try
            {
                using (bankasiaNSEntities db = new bankasiaNSEntities())
                {
                    clients cl = db.clients.FirstOrDefault(x => x.kl_kod == _ClCode);
                    cl.kl_nam = pCl.first_name + " " + pCl.last_name + " " + pCl.patronymic;
                    cl.kl_tel1 = pCl.contact_phone;
                    cl.kl_tel2 = pCl.home_phone != null ? pCl.home_phone : cl.kl_tel2;
                    cl.kl_otr = (short)pCl.odb_industry;
                    cl.kl_vidsob = (byte)pCl.odb_property_type;
                    //kl_kodter = "01",
                    db.SaveChanges();
                    UpdateClientPaspData(pCl);

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
        /// Обновление паспортных данных - таблица client_paspdata
        /// </summary>
        /// <param name="pCl"></param>
        /// <param name="pClCode"></param>
        void UpdateClientPaspData(ClientInfo pCl)
        {
            using (bankasiaNSEntities db = new bankasiaNSEntities())
            {
                client_paspdata cl_pasp = db.client_paspdata.FirstOrDefault(x => x.kl_kod == _ClCode);
                cl_pasp.p_fam = pCl.last_name;
                cl_pasp.p_name = pCl.first_name;
                cl_pasp.p_otch = pCl.patronymic;
                //p_grajd = "417",
                cl_pasp.p_viddoksfr = Convert.ToDateTime(pCl.passport_info.issued_date) >= DateTime.Parse("2017-01-01") ? "15" : "05";
                cl_pasp.p_viddok = pCl.passport_info.passport_type;
                cl_pasp.p_srdok = pCl.passport_info.series;
                cl_pasp.p_ndok = pCl.passport_info.number;
                cl_pasp.p_mvd = pCl.passport_info.issued_by;
                cl_pasp.p_enddok = Convert.ToDateTime(pCl.passport_info.deadline);
                cl_pasp.p_datev = Convert.ToDateTime(pCl.passport_info.issued_date);
                    //p_strb = "417",
                    //p_db = Convert.ToDateTime(pCl.birthday),
                    //p_pol = pCl.gender == null ? (byte)0 : (byte)1,
                    //p_nation = pCl.nationality,
                    //p_family_status = 1
                db.SaveChanges();
                UpdateClienAdress(pCl);
            }
        }



        /// <summary>
        /// Добавление информации в таблицу client_adress
        /// </summary>
        /// <param name="pCl"></param>
        /// <param name="pClCode"></param>
        void UpdateClienAdress(ClientInfo pCl)
        {
            using (bankasiaNSEntities db = new bankasiaNSEntities())
            {
                //Добавление фактического адреса
                client_adress cl_addr_f = db.client_adress.FirstOrDefault(x => x.kl_kod == _ClCode && x.a_typeadress == "F");
                cl_addr_f.a_dom = pCl.real_address.house;
                cl_addr_f.a_kvart = pCl.real_address.apartment;
                cl_addr_f.a_ul = pCl.real_address.street;

                db.SaveChanges();

                //Добавление адреса по прописке
                client_adress cl_addr_u = db.client_adress.FirstOrDefault(x => x.kl_kod == _ClCode && x.a_typeadress == "U");
                cl_addr_u.a_dom = pCl.jur_address.house;
                cl_addr_u.a_kvart = pCl.jur_address.apartment;
                cl_addr_u.a_ul = pCl.jur_address.street;

                db.SaveChanges();

                ////Добавление адреса работы
                //cl_addr = new client_adress
                //{
                //    kl_kod = pClCode,
                //    dl_kod = pDlCode,
                //    a_typeadress = "W"             
                //};
                //db.client_adress.Add(cl_addr);

                ////Добавление адреса бизнеса 
                //cl_addr = new client_adress
                //{
                //    kl_kod = pClCode,
                //    dl_kod = pDlCode,
                //    a_typeadress = "B"
                //};
                //db.client_adress.Add(cl_addr);

                //db.SaveChanges();
            }
        }


    }
}