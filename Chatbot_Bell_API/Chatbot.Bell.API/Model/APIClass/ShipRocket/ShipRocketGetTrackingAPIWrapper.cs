using Chatbot.Bell.API.Model.ShipRocket;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace Chatbot.Bell.API.Model.APIClass.ShipRocket
{
    public class ShipRocketGetTrackingAPIWrapper
    {
        #region Private Function
        private string GetTracking(string url, string AuthKey)
        {
            //ServiceLogger.LogAuditTrail("Get Token Response start.");
            string responseStr = string.Empty;
            try
            {

                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
                webReq.ContentType = "application/json";
                webReq.Method = "GET";
                webReq.Headers.Add("Authorization", "Bearer " + AuthKey);
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

                throw;
            }
            //ServiceLogger.LogAuditTrail("Get Token Response end.");
            return responseStr;
        }

        private GetTrackingResponse GenerateResponse(string response)
        {
            GetTrackingResponse responses = new GetTrackingResponse();
            try
            {
                responses = JsonConvert.DeserializeObject<GetTrackingResponse>(response);
            }
            catch
            {
                throw;
            }
            return responses;
        }
        #endregion
        #region Public Method
        public GetTrackingResponse CallGetTracking_Click(string awb_no, string AuthKey)
        {
            GetTrackingResponse getTrackingResponse = new GetTrackingResponse();
            string response = String.Empty;
            try
            {
                //Hit Generate Token API 
                response = GetTracking(AppConfig.GetTracking + awb_no, AuthKey);
                getTrackingResponse = GenerateResponse(response);

            }
            catch
            {

                throw;
            }
            return getTrackingResponse;
        }
        #endregion
    }
}
