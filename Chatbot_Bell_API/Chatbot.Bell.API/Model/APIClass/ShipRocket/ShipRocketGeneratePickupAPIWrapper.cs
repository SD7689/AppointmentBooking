using Chatbot.Bell.API.Model.ShipRocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model.APIClass.ShipRocket
{
    public class ShipRocketGeneratePickupAPIWrapper
    {
        #region Private Function
        private APIResponse GeneratePickup(string request, string url, string AuthKey)
        {
            APIResponse responses = new APIResponse();
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
                webReq.Headers.Add("Authorization", "Bearer " + AuthKey);
                Stream requestStream = webReq.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)webReq.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    responseStr = new StreamReader(responseStream).ReadToEnd();
                    responses.Data = JsonConvert.DeserializeObject<GeneratePickupResponse>(responseStr);
                }

            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        responseStr = reader.ReadToEnd();
                        responses.Data = JsonConvert.DeserializeObject<GenericAPIResponse>(responseStr);
                        responses.isError = true;
                    } 
                }
            }
            catch (Exception)
            {

                throw;
            }
            //ServiceLogger.LogAuditTrail("Get Token Response end.");
            return responses;
        }

        private APIResponse GenerateResponse(string response)
        {
            APIResponse responses = new APIResponse();
            try
            {
                responses.Data = JsonConvert.DeserializeObject<GeneratePickupResponse>(response);
            }
            catch
            {
                throw;
            }
            return responses;
        }
        #endregion
        #region Public Method
        public APIResponse CallGeneratePickup_Click(string request, string AuthKey)
        {
            APIResponse generatePickupResponse = new APIResponse();
            string response = String.Empty;
            try
            {
                //Hit Generate Token API 
                generatePickupResponse = GeneratePickup(request, AppConfig.GeneratePickup, AuthKey);
                //generatePickupResponse = GenerateResponse(response);

            }
            catch
            {

                throw;
            }
            return generatePickupResponse;
        }
        #endregion
    }
}
