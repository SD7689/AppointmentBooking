using Chatbot.Bell.API.Model.ShipRocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model.APIClass.ShipRocket
{
    public class ShipRocketCustomOrderAPIWrapper
    {
        #region Private Function
        private string GetCustomOrder(string request, string url, string AuthKey)
        {
            //ServiceLogger.LogAuditTrail("Get Token Response start.");
            string responseStr = string.Empty;
            try
            {
                //using (var client = new HttpClient())
                //{
                //    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthKey);
                //    var req = new HttpRequestMessage
                //    {
                //        RequestUri = new Uri(url),
                //        Method = HttpMethod.Post,

                //    };

                //    req.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(request));

                //    req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                //    // req.Content.Headers.Add("Authorization", "Bearer " + AuthKey);
                //    //req.de
                //    var result = client.SendAsync(req).Result;
                //    result.EnsureSuccessStatusCode();
                //    string finalresult = result.Content.ReadAsStringAsync().Result;
                //    responseStr = finalresult;
                //}



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
                }

            }
            catch (Exception)
            {

                throw;
            }
            //ServiceLogger.LogAuditTrail("Get Token Response end.");
            return responseStr;
        }

        private APIResponse GenerateResponse(string response)
        {
            CreateCustomOrderResponse responses = new CreateCustomOrderResponse();
            APIResponse objResponse = new APIResponse();
            try
            {
                responses = JsonConvert.DeserializeObject<CreateCustomOrderResponse>(response);
                objResponse.Data = objResponse;
                
            }
            catch
            {
                throw;
            }
            return objResponse;
        }
        #endregion
        #region Public Method
        public APIResponse CallCustomOrder_Click(string request, string AuthKey)
        {
            APIResponse createCustomOrderResponse = new APIResponse();
            string response = String.Empty;
            try
            {
                //Hit Generate Token API 
                response = GetCustomOrder(request, AppConfig.CreateCustomeOrder, AuthKey);
                createCustomOrderResponse = GenerateResponse(response);

            }
            catch
            {

                throw;
            }
            return createCustomOrderResponse;
        }
        #endregion
    }
}
