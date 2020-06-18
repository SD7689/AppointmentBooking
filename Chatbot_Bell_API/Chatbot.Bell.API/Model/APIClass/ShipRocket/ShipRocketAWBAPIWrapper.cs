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
    public class ShipRocketAWBAPIWrapper
    {
        #region Private Function
        private string GenrateAWB(string request, string url, string AuthKey)
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
            catch (Exception ex)
            {
                //ServiceLogger.LogAuditTrail("Error while Generating Security Token.");
                //ServiceLogger.LogError("Error while Generating Security Token. Message:- " + ex.Message + "\n Stacktrace:- " + ex.StackTrace);

                throw;
            }
            //ServiceLogger.LogAuditTrail("Get Token Response end.");
            return responseStr;
        }

        private AWBAPIResponse GenerateResponse(string response, string orderid, string request, string shipmentid, string courier_id)
        {
            AWBAPIResponse apiresponses = new AWBAPIResponse();
            try
            {
                apiresponses = JsonConvert.DeserializeObject<AWBAPIResponse>(response);
                //
                try
                {
                    List<CsvColumn> csvs = new List<CsvColumn>()
                {
                    new CsvColumn{ awb_code = apiresponses.Response.Data.AwbCode , courier_id=courier_id, order_id = orderid, Request = request,shipment_id=shipmentid  }
                };


                    string stringfileName = string.Format("{0}AWBRequest_{1}.csv", System.AppContext.BaseDirectory, DateTime.Now.ToString("dd-MMM-yyyy"));
                    bool isFileExists = false;
                    if (File.Exists(stringfileName))
                    {
                        isFileExists = true;
                    }
                    CsvWriter csvWriter = new CsvWriter();
                    csvWriter.Write(csvs, stringfileName, isFileExists ? false : true);
                }
                catch (Exception ex)
                {

                }
                //
            }
            catch
            {
                throw;
            }
            return apiresponses;
        }

        #endregion
        #region Public Method
        public AWBAPIResponse CallAWBGenrateAPI_Click(string request, string AuthKey, string orderid, string shipmentid, string courier_id)
        {
            AWBAPIResponse aPIAWBResponse = new AWBAPIResponse();

            try
            {
                string Response = GenrateAWB(request, AppConfig.AWBApiURL, AuthKey);
                aPIAWBResponse = GenerateResponse(Response, orderid, request, shipmentid, courier_id);
            }
            catch
            {

                throw;
            }
            return aPIAWBResponse;
        }


        #endregion
    }
}
