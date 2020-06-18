using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model.APIClass
{
    public class ShipRocketAPIWrapperDelete
    {
        #region Private Function
        private string GetSRAuthKey(string request, string url)
        {
            //ServiceLogger.LogAuditTrail("Get Token Response start.");
            string responseStr = string.Empty;
            try
            {
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(request);
                webReq.ContentType = "application/json";
                webReq.ContentLength = bytes.Length;
                webReq.Method = "POST";
                //webReq.Headers.Add("Authorization", AppConfig.KarnivalAuthorizationKey);
                Stream requestStream = webReq.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)webReq.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    responseStr = new StreamReader(responseStream).ReadToEnd();
                }

            }
            catch (Exception)
            {
                //ServiceLogger.LogAuditTrail("Error while Generating Security Token.");
                //ServiceLogger.LogError("Error while Generating Security Token. Message:- " + ex.Message + "\n Stacktrace:- " + ex.StackTrace);

                throw;
            }
            //ServiceLogger.LogAuditTrail("Get Token Response end.");
            return responseStr;
        }

        #endregion
        #region Public Method
        public string CallSRAuthAPI_Click(string request)
        {
            string response = String.Empty;
            try
            {
                //Hit Generate Token API 
                 response = GetSRAuthKey(request, AppConfig.SRAuthAPI);

                //Get response 

                //shortURL = GenerateShortURLResponse(response);

            }
            catch
            {

                throw;
            }
            return response;
        }
        #endregion
    }
}
