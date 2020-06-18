using Chatbot.Bell.API.Model;
using Chatbot.Bell.API.Model.ShipRocket;
using Chatbot.Bell.API.Processor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Chatbot.Bell.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShoppingBagController : ControllerBase
    {
        #region Variable_Declaration
        private readonly ShoppingBagProcessor _ShoppingBagProcessor;
        private readonly ChatbotBellProcessor _chatbotBellProcessor;
        private readonly ILogger<ShoppingBagController> _logger;

        public ShoppingBagController(ShoppingBagProcessor shoppingBagProcessor, ChatbotBellProcessor chatbotBellProcessor, ILogger<ShoppingBagController> logger)
        {
            _chatbotBellProcessor = chatbotBellProcessor;
            _logger = logger;
            _ShoppingBagProcessor = shoppingBagProcessor;
        }
        #endregion

        [Authorize]
        [HttpPost]
        public ActionResult AddShoppingBagItems(ShoppingBag ObjShoppingBagInfo)
        {
            try
            {
                //bool IsProgramConfigured = false;
                UserATVDetails objUserATVDetails = new UserATVDetails();
                //objUserATVDetails = _chatbotBellProcessor.GetUserATVDetails(ObjUserInfo.MobileNumber.Trim(), ObjUserInfo.ProgramCode.Trim(), out IsProgramConfigured);
                //if (!IsProgramConfigured)
                //    return BadRequest(new { StatusCode = "225", Message = "Program not configured." });
                //else
                return Ok(objUserATVDetails);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex.Message);
                return BadRequest(new { StatusCode = "1001", Message = "Technical error occured. Please try after sometime." });
            }
        }

        #region Manoj Test Authorization function
        //[HttpPost]
        //[Authorize]
        //public ActionResult<IEnumerable<string>> GetData(UserModel login)
        //{
        //    var currentUser = HttpContext.User;
        //    int spendingTimeWithCompany = 0;

        //    if (currentUser.HasClaim(c => c.Type == "DateOfJoing"))
        //    {
        //        DateTime date = DateTime.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "DateOfJoing").Value);
        //        spendingTimeWithCompany = DateTime.Today.Year - date.Year;
        //    }

        //    if (spendingTimeWithCompany > 5)
        //    {
        //        return new string[] { "High Time1", "High Time2", "High Time3", "High Time4", "High Time5" };
        //    }
        //    else
        //    {
        //        return new string[] { "value1", "value2", "value3", "value4", "value5" };
        //    }
        //} 
        #endregion


        [HttpPost]
        public ActionResult GetSAuthentication(SRAuthRequest sRAuthRequest)
        {
            try
            {

                return Ok();
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex.Message);
                return BadRequest(new { StatusCode = "1001", Message = "Technical error occured. Please try after sometime." });
            }
        }

        /// <summary>
        /// This API is used to genrate AWB number and courier partner details
        /// </summary>
        /// <param name="chkCouriersPartnerwithAWB"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCouriersPartnerAndAWBCode(ReqCouriersServiceabilitywithAWB chkCouriersPartnerwithAWB)
        {
            try
            {
                ReqCouriersServiceability reqCouriersServiceability = new ReqCouriersServiceability();
                reqCouriersServiceability.pickup_postcode = chkCouriersPartnerwithAWB.pickup_postcode;
                reqCouriersServiceability.delivery_postcode = chkCouriersPartnerwithAWB.delivery_postcode;
                reqCouriersServiceability.cod = chkCouriersPartnerwithAWB.cod;
                reqCouriersServiceability.weight = chkCouriersPartnerwithAWB.weight;

                AvailableCourierCompany reqCouriers = new AvailableCourierCompany();
                reqCouriers = _ShoppingBagProcessor.CallSRCouriorAvailable_Click(reqCouriersServiceability);
                SBAWBResponse awbResponse = new SBAWBResponse();
                if (!String.IsNullOrEmpty(reqCouriers.CourierName))
                {
                    CreateCustomOrderResponse reqCustomOrder = new CreateCustomOrderResponse();
                    APIResponse objResponse = new APIResponse();
                    objResponse = _ShoppingBagProcessor.CallCustomOrder_Click(chkCouriersPartnerwithAWB.orderDetails);
                    reqCustomOrder = (CreateCustomOrderResponse)objResponse.Data;

                    AWBRequest aWBRequest = new AWBRequest();
                    aWBRequest.shipment_id = reqCustomOrder.shipment_id;
                    aWBRequest.courier_id = reqCouriers.CourierCompanyId;
                    string orderid = Convert.ToString(reqCustomOrder.order_id);
                    awbResponse = _ShoppingBagProcessor.CallSRAWBGenrate_Click(aWBRequest, orderid);
                    awbResponse.rate = reqCouriers.Rate;
                    awbResponse.is_custom_rate = Convert.ToString(reqCouriers.IsCustomRate);
                    awbResponse.cod_multiplier = Convert.ToString(reqCouriers.CodMultiplier);
                    awbResponse.cod_charges = Convert.ToString(reqCouriers.CodCharges);
                    awbResponse.freight_charge = Convert.ToString(reqCouriers.FreightCharge);
                    awbResponse.rto_charges = Convert.ToString(reqCouriers.RtoCharges);
                    awbResponse.min_weight = Convert.ToString(reqCouriers.MinWeight);
                    awbResponse.etd_hours = Convert.ToString(reqCouriers.EtdHours);
                    awbResponse.etd = Convert.ToString(reqCouriers.Etd);
                    awbResponse.estimated_delivery_days = Convert.ToString(reqCouriers.EstimatedDeliveryDays);
                    return Ok(new { StatusCode = "200", data = awbResponse });
                }
                else
                    return Ok(new { StatusCode = "201", data = "No courier partner available" });
            }
            catch (Exception Ex)
            {
                return BadRequest(new { StatusCode = "1001", Message = "Technical error occured. Please try after sometime." });
            }
        }
        /// <summary>
        /// This API is used to create Shipment ID and order id. For this we required Order details, Shipment address
        /// </summary>
        /// <param name="createCustomOrderRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateCustomOrder(CreateCustomOrderRequest createCustomOrderRequest)
        {
            try
            {
                CreateCustomOrderResponse reqCustomOrder = new CreateCustomOrderResponse();
                APIResponse objResponse = new APIResponse();
                objResponse = _ShoppingBagProcessor.CallCustomOrder_Click(createCustomOrderRequest);
                reqCustomOrder = (CreateCustomOrderResponse)objResponse.Data;
                return Ok();
            }
            catch (Exception Ex)
            {
                return BadRequest(new { StatusCode = "1001", Message = "Technical error occured. Please try after sometime." });
            }
        }

        /// <summary>
        /// Use this API to create a pickup request for your order shipment. The API returns the pickup status along with the estimated pickup time. 
        /// You will have to call the 'Generate Manifest' API after the successful response of this API.
        /// The AWB must be already generated for the shipment id in order to generate the pickup request.
        /// </summary>
        /// <param name="generatePickupRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GeneratePickup([FromBody]GeneratePickupRequest generatePickupRequest)
        {
            try
            {
                APIResponse objResponse = new APIResponse();
                //GeneratePickupResponse reqGeneratePickup = new GeneratePickupResponse();
                objResponse = _ShoppingBagProcessor.CallGeneratePickup_Click(generatePickupRequest);
                if (objResponse.isError)
                {
                    return BadRequest((GenericAPIResponse)objResponse.Data);
                }
                else
                    return Ok((GeneratePickupResponse)objResponse.Data);
            }
            catch (Exception Ex)
            {
                return BadRequest(new { StatusCode = "1001", Message = "Technical error occured. Please try after sometime." });
            }
        }

        /// <summary>
        /// Using this API, you can generate the manifest for your order. This API generates the manifest and displays the download URL of the same.
        /// </summary>
        /// <param name="generateManifestRequest">shipment_id, Multiple ids can be passed as an array, seperated by commas.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GenerateManifest([FromBody]GeneratePickupRequest generateManifestRequest)
        {
            try
            {
                APIResponse objResponse = new APIResponse();
                //GenerateManifestResponse reqGenerateManifest = new GenerateManifestResponse();
                objResponse = _ShoppingBagProcessor.CallGenerateManifest_Click(generateManifestRequest);
                if (objResponse.isError)
                {
                    return BadRequest((GenericAPIResponse)objResponse.Data);
                }
                else
                    return Ok((GenerateManifestResponse)objResponse.Data);
            }
            catch (Exception Ex)
            {
                return BadRequest(new { StatusCode = "1001", Message = "Technical error occured. Please try after sometime." });
            }
        }

        /// <summary>
        /// Use this API to check the availability of couriers between the pickup and delivery postal codes. 
        /// Further details like the estimated time of delivery, the rates along with the ids are also shown.
        /// One of either the 'order_id' or 'cod' and 'weight' is required. In case you specify the order id, the cod and weight fields are not required and vice versa.
        /// You can add further fields to add the shipment details and filter the search.
        /// </summary>
        /// <param name="chkCouriersPartner"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChkCourierAvailibilty(ReqCouriersServiceability chkCouriersPartner)
        {
            try
            {
                AvailableCourierCompany reqCouriers = new AvailableCourierCompany();
                reqCouriers = _ShoppingBagProcessor.CallSRCouriorAvailable_Click(chkCouriersPartner);
                if (String.IsNullOrEmpty(reqCouriers.CourierName))
                    return Ok(new { StatusCode = "200", Available = false });
                else
                    return Ok(new { StatusCode = "200", Available = true });
            }
            catch (Exception Ex)
            {
                return BadRequest(new { StatusCode = "1001", Message = "Technical error occured. Please try after sometime." });
            }
        }

        /// <summary>
        /// Use this API to print the generated manifest of orders in an individual level.
        /// Mnaifest needs to be generated first in order for this API to print it. Use the 'Generate Manifest' API to do the same.
        /// Multiple order ids can be passed together.
        /// </summary>
        /// <param name="printManifestRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PrintManifest([FromBody]PrintManifestRequest printManifestRequest)
        {
            try
            {
                PrintManifestResponse reqPrintManifest = new PrintManifestResponse();
                reqPrintManifest = _ShoppingBagProcessor.CallprintManifest_Click(printManifestRequest);
                return Ok();
            }
            catch (Exception Ex)
            {
                return BadRequest(new { StatusCode = "1001", Message = "Technical error occured. Please try after sometime." });
            }
        }

        /// <summary>
        /// Generate the label of an order by passing the shipment id in the form of an array. This API displays the URL of the generated label.
        /// The AWB must be assigned on the shipment to generate the label.
        /// 'shipment_id' must be passed as an array.
        /// </summary>
        /// <param name="generateLabelReq"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GenerateLabel([FromBody]GeneratePickupRequest generateLabelReq)
        {
            try
            {
                GenerateLabelResponse reqGenerateLabel = new GenerateLabelResponse();
                reqGenerateLabel = _ShoppingBagProcessor.CallGenerateLabel_Click(generateLabelReq);
                return Ok();
            }
            catch (Exception Ex)
            {
                return BadRequest(new { StatusCode = "1001", Message = "Technical error occured. Please try after sometime." });
            }
        }

        /// <summary>
        /// Use this API to generate the invoice for your order by passing the respective Shiprocket order ids.
        /// The generated invoice URL is desplayed as a response. Multiple ids can be passed together as an array.
        /// Order ids must be passed as an array.
        /// </summary>
        /// <param name="printInvoiceReq"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PrintInvoice([FromBody]PrintInvoiceRequest printInvoiceReq)
        {
            try
            {
                PrintInvoiceResponse resPrintInvoice = new PrintInvoiceResponse();
                resPrintInvoice = _ShoppingBagProcessor.CallPrintInvoice_Click(printInvoiceReq);
                return Ok();
            }
            catch (Exception Ex)
            {
                return BadRequest(new { StatusCode = "1001", Message = "Technical error occured. Please try after sometime." });
            }
        }

        /// <summary>
        /// Get the tracking details of your shipment by entering the AWB code of the same in the endpoint URL itself. No other body parameters are required to access this API.
        /// The response is displayed in a JSON format.
        /// </summary>
        /// <param name="getTrackingRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTracking([FromBody] GetTrackingRequest getTrackingRequest)
        {
            try
            {
                GetTrackingResponse resGetTrackingResponse = new GetTrackingResponse();
                resGetTrackingResponse = _ShoppingBagProcessor.CallGetTracking_Click(getTrackingRequest.awb_no);
                return Ok();
            }
            catch (Exception Ex)
            {
                return BadRequest(new { StatusCode = "1001", Message = "Technical error occured. Please try after sometime." });
            }
        }
    }
}