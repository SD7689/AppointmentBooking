using Chatbot.Bell.API.Model;
using Chatbot.Bell.API.Model.APIClass.ShipRocket;
using Chatbot.Bell.API.Model.ShipRocket;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Processor
{
    public class ShoppingBagProcessor
    {
        #region Variable_Declaration
        //private readonly ShoppingBagDataAccess _shoppingBagDataAccess;
        private readonly ILogger<ShoppingBagProcessor> _logger;
        private readonly IMemoryCache _memoryCache;

        public ShoppingBagProcessor( ILogger<ShoppingBagProcessor> logger, IMemoryCache memoryCache)
        {
             _logger = logger;
            _memoryCache = memoryCache;
        }
        #endregion

        public bool QueueShoppingBagItems(string ObjShoppingBagInfo, string ProgCode)
        {
            bool isError = false;
            try
            {
                var factory = new ConnectionFactory() { HostName = AppConfig.RabitMQHostName, UserName = AppConfig.RabbitMQUserName, Password = AppConfig.RabbitMQPassword };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    Random rnd = new Random();
                    string Index = Convert.ToString(rnd.Next(1, Convert.ToInt16(AppConfig.RabbitMQQueuesLength)));
                    string currentRabbitMQQueuesName = String.Format(AppConfig.RabbitMQQueuesName, Convert.ToString(ProgCode).Trim().ToLower(), Index);
                    channel.QueueDeclare(queue: currentRabbitMQQueuesName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    //var body = Encoding.UTF8.GetBytes(ObjShoppingBagInfo);
                    byte[] messagebuffer = Encoding.Default.GetBytes(ObjShoppingBagInfo);


                    channel.BasicPublish(exchange: "",
                                                     routingKey: currentRabbitMQQueuesName,
                                                     basicProperties: null,
                                                     body: messagebuffer);

                }
            }
            catch (Exception Ex)
            {

            }
            return isError;
        }


        #region Shopping Bag
        internal string GetSRAuthKey()
        {
            ShipRocketAuthAPIWrapper shipRocketAPIWrapper = new ShipRocketAuthAPIWrapper();
            AuthKeyRequest authKeyRequest = new AuthKeyRequest();
            string[] APIUser = AppConfig.APIUser.Split('+');
            authKeyRequest.email = Convert.ToString(APIUser[0]);
            authKeyRequest.password = Convert.ToString(APIUser[1]);
            var strAuthRequest = JsonConvert.SerializeObject(authKeyRequest);
            string strResponse = shipRocketAPIWrapper.CallSRAuthAPI_Click(strAuthRequest);
            return strResponse;
        }
        internal AvailableCourierCompany CallSRCouriorAvailable_Click(ReqCouriersServiceability reqCouriersServiceability)
        {
            string AuthAPI = string.Empty;
            AuthAPI = _memoryCache.Get<string>("ShoopingBag_ShipRocket") == null ? AuthAPI : Convert.ToString(_memoryCache.Get<string>("ShoopingBag_ShipRocket"));
            if (String.IsNullOrEmpty(AuthAPI))
            {
                AuthAPI = GetSRAuthKey();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                                .SetSlidingExpiration(TimeSpan.FromMinutes(Convert.ToDouble(AppConfig.SecurityTokenExpirationMinutes)));
                _memoryCache.Set("ShoopingBag_ShipRocket", AuthAPI, cacheEntryOptions);

            }
            ShipRocketCourierAPIWrapper shipRocketCourierAPI = new ShipRocketCourierAPIWrapper();
            var strRequest = JsonConvert.SerializeObject(reqCouriersServiceability);
            AvailableCourierCompany[] serviceAvailablilities = shipRocketCourierAPI.CallSRCouriorAvailable_Click(strRequest, AuthAPI);
            AvailableCourierCompany service = new AvailableCourierCompany();
            if (serviceAvailablilities.Length > 0)
            {
                service = serviceAvailablilities.OrderBy(c => c.Rate).ThenByDescending(r => r.Rating).FirstOrDefault();
            }
            return service;
        }
        internal SBAWBResponse CallSRAWBGenrate_Click(AWBRequest reqAWBRequest, string orderID)
        {
            string AuthAPI = string.Empty;
            SBAWBResponse sBAWBResponse = new SBAWBResponse();
            AuthAPI = _memoryCache.Get<string>("ShoopingBag_ShipRocket") == null ? AuthAPI : Convert.ToString(_memoryCache.Get<string>("ShoopingBag_ShipRocket"));
            if (String.IsNullOrEmpty(AuthAPI))
            {
                AuthAPI = GetSRAuthKey();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                               .SetSlidingExpiration(TimeSpan.FromMinutes(Convert.ToDouble(AppConfig.SecurityTokenExpirationMinutes)));
                _memoryCache.Set("ShoopingBag_ShipRocket", AuthAPI, cacheEntryOptions);

            }
            ShipRocketAWBAPIWrapper shipRocketAWBAPI = new ShipRocketAWBAPIWrapper();

            var strRequest = JsonConvert.SerializeObject(reqAWBRequest);
            AWBAPIResponse Response = shipRocketAWBAPI.CallAWBGenrateAPI_Click(strRequest, AuthAPI, orderID, Convert.ToString(reqAWBRequest.shipment_id), Convert.ToString(reqAWBRequest.courier_id));
            sBAWBResponse.awb_code = Response.Response.Data.AwbCode;
            sBAWBResponse.courier_company_id = Convert.ToString(Response.Response.Data.CourierCompanyId);
            sBAWBResponse.courier_name = Response.Response.Data.CourierName;
            sBAWBResponse.order_id = Convert.ToString(Response.Response.Data.OrderId);
            sBAWBResponse.shipment_id = Convert.ToString(Response.Response.Data.ShipmentId);
            return sBAWBResponse;
        }
        internal APIResponse CallCustomOrder_Click(CreateCustomOrderRequest createCustomOrderRequest)
        {
            APIResponse objResponse = new APIResponse();
            ShipRocketCustomOrderAPIWrapper shipRocketCustomOrderAPI = new ShipRocketCustomOrderAPIWrapper();
            string AuthAPI = string.Empty;
            AuthAPI = _memoryCache.Get<string>("ShoopingBag_ShipRocket") == null ? AuthAPI : Convert.ToString(_memoryCache.Get<string>("ShoopingBag_ShipRocket"));
            if (String.IsNullOrEmpty(AuthAPI))
            {
                AuthAPI = GetSRAuthKey();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                               .SetSlidingExpiration(TimeSpan.FromMinutes(Convert.ToDouble(AppConfig.SecurityTokenExpirationMinutes)));
                _memoryCache.Set("ShoopingBag_ShipRocket", AuthAPI, cacheEntryOptions);

            }
            var strRequest = JsonConvert.SerializeObject(createCustomOrderRequest);
            objResponse = shipRocketCustomOrderAPI.CallCustomOrder_Click(strRequest, AuthAPI);
            return objResponse;
        }
        internal APIResponse CallGeneratePickup_Click(GeneratePickupRequest generatePickupRequest)
        {
            APIResponse objResponse = new APIResponse();
            ShipRocketGeneratePickupAPIWrapper shipRocketGeneratePickupAPI = new ShipRocketGeneratePickupAPIWrapper();
            string AuthAPI = string.Empty;
            AuthAPI = _memoryCache.Get<string>("ShoopingBag_ShipRocket") == null ? AuthAPI : Convert.ToString(_memoryCache.Get<string>("ShoopingBag_ShipRocket"));
            if (String.IsNullOrEmpty(AuthAPI))
            {
                AuthAPI = GetSRAuthKey();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                               .SetSlidingExpiration(TimeSpan.FromMinutes(Convert.ToDouble(AppConfig.SecurityTokenExpirationMinutes)));
                _memoryCache.Set("ShoopingBag_ShipRocket", AuthAPI, cacheEntryOptions);

            }
            var strRequest = JsonConvert.SerializeObject(generatePickupRequest);
            objResponse = shipRocketGeneratePickupAPI.CallGeneratePickup_Click(strRequest, AuthAPI);

            return objResponse;
        }
        internal APIResponse CallGenerateManifest_Click(GeneratePickupRequest generateManifestRequest)
        {
            APIResponse objResponse = new APIResponse();
            ShipRocketGenerateManifestAPIWrapper shipRocketGenerateManifestAPI = new ShipRocketGenerateManifestAPIWrapper();
            string AuthAPI = string.Empty;
            AuthAPI = _memoryCache.Get<string>("ShoopingBag_ShipRocket") == null ? AuthAPI : Convert.ToString(_memoryCache.Get<string>("ShoopingBag_ShipRocket"));
            if (String.IsNullOrEmpty(AuthAPI))
            {
                AuthAPI = GetSRAuthKey();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                               .SetSlidingExpiration(TimeSpan.FromMinutes(Convert.ToDouble(AppConfig.SecurityTokenExpirationMinutes)));
                _memoryCache.Set("ShoopingBag_ShipRocket", AuthAPI, cacheEntryOptions);

            }
            var strRequest = JsonConvert.SerializeObject(generateManifestRequest);
             objResponse = shipRocketGenerateManifestAPI.CallGenerateManifest_Click(strRequest, AuthAPI);

            return objResponse;
        }
        internal PrintManifestResponse CallprintManifest_Click(PrintManifestRequest printManifestRequest)
        {
            ShipRocketPrintManifestAPIWrapper shipRocketPrintManifestAPI = new ShipRocketPrintManifestAPIWrapper();
            string AuthAPI = GetSRAuthKey();
            var strRequest = JsonConvert.SerializeObject(printManifestRequest);
            PrintManifestResponse printManifestResponse = shipRocketPrintManifestAPI.CallPrintManifest_Click(strRequest, AuthAPI);

            return printManifestResponse;
        }

        internal GenerateLabelResponse CallGenerateLabel_Click(GeneratePickupRequest generateLabelRequest)
        {
            ShipRocketGenerateLabelAPIWrapper shipRocketgenerateLabeltAPI = new ShipRocketGenerateLabelAPIWrapper();
            string AuthAPI = GetSRAuthKey();
            var strRequest = JsonConvert.SerializeObject(generateLabelRequest);
            GenerateLabelResponse generateLabelResponse = shipRocketgenerateLabeltAPI.CallGeneratePLabel_Click(strRequest, AuthAPI);

            return generateLabelResponse;
        }

        internal PrintInvoiceResponse CallPrintInvoice_Click(PrintInvoiceRequest printInvoiceRequest)
        {
            ShipRocketPrintInvoiceAPIWrapper shipRocketPrintInvoiceAPI = new ShipRocketPrintInvoiceAPIWrapper();
            string AuthAPI = GetSRAuthKey();
            var strRequest = JsonConvert.SerializeObject(printInvoiceRequest);
            PrintInvoiceResponse printInvoiceResponse = shipRocketPrintInvoiceAPI.CallPrintInvoice_Click(strRequest, AuthAPI);

            return printInvoiceResponse;
        }

        internal GetTrackingResponse CallGetTracking_Click(string awb_code)
        {
            ShipRocketGetTrackingAPIWrapper shipRocketGetTrackingAPI = new ShipRocketGetTrackingAPIWrapper();
            string AuthAPI = GetSRAuthKey();
            //var strRequest = JsonConvert.SerializeObject(printInvoiceRequest);
            GetTrackingResponse getTrackingResponse = shipRocketGetTrackingAPI.CallGetTracking_Click(awb_code, AuthAPI);

            return getTrackingResponse;
        }
        #endregion
    }
}
