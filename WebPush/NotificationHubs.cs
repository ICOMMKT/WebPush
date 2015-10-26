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
using System.Threading;
using iComMkt.Generic.Logic;


namespace WebPush
{
    /// <summary>
    /// 
    /// </summary>
    [HubName("iComMktHub")]
    public class NotificationHubs : Hub 
    {   
        private static lstClients oClients = new lstClients();
        private System.Threading.Timer oThreadNotification;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="clientApyKey"></param>
        public void registerClient(string idUser, string clientApiKey, int pageIndex = 1, int pageSize = 10)
        {

            lock (oClients)
            {
                bool existClient = oClients.FindAll(clientInfo => clientInfo.idUser == idUser).Count > 0;
                clientInfo oClient = new clientInfo();
                if (!existClient)
                {   
                    oClient.idUser = idUser;
                    oClient.clientCaller = Clients.Caller;
                    oClient.clientApyKey = clientApiKey;
                    //oClient.creationDate = DateTime.Now.AddSeconds(10).ToString("yyyyMMdd HH:mm:ss");
                    oClient.creationDate = DateTime.Now.AddSeconds(10).ToString("yyyyMMdd HH:mm:ss");
                    oClients.Add(oClient);
                }
                else
                {
                    oClient = oClients.FindAll(clientInfo => clientInfo.idUser == idUser)[0];                    
                    //oClient.creationDate = DateTime.Now.ToString("dd/MM/YYYY");
                    oClients.FindAll(clientInfo => clientInfo.idUser == idUser)[0].creationDate = DateTime.Now.AddSeconds(10).ToString("yyyyMMdd HH:mm:ss");                
                }

                listNotifications(clientApiKey, pageIndex, pageSize);
                
                //ThreadNotification(oClient.clientApyKey, oClient.creationDate);
                ThreadNotification(oClient);
                
            }
        }   


        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientApyKey"></param>
        //public void ThreadNotification(string clientApiKey, string clientDateFromtoSearch) {
        public void ThreadNotification(clientInfo oClient) {
            
            StateObjClass StateObj = new StateObjClass();
            StateObj.TimerCanceled = false;
            StateObj.clientApiKey = oClient.clientApyKey; //clientApiKey;
            StateObj.searchfromDate = oClient.creationDate; //clientDateFromtoSearch;
            StateObj.clientId = oClient.idUser;
            

            System.Threading.TimerCallback TimerDelegate = new System.Threading.TimerCallback(listNotificationsThread);
            oThreadNotification = new Timer(TimerDelegate, StateObj, 0, Convert.ToInt32(ConfigurationSettings.AppSettings["timer"]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="StateObj"></param>
        public void listNotificationsThread(object StateObj)
        {

            StateObjClass State = (StateObjClass)StateObj;
            /*clientInfo oClient = new clientInfo();
            oClient = oClients.FindAll(clientInfo => clientInfo.idUser == State.clientId)[0];*/

            int pageIndex = 1;
            int pageSize = 10;
            Notifications oNotifications = new Notifications(State.clientApiKey);
            string strNotifications = oNotifications.searchNotifications(pageSize, pageIndex, 0, State.searchfromDate);

            
            
            //actualizar la hora de cada notificacion del lado del cliente. al hacer click sobre el icono de notificaciones

            if (strNotifications != "{\"ListNotificationsJsonResult\":\"[]\"}")
            {
                Clients.Caller.showAlerts(strNotifications, 1);
                //actualizar la fecha searchfromDate para que el trhead devuelva sólo los nuevos eventos.                            
                State.searchfromDate = DateTime.Now.AddSeconds(10).ToString("yyyyMMdd HH:mm:ss");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientApiKey"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        public void listNotifications(string clientApiKey, int pageIndex, int pageSize){

            Notifications oNotifications = new Notifications(clientApiKey);
            string strNotifications = oNotifications.searchNotifications(pageSize, pageIndex);            
            Clients.Caller.showAlerts(strNotifications,0);        
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientApiKey"></param>
        public void viewAllNotifications(string clientApiKey) {

            Notifications oNotifications = new Notifications(clientApiKey);
            oNotifications.viewAllNotifications();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iNotificationCode"></param>
        /// <param name="clientApiKey"></param>
        public void readnotification(string iNotificationCode, string clientApiKey)
        {
            Notifications oNotification = new Notifications(clientApiKey);
            oNotification.NotificationViewed(iNotificationCode);
            Clients.Caller.ReadNotificationReturn("");
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class StateObjClass
    {
        // Used to hold parameters for calls to TimerTask. 
        public string clientApiKey;
        public System.Threading.Timer TimerReference;
        public string clientId;
        public bool TimerCanceled;
        public string searchfromDate = "";
    }
}