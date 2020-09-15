using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Web;

namespace TrafficWaveService
{
    public class Auth
    {
        WebHeaderCollection _headers;
        public Auth(WebHeaderCollection headers)
        {
            _headers = headers;
        }
        public bool CheckUser()
        {
            bool status = true;
            using (bankasiaNSEntities db = new bankasiaNSEntities())
            {
                int id = Convert.ToInt32(_headers["pUserId"]);
                sprotv otv = db.sprotv.FirstOrDefault(x=>x.OT_NOM == id);
                if(otv != null && !(bool)otv.LOCKED)
                {
                    status = false;
                }
            }       
            return status;
        }
    }
}