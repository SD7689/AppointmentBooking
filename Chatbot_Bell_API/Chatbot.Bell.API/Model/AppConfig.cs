using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Chatbot.Bell.API.Model
{
    public class AppConfig
    {
        static IConfiguration apConfig = new ConfigurationBuilder()
       .AddJsonFile("appsettings.json", true, true)
       .Build();

        public static string ConnectionString { get { return apConfig["ConnectionStrings:DefaultConnection"]; } }
        public static string GetATVDataByMobileNumber { get { return apConfig["USP_Bell_GetUserDetails"]; } }
        public static string S8 { get { return apConfig["ConnectionStrings:S8"]; } }
        public static string S1 { get { return apConfig["ConnectionStrings:S1"]; } }
        public static string S2 { get { return apConfig["ConnectionStrings:S2"]; } }
        public static string S3 { get { return apConfig["ConnectionStrings:S3"]; } }
        public static string S4 { get { return apConfig["ConnectionStrings:S4"]; } }
        public static string S5 { get { return apConfig["ConnectionStrings:S5"]; } }
        public static string S11 { get { return apConfig["ConnectionStrings:S11"]; } }

        public static string TestConnection { get { return apConfig["ConnectionStrings:TestConnection"]; } }
        public static string BellConnection { get { return apConfig["ConnectionStrings:BellConnection"]; } }
        public static string BellDBConnection { get { return apConfig["ConnectionStrings:BellDBConnection"]; } }

        public static string GetStoreLocator { get { return apConfig["GetStoreLocator"]; } }
        public static string UserName { get { return apConfig["UserName"]; } }
        public static string Password { get { return apConfig["Password"]; } }
        public static string Host { get { return apConfig["Host"]; } }
        public static string Exchange { get { return apConfig["Exchange"]; } }
        public static string SendCampaignQueue { get { return apConfig["SendCampaignQueue"]; } }
        public static string SendMessageQueue { get { return apConfig["SendMessageQueue"]; } }
        public static string DBConnectionToGetWACreds { get { return apConfig["DBConnectionToGetWACreds"]; } }
        public static string SPtoGetWACreds { get { return apConfig["SPtoGetWACreds"]; } }
        public static string SPToGetQueueDetails { get { return apConfig["SPToGetQueueDetails"]; } }

        public static string CustomerResponseQueue { get { return apConfig["CustomerResponseQueue"]; } }
        public static string SendImageQueue { get { return apConfig["SendImageQueue"]; } }
        public static string SMSAPIURL { get { return apConfig["SMSSettings:SMSAPIURL"]; } }
        public static string SMSAPIUserName { get { return apConfig["SMSSettings:SMSAPIUserName"]; } }
        public static string SMSAPIPassword { get { return apConfig["SMSSettings:SMSAPIPassword"]; } }
        public static string UDH { get { return apConfig["SMSSettings:UDH"]; } }
        public static string CODING { get { return apConfig["SMSSettings:CODING"]; } }
        public static string PROPERTY { get { return apConfig["SMSSettings:PROPERTY"]; } }
        public static string DLR { get { return apConfig["SMSSettings:DLR"]; } }
        public static string VALIDITY { get { return apConfig["SMSSettings:VALIDITY"]; } }
        public static string SEND_ON { get { return apConfig["SMSSettings:SEND_ON"]; } }
        public static string TAG { get { return apConfig["SMSSettings:TAG"]; } }
        public static string IsSendOn { get { return apConfig["SMSSettings:IsSendOn"]; } }
        public static string IsTag { get { return apConfig["SMSSettings:IsTag"]; } }

        public static string GetProgramWiseConnection { get { return apConfig["GetProgramWiseConnection"]; } }
        public static string SptoSaveAppointment { get { return apConfig["SptoSaveAppointment"]; } }

        public static string RabitMQHostName { get { return apConfig["RabiitMQDigital:RabitMQHostName"]; } }
        public static string RabbitMQUserName { get { return apConfig["RabiitMQDigital:RabbitMQUserName"]; } }
        public static string RabbitMQPassword { get { return apConfig["RabiitMQDigital:RabbitMQPassword"]; } }
        public static string RabbitMQQueuesName { get { return apConfig["RabiitMQDigital:RabbitMQQueuesName"]; } }
        public static string RabbitMQQueuesCommunicationName { get { return apConfig["RabiitMQDigital:CommunicationName"]; } }
        public static string RabbitMQQueuesLength { get { return apConfig["RabiitMQDigital:RabbitMQQueuesLength"]; } }
        public static string CommunicationMQQueuesLength { get { return apConfig["RabiitMQDigital:CommunicationMQQueuesLength"]; } }

        public static string SPtoCheckProgramBellStatus { get { return apConfig["SPtoCheckProgramBellStatus"]; } }
        public static string MessageNoStoreCodeFound { get { return apConfig["MessageNoStoreCodeFound"]; } }
        public static string DefaultStoreCode { get { return apConfig["DefaultStoreCode"]; } }
        public static string InfoAPISearchUrl { get { return apConfig["ElasticSearchSettings:InfoAPISearchUrl"]; } }
        public static string InfoAPILogUrl { get { return apConfig["ElasticSearchSettings:InfoAPILogUrl"]; } }
        public static string RabbitMQName { get { return apConfig["ElasticSearchSettings:RabbitMQName"]; } }
        public static string IsMultiQueue { get { return apConfig["ElasticSearchSettings:IsMultiQueue"]; } }
        public static string IsRabbitMQ { get { return apConfig["ElasticSearchSettings:IsRabbitMQ"]; } }

        #region JWT Token
        public static string SecurityKey { get { return apConfig["Jwt:Key"]; } }
        public static string KeyIssuer { get { return apConfig["Jwt:Issuer"]; } }
        public static string Audience_DR { get { return apConfig["Jwt:Audience_DR"]; } }
        public static string TokenExpiryTimeInMinuts { get { return apConfig["Jwt:TokenExpiryTimeInMinuts"]; } }
        public static string AuthenticateUsername_DR { get { return apConfig["Jwt:AuthenticateUsername_DR"]; } }
        public static string AuthenticatePassword_DR { get { return apConfig["Jwt:AuthenticatePassword_DR"]; } }
        #endregion

        #region ShipRocket
        public static string SRAuthAPI { get { return apConfig["ShipRocket:SRAuthAPI"]; } }
        public static string AuthApi { get { return apConfig["ShipRocket:AuthApi"]; } }
        public static string APIUser { get { return apConfig["ShipRocket:APIUser"]; } }
        public static string CouriorAvailable { get { return apConfig["ShipRocket:CouriorAvailable"]; } }
        public static string AWBApiURL { get { return apConfig["ShipRocket:AWBApiURL"]; } }
        public static string SecurityTokenExpirationMinutes { get { return apConfig["ShipRocket:SecurityTokenExpirationMinutes"]; } }
        public static string CreateCustomeOrder { get { return apConfig["ShipRocket:CreateCustomeOrder"]; } }
        public static string GeneratePickup { get { return apConfig["ShipRocket:GeneratePickup"]; } }
        public static string GenerateManifest { get { return apConfig["ShipRocket:GenerateManifest"]; } }
        public static string PrintManifest { get { return apConfig["ShipRocket:PrintManifest"]; } }
        public static string GenerateLabel { get { return apConfig["ShipRocket:GenerateLabel"]; } }
        public static string PrintInvoice { get { return apConfig["ShipRocket:PrintInvoice"]; } }
        public static string GetTracking { get { return apConfig["ShipRocket:GetTracking"]; } }
        #endregion

        public static string LastShoppingReceipt { get { return apConfig["LastShoppingReceipt"]; } }

        #region Digital Receipt
        public static string FailoverMQQueuesVendorName { get { return apConfig["RabiitMQDigital:FailoverMQQueuesVendorName"]; } }

        #endregion
        public static string InfoAPIDigitalReceiptUrl { get { return apConfig["InfoAPIDigitalReceiptUrl"]; } }
    }
}
