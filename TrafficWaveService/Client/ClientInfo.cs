using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrafficWaveService.Client
{
    public class PassportInfo
    {
        public int id { get; set; }
        public string passport_type { get; set; }
        public string number { get; set; }
        public string series { get; set; }
        public string issued_by { get; set; }
        public string issued_date { get; set; }
        public string deadline { get; set; }
        public object photo { get; set; }
    }

    public class JurAddress
    {
        public int id { get; set; }
        public string country { get; set; }
        public string region { get; set; }
        public string locality { get; set; }
        public string street { get; set; }
        public string house { get; set; }
        public string apartment { get; set; }
    }

    public class RealAddress
    {
        public int id { get; set; }
        public string country { get; set; }
        public string region { get; set; }
        public string locality { get; set; }
        public string street { get; set; }
        public string house { get; set; }
        public string apartment { get; set; }
    }

    public class CheckResult
    {
        public int id { get; set; }
        public bool in_list { get; set; }
        public bool in_db { get; set; }
        public object client_code { get; set; }
        public object client_photo { get; set; }
        public object request_filing_history { get; set; }
        public object credit_issuance_history { get; set; }
        public object kin_prescoring { get; set; }
    }

    public class PhotoClient
    {
        public int id { get; set; }
        public string photo_url { get; set; }
    }

    public class PropertyType
    {
        public int id { get; set; }
        public string name { get; set; }
        public int id_odb { get; set; }
    }

    public class SocialStatus
    {
        public int id { get; set; }
        public string name { get; set; }
        public int id_odb { get; set; }
    }

    public class ClientInfo
    {
    public int id { get; set; }
    public string inn { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string patronymic { get; set; }
    public string birthday { get; set; }
    public PassportInfo passport_info { get; set; }
    public JurAddress jur_address { get; set; }
    public RealAddress real_address { get; set; }
    public CheckResult check_result { get; set; }
    public List<object> credit_history { get; set; }
    public List<object> guarantors { get; set; }
    public PhotoClient photo_client { get; set; }
    public int person_type { get; set; }
    public string client_type { get; set; }
    public string home_phone { get; set; }
    public string contact_phone { get; set; }
    public object email { get; set; }
    public object gender { get; set; }
    public object address_grs { get; set; }
    public string marital_status { get; set; }
    public object marital_status_grs { get; set; }
    public object marital_status_doc { get; set; }
    public int child_count { get; set; }
    public string nationality { get; set; }
    public object territory_code { get; set; }
    public int number_of_overdue_instalments_max { get; set; }
    public int number_of_overdue_instalments_max_last_twelve { get; set; }
    public int negative_status_presence { get; set; }
    public int credit_history_quality { get; set; }
    public int number_of_queries { get; set; }
    public object work_address { get; set; }
    public object salary { get; set; }
    public int pkb_scoring { get; set; }
    public bool pkb_retrieved { get; set; }
    public bool has_credit_history { get; set; }
    public object pkb_report_data { get; set; }
    public string last_kib_request { get; set; }
    public object grade { get; set; }
    public string kib_file { get; set; }
    public bool in_black_list { get; set; }
    public bool in_affiliate { get; set; }
    public object client_coddsfe { get; set; }
    public object request_filing_history { get; set; }
    public object credit_issuance_history { get; set; }
    public string pkb_state { get; set; }
    public object grs_get_date { get; set; }
    public object self_id { get; set; }
    public SocialStatus social_status { get; set; }
    public PropertyType property_type { get; set; }
    public int odb_property_type { get; set; }
    public int odb_social_status { get; set; }
    public int odb_industry { get; set; }
    public int odb_primary_occupation { get; set; }
    public string position { get; set; }
    }
}