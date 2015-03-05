using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace WCFServiceWebRole1
{
    public class AndroidModuleLib
    {
        public string SendAndroidNotification(string AppKey, string NotificationUrl, NotificationMessage Content)
        {
            StringBuilder returnStr = new StringBuilder();//要回傳的字串
            //準備對GCM Server發出Http post
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://android.googleapis.com/gcm/send");
            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8;";
            request.Headers.Add(string.Format("Authorization: key={0}", AppKey));

            string RegistrationID = NotificationUrl;
            var postData =
            new
            {
                data = new
                {
                    message = Content //message這個tag要讓前端開發人員知道
                },

                registration_ids = new string[] { RegistrationID }
            };

            string p = JsonConvert.SerializeObject(postData);//將Linq to json轉為字串
            byte[] byteArray = Encoding.UTF8.GetBytes(p);//要發送的字串轉為byte[]
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            //發出Request
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string responseStr = reader.ReadToEnd();
            reader.Close();
            responseStream.Close();
            response.Close();

            returnStr.Append(responseStr + "\n");
            return returnStr.ToString();
        }
    }
}