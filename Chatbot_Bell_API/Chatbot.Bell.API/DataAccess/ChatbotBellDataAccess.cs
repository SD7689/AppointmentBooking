using Chatbot.Bell.API.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32.SafeHandles;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.DataAccess
{
    public class ChatbotBellDataAccess
    {
        #region Dispose
        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
            }

            disposed = true;
        }
        #endregion

        private readonly IOptions<Sqlqueries> _sqlQueries;
        private readonly ILogger<ChatbotBellDataAccess> _logger;
        public ChatbotBellDataAccess(IOptions<Sqlqueries> sqlQueries, ILogger<ChatbotBellDataAccess> logger)
        {
            _sqlQueries = sqlQueries;
            _logger = logger;
        }
        public UserATVDetails GetUserATVDetails(string MobileNumber, string ProgramCode, out bool IsProgramConfigured)
        {
            UserATVDetails objUserATVDetails = new UserATVDetails();
            IsProgramConfigured = true;
            try
            {
                string strConnection = GetConnection(ProgramCode, out bool isProgExists);
                if (isProgExists)
                {
                    using (MySqlConnection Conn = new MySqlConnection(strConnection))
                    {
                        if (Conn.State != ConnectionState.Open)
                            Conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(AppConfig.GetATVDataByMobileNumber, Conn))
                        {
                            //    string SQLCommand = "SELECT Mobile, CASE WHEN firstname<>'' OR lastname<>''THEN CONCAT(FirstName,' ',Lastname) ELSE 'Member' END AS NAME, " +
                            //                        " Tiername AS Tier,IFNULL(`Total Spends`, 0) AS Lifetime_Value, `Total Visits`AS Visit_count " +
                            //                        " FROM program_single_view WHERE mobile = '" + MobileNumber + "'; ";
                            string SQLCommand = Sqlqueries.GetUserATVDetails.Replace("##MobileNumber", MobileNumber);
                            cmd.CommandText = SQLCommand;
                            MySqlDataReader objReader = cmd.ExecuteReader();
                            if (objReader.HasRows)
                            {
                                while (objReader.Read())
                                {
                                    objUserATVDetails.Name = Convert.ToString(objReader["NAME"]);
                                    objUserATVDetails.MobileNumber = Convert.ToString(objReader["Mobile"]);
                                    objUserATVDetails.Tiername = Convert.ToString(objReader["Tier"]);
                                    objUserATVDetails.LifeTimeValue = Convert.ToString(objReader["Lifetime_Value"]);
                                    objUserATVDetails.VisitCount = Convert.ToString(objReader["Visit_count"]);

                                }
                            }
                        }
                    }
                }
                else { IsProgramConfigured = false; }

            }
            catch (Exception Ex)
            {
                _logger.LogInformation(Ex.ToString());
            }
            return objUserATVDetails;
        }

        public async Task<APIResponse> GetUserKeyInsight(string MobileNumber, string ProgramCode)
        {
            APIResponse objResponse = new APIResponse();
            List<KeyInsight> objInsightList = new List<KeyInsight>();
            try
            {
                string strConnection = GetConnection(ProgramCode, out bool isProgExists);
                if (isProgExists)
                {
                    using (MySqlConnection Conn = new MySqlConnection(strConnection))
                    {
                        if (Conn.State != ConnectionState.Open)
                            Conn.Open();
                        List<KeyRawInsight> objListKeyInsightRaw = new List<KeyRawInsight>();

                        string SQLQuery = Sqlqueries.GetUserKeyInsight.Replace("##MobileNumber", MobileNumber);

                        using (MySqlCommand cmd = new MySqlCommand(SQLQuery, Conn))
                        {
                            using (DbDataReader objReader = await cmd.ExecuteReaderAsync())
                            {
                                if (objReader.HasRows)
                                {
                                    while (await objReader.ReadAsync())
                                    {
                                        KeyRawInsight objKeyInsightRaw = new KeyRawInsight();
                                        objKeyInsightRaw.InsightID = Convert.ToString(objReader["insightid"]);
                                        objKeyInsightRaw.TagsDict = Convert.ToString(objReader["tagsDict"]);
                                        objKeyInsightRaw.Message = Convert.ToString(objReader["message"]);
                                        objKeyInsightRaw.MobileNumber = Convert.ToString(objReader["mobile"]);
                                        objListKeyInsightRaw.Add(objKeyInsightRaw);
                                    }
                                }
                            }
                        }
                        if (objListKeyInsightRaw.Count > 0)
                        {
                            objInsightList = GetKeyInsightMessage(objListKeyInsightRaw);
                        }
                        objResponse.Data = objInsightList;
                    }
                }
                else
                {
                    objResponse.isError = true;
                    objResponse.Data = 225;
                }
            }
            catch (Exception Ex)
            {
                _logger.LogInformation(Ex.ToString());
            }
            return objResponse;

        }

        public List<KeyInsight> GetKeyInsightMessage(List<KeyRawInsight> objListKeyInsightRaw)
        {
            List<KeyInsight> objKeyInsight = new List<KeyInsight>();
            try
            {
                foreach (KeyRawInsight objRawInsight in objListKeyInsightRaw)
                {

                    string Insghtid = objRawInsight.InsightID;
                    string Mobilenumber = objRawInsight.MobileNumber;
                    string TagDist = objRawInsight.TagsDict;
                    string Message = objRawInsight.Message;
                    string[] strTagDistinct = TagDist.Replace("\",\"", "!@#;").Split("!@#;");
                    List<KeyValuePair<string, string>> kvpTagList = new List<KeyValuePair<string, string>>();
                    int startMessageIndex = 0;
                    foreach (string str in strTagDistinct)
                    {
                        string replaceValue = str.Substring(str.IndexOf(":") + 1, str.Length - str.IndexOf(":") - 1);
                        kvpTagList.Insert(startMessageIndex, new KeyValuePair<string, string>(startMessageIndex.ToString(), replaceValue));
                        startMessageIndex = startMessageIndex + 1;
                    }

                    string[] strMessage = Message.Replace("##", "#").Split("#");
                    List<KeyValuePair<string, string>> kvpList = new List<KeyValuePair<string, string>>();
                    int startIndex = 0;
                    foreach (string str in strMessage)
                    {
                        if (str.ToUpper().Contains("TAG"))
                        {
                            string key = str.ToUpper().Replace("TAG", "");
                            string value = str;
                            kvpList.Insert(startIndex, new KeyValuePair<string, string>(key, value));
                            startIndex = startIndex + 1;
                        }
                    }
                    if (kvpList.Count == kvpTagList.Count)
                    {
                        Message = Message.Replace("##", "# #");
                        for (int i = 0; i < kvpList.Count; i++)
                        {
                            string keyID = kvpList[i].Key;
                            string TextToReplace = kvpList[i].Value;
                            string ReplaceBy = kvpTagList.Where(k => k.Key == (Convert.ToInt16(keyID) - 1).ToString()).Select(s => s.Value).FirstOrDefault();
                            if (i == kvpList.Count - 1)
                                ReplaceBy = ReplaceBy.TrimEnd('}');
                            Message = Message.Replace("#" + TextToReplace + "#", ReplaceBy).Replace(@"""", "");
                        }
                    }
                    KeyInsight objKInsight = new KeyInsight();
                    objKInsight.InsightID = Insghtid;
                    //objKInsight.MobileNumber = Mobilenumber;
                    objKInsight.InsightMessage = Message;
                    objKeyInsight.Add(objKInsight);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogInformation(Ex.ToString());
            }
            return objKeyInsight;
        }

        public async Task<APIResponse> GetUserRecommendation(string MobileNumber, string ProgramCode)
        {
            APIResponse objResponse = new APIResponse();
            List<Recommendation> objRecommendationList = new List<Recommendation>();
            try
            {
                string strConnection = GetConnection(ProgramCode, out bool isProgExists);
                if (isProgExists)
                {
                    using (MySqlConnection Conn = new MySqlConnection(strConnection))
                    {
                        if (Conn.State != ConnectionState.Open)
                            Conn.Open();
                        string SQLQuery = Sqlqueries.GetUserRecommendation.Replace("##MobileNumber", MobileNumber);

                        using (MySqlCommand cmd = new MySqlCommand(SQLQuery, Conn))
                        {
                            using (DbDataReader objReader = await cmd.ExecuteReaderAsync())
                            {
                                if (objReader.HasRows)
                                {
                                    while (await objReader.ReadAsync())
                                    {
                                        Recommendation objRecommendation = new Recommendation();
                                        objRecommendation.MobileNumber = Convert.ToString(objReader["Mobile"]);
                                        objRecommendation.ItemCode = Convert.ToString(objReader["Itemcode"]);
                                        objRecommendation.Category = Convert.ToString(objReader["Cateogary"]);
                                        objRecommendation.SubCategory = Convert.ToString(objReader["Subcategory"]);
                                        objRecommendation.Brand = Convert.ToString(objReader["Brand"]);
                                        objRecommendation.Color = Convert.ToString(objReader["Colour"]);
                                        objRecommendation.size = Convert.ToString(objReader["Size"]);
                                        objRecommendation.Price = Convert.ToString(objReader["Price"]);
                                        objRecommendation.URL = Convert.ToString(objReader["Url"]);
                                        objRecommendation.ImageURL = Convert.ToString(objReader["Imageurl"]);
                                        objRecommendationList.Add(objRecommendation);
                                    }
                                }
                            }
                        }
                        objResponse.Data = objRecommendationList;
                    }
                }
                else
                {
                    objResponse.isError = true;
                    objResponse.Data = 225;
                }
            }
            catch (Exception Ex)
            {
                _logger.LogInformation(Ex.ToString());
            }
            return objResponse;
        }

        public async Task<APIResponse> GetLastTransaction(string MobileNumber, string ProgramCode)
        {
            APIResponse objResponse = new APIResponse();
            Transaction objLastTransaction = new Transaction();
            try
            {
                string strConnection = GetConnection(ProgramCode, out bool isProgExists);
                if (isProgExists)
                {
                    using (MySqlConnection Conn = new MySqlConnection(strConnection))
                    {
                        if (Conn.State != ConnectionState.Open)
                            Conn.Open();
                        string SQLQuery = Sqlqueries.GetLastTransaction.Replace("##MobileNumber", MobileNumber);
                        using (MySqlCommand cmd = new MySqlCommand(SQLQuery, Conn))
                        {
                            using (DbDataReader objReader = await cmd.ExecuteReaderAsync())
                            {
                                if (objReader.HasRows)
                                {
                                    while (await objReader.ReadAsync())
                                    {
                                        objLastTransaction.MobileNo = Convert.ToString(objReader["Txnmappedmobile"]);
                                        objLastTransaction.BillDate = Convert.ToDateTime(objReader["Date"]).ToString("dd-MMM-yyyy");
                                        objLastTransaction.BillNo = Convert.ToString(objReader["Billno"]);
                                        objLastTransaction.StoreName = Convert.ToString(objReader["Store"]);
                                        objLastTransaction.Amount = Convert.ToString(objReader["Amount"]);
                                    }
                                    objReader.NextResult();
                                    List<ItemDetail> objItemDetails = new List<ItemDetail>();
                                    while (await objReader.ReadAsync())
                                    {
                                        ItemDetail objItemDetail = new ItemDetail();
                                        objItemDetail.MobileNo = Convert.ToString(objReader["txnmappedmobile"]);
                                        objItemDetail.Article = Convert.ToString(objReader["Article"]).Trim();
                                        objItemDetail.Quantity = Convert.ToString(objReader["Qty"]);
                                        objItemDetail.Amount = Convert.ToString(objReader["Amount"]);
                                        objItemDetails.Add(objItemDetail);
                                    }
                                    objLastTransaction.ItemDetails = objItemDetails;
                                    objResponse.Data = objLastTransaction;
                                }
                                else
                                {
                                    objResponse.isError = true;
                                    objResponse.Data = 1002;
                                }
                            }
                        }
                    }
                }
                else
                {
                    objResponse.isError = true;
                    objResponse.Data = 225;
                }
            }
            catch (Exception Ex)
            {
                _logger.LogInformation(Ex.ToString());
            }
            return objResponse;
        }

        //public List<Store> GetNearbyStoreDetails(string ProgramCode, string latitude, string longitude, string distance, string Pincode, out bool IsProgramConfigured)
        //{
        //    List<Store> objStoreList = new List<Store>();
        //    IsProgramConfigured = true;
        //    try
        //    {

        //        string strConnection = GetConnection(ProgramCode, out bool isProgExists);
        //        if (isProgExists)
        //        {
        //            using (MySqlConnection Conn = new MySqlConnection(strConnection))
        //            //using (MySqlConnection Conn = new MySqlConnection(AppConfig.TestConnection))
        //            {
        //                if (Conn.State != ConnectionState.Open)
        //                    Conn.Open();

        //                using (MySqlCommand cmd = new MySqlCommand(AppConfig.GetStoreLocator, Conn))
        //                {
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.Parameters.Add("p_lattitude", MySqlDbType.VarChar).Value = latitude;
        //                    cmd.Parameters.Add("p_logitude", MySqlDbType.VarChar).Value = longitude;
        //                    cmd.Parameters.Add("p_distance", MySqlDbType.Decimal).Value = distance;
        //                    cmd.Parameters.Add("p_Pincode", MySqlDbType.Decimal).Value = Pincode;

        //                    MySqlDataReader objReader = cmd.ExecuteReader();
        //                    if (objReader.HasRows)
        //                    {
        //                        while (objReader.Read())
        //                        {
        //                            Store objStore = new Store();
        //                            objStore.StoreCode = Convert.ToString(objReader["StoreCode"]);
        //                            objStore.StoreName = Convert.ToString(objReader["StoreName"]);
        //                            objStore.StoreStatus = Convert.ToString(objReader["StoreStatus"]);
        //                            objStore.Address1 = Convert.ToString(objReader["Address1"]);
        //                            objStore.Address2 = Convert.ToString(objReader["Address2"]);
        //                            objStore.City = Convert.ToString(objReader["City"]);
        //                            objStore.State = Convert.ToString(objReader["State"]);
        //                            objStore.Zone = Convert.ToString(objReader["Zone"]);
        //                            objStore.Region = Convert.ToString(objReader["Region"]);
        //                            objStore.Country = Convert.ToString(objReader["Country"]);
        //                            objStore.StoreTiming = Convert.ToString(objReader["StoreTiming"]);
        //                            objStore.latitude = Convert.ToString(objReader["Latitude"]);
        //                            objStore.longitude = Convert.ToString(objReader["Longitude"]);
        //                            objStore.PinCode = Convert.ToString(objReader["Pincode"]);
        //                            objStoreList.Add(objStore);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //            IsProgramConfigured = false;
        //    }
        //    catch (Exception Ex)
        //    { throw Ex; }
        //    return objStoreList;
        //}

        public string GetConnection(string ProgramCode, out bool isProgExists)
        {
            string strConnection = string.Empty;
            isProgExists = true;
            try
            {
                string ProgramServerCode = string.Empty;
                using (MySqlConnection Conn = new MySqlConnection(AppConfig.ConnectionString))
                {
                    if (Conn.State != ConnectionState.Open)
                        Conn.Open();
                    string SQLQuery = Sqlqueries.GetMasterConnection.Replace("##ProgramCode", ProgramCode);
                    using (MySqlCommand cmd = new MySqlCommand(SQLQuery, Conn))
                    {
                        MySqlDataReader objReader = cmd.ExecuteReader();
                        if (objReader.HasRows)
                        {
                            while (objReader.Read())
                            {
                                ProgramServerCode = Convert.ToString(objReader["ProgramServercode"]);
                            }
                        }
                        else
                        {
                            isProgExists = false;
                        }
                    }
                    if (ProgramServerCode != null && isProgExists == true)
                    {
                        strConnection = (string)typeof(AppConfig).GetProperty(ProgramServerCode).GetValue(null, null);
                        strConnection = strConnection.Replace("#Brandname#", ProgramCode.ToLower());
                    }
                }
            }
            catch (Exception Ex)
            {
                _logger.LogInformation(Ex.ToString());
            }
            return strConnection;
        }

        public bool SendCampaign(SendCampaignMessageDetails message)
        {
            bool IsSuccess = false;
            bool IsProgrmExist = false;
            try
            {
                SendCampaignPayload payload = new SendCampaignPayload();
                payload.AdditionalInfo = message.AdditionalInfo;
                payload.DbConnection = GetConnection(message.ProgramCode, out IsProgrmExist);
                payload.ProgramCode = message.ProgramCode;
                payload.TemplateName = message.TemplateName;
                payload.WhatsAppCredentials = GetWACredentials(message.ProgramCode);
                payload.To = message.To;

                ProgramQueueDetails programQueueDetails = new ProgramQueueDetails();
                programQueueDetails = GetProgramQueueDetails(message.ProgramCode);

                IsSuccess = PushDataInExchange(JsonConvert.SerializeObject(payload), programQueueDetails.ExchangeName, programQueueDetails.SendCampaignQueue, programQueueDetails.RabbitUserName, programQueueDetails.RabbitPassword, programQueueDetails.RabbitHost);
            }
            catch (Exception Ex)
            {
                _logger.LogInformation(Ex.ToString());
            }
            return IsSuccess;
        }

        public bool SendTextAsync(SendTextMessageDetails message)
        {
            bool IsSuccess = false;
            bool IsProgramExist = false;
            try
            {
                SendMessageQueuePayload payload = new SendMessageQueuePayload();
                payload.TextToReply = message.TextToReply;
                payload.To = message.To;
                payload.WhatsAppCredentials = GetWACredentials(message.ProgramCode);
                //payload.DbConnection = GetConnection(message.ProgramCode);
                payload.DbConnection = GetConnection(message.ProgramCode, out IsProgramExist);

                ProgramQueueDetails programQueueDetails = new ProgramQueueDetails();
                programQueueDetails = GetProgramQueueDetails(message.ProgramCode);

                IsSuccess = PushDataInExchange(JsonConvert.SerializeObject(payload), programQueueDetails.ExchangeName, programQueueDetails.SendMessageQueue, programQueueDetails.RabbitUserName, programQueueDetails.RabbitPassword, programQueueDetails.RabbitHost);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return IsSuccess;
        }

        private string GetConnection(string ProgramCode)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("@program", ProgramCode);
            var data = GetDataFromDatabase(AppConfig.DBConnectionToGetWACreds, AppConfig.GetProgramWiseConnection, param);
            var connection = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(data);
            return connection[0]["connection"];
        }

        public bool SendImageAsync(SendImageMessageDetails message)
        {
            bool IsSuccess = false;
            try
            {
                SendImageQueuePayload payload = new SendImageQueuePayload();
                payload.TextToReply = message.TextToReply;
                payload.To = message.To;
                payload.WhatsAppCredentials = GetWACredentials(message.ProgramCode);
                payload.ImageUrl = message.ImageUrl;

                ProgramQueueDetails programQueueDetails = new ProgramQueueDetails();
                programQueueDetails = GetProgramQueueDetails(message.ProgramCode);

                IsSuccess = PushDataInExchange(JsonConvert.SerializeObject(payload), programQueueDetails.ExchangeName, programQueueDetails.SendImageQueue, programQueueDetails.RabbitUserName, programQueueDetails.RabbitPassword, programQueueDetails.RabbitHost);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return IsSuccess;
        }


        public List<CustomerResponseModel> GetCustomerResponse()
        {
            List<CustomerResponseModel> response = new List<CustomerResponseModel>();
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = AppConfig.Host,
                    //Port = Convert.ToInt32(_queueCredentials.Port),
                    UserName = AppConfig.UserName,
                    Password = AppConfig.Password
                };

                using (IConnection connection = factory.CreateConnection())
                {
                    using (IModel channel = connection.CreateModel())
                    {
                        BasicGetResult result;
                        do
                        {
                            channel.QueueDeclare(AppConfig.CustomerResponseQueue, true, false, false, null);
                            var consumer = new EventingBasicConsumer(channel);
                            result = channel.BasicGet(AppConfig.CustomerResponseQueue, true);
                            if (result != null)
                            {
                                var message = JsonConvert.DeserializeObject<CustomerResponseModel>(Encoding.UTF8.GetString(result.Body.ToArray()));
                                response.Add(message);
                            }
                        } while (result != null);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.ToString());
            }
            return response;
        }

        #region Commented Code for Rabbit MQ Push
        /*
        private bool PushDataInExchange(string message, string routingKey)
        {
            bool IsSucess = false;
            try
            {
                var connectionFactory = new ConnectionFactory()
                {
                    UserName = AppConfig.UserName,
                    Password = AppConfig.Password,
                    HostName = AppConfig.Host
                };

                var connection = connectionFactory.CreateConnection();
                var model = connection.CreateModel();
                var properties = model.CreateBasicProperties();
                properties.Persistent = true;
                byte[] messagebuffer = Encoding.Default.GetBytes(message);
                model.BasicPublish(AppConfig.Exchange, routingKey, properties, messagebuffer);
                IsSucess = true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return IsSucess;
        }
        */
        #endregion

        private bool PushDataInExchange(string message, string exchange, string routingKey, string username, string password, string host)
        {
            bool IsSuccess = false;
            try
            {
                var factory = new ConnectionFactory()
                {
                    UserName = username,
                    Password = password,
                    HostName = host
                };

                if (factory != null)
                {
                    using (var connection = factory.CreateConnection())
                    {
                        if (connection != null)
                        {
                            using (var channel = connection.CreateModel())
                            {
                                if (channel != null)
                                {
                                    byte[] messagebuffer = Encoding.Default.GetBytes(message);
                                    if (messagebuffer != null)
                                    {
                                        var properties = channel.CreateBasicProperties();
                                        properties.Persistent = true;
                                        channel.BasicPublish(exchange, routingKey, properties, messagebuffer);
                                        IsSuccess = true;
                                    }
                                }
                                else
                                {
                                    IsSuccess = false;
                                }
                            }
                        }
                        else
                        {
                            IsSuccess = false;
                        }
                    }
                }
                else
                {
                    IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return IsSuccess;
        }

        private WhatsAppApiCredentials GetWACredentials(string ProgramCode)
        {
            string data = string.Empty;
            List<WhatsAppApiCredentials> lstWhatsAppApiCredentials = null;
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("@program", ProgramCode);
                data = GetDataFromDatabase(AppConfig.DBConnectionToGetWACreds, AppConfig.SPtoGetWACreds, param);
                if (!string.IsNullOrEmpty(data))
                {
                    lstWhatsAppApiCredentials = new List<WhatsAppApiCredentials>();
                    lstWhatsAppApiCredentials = JsonConvert.DeserializeObject<List<WhatsAppApiCredentials>>(data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return lstWhatsAppApiCredentials[0];
        }

        private ProgramQueueDetails GetProgramQueueDetails(string ProgramCode)
        {
            string data = string.Empty;
            List<ProgramQueueDetails> lstProgramQueueDetails = null;
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("@program", ProgramCode);
                data = GetDataFromDatabase(AppConfig.DBConnectionToGetWACreds, AppConfig.SPToGetQueueDetails, param);
                if (!string.IsNullOrEmpty(data))
                {
                    lstProgramQueueDetails = new List<ProgramQueueDetails>();
                    lstProgramQueueDetails = JsonConvert.DeserializeObject<List<ProgramQueueDetails>>(data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return lstProgramQueueDetails[0];
        }

        private string GetDataFromDatabase(string connectionString, string command, IDictionary<string, string> additionalFilter = null)
        {
            string dataResult = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(command, connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 30;

                        if (additionalFilter != null)
                        {
                            foreach (var data in additionalFilter)
                            {
                                cmd.Parameters.AddWithValue(data.Key, data.Value);
                            }
                        }

                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            DataSet data = new DataSet();
                            da.Fill(data);
                            dataResult = JsonConvert.SerializeObject(data.Tables[0], Formatting.Indented);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return dataResult;
        }

        public List<ItemDetails> GetItemsByArticlesSKUID(string SearchCriteria, string ProgramCode, out bool IsProgramConfigured)
        {
            List<ItemDetails> objItemDetails = new List<ItemDetails>();
            IsProgramConfigured = true;
            try
            {
                string strConnection = GetConnection(ProgramCode, out bool isProgExists);
                if (isProgExists)
                {
                    using (MySqlConnection Conn = new MySqlConnection(strConnection))
                    {
                        if (Conn.State != ConnectionState.Open)
                            Conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(AppConfig.GetATVDataByMobileNumber, Conn))
                        {

                            string SQLCommand = Sqlqueries.GetItemsByArticlesSKUID.Replace("##SearchCriteria", SearchCriteria);

                            cmd.CommandText = SQLCommand;
                            MySqlDataReader objReader = cmd.ExecuteReader();
                            if (objReader.HasRows)
                            {
                                while (objReader.Read())
                                {
                                    ItemDetails objItemDetail = new ItemDetails();
                                    if (ProgramCode.Trim().ToLower() == "bataclub") /// this change made by Shalini, that for bataclub only 4 properties show, for other all
                                    {
                                        objItemDetail.ProductName = Convert.ToString(objReader["ProductName"]);
                                        objItemDetail.UniqueItemCode = Convert.ToString(objReader["uniqueitemcode"]);
                                        objItemDetail.Price = Convert.ToString(objReader["price"]);
                                        objItemDetail.ImageURL = Convert.ToString(objReader["imageurl"]);
                                        objItemDetail.Discount = string.Empty;
                                        objItemDetail.URL = string.Empty;
                                    }
                                    else
                                    {
                                        objItemDetail.ProductName = Convert.ToString(objReader["ProductName"]);
                                        objItemDetail.UniqueItemCode = Convert.ToString(objReader["uniqueitemcode"]);
                                        objItemDetail.CategoryName = Convert.ToString(objReader["CategoryName"]);
                                        objItemDetail.SubCategoryName = Convert.ToString(objReader["SubCategoryName"]);
                                        objItemDetail.Color = Convert.ToString(objReader["Colour"]);
                                        //objItemDetail.ColorCode = Convert.ToString(objReader["ColorCode"]);
                                        objItemDetail.BrandName = Convert.ToString(objReader["BrandName"]);
                                        objItemDetail.Price = Convert.ToString(objReader["price"]);
                                        objItemDetail.Discount = Convert.ToString(objReader["discount"]);
                                        objItemDetail.URL = Convert.ToString(objReader["url"]);
                                        objItemDetail.ImageURL = Convert.ToString(objReader["imageurl"]);
                                        objItemDetail.Size = Convert.ToString(objReader["Size"]);
                                    }
                                    objItemDetails.Add(objItemDetail);
                                }
                            }
                        }
                    }
                }
                else
                {
                    IsProgramConfigured = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return objItemDetails;
        }


        public async Task<APIResponse> GetNearbyStoreDetails(string ProgramCode, string latitude, string longitude, string distance, string Pincode)
        {
            APIResponse objResponse = new APIResponse();
            List<Store> objStoreList = new List<Store>();
            try
            {
                string strConnection = GetConnection(ProgramCode, out bool isProgExists);
                if (isProgExists)
                {
                    using (MySqlConnection Conn = new MySqlConnection(strConnection))
                    {
                        if (Conn.State != ConnectionState.Open)
                            Conn.Open();
                        string SQLQuery = String.Empty;
                        if (Pincode == null)
                            SQLQuery = Sqlqueries.GetNearbyStoreDetails.Replace("##p_lattitude", latitude)
                            .Replace("##p_logitude", longitude).Replace("##p_distance", distance).Replace("##ProgramCode", ProgramCode);
                        else
                            SQLQuery = Sqlqueries.GetNearbyStoreDetailsByPincode.Replace("##p_pincode", Pincode).Replace("##ProgramCode", ProgramCode);

                        using (MySqlCommand cmd = new MySqlCommand(SQLQuery, Conn))
                        {
                            using (DbDataReader objReader = await cmd.ExecuteReaderAsync())
                            {
                                if (objReader.HasRows)
                                {
                                    while (await objReader.ReadAsync())
                                    {
                                        Store objStore = new Store();
                                        objStore.StoreCode = Convert.ToString(objReader["StoreCode"]);
                                        objStore.StoreName = Convert.ToString(objReader["StoreName"]);
                                        objStore.StoreStatus = Convert.ToString(objReader["StoreStatus"]);
                                        objStore.Address1 = Convert.ToString(objReader["Address1"]);
                                        objStore.Address2 = Convert.ToString(objReader["Address2"]);
                                        objStore.City = Convert.ToString(objReader["City"]);
                                        objStore.State = Convert.ToString(objReader["State"]);
                                        objStore.Zone = Convert.ToString(objReader["Zone"]);
                                        objStore.Region = Convert.ToString(objReader["Region"]);
                                        objStore.Country = Convert.ToString(objReader["Country"]);
                                        objStore.StoreTiming = Convert.ToString(objReader["StoreTiming"]);
                                        objStore.latitude = Convert.ToString(objReader["Latitude"]);
                                        objStore.longitude = Convert.ToString(objReader["Longitude"]);
                                        objStore.PinCode = Convert.ToString(objReader["Pincode"]);
                                        objStore.CurrentStatus = Convert.ToString(objReader["DayStatus"]);
                                        objStore.ContactNumber = Convert.ToString(objReader["StoreManagerMobile"]);
                                        objStoreList.Add(objStore);
                                    }
                                    objResponse.Data = objStoreList;
                                }
                                else
                                {
                                    objResponse.isError = true;
                                    objResponse.Data = 1002;
                                }
                            }
                        }
                    }
                }
                else
                {
                    objResponse.isError = true;
                    objResponse.Data = 225;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return objResponse;
        }

        public async Task<List<WhatsappMessageDetailsResponse>> GetWhatsappMessageDetails(string ProgramCode)
        {
            List<WhatsappMessageDetailsResponse> lstWhatsappMessageDetailsResponse = null;
            try
            {
                using (MySqlConnection Conn = new MySqlConnection(AppConfig.DBConnectionToGetWACreds))
                {
                    if (Conn.State != ConnectionState.Open)
                    {
                        Conn.Open();
                        string SQLQuery = "SELECT Whatsapp_Program_Code, Whatsapp_Template_Name, Whatsapp_Template_Namespace, Whatsapp_Template_Text, Whatsapp_Language, Remarks FROM cb_program_whatsapp_message_details WHERE IS_Active = 1 AND Whatsapp_Program_Code = '" + ProgramCode + "';";
                        using (MySqlCommand cmd = new MySqlCommand(SQLQuery, Conn))
                        {
                            MySqlDataReader objReader = cmd.ExecuteReader();
                            if (objReader.HasRows)
                            {
                                lstWhatsappMessageDetailsResponse = new List<WhatsappMessageDetailsResponse>();
                                while (await objReader.ReadAsync())
                                {
                                    WhatsappMessageDetailsResponse objWhatsappMessageDetailsResponse = new WhatsappMessageDetailsResponse()
                                    {
                                        ProgramCode = objReader[0] == DBNull.Value ? string.Empty : Convert.ToString(objReader[0]),
                                        TemplateName = objReader[1] == DBNull.Value ? string.Empty : Convert.ToString(objReader[1]),
                                        TemplateNamespace = objReader[2] == DBNull.Value ? string.Empty : Convert.ToString(objReader[2]),
                                        TemplateText = objReader[3] == DBNull.Value ? string.Empty : Convert.ToString(objReader[3]),
                                        TemplateLanguage = objReader[4] == DBNull.Value ? string.Empty : Convert.ToString(objReader[4]),
                                        Remarks = objReader[5] == DBNull.Value ? string.Empty : Convert.ToString(objReader[5])
                                    };
                                    lstWhatsappMessageDetailsResponse.Add(objWhatsappMessageDetailsResponse);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return lstWhatsappMessageDetailsResponse;
        }

        public async Task<APIResponse> GetStoreDetailsByStoreCode(string ProgramCode, string StoreCode)
        {
            APIResponse objResponse = new APIResponse();
            StoreInfo objStore = new StoreInfo();
            try
            {

                string strConnection = GetConnection(ProgramCode, out bool isProgExists);
                if (isProgExists)
                {
                    using (MySqlConnection Conn = new MySqlConnection(strConnection))
                    {
                        if (Conn.State != ConnectionState.Open)
                            Conn.Open();
                        string SQLQuery = String.Empty;
                        SQLQuery = Sqlqueries.GetStoreDetailsByStoreCode.Replace("##p_storecode", StoreCode);

                        using (MySqlCommand cmd = new MySqlCommand(SQLQuery, Conn))
                        {
                            using (DbDataReader objReader = await cmd.ExecuteReaderAsync())
                            {
                                if (objReader.HasRows)
                                {
                                    while (await objReader.ReadAsync())
                                    {
                                        objStore.StoreCode = Convert.ToString(objReader["StoreCode"]);
                                        objStore.StoreName = Convert.ToString(objReader["DisplayName"]);
                                        objStore.Address = Convert.ToString(objReader["Address"]);
                                        objStore.City = Convert.ToString(objReader["City"]);
                                        objStore.State = Convert.ToString(objReader["State"]);
                                        objStore.Zone = Convert.ToString(objReader["Zone"]);
                                        objStore.Region = Convert.ToString(objReader["Region"]);
                                        objStore.Country = Convert.ToString(objReader["country"]);
                                        objStore.StoreTiming = Convert.ToString(objReader["storetiming"]);
                                        objStore.latitude = Convert.ToString(objReader["latitude"]);
                                        objStore.longitude = Convert.ToString(objReader["longitude"]);
                                        objStore.PinCode = Convert.ToString(objReader["pincode"]);
                                        objStore.StoreGSTNo = Convert.ToString(objReader["StoreGSTNo"]);
                                        objStore.StoreCIN = Convert.ToString(objReader["StoreCIN"]);
                                        objStore.StorePan = Convert.ToString(objReader["StorePan"]);
                                    }
                                    objResponse.Data = objStore;
                                }
                                else
                                {
                                    objResponse.isError = true;
                                    objResponse.Data = 1002;
                                }
                            }
                        }

                    }
                }
                else
                {
                    objResponse.isError = true;
                    objResponse.Data = 225;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return objResponse;
        }

        //public async Task<APIResponse> GetSaveAppointmentResponse(SaveAppointmentRequest saveAppointmentRequest)
        //{
        //    APIResponse objResponse = new APIResponse();
        //    try
        //    {
        //        Dictionary<string, string> param = new Dictionary<string, string>();

        //        param.Add("@Appointment_Date", saveAppointmentRequest.AppointmentDate.ToString("dd-MMM-yyyy"));
        //        param.Add("@Mobile_No", saveAppointmentRequest.MobileNo);
        //        param.Add("@ProgCode", saveAppointmentRequest.ProgCode);
        //        param.Add("@StoreCode", saveAppointmentRequest.StoreCode);
        //        param.Add("@Slot_ID", saveAppointmentRequest.SlotID.ToString());
        //        param.Add("@NOof_People", saveAppointmentRequest.NOofPeople.ToString());
        //        var data = GetDataFromDatabase(AppConfig.BellConnection, AppConfig.SptoSaveAppointment, param);
        //        List<SaveAppointmentResponse> objApp = new List<SaveAppointmentResponse>();
        //        objApp  = await Task.Run(() => JsonConvert.DeserializeObject<List<SaveAppointmentResponse>>(data));
        //        if (objApp[0].AppointmentID == -1)
        //        {
        //            objResponse.isError = true;
        //            objResponse.Data = 1004;
        //        }
        //        else
        //            objResponse.Data = objApp;
        //    }
        //    catch (Exception ex)
        //    {
        //        objResponse.isError = true; 
        //        throw ex;
        //    }

        //    return objResponse;
        //}

        public async Task<APIResponse> GetHSNCodeBySKUID(ItemSearch objItemSearch)
        {
            APIResponse objResponse = new APIResponse();
            ItemHSN objItemDetails = new ItemHSN();
            try
            {
                string strConnection = GetConnection(objItemSearch.programcode.Trim(), out bool isProgExists);
                if (isProgExists)
                {
                    using (MySqlConnection Conn = new MySqlConnection(strConnection))
                    {
                        if (Conn.State != ConnectionState.Open)
                            Conn.Open();
                        string SQLQuery = String.Empty;

                        SQLQuery = Sqlqueries.GetHSNCodeBySKUID.Replace("##SKUID", objItemSearch.SearchCriteria.Trim());

                        using (MySqlCommand cmd = new MySqlCommand(SQLQuery, Conn))
                        {
                            using (DbDataReader objReader = await cmd.ExecuteReaderAsync())
                            {
                                if (objReader.HasRows)
                                {
                                    while (await objReader.ReadAsync())
                                    {
                                        objItemDetails.HSNCode = Convert.ToString(objReader["HSNCode"]);
                                        objItemDetails.SKUID = Convert.ToString(objReader["UniqueItemCode"]);
                                        objItemDetails.ItemName = Convert.ToString(objReader["UniqueItemName"]);
                                    }
                                    objResponse.Data = objItemDetails;
                                }
                                else
                                {
                                    objResponse.isError = true;
                                    objResponse.Data = 1002;
                                }
                            }
                        }
                    }
                }
                else
                {
                    objResponse.isError = true;
                    objResponse.Data = 225;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return objResponse;
        }

        public APIResponse GetTenantConfig(string ProgramCode)
        {
            APIResponse objResponse = new APIResponse();
            try
            {
                using (MySqlConnection Conn = new MySqlConnection(AppConfig.ConnectionString))
                {
                    if (Conn.State != ConnectionState.Open)
                        Conn.Open();
                    string SQLQuery = String.Empty;

                    SQLQuery = Sqlqueries.GetProgConfigDetails.Replace("##ProgCode", ProgramCode);

                    using (MySqlCommand cmd = new MySqlCommand(SQLQuery, Conn))
                    {

                        using (DbDataReader objReader = cmd.ExecuteReader())
                        {
                            if (objReader.HasRows)
                            {
                                TenantConfig tenantConfig = new TenantConfig();
                                while (objReader.Read())
                                {
                                    tenantConfig.ProgramCode = Convert.ToString(objReader["ProgramCode"]);
                                    tenantConfig.ProgramName = Convert.ToString(objReader["ProgramName"]);
                                    tenantConfig.IsWhatsApp = Convert.ToChar(objReader["IsWhatsApp"]);
                                    tenantConfig.IsBell = Convert.ToChar(objReader["IsBell"]);
                                    tenantConfig.IsWebBot = Convert.ToChar(objReader["IsWebBot"]);
                                    tenantConfig.IsShoppingBag = Convert.ToChar(objReader["IsShoppingBag"]);
                                    tenantConfig.IsERPayment = Convert.ToChar(objReader["IsERPayment"]);
                                    tenantConfig.IsERShipping = Convert.ToChar(objReader["IsERShipping"]);
                                    tenantConfig.IsOrderSync = Convert.ToChar(objReader["IsOrderSync"]);
                                    tenantConfig.IsSendToShipmentManual = Convert.ToChar(objReader["IsSendToShipmentManual"]);
                                    tenantConfig.IsDigitalReceipt = Convert.ToChar(objReader["IsDigitalReceipt"]);
                                    tenantConfig.IsTaxBeforDiscount = Convert.ToChar(objReader["IsTaxBeforDiscount"]);
                                }
                                objResponse.Data = tenantConfig;
                            }
                            else
                            {
                                objResponse.isError = true;
                                objResponse.Data = 225;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return objResponse;
        }

        public APIResponse GetTenantMaster()
        {
            APIResponse objResponse = new APIResponse();
            try
            {
                using (MySqlConnection Conn = new MySqlConnection(AppConfig.ConnectionString))
                {
                    if (Conn.State != ConnectionState.Open)
                        Conn.Open();
                    string SQLQuery = String.Empty;

                    SQLQuery = Sqlqueries.GetProgList;

                    using (MySqlCommand cmd = new MySqlCommand(SQLQuery, Conn))
                    {
                        using (DbDataReader objReader = cmd.ExecuteReader())
                        {
                            if (objReader.HasRows)
                            {
                                List<TenantConfig> tenantConfigs = new List<TenantConfig>();
                                while (objReader.Read())
                                {
                                    TenantConfig tenantConfig = new TenantConfig();
                                    tenantConfig.ProgramCode = Convert.ToString(objReader["ProgramCode"]);
                                    tenantConfig.ProgramName = Convert.ToString(objReader["ProgramName"]);
                                    tenantConfig.IsWhatsApp = Convert.ToChar(objReader["IsWhatsApp"]);
                                    tenantConfig.IsBell = Convert.ToChar(objReader["IsBell"]);
                                    tenantConfig.IsWebBot = Convert.ToChar(objReader["IsWebBot"]);
                                    tenantConfig.IsShoppingBag = Convert.ToChar(objReader["IsShoppingBag"]);
                                    tenantConfig.IsERPayment = Convert.ToChar(objReader["IsERPayment"]);
                                    tenantConfig.IsERShipping = Convert.ToChar(objReader["IsERShipping"]);
                                    tenantConfig.IsOrderSync = Convert.ToChar(objReader["IsOrderSync"]);
                                    tenantConfig.IsSendToShipmentManual = Convert.ToChar(objReader["IsSendToShipmentManual"]);
                                    tenantConfig.IsDigitalReceipt = Convert.ToChar(objReader["IsDigitalReceipt"]);
                                    tenantConfig.IsTaxBeforDiscount = Convert.ToChar(objReader["IsTaxBeforDiscount"]);
                                    tenantConfigs.Add(tenantConfig);
                                }
                                objResponse.Data = tenantConfigs;
                            }
                            else
                            {
                                objResponse.isError = true;
                                objResponse.Data = 225;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return objResponse;
        }

        public StoreCodeDetails GetShopsterStoreCode(string MobileNumber, string ProgramCode)
        {
            StoreCodeDetails objResponse = new StoreCodeDetails();
            Transaction objLastTransaction = new Transaction();
            try
            {
                var UserStore = "DefaultLogic"; // store selected 
                GetConnection(ProgramCode, out bool isProgExists);
                string strConnection = string.Format(AppConfig.BellDBConnection, ProgramCode);
                if (isProgExists)
                {
                    //Dictionary<string, string> param = new Dictionary<string, string>();
                    //param.Add("Program_Code", ProgramCode);
                    //var data = GetDataFromDatabase(AppConfig.ConnectionString, AppConfig.SPtoCheckProgramBellStatus, param);
                    //var connection = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(data);
                    //if (connection[0]["IsBell"].ToString().ToUpper() == "Y")
                    //{
                    string StoreCode = GetStorecode(UserStore, MobileNumber, strConnection);
                    if (!string.IsNullOrEmpty(StoreCode))
                    {
                        objResponse.StoreMessage = "Store Found";
                        objResponse.StoreCode = StoreCode;
                        objResponse.OpeningTime = string.Empty;
                        objResponse.ClosingTime = string.Empty;
                        objResponse.StartDate = string.Empty;
                        objResponse.EndDate = string.Empty;
                        objResponse.Address = string.Empty;
                        objResponse.Pincode = string.Empty;
                        objResponse.Latitude = string.Empty;
                        objResponse.Longitude = string.Empty;
                    }
                    else
                    {
                        objResponse.StoreMessage = AppConfig.MessageNoStoreCodeFound;
                        objResponse.StoreCode = string.Empty;
                        objResponse.OpeningTime = string.Empty;
                        objResponse.ClosingTime = string.Empty;
                        objResponse.StartDate = string.Empty;
                        objResponse.EndDate = string.Empty;
                        objResponse.Address = string.Empty;
                        objResponse.Pincode = string.Empty;
                        objResponse.Latitude = string.Empty;
                        objResponse.Longitude = string.Empty;
                    }

                    //string SQLQuery = String.Empty;

                    //SQLQuery = Sqlqueries.GetStoreDetailsMaster.Replace("##p_storecode", storeCode);

                    //using (MySqlConnection Conn = new MySqlConnection(strConnection))
                    //{
                    //    if (Conn.State != ConnectionState.Open)
                    //    {
                    //        Conn.Open();
                    //    }
                    //    using (MySqlCommand cmd = new MySqlCommand(SQLQuery, Conn))
                    //    {
                    //        using (DbDataReader objReader = cmd.ExecuteReader())
                    //        {
                    //            if (objReader.HasRows)
                    //            {
                    //                while (objReader.Read())
                    //                {
                    //                    objResponse.StoreMessage = "Store Found";
                    //                    objResponse.StoreCode = Convert.ToString(objReader["StoreCode"]);
                    //                    objResponse.OpeningTime = Convert.ToString(objReader["OpeningTime"]);
                    //                    objResponse.ClosingTime = Convert.ToString(objReader["ClosingTime"]);
                    //                    objResponse.StartDate = string.Empty;//Convert.ToString(objReader["StartDate"]);
                    //                    objResponse.EndDate = string.Empty; //Convert.ToString(objReader["EndDate"]);
                    //                    objResponse.Address = Convert.ToString(objReader["Address"]);
                    //                    objResponse.Pincode = Convert.ToString(objReader["pincode"]);
                    //                    objResponse.Latitude = Convert.ToString(objReader["latitude"]);
                    //                    objResponse.Longitude = Convert.ToString(objReader["longitude"]);
                    //                }
                    //            }
                    //            else
                    //            {
                    //                objResponse.StoreMessage = AppConfig.MessageNoStoreCodeFound;
                    //            }
                    //        }
                    //    }
                    //}
                    //}
                    //else
                    //{
                    //    objResponse.StoreMessage = AppConfig.MessageNoStoreCodeFound;
                    //    objResponse.StoreCode = string.Empty;
                    //    objResponse.OpeningTime = string.Empty;
                    //    objResponse.ClosingTime = string.Empty;
                    //    objResponse.StartDate = string.Empty;
                    //    objResponse.EndDate = string.Empty;
                    //    objResponse.Address = string.Empty;
                    //    objResponse.Pincode = string.Empty;
                    //    objResponse.Latitude = string.Empty;
                    //    objResponse.Longitude = string.Empty;
                    //}
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return objResponse;
        }

        private string GetStorecode(string selectedStore, string mobileNo, string strConnection)
        {
            string storeCode = "";
            string SQLQuery = String.Empty;
            try
            {
                if (selectedStore == "DefaultLogic")
                {
                    SQLQuery = String.Empty;
                    SQLQuery = Sqlqueries.GetStoreCodeByCampaignDetail.Replace("##p_Mobile", mobileNo);
                    storeCode = returnedStoreCode(mobileNo, strConnection, SQLQuery);

                    //if (storeCode.Trim() == "")
                    //{
                    //    SQLQuery = String.Empty;

                    //    SQLQuery = Sqlqueries.GetStoreCodeByLastTransaction.Replace("##p_Mobile", mobileNo);
                    //    storeCode = returnedStoreCode(mobileNo, strConnection, SQLQuery);

                    //    if (storeCode.Trim() == "")
                    //    {
                    //        SQLQuery = String.Empty;

                    //        SQLQuery = Sqlqueries.GetStoreCodeByMemberReport.Replace("##p_Mobile", mobileNo);
                    //        storeCode = returnedStoreCode(mobileNo, strConnection, SQLQuery);

                    //        if (storeCode.Trim() == "")
                    //        {
                    //            storeCode = AppConfig.DefaultStoreCode.ToString();
                    //        }
                    //    }
                    //}
                }
                else if (selectedStore == "CampaignStore")
                {
                    SQLQuery = String.Empty;

                    SQLQuery = Sqlqueries.GetStoreCodeByCampaignDetail.Replace("##p_Mobile", mobileNo);
                    storeCode = returnedStoreCode(mobileNo, strConnection, SQLQuery);
                }
                else if (selectedStore == "LastTransaction")
                {
                    SQLQuery = String.Empty;

                    SQLQuery = Sqlqueries.GetStoreCodeByLastTransaction.Replace("##p_Mobile", mobileNo);
                    storeCode = returnedStoreCode(mobileNo, strConnection, SQLQuery);
                }
                else if (selectedStore == "MemberStore")
                {
                    SQLQuery = String.Empty;

                    SQLQuery = Sqlqueries.GetStoreCodeByMemberReport.Replace("##p_Mobile", mobileNo);
                    storeCode = returnedStoreCode(mobileNo, strConnection, SQLQuery);
                }
                else if (selectedStore == "Default")
                {
                    storeCode = AppConfig.DefaultStoreCode.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return storeCode;
        }

        private string returnedStoreCode(string mobileNo, string strConnection, string SQLQuery)
        {
            string StoreCode = "";
            using (MySqlConnection Conn = new MySqlConnection(strConnection))
            {
                if (Conn.State != ConnectionState.Open)
                {
                    Conn.Open();
                }
                using (MySqlCommand cmd = new MySqlCommand(SQLQuery, Conn))
                {
                    using (DbDataReader objReader = cmd.ExecuteReader())
                    {
                        if (objReader.HasRows)
                        {
                            while (objReader.Read())
                            {
                                StoreCode = Convert.ToString(objReader["StoreCode"]);
                            }
                        }
                    }
                }
            }
            return StoreCode;
        }

        public ItemImageLocationResponse GetItemImageLocation(string ProgramCode)
        {
            ItemImageLocationResponse objResponse = new ItemImageLocationResponse();
            try
            {
                string strConnection = GetConnection(ProgramCode, out bool isProgExists);
                if (isProgExists)
                {
                    string ImageUrl = GetImageLocation(ProgramCode, strConnection);
                    if (!string.IsNullOrEmpty(ImageUrl))
                    {
                        objResponse.ProgramCode = ProgramCode;
                        objResponse.ImageUrl = ImageUrl;
                    }
                    else
                    {
                        objResponse.ProgramCode = ProgramCode;
                        objResponse.ImageUrl = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return objResponse;
        }

        private string GetImageLocation(string programCode, string strConnection)
        {
            string ItemImagelocation = string.Empty;
            string SQLQuery = String.Empty;
            try
            {
                SQLQuery = String.Empty;
                SQLQuery = Sqlqueries.GetImageLocation.Replace("##ProgCode", programCode);
                using (MySqlConnection Conn = new MySqlConnection(strConnection))
                {
                    if (Conn.State != ConnectionState.Open)
                    {
                        Conn.Open();
                    }
                    using (MySqlCommand cmd = new MySqlCommand(SQLQuery, Conn))
                    {
                        using (DbDataReader objReader = cmd.ExecuteReader())
                        {
                            if (objReader.HasRows)
                            {
                                while (objReader.Read())
                                {
                                    ItemImagelocation = Convert.ToString(objReader["ImageUrl"]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return ItemImagelocation;
        }

        public List<LastShoppingInvoice> GetLastShoppingReceipt(LastShoppingInvoiceRequest ObjLastShoppingInvoiceRequest)
        {
            string response = string.Empty;
            List<LastShoppingInvoice> responseList = new List<LastShoppingInvoice>();
            try
            {
                if (ObjLastShoppingInvoiceRequest.MobileNumber.Trim() != string.Empty)
                {
                    // // Method to prepare the json request Body for Elastic Search.
                    string ApiRequest = PrepareJsonRequestForDigitalReceiptElasticSearch(ObjLastShoppingInvoiceRequest);

                    // // Elastic search Url creation for Last Invoice and get result from api.
                    string APIURL = string.Format(AppConfig.InfoAPIDigitalReceiptUrl, AppConfig.LastShoppingReceipt.ToLower(), AppConfig.LastShoppingReceipt.ToLower());
                    response = GetAPIResponse(ApiRequest, APIURL);

                    // // Deserialize json response in Model format
                    var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response);

                    responseList = new List<LastShoppingInvoice>();
                    foreach (var items in jsonResult.hits.hits)
                    {
                        LastShoppingInvoice objResponse = new LastShoppingInvoice();
                        objResponse.BillDate = (DateTime)items["_source"]["BillDate"];
                        objResponse.ShoppingInvoice = items["_source"]["BitLy"];
                        responseList.Add(objResponse);
                    }

                    if (responseList.Count > 0)
                    {
                        // // Sort the last 3 months records in descending order and take only latest one.
                        responseList = responseList.Where(m => m.BillDate > DateTime.Today.AddMonths(-3))
                                                              .OrderByDescending(m => m.BillDate).Take(1).ToList();
                        return responseList;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return responseList;
        }

        private string PrepareJsonRequestForDigitalReceiptElasticSearch(LastShoppingInvoiceRequest ObjLastShoppingInvoiceRequest)
        {
            string jsonRequest = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"query\": {");
                sb.Append("\"bool\": {");
                sb.Append("\"must\": [");
                sb.Append("{");
                sb.Append("\"match\": {");
                sb.AppendFormat("\"MobileNo\": \"{0}\"", ObjLastShoppingInvoiceRequest.MobileNumber);
                sb.Append("}");
                sb.Append("}");

                sb.Append(",");

                sb.Append("{");
                sb.Append("\"match\": {");
                sb.AppendFormat("\"ProgCode\": \"{0}\"", ObjLastShoppingInvoiceRequest.ProgramCode);
                sb.Append("}");
                sb.Append("}");

                sb.Append("]");
                sb.Append("}");
                sb.Append("}");
                sb.Append("}");

                jsonRequest = sb.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return jsonRequest;
        }

        private string GetAPIResponse(string request, string WebApiUrl)
        {
            string responseStr = string.Empty;
            string requestStr = Regex.Replace(request, @"\t|\n|\r", "");

            try
            {
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(WebApiUrl.ToLower());
                byte[] bytes = Encoding.ASCII.GetBytes(requestStr);
                webReq.ContentType = "application/json; encoding='utf-8'";
                webReq.ContentLength = bytes.Length;
                webReq.Method = "POST";
                Stream requestStream = webReq.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream responseStream = response.GetResponseStream();
                        responseStr = new StreamReader(responseStream).ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            return responseStr;
        }


        //public List<AppointmentDetails> ScheduleVisit(AppointmentMaster appointmentMaster)
        //{

        //    DataSet ds = new DataSet();
        //    MySqlConnection connection= null;
        //    string conn= "server=10.14.0.11;user=MukeshBenjwal;database=chatbotbtoc;port=3306;password=Mukesh@321;SslMode=none;";
        //    List<AppointmentDetails> lstAppointmentDetails = new List<AppointmentDetails>();
        //    try
        //    {
        //        using (connection = new MySqlConnection(conn))
        //        {

        //            using (MySqlCommand cmd = new MySqlCommand("SP_HSScheduleVisit", connection))
        //            {

        //                cmd.Parameters.AddWithValue("@Customer_ID", appointmentMaster.CustomerID);
        //                cmd.Parameters.AddWithValue("@Appointment_Date", appointmentMaster.AppointmentDate);
        //                cmd.Parameters.AddWithValue("@Slot_ID", appointmentMaster.SlotID);
        //                cmd.Parameters.AddWithValue("@Tenant_ID", appointmentMaster.TenantID);
        //                cmd.Parameters.AddWithValue("@Created_By", appointmentMaster.CreatedBy);
        //                cmd.Parameters.AddWithValue("@NOof_People", appointmentMaster.NOofPeople);
        //                cmd.Parameters.AddWithValue("@Mobile_No", appointmentMaster.MobileNo);
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                //message = Convert.ToInt32(cmd.ExecuteScalar());
        //                MySqlDataAdapter da = new MySqlDataAdapter
        //                {
        //                    SelectCommand = cmd
        //                };
        //                da.Fill(ds);
        //                if (ds != null && ds.Tables[0] != null)
        //                {
        //                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                    {
        //                        AppointmentDetails appointmentDetails = new AppointmentDetails();
        //                        appointmentDetails.AppointmentID = Convert.ToInt32(ds.Tables[0].Rows[i]["AppointmentID"]);
        //                        appointmentDetails.CustomerName = ds.Tables[0].Rows[i]["CustomerName"] == DBNull.Value ? string.Empty : Convert.ToString(ds.Tables[0].Rows[i]["CustomerName"]);
        //                        appointmentDetails.CustomerMobileNo = ds.Tables[0].Rows[i]["MobileNo"] == DBNull.Value ? string.Empty : Convert.ToString(ds.Tables[0].Rows[i]["MobileNo"]);
        //                        appointmentDetails.StoreName = ds.Tables[0].Rows[i]["StoreName"] == DBNull.Value ? string.Empty : Convert.ToString(ds.Tables[0].Rows[i]["StoreName"]);
        //                        appointmentDetails.StoreAddress = ds.Tables[0].Rows[i]["StoreAddress"] == DBNull.Value ? string.Empty : Convert.ToString(ds.Tables[0].Rows[i]["StoreAddress"]);
        //                        appointmentDetails.NoOfPeople = ds.Tables[0].Rows[i]["NOofPeople"] == DBNull.Value ? string.Empty : Convert.ToString(ds.Tables[0].Rows[i]["NOofPeople"]);
        //                        appointmentDetails.StoreManagerMobile = ds.Tables[0].Rows[i]["StoreManagerMobile"] == DBNull.Value ? string.Empty : Convert.ToString(ds.Tables[0].Rows[i]["StoreManagerMobile"]);

        //                        lstAppointmentDetails.Add(appointmentDetails);
        //                    }
        //                }
        //            }
        //        }

        //        // int response = SendMessageToCustomer( /*ChatID*/0, appointmentMaster.MobileNo, appointmentMaster.ProgramCode, appointmentMaster.MessageToReply,/*ClientAPIURL*/"",appointmentMaster.CreatedBy);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        if (conn != null)
        //        {
        //            connection.Close();
        //        }
        //    }
        //    return lstAppointmentDetails;
        //}


        public List<AppointmentResponse> GetTimeSlot(string StoreCode, int tenantID)
        {
            DataSet ds = new DataSet();
         //   MySqlCommand cmd = new MySqlCommand();
            List<DateofSchedule> lstdateofSchedule = new List<DateofSchedule>();
            MySqlConnection connection = null;
            string conn = "server=10.14.0.11;user=MukeshBenjwal;database=bataclubqa;port=3306;password=Mukesh@321;SslMode=none;";
            List<AppointmentResponse> lstAppointmentDetails = new List<AppointmentResponse>();
            AppointmentResponse appointmentResponse = new AppointmentResponse();

            try
            {
                using (connection = new MySqlConnection(conn))
                {
                    MySqlCommand cmd1 = new MySqlCommand("SP_HSGetTimeSlot_AshutoshG", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd1.Parameters.AddWithValue("@Tenant_Id", tenantID);
                    cmd1.Parameters.AddWithValue("@Store_Code", StoreCode);
                    //   cmd1.Parameters.AddWithValue("@Program_Code", ProgramCode);
                    //cmd1.Parameters.AddWithValue("@store_ID", storeID);
                    MySqlDataAdapter da = new MySqlDataAdapter
                    {
                        SelectCommand = cmd1
                    };
                    da.Fill(ds);

                    if (ds != null && ds.Tables[0] != null)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            appointmentResponse.BrandLogo = ds.Tables[0].Rows[i]["BrandLogo"] == DBNull.Value ? string.Empty : Convert.ToString(ds.Tables[0].Rows[i]["BrandLogo"]);
                            appointmentResponse.BrandName = ds.Tables[0].Rows[i]["BrandName"] == DBNull.Value ? string.Empty : Convert.ToString(ds.Tables[0].Rows[i]["BrandName"]);
                            appointmentResponse.StoreAddress = ds.Tables[0].Rows[i]["Address"] == DBNull.Value ? string.Empty : Convert.ToString(ds.Tables[0].Rows[i]["Address"]);
                            appointmentResponse.StoreContactDetails = ds.Tables[0].Rows[i]["StorePhoneNo"] == DBNull.Value ? string.Empty : Convert.ToString(ds.Tables[0].Rows[i]["StorePhoneNo"]);
                            appointmentResponse.StoreName = ds.Tables[0].Rows[i]["StoreName"] == DBNull.Value ? string.Empty : Convert.ToString(ds.Tables[0].Rows[i]["StoreName"]);
                                List<DateofSchedule> lstScheduleDetail = new List<DateofSchedule>();

                            for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
                            {
                                DateofSchedule dateofSchedule = new DateofSchedule();
                                dateofSchedule.Id = ds.Tables[1].Rows[j]["id"] == DBNull.Value ? string.Empty : Convert.ToString(ds.Tables[1].Rows[j]["id"]);
                                dateofSchedule.Day = ds.Tables[1].Rows[j]["Today"] == DBNull.Value ? string.Empty : Convert.ToString(ds.Tables[1].Rows[j]["Today"]);
                                dateofSchedule.Dates = ds.Tables[1].Rows[j]["Dates"] == DBNull.Value ? string.Empty : Convert.ToString(ds.Tables[1].Rows[j]["Dates"]);
                                DataTable dataTable = new DataTable();

                                if (j == 0)
                                {
                                    dataTable = ds.Tables[2];
                                }
                                else if (j == 1)
                                {
                                    dataTable = ds.Tables[3];
                                }
                                else if (j == 2)
                                {
                                    dataTable = ds.Tables[4];
                                }

                                List<AlreadyScheduleDetail> lstAlreadyScheduleDetail = new List<AlreadyScheduleDetail>();
                                if (dataTable != null)
                                {
                                    for (int k = 0; k < dataTable.Rows.Count; k++)
                                    {
                                        AlreadyScheduleDetail alreadyScheduleDetail = new AlreadyScheduleDetail();
                                        alreadyScheduleDetail.TimeSlotId = dataTable.Rows[k]["SlotId"] == DBNull.Value ? 0 : Convert.ToInt32(dataTable.Rows[k]["SlotId"]);
                                        alreadyScheduleDetail.AppointmentDate = dataTable.Rows[k]["AppointmentDate"] == DBNull.Value ? string.Empty : Convert.ToString(dataTable.Rows[k]["AppointmentDate"]);
                                        alreadyScheduleDetail.VisitedCount = dataTable.Rows[k]["VisitedCount"] == DBNull.Value ? 0 : Convert.ToInt32(dataTable.Rows[k]["VisitedCount"]);
                                        alreadyScheduleDetail.MaxCapacity = dataTable.Rows[k]["MaxCapacity"] == DBNull.Value ? 0 : Convert.ToInt32(dataTable.Rows[k]["MaxCapacity"]);
                                        alreadyScheduleDetail.Remaining = dataTable.Rows[k]["Remaining"] == DBNull.Value ? 0 : Convert.ToInt32(dataTable.Rows[k]["Remaining"]);
                                        alreadyScheduleDetail.TimeSlot = dataTable.Rows[k]["TimeSlot"] == DBNull.Value ? string.Empty : Convert.ToString(dataTable.Rows[k]["TimeSlot"]);
                                        alreadyScheduleDetail.StoreId = dataTable.Rows[k]["StoreId"] == DBNull.Value ? 0 : Convert.ToInt32(dataTable.Rows[k]["StoreId"]);
                                        alreadyScheduleDetail.IsDisabled = dataTable.Rows[k]["IsDisabled"] == DBNull.Value ? false : Convert.ToBoolean(dataTable.Rows[k]["IsDisabled"]);
                                        lstAlreadyScheduleDetail.Add(alreadyScheduleDetail);
                                    }
                                    dateofSchedule.AlreadyScheduleDetails = lstAlreadyScheduleDetail;
                                }
                                lstdateofSchedule.Add(dateofSchedule);
                            }
                            appointmentResponse.dateofSchedules = lstdateofSchedule;

                        }
                        lstAppointmentDetails.Add(appointmentResponse);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (conn != null)
                {
                    connection.Close();
                }
            }
            return lstAppointmentDetails;
        }


        //public int sendmessagetocustomerforvisit(AppointmentDetails appointmentmaster, string clientapiurl, int createdby)
        //{
        //    mysqlcommand cmd = new mysqlcommand();
        //    int resultcount = 0;
        //    clientcustomsendtextmodel sendtextrequest = new clientcustomsendtextmodel();
        //    string clientapiresponse = string.empty;

        //    try
        //    {

        //        string texttoreply = "dear" + appointmentmaster.customername + ",your visit for our store is schedule on" + appointmentmaster.appointmentdate +
        //            "on time between" + appointmentmaster.timeslot;

        //        #region call client api for sending message to customer

        //        sendtextrequest.to = appointmentmaster.mobileno;
        //        sendtextrequest.texttoreply = texttoreply;
        //        sendtextrequest.programcode = appointmentmaster.programcode;

        //        string jsonrequest = jsonconvert.serializeobject(sendtextrequest);

        //        clientapiresponse = commonservice.sendapirequest(clientapiurl + "api/chatbotbell/sendtext", jsonrequest);

        //        #endregion


        //    }
        //    catch (exception)
        //    {
        //        throw;
        //    }

        //    return resultcount;
        //}

    }
}
