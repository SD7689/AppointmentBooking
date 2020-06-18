using Chatbot.Bell.API.Model.ShipRocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Model.APIClass.ShipRocket
{
    public class ShipRocketCourierAPIWrapper
    {
        #region Private Function
        private string GetSRCouriorList(string request, string url, string AuthKey)
        {
            //ServiceLogger.LogAuditTrail("Get Token Response start.");
            string responseStr = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AuthKey);
                    var req = new HttpRequestMessage
                    {
                        RequestUri = new Uri(url),
                        Method = HttpMethod.Get,

                    };

                    req.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(request));

                    req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    // req.Content.Headers.Add("Authorization", "Bearer " + AuthKey);
                    //req.de
                    var result = client.SendAsync(req).Result;
                    result.EnsureSuccessStatusCode();
                    string finalresult = result.Content.ReadAsStringAsync().Result;
                    responseStr = finalresult;
                }



                //    HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
                //    byte[] bytes;
                //    bytes = System.Text.Encoding.ASCII.GetBytes(request);
                //    webReq.ContentType = "application/json";
                //    webReq.ContentLength = bytes.Length;
                //    webReq.Method = "GET";
                //    webReq.Headers.Add("Authorization", "Bearer " + AuthKey);
                //    Stream requestStream = webReq.GetRequestStream();
                //    requestStream.Write(bytes, 0, bytes.Length);
                //    requestStream.Close();
                //    HttpWebResponse response;
                //    response = (HttpWebResponse)webReq.GetResponse();
                //    if (response.StatusCode == HttpStatusCode.OK)
                //    {
                //        Stream responseStream = response.GetResponseStream();
                //        responseStr = new StreamReader(responseStream).ReadToEnd();
                //    }

            }
            catch (Exception)
            {

                throw;
            }
            //ServiceLogger.LogAuditTrail("Get Token Response end.");
            return responseStr;
        }

        private ServiceAvailablilityResponse GenerateResponse(string response)
        {
            ServiceAvailablilityResponse responses = new ServiceAvailablilityResponse();
            try
            {
                responses = JsonConvert.DeserializeObject<ServiceAvailablilityResponse>(response);
            }
            catch
            {
                throw;
            }
            return responses;
        }
        #endregion
        #region Public Method
        public AvailableCourierCompany[] CallSRCouriorAvailable_Click(string request, string AuthKey)
        {
            ServiceAvailablilityResponse serviceAvailablilities = new ServiceAvailablilityResponse();
            AvailableCourierCompany[] availableCourierCompany;
            string response = String.Empty;
            try
            {
                //Hit Generate Token API 
                response = GetSRCouriorList(request, AppConfig.CouriorAvailable, AuthKey);
                serviceAvailablilities = GenerateResponse(response);
                //if (serviceAvailablilities.Data != null)
                if (serviceAvailablilities.Status == 200)
                    availableCourierCompany = (AvailableCourierCompany[])serviceAvailablilities.Data.AvailableCourierCompanies;
                else
                    availableCourierCompany = new AvailableCourierCompany[0];
            }
            catch
            {

                throw;
            }
            return availableCourierCompany;
        }
        #endregion
    }
}
