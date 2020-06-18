using Chatbot.Bell.API.DataAccess;
using Chatbot.Bell.API.Model;
using Chatbot.Bell.API.Model.APIClass;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Chatbot.Bell.API.Processor
{
    public class ChatbotBellProcessor
    {
        #region Variable_Declaration
        private readonly ChatbotBellDataAccess _chatbotBellDataAccess;
        private readonly ILogger<ChatbotBellProcessor> _logger;

        public ChatbotBellProcessor(ChatbotBellDataAccess chatbotBellDataAccess, ILogger<ChatbotBellProcessor> logger)
        {
            _chatbotBellDataAccess = chatbotBellDataAccess;
            _logger = logger;
        }
        #endregion

        public UserATVDetails GetUserATVDetails(string MobileNumber, string ProgramCode, out bool IsProgramConfigured)
        {
            return _chatbotBellDataAccess.GetUserATVDetails(MobileNumber, ProgramCode, out IsProgramConfigured);
        }

        public async Task<APIResponse> GetUserKeyInsight(string MobileNumber, string ProgramCode)
        {
            return await _chatbotBellDataAccess.GetUserKeyInsight(MobileNumber, ProgramCode);
        }

        public async Task<APIResponse> GetLastTransaction(string MobileNumber, string ProgramCode)
        {
            return await _chatbotBellDataAccess.GetLastTransaction(MobileNumber, ProgramCode);
        }

        public async Task<APIResponse> GetUserRecommendation(string MobileNumber, string ProgramCode)
        {
            return await _chatbotBellDataAccess.GetUserRecommendation(MobileNumber, ProgramCode);
        }


        //public List<Store> GetNearbyStoreDetails(string ProgramCode, string latitude, string longitude, string distance, out bool IsProgramConfigured)
        //{
        //    return _chatbotBellDataAccess.GetNearbyStoreDetails(ProgramCode, latitude, longitude, distance, null, out IsProgramConfigured);
        //}


        //public List<Store> GetNearbyStoreDetails(string ProgramCode, string Pincode, out bool IsProgramConfigured)
        //{
        //    return _chatbotBellDataAccess.GetNearbyStoreDetails(ProgramCode, null, null, null, Pincode, out IsProgramConfigured);
        //}

        public bool SendCampaign(SendCampaignMessageDetails campaignDetails)
        {
            return _chatbotBellDataAccess.SendCampaign(campaignDetails);
        }

        public bool SendText(SendTextMessageDetails campaignDetails)
        {
            return _chatbotBellDataAccess.SendTextAsync(campaignDetails);
        }

        public bool SendImage(SendImageMessageDetails campaignDetails)
        {
            return _chatbotBellDataAccess.SendImageAsync(campaignDetails);
        }

        public List<CustomerResponseModel> GetCustomerResponse()
        {
            return _chatbotBellDataAccess.GetCustomerResponse();
        }

        private string PrepareSMSXML(string MobileNumber, string SenderId, string SMSText)
        {
            StringBuilder sbSMSXML = null;
            string USERNAME = string.Empty;
            string PASSWORD = string.Empty;
            string UDH = string.Empty;
            string CODING = string.Empty;
            string TEXT = string.Empty;
            string PROPERTY = string.Empty;
            string ID = string.Empty;
            string DLR = string.Empty;
            string VALIDITY = string.Empty;
            string FROM = string.Empty;
            string TO = string.Empty;
            string SEQ = string.Empty;
            string TAG = string.Empty;
            string SEND_ON = string.Empty;
            string SENDONVAL = string.Empty;
            string TAGVAL = string.Empty;
            bool IsSendOn = false;
            bool IsTag = false;

            try
            {
                sbSMSXML = new StringBuilder();

                USERNAME = AppConfig.SMSAPIUserName;
                PASSWORD = AppConfig.SMSAPIPassword;
                UDH = AppConfig.UDH;
                CODING = AppConfig.CODING;
                PROPERTY = AppConfig.PROPERTY;
                DLR = AppConfig.DLR;
                VALIDITY = AppConfig.VALIDITY;
                SENDONVAL = AppConfig.SEND_ON;
                TAGVAL = AppConfig.TAG;
                IsSendOn = Convert.ToBoolean(AppConfig.IsSendOn);
                IsTag = Convert.ToBoolean(AppConfig.IsTag);

                /* 
                    <?xml version="1.0" encoding="ISO-8859-1"?> 
                    <!DOCTYPE MESSAGE SYSTEM "http://127.0.0.1/psms/dtd/message.dtd" > 
                    <MESSAGE> <USER USERNAME="mycompany" PASSWORD="mycompany"/> 
                    <SMS UDH="0" CODING="1" TEXT="hey this is a real test" PROPERTY="" ID="1" DLR="0" VALIDITY="0"> 
                    <ADDRESS FROM="9812345678" TO="919812345678" SEQ="1" TAG="some client side random data" /> 
                    <ADDRESS FROM="9812345678" TO="mycompany" SEQ="2" /> <ADDRESS FROM="VALUEFIRST" TO="919812345678" SEQ="3" /> 
                    </SMS> 
                    <SMS UDH="0" CODING="1" TEXT="hey this is a real test" PROPERTY="" ID="2" SEND_ON="2007-10-15 20:10:10 +0530"> 
                    <ADDRESS FROM="9812345678" TO="919812345678" SEQ="1" /> <ADDRESS FROM="9812345678" TO="919812345678" SEQ="2" /> 
                    <ADDRESS FROM="VALUEFIRST" TO="919812345678" SEQ="3" /> <ADDRESS FROM="9812345678" TO="919812345678" SEQ="4" /> 
                    <ADDRESS FROM="VALUEFIRST" TO="919812345678" SEQ="5" /> <ADDRESS FROM="VALUEFIRST" TO="919812345678" SEQ="6" /> 
                    </SMS> 
                    </MESSAGE>
                */

                sbSMSXML.Append(@"<?xml version= ""1.0"" encoding= ""ISO-8859-1""?>");
                sbSMSXML.Append(@"<!DOCTYPE MESSAGE SYSTEM ""http://127.0.0.1:80/psms/dtd/messagev12.dtd"" >");
                sbSMSXML.Append(@"<MESSAGE VER=""1.2"">");
                sbSMSXML.Append(string.Format(@"<USER USERNAME=""{0}"" PASSWORD=""{1}""/>", USERNAME, PASSWORD));

                TEXT = SMSText;
                ID = (BitConverter.ToUInt32(Guid.NewGuid().ToByteArray(), 0) / 10000000).ToString();
                FROM = SenderId;
                TO = MobileNumber;
                SEQ = (BitConverter.ToUInt32(Guid.NewGuid().ToByteArray(), 0) / 10000000).ToString();
                TAG = ID + "-" + (BitConverter.ToUInt32(Guid.NewGuid().ToByteArray(), 0) / 10000000).ToString();
                SEND_ON = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt");

                if (Convert.ToBoolean(IsSendOn))
                {
                    sbSMSXML.Append(string.Format(@"<SMS UDH=""{0}"" CODING=""{1}"" TEXT=""{2}"" PROPERTY=""{3}"" ID=""{4}"" SEND_ON=""{5}"">", UDH, CODING, TEXT, PROPERTY, ID, SEND_ON));
                }
                else
                {
                    sbSMSXML.Append(string.Format(@"<SMS UDH=""{0}"" CODING=""{1}"" TEXT=""{2}"" PROPERTY=""{3}"" ID=""{4}"" DLR=""{5}"" VALIDITY=""{6}"">", UDH, CODING, TEXT, PROPERTY, ID, DLR, VALIDITY));
                }
                if (Convert.ToBoolean(IsTag))
                {
                    sbSMSXML.Append(string.Format(@"<ADDRESS FROM=""{0}"" TO=""{1}"" SEQ=""{2}"" TAG=""{3}"" />", FROM, TO, SEQ, TAG));
                }
                else
                {
                    sbSMSXML.Append(string.Format(@"<ADDRESS FROM=""{0}"" TO=""{1}"" SEQ=""{2}"" />", FROM, TO, SEQ));
                }
                sbSMSXML.Append("</SMS>");

                sbSMSXML.Append("</MESSAGE>");
            }
            catch (Exception ex)
            {
                sbSMSXML = null;
                _logger.LogInformation(ex.ToString());
            }
            return sbSMSXML.ToString();
        }

        private string SendSMSWebRequest(string BulkXML)
        {
            string UserName = string.Empty;
            string Password = string.Empty;
            string responseFromServer = string.Empty;
            string EncodedBulkXML = string.Empty;
            string strAPIURL = string.Empty;
            string strPost = string.Empty;
            try
            {
                UserName = AppConfig.SMSAPIUserName;
                Password = AppConfig.SMSAPIPassword;

                EncodedBulkXML = System.Web.HttpUtility.UrlEncode(BulkXML);
                //EncodedBulkXML = BulkXML;

                //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                //Creation of Request object to api 
                strAPIURL = AppConfig.SMSAPIURL;
                strPost = "data=" + EncodedBulkXML + "&action=send";

                HttpWebRequest objRequest = (HttpWebRequest)(WebRequest.Create(strAPIURL));
                objRequest.Method = "POST";
                objRequest.ContentType = "application/x-www-form-urlencoded";
                objRequest.ContentLength = strPost.Length;

                //Get the requeststream object and write characters to the stream
                StreamWriter myWriter = new StreamWriter(objRequest.GetRequestStream());
                objRequest.KeepAlive = false;
                myWriter.Write(strPost);
                myWriter.Flush();
                myWriter.Close();

                //Response from api 
                System.Net.HttpWebResponse objResponse = (System.Net.HttpWebResponse)(objRequest.GetResponse());
                StreamReader sr = new StreamReader(objResponse.GetResponseStream());
                responseFromServer = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception ex)
            {
                responseFromServer = ex.Message.ToString();
            }
            return responseFromServer;
        }

        private SMSSendResponse ParseSendBulkSMSResponse(string ResponseXML)
        {
            SMSSendResponse objSMSResponse = null;
            try
            {
                foreach (XElement level1Element in XElement.Parse(ResponseXML).Elements())
                {
                    objSMSResponse = new SMSSendResponse();
                    objSMSResponse.GUID = level1Element.Attribute("GUID") != null ? level1Element.Attribute("GUID").Value : string.Empty;
                    objSMSResponse.SubmitDate = level1Element.Attribute("SUBMITDATE") != null ? level1Element.Attribute("SUBMITDATE").Value : string.Empty;
                    objSMSResponse.ID = level1Element.Attribute("ID") != null ? level1Element.Attribute("ID").Value : string.Empty;

                    foreach (XElement level2Element in level1Element.Elements("ERROR"))
                    {
                        objSMSResponse.ErrorSEQ = level2Element.Attribute("SEQ") != null ? level2Element.Attribute("SEQ").Value : string.Empty;
                        objSMSResponse.ErrorCODE = level2Element.Attribute("CODE") != null ? level2Element.Attribute("CODE").Value : string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return objSMSResponse;
        }

        public SMSSendResponse SendSMSResponse(string MobileNumber, string SenderId, string SMSText)
        {
            string Request = string.Empty;
            string Response = string.Empty;
            SMSSendResponse sMSSendResponse = null;
            try
            {
                Request = PrepareSMSXML(MobileNumber, SenderId, SMSText);
                if (!string.IsNullOrEmpty(Request))
                {
                    Response = SendSMSWebRequest(Request);
                    if (!string.IsNullOrEmpty(Response))
                    {
                        sMSSendResponse = new SMSSendResponse();
                        sMSSendResponse = ParseSendBulkSMSResponse(Response);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return sMSSendResponse;
        }

        public List<ItemDetails> GetItemsByArticlesSKUID(string SearchCriteria, string ProgramCode, out bool IsProgramConfigured)
        {
            return _chatbotBellDataAccess.GetItemsByArticlesSKUID(SearchCriteria, ProgramCode, out IsProgramConfigured);
        }

        public async Task<APIResponse> GetNearbyStoreDetails(string ProgramCode, string latitude, string longitude, string distance)
        {
            return await _chatbotBellDataAccess.GetNearbyStoreDetails(ProgramCode, latitude, longitude, distance, null);
        }

        public async Task<APIResponse> GetNearbyStoreDetailsByPincode(string ProgramCode, string Pincode)
        {
            return await _chatbotBellDataAccess.GetNearbyStoreDetails(ProgramCode, null, null, null, Pincode);
        }

        public async Task<List<WhatsappMessageDetailsResponse>> GetWhatsappMessageDetails(string ProgramCode)
        {
            return await _chatbotBellDataAccess.GetWhatsappMessageDetails(ProgramCode);
        }

        public async Task<APIResponse> GetStoreDetailsByStoreCode(string ProgramCode, string StoreCode)
        {
            return await _chatbotBellDataAccess.GetStoreDetailsByStoreCode(ProgramCode, StoreCode);
        }

        public async Task<APIResponse> GetHSNCodeBySKUID(ItemSearch objItemSearch)
        {
            return await _chatbotBellDataAccess.GetHSNCodeBySKUID(objItemSearch);
        }
        
        //internal List<AppointmentDetails> ScheduleVisit(AppointmentMaster appointmentMaster)
        //{
        //    return  _chatbotBellDataAccess.ScheduleVisit(appointmentMaster);
        //}

        internal List<AppointmentResponse> GetTimeSlot(string StoreCode, int tenantID)
        {
            return _chatbotBellDataAccess.GetTimeSlot(StoreCode, tenantID);
        }

        //public async Task<APIResponse> GetSaveAppointmentResponse(SaveAppointmentRequest saveAppointmentRequest)
        //{
        //    return await _chatbotBellDataAccess.GetSaveAppointmentResponse(saveAppointmentRequest);
        //}

        public APIResponse GetTenantConfig(string ProgramCode)
        {
            return _chatbotBellDataAccess.GetTenantConfig(ProgramCode);
        }

        public APIResponse GetTenantMaster()
        {
            return _chatbotBellDataAccess.GetTenantMaster();
        }

        //public bool QueueDigitalReceiptItems(string billIMaster, string ProgCode)
        //{
        //    bool isError = false;
        //    try
        //    {
        //        var factory = new ConnectionFactory() { HostName = AppConfig.RabitMQHostName, UserName = AppConfig.RabbitMQUserName, Password = AppConfig.RabbitMQPassword };
        //        using (var connection = factory.CreateConnection())
        //        using (var channel = connection.CreateModel())
        //        {
        //            Random rnd = new Random();
        //            string Index = Convert.ToString(rnd.Next(1, Convert.ToInt16(AppConfig.RabbitMQQueuesLength)));
        //            string currentRabbitMQQueuesName = String.Format(AppConfig.RabbitMQQueuesName, Convert.ToString(ProgCode).Trim().ToLower(), Index);
        //            channel.QueueDeclare(queue: currentRabbitMQQueuesName,
        //                                 durable: true,
        //                                 exclusive: false,
        //                                 autoDelete: false,
        //                                 arguments: null);

        //            //var body = Encoding.UTF8.GetBytes(ObjShoppingBagInfo);
        //            byte[] messagebuffer = Encoding.Default.GetBytes(billIMaster);


        //            channel.BasicPublish(exchange: "",
        //                                             routingKey: currentRabbitMQQueuesName,
        //                                             basicProperties: null,
        //                                             body: messagebuffer);

        //        }
        //    }
        //    catch (Exception Ex)
        //    {

        //    }
        //    return isError;
        //}
        internal StoreCodeDetails GetShopsterStoreCode(string MobileNumber, string ProgramCode)
        {
             return  _chatbotBellDataAccess.GetShopsterStoreCode(MobileNumber,ProgramCode);
        }

        #region Shopping Bag
        internal string GetSAuthentication(SRAuthRequest sRAuthRequest)
        {
            ShipRocketAPIWrapperDelete shipRocketAPIWrapper = new ShipRocketAPIWrapperDelete();
            var strAuthRequest = JsonConvert.SerializeObject(sRAuthRequest);
            string strResponse = shipRocketAPIWrapper.CallSRAuthAPI_Click(strAuthRequest);
            return strResponse;
        }

        internal ItemImageLocationResponse GetItemImageLocation(string ProgramCode)
        {
            return _chatbotBellDataAccess.GetItemImageLocation(ProgramCode);
        }
        #endregion

        internal List<LastShoppingInvoice> GetLastShoppingReceipt(LastShoppingInvoiceRequest ObjLastShoppingInvoiceRequest)
        {
            return _chatbotBellDataAccess.GetLastShoppingReceipt(ObjLastShoppingInvoiceRequest);
        }
    }
}
