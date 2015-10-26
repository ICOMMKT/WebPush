using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebPush
{
    /// <summary>
    /// 
    /// </summary>
    public class clientInfo
    {
        public string idUser;
        public dynamic clientCaller;
        public string clientApyKey = "";
        public string creationDate = DateTime.Now.ToString("yyyyMMdd hh:mm");
    }

    public class lstClients : List<clientInfo>
    {

    }    
}