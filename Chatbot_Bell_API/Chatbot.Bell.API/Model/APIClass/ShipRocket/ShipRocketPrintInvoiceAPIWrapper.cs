using Chatbot.Bell.API.Model.ShipRocket;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace Chatbot.Bell.API.Model.APIClass.ShipRocket
{
    public class ShipRocketPrintInvoiceAPIWrapper
    {
        #region Private Function
        private string PrintInvoice(string request, string url, string AuthKey)
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

        private PrintInvoiceResponse GenerateResponse(string response)
        {
            PrintInvoiceResponse responses = new PrintInvoiceResponse();
            try
            {
                responses = JsonConvert.DeserializeObject<PrintInvoiceResponse>(response);
            }
            catch
            {
                throw;
            }
            return responses;
        }
        #endregion
        #region Public Method
        public PrintInvoiceResponse CallPrintInvoice_Click(string request, string AuthKey)
        {
            PrintInvoiceResponse printInvoiceResponse = new PrintInvoiceResponse();
            string response = String.Empty;
            try
            {
                //Hit Generate Token API 
                response = PrintInvoice(request, AppConfig.GenerateLabel, AuthKey);
                printInvoiceResponse = GenerateResponse(response);

            }
            catch
            {

                throw;
            }
            return printInvoiceResponse;
        }
        #endregion
    }
}
