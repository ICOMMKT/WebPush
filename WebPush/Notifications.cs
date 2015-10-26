using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Net;
using System.IO;
using System.Configuration;
using System.Web.Services;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Text;

namespace WebPush
{
    public class Notifications : Hub 
    {
        private string clientApiKey;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sClientApiKey"></param>
        public Notifications(string sClientApiKey) {
            clientApiKey = sClientApiKey;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string searchNotifications(int pageSize = 10, int pageIndex = 1, int iNoleidos = -1, string dateToSearch = ""){

            string content;
            string sResult = string.Empty;

            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationSettings.AppSettings["RootServices"] + "/NotificationsService.svc/ListNotifications.Json/");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = true;
            request.Headers.Add("Authorization", clientApiKey);
            content = "{\"iNotificationType\":\"0\", \"iNoleidos\":\" " + iNoleidos + "\",\"dateTosearch\":\"" + dateToSearch + "\",\"iPageSize\":\"" + pageSize + "\",\"iPageIndex\":\"" + pageIndex + "\"}";

            byte[] _byteVersion = new ASCIIEncoding().GetBytes(content);

            request.ContentLength = _byteVersion.Length;

            Stream stream = request.GetRequestStream();
            stream.Write(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {                
                sResult = reader.ReadToEnd();
            }

            return sResult;
        }


        /// <summary>
        /// 
        /// </summary>
        public void viewAllNotifications() {
            string content;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationSettings.AppSettings["RootServices"] + "NotificationsService.svc/ViewAllNotifications.Json/");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = true;
            request.Headers.Add("Authorization", clientApiKey);
            content = "";

            byte[] _byteVersion = new ASCIIEncoding().GetBytes(content);

            request.ContentLength = _byteVersion.Length;

            Stream stream = request.GetRequestStream();
            stream.Write(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NotificationCode"></param>
        /// <returns></returns>
        public string NotificationViewed(string NotificationCode) {

            string content;
            string sResult = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConfigurationSettings.AppSettings["RootServices"] + "NotificationsService.svc/ReadNotification.Json/");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.KeepAlive = true;
            request.Headers.Add("Authorization", clientApiKey);
            content = "{\"iNotificationCode\":\"" + NotificationCode + "\"}";

            byte[] _byteVersion = new ASCIIEncoding().GetBytes(content);

            request.ContentLength = _byteVersion.Length;

            Stream stream = request.GetRequestStream();
            stream.Write(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                //Clients.Caller.ReadNotificationReturn(reader.ReadToEnd());
                sResult = reader.ReadToEnd();
            }

            return sResult;
        }


        public void listNotifications(object StateObj)
        {   
            int pageIndex = 1;
            int pageSize = 10;
            Notifications oNotifications = new Notifications(clientApiKey);
            string strNotifications = oNotifications.searchNotifications(pageSize, pageIndex);

            Clients.Caller.showAlerts(strNotifications, 0);
            
        }
    }
}