using Chatbot.Bell.API.Model.ShipRocket;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace Chatbot.Bell.API.Model.APIClass.ShipRocket
{
    public class ShipRocketGenerateManifestAPIWrapper
    {
        #region Private Function
        private APIResponse GenerateManifest(string request, string url, string AuthKey)
        {
            //ServiceLogger.LogAuditTrail("Get Token Response start.");
            APIResponse responses = new APIResponse();
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
                    responses.Data = JsonConvert.DeserializeObject<GenerateManifestResponse>(responseStr);
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

        private GenerateManifestResponse GenerateResponse(string response)
        {
            GenerateManifestResponse responses = new GenerateManifestResponse();
            try
            {
                responses = JsonConvert.DeserializeObject<GenerateManifestResponse>(response);
            }
            catch
            {
                throw;
            }
            return responses;
        }
        #endregion
        #region Public Method
        public APIResponse CallGenerateManifest_Click(string request, string AuthKey)
        {
            APIResponse generateManifestResponse = new APIResponse();
            string response = String.Empty;
            try
            {
                //Hit Generate Token API 
                generateManifestResponse = GenerateManifest(request, AppConfig.GenerateManifest, AuthKey);
                //generateManifestResponse = GenerateResponse(response);

            }
            catch
            {

                throw;
            }
            return generateManifestResponse;
        }
        #endregion
    }
}
