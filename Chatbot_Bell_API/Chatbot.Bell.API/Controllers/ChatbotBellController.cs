using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chatbot.Bell.API.ErrorMap;
using Chatbot.Bell.API.Model;
using Chatbot.Bell.API.Processor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Chatbot.Bell.API.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ChatbotBellController : ControllerBase
    {
        #region Variable_Declaration
        private readonly ChatbotBellProcessor _chatbotBellProcessor;
        private readonly ILogger<ChatbotBellController> _logger;

        public ChatbotBellController(ChatbotBellProcessor chatbotBellProcessor, ILogger<ChatbotBellController> logger)
        {
            _chatbotBellProcessor = chatbotBellProcessor;
            _logger = logger;
        }
        #endregion

        [HttpPost]
        public ActionResult GetUserATVDetails(UserInfoRequest ObjUserInfo)
        {
            //var errlogger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            try
            {
                bool IsProgramConfigured = false;
                UserATVDetails objUserATVDetails = new UserATVDetails();
                objUserATVDetails = _chatbotBellProcessor.GetUserATVDetails(ObjUserInfo.MobileNumber.Trim(), ObjUserInfo.ProgramCode.Trim(), out IsProgramConfigured);
                if (!IsProgramConfigured)
                    return BadRequest(new { StatusCode = "225", Message = "Program not configured." });
                else
                    return Ok(objUserATVDetails);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex.Message);
                return BadRequest(new { StatusCode = "1001", Message = "Technical error occured. Please try after sometime." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetKeyInsight(UserInfoRequest ObjUserInfo)
        {
            try
            {
                APIResponse objResponse = new APIResponse();
                objResponse = await _chatbotBellProcessor.GetUserKeyInsight(ObjUserInfo.MobileNumber.Trim(), ObjUserInfo.ProgramCode.Trim());
                if (objResponse.isError)
                {
                    return BadRequest(new { StatusCode = objResponse.Data, Message = StoreLocatorErrMap.ErrorStoreCodeDic[Convert.ToString(objResponse.Data)] });
                }
                else
                    return Ok((List<KeyInsight>)objResponse.Data);
            }
            catch (Exception Ex)
            {
                _logger.LogError("Error in GetKeyInsight -- " + Ex.Message);
                return BadRequest(new { StatusCode = 1001, Message = StoreLocatorErrMap.ErrorStoreCodeDic["1001"] });
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetRecommendedList(UserInfoRequest ObjUserInfo)
        {
            try
            {
                APIResponse objResponse = new APIResponse();
                objResponse = await _chatbotBellProcessor.GetUserRecommendation(ObjUserInfo.MobileNumber.Trim(), ObjUserInfo.ProgramCode.Trim());
                if (objResponse.isError)
                {
                    return BadRequest(new { StatusCode = objResponse.Data, Message = StoreLocatorErrMap.ErrorStoreCodeDic[Convert.ToString(objResponse.Data)] });
                }
                else
                    return Ok((List<Recommendation>)objResponse.Data);
            }
            catch (Exception Ex)
            {
                _logger.LogError("Error in GetRecommendedList -- " + Ex.Message);
                return BadRequest(new { StatusCode = 1001, Message = StoreLocatorErrMap.ErrorStoreCodeDic["1001"] });
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetLastTransactionDetails(UserInfoRequest ObjUserInfo)
        {
            try
            {
                APIResponse objResponse = new APIResponse();
                objResponse = await _chatbotBellProcessor.GetLastTransaction(ObjUserInfo.MobileNumber.Trim(), ObjUserInfo.ProgramCode.Trim());
                if (objResponse.isError)
                {
                    return BadRequest(new { StatusCode = objResponse.Data, Message = TransactionErrorMap.ErrorTransactionDic[Convert.ToString(objResponse.Data)] });
                }
                else
                    return Ok((Transaction)objResponse.Data);
            }
            catch (Exception Ex)
            {
                _logger.LogError("Error in GetLastTransactionDetails -- " + Ex.Message);
                return BadRequest(new { StatusCode = 1001, Message = StoreLocatorErrMap.ErrorStoreCodeDic["1001"] });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetNearbyStoreDetails(StoreSearch ObjStoreSearch)
        {
            try
            {
                APIResponse objResponse = new APIResponse();
                objResponse = await _chatbotBellProcessor.GetNearbyStoreDetails(ObjStoreSearch.programcode.Trim(), ObjStoreSearch.latitude.Trim(), ObjStoreSearch.longitude.Trim(), ObjStoreSearch.distance.Trim());
                if (objResponse.isError)
                {
                    return BadRequest(new { StatusCode = objResponse.Data, Message = StoreLocatorErrMap.ErrorStoreCodeDic[Convert.ToString(objResponse.Data)] });
                }
                else
                    return Ok((List<Store>)objResponse.Data);
            }
            catch (Exception Ex)
            {
                _logger.LogError("Error in GetNearbyStoreDetails -- " + Ex.Message);
                return BadRequest(new { StatusCode = 1001, Message = StoreLocatorErrMap.ErrorStoreCodeDic["1001"] });
            }
            //try
            //{
            //    List<Store> objStoreList = new List<Store>();
            //    bool IsProgramConfigured = false;
            //    objStoreList = _chatbotBellProcessor.GetNearbyStoreDetails(ObjStoreSearch.programcode.Trim(), ObjStoreSearch.latitude.Trim(), ObjStoreSearch.longitude.Trim(), ObjStoreSearch.distance.Trim(), out IsProgramConfigured);
            //    if (!IsProgramConfigured)
            //        return BadRequest(new { StatusCode = "225", Message = "Program not configured" });
            //    else
            //        return Ok(objStoreList);
            //}
            //catch
            //{
            //    return BadRequest();
            //}
        }
        [HttpPost]
        public async Task<IActionResult> GetNearbyStoreDetailsByPincode(StoreSearchByPin objSearchBYPincode)
        {
            try
            {
                APIResponse objResponse = new APIResponse();
                objResponse = await _chatbotBellProcessor.GetNearbyStoreDetailsByPincode(objSearchBYPincode.programcode.Trim(), objSearchBYPincode.Pincode.Trim());
                if (objResponse.isError)
                {
                    return BadRequest(new { StatusCode = objResponse.Data, Message = StoreLocatorErrMap.ErrorStoreCodeDic[Convert.ToString(objResponse.Data)] });
                }
                else
                    return Ok((List<Store>)objResponse.Data);
            }
            catch (Exception Ex)
            {
                _logger.LogError("Error in GetNearbyStoreDetailsByPincode -- " + Ex.Message);
                return BadRequest(new { StatusCode = 1001, Message = StoreLocatorErrMap.ErrorStoreCodeDic["1001"] });
            }
        }

        [HttpPost]
        public ActionResult SendCampaign([FromBody]SendCampaignMessageDetails campaignDetails)
        {
            try
            {
                var result = _chatbotBellProcessor.SendCampaign(campaignDetails);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public ActionResult SendTextAsync([FromBody]SendTextMessageDetails textDetails)
        {
            try
            {
                var result = _chatbotBellProcessor.SendText(textDetails);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        [HttpGet]
        public ActionResult GetCustomerResponse()
        {
            try
            {
                var result = _chatbotBellProcessor.GetCustomerResponse();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public ActionResult SendImageAsync([FromBody]SendImageMessageDetails imageDetails)
        {
            try
            {
                var result = _chatbotBellProcessor.SendImage(imageDetails);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        [HttpPost]
        public ActionResult SendSMSAsync(SMSSendRequest sendSMSRequest)
        {
            try
            {
                var result = _chatbotBellProcessor.SendSMSResponse(sendSMSRequest.MobileNumber, sendSMSRequest.SenderId, sendSMSRequest.SMSText);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public ActionResult GetItemsByArticlesSKUID(ItemSearch objItemSearch)
        {
            try
            {
                List<ItemDetails> objItemDetails = new List<ItemDetails>();
                bool IsProgramConfigured = false;
                objItemDetails = _chatbotBellProcessor.GetItemsByArticlesSKUID(objItemSearch.SearchCriteria.Trim(), objItemSearch.programcode.Trim(), out IsProgramConfigured);
                if (!IsProgramConfigured)
                    return BadRequest(new { StatusCode = "225", Message = "Program not configured" });
                else
                    return Ok(objItemDetails);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetHSNCodeBySKUID([FromBody]ItemSearch objItemSearch)
        {
            try
            {
                APIResponse objResponse = new APIResponse();
                objResponse = await _chatbotBellProcessor.GetHSNCodeBySKUID(objItemSearch);
                if (objResponse.isError)
                {
                    return Ok(new { StatusCode = objResponse.Data, Message = ItemErrorMap.ErrorItemDic[Convert.ToString(objResponse.Data)] });
                }
                else
                    return Ok((ItemHSN)objResponse.Data);
            }
            catch(Exception Ex)
            {
                _logger.LogError(Ex.ToString());
                return BadRequest();
            }
        }
        //[HttpPost]
        //public async Task<IActionResult> GetNearbyStoreDetailsByPincodeTest(StoreSearchByPin objSearchBYPincode)
        //{
        //    try
        //    {
        //        APIResponse objResponse = new APIResponse();
        //        objResponse = await _chatbotBellProcessor.GetNearbyStoreDetailsTest(objSearchBYPincode.programcode.Trim(), objSearchBYPincode.Pincode.Trim());
        //        if (objResponse.isError)
        //        {
        //            return BadRequest(new { StatusCode = objResponse.Data, Message = StoreLocatorErrMap.ErrorStoreCodeDic[Convert.ToString(objResponse.Data)] });
        //        }
        //        else
        //            return Ok((List<Store>)objResponse.Data); 
        //    }
        //    catch
        //    {
        //        return BadRequest(new { StatusCode = 1001, Message = StoreLocatorErrMap.ErrorStoreCodeDic["1001"] });
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> GetWhatsappMessageDetails(WhatsappMessageDetailsRequest whatsappMessageDetailsRequest)
        {
            try
            {
                var result = await _chatbotBellProcessor.GetWhatsappMessageDetails(whatsappMessageDetailsRequest.ProgramCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetStoreDetailsByStoreCode(StoreSearchByStoreCode ObjStoreSearch)
        {
            try
            {
                APIResponse objResponse = new APIResponse();
                objResponse = await _chatbotBellProcessor.GetStoreDetailsByStoreCode(ObjStoreSearch.programcode.Trim(), ObjStoreSearch.StoreCode.Trim());
                if (objResponse.isError)
                {
                    return Ok(new { StatusCode = objResponse.Data, Message = StoreLocatorErrMap.ErrorStoreCodeDic[Convert.ToString(objResponse.Data)] });
                }
                else
                    return Ok((StoreInfo)objResponse.Data);
            }
            catch (Exception Ex)
            {
                _logger.LogError("Error in GetNearbyStoreDetails -- " + Ex.Message);
                return BadRequest(new { StatusCode = 1001, Message = StoreLocatorErrMap.ErrorStoreCodeDic["1001"] });
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> SaveAppointment([FromBody]SaveAppointmentRequest saveAppointmentRequest)
        //{
        //    //MainResponse mainResponse = new MainResponse();
        //    //try
        //    //{
        //    //    saveAppointmentRequest.CreatedBy = "6";
        //    //    List<SaveAppointmentResponse> saveAppointmentResponse = _chatbotBellProcessor.GetSaveAppointmentResponse(saveAppointmentRequest);
        //    //    mainResponse.message = "Success";
        //    //    mainResponse.status = true;
        //    //    mainResponse.responseData = saveAppointmentResponse;
        //    //    mainResponse.statusCode = 200;
        //    //    return Ok(mainResponse);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    return BadRequest();
        //    //}
        //    try
        //    {
        //        APIResponse objResponse = new APIResponse();
        //        try
        //        {
        //            saveAppointmentRequest.AppointmentDate = Convert.ToDateTime(saveAppointmentRequest.AppointmentDate);
        //        }
        //        catch
        //        {
        //            return BadRequest(new { StatusCode = objResponse.Data, Message = AppointmentErrorMap.ErrorAppointmentDic["1003"] });

        //        }
        //        //objResponse = await _chatbotBellProcessor.GetSaveAppointmentResponse(saveAppointmentRequest);
        //        if (objResponse.isError)
        //        {
        //            return BadRequest(new { StatusCode = objResponse.Data, Message = AppointmentErrorMap.ErrorAppointmentDic[Convert.ToString(objResponse.Data)] });
        //        }
        //        else
        //            return Ok((List<SaveAppointmentResponse>)objResponse.Data);
        //    }
        //    catch (Exception Ex)
        //    {
        //        _logger.LogError("Error in SaveAppointment -- " + Ex.Message);
        //        return BadRequest(new { StatusCode = 1001, Message = StoreLocatorErrMap.ErrorStoreCodeDic["1001"] });
        //    }
        //}

        /// <summary>
        /// GetTenantConfig method is used to get tenant config details, like WhatsApp, Bell, WebBot, ShoppingBag, ERPayment, ERShipping, Syncing Order, digital Receipt is enable.
        /// it will return 'Y' for enable and 'N' for Not enable.
        /// </summary>
        /// <param name="ProgramCode"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetTenantConfig([FromBody]ProgramConfigSearch ProgramCode)
        {
            APIResponse objResponse = new APIResponse();
            try
            {
                if(ProgramCode.Programcode.Trim().ToString() == "")
                {
                    objResponse.Data = "124";
                    return Ok(new { StatusCode = objResponse.Data, Message = ProgConfigErrorMap.ErrorProgConfigDic[Convert.ToString(objResponse.Data)] });
                }
                objResponse =  _chatbotBellProcessor.GetTenantConfig(ProgramCode.Programcode.Trim().ToString());
                if (objResponse.isError)
                {
                    return Ok(new { StatusCode = objResponse.Data, Message = ProgConfigErrorMap.ErrorProgConfigDic[Convert.ToString(objResponse.Data)] });
                }
                else
                    return Ok((TenantConfig)objResponse.Data);
            }
            catch (Exception Ex)
            {
                objResponse.Data = "1001";
                _logger.LogError(Ex.ToString());
                return Ok(new { StatusCode = objResponse.Data, Message = ProgConfigErrorMap.ErrorProgConfigDic[Convert.ToString(objResponse.Data)] });
            }
        }

        [HttpGet]
        public IActionResult GetTenantMaster()
        {
            APIResponse objResponse = new APIResponse();
            try
            { 
                objResponse = _chatbotBellProcessor.GetTenantMaster();
                if (objResponse.isError)
                {
                    return Ok(new { StatusCode = objResponse.Data, Message = ProgConfigErrorMap.ErrorProgConfigDic[Convert.ToString(objResponse.Data)] });
                }
                else
                    return Ok((List<TenantConfig>)objResponse.Data);
            }
            catch (Exception Ex)
            {
                objResponse.Data = "1001";
                _logger.LogError(Ex.ToString());
                return Ok(new { StatusCode = objResponse.Data, Message = ProgConfigErrorMap.ErrorProgConfigDic[Convert.ToString(objResponse.Data)] });
            }
        }

        [HttpPost]
        public ActionResult GetShopsterStoreCode(UserInfoRequest ObjUserInfo)
        {
            try
            {
                StoreCodeDetails objResponse = new StoreCodeDetails();
                objResponse = _chatbotBellProcessor.GetShopsterStoreCode(ObjUserInfo.MobileNumber.Trim(), ObjUserInfo.ProgramCode.Trim());
                return Ok(objResponse);
            }
            catch (Exception Ex)
            {
                _logger.LogError("Error in GetShopsterStoreCode -- " + Ex.Message);
                return BadRequest(new { StatusCode = 1001, Message = StoreLocatorErrMap.ErrorStoreCodeDic["1001"] });
            }
        }

        [HttpPost]
        public ActionResult GetItemImageUrl(ItemImageLocationRequest itemImageLocationRequest)
        {
            try 
            {
                ItemImageLocationResponse objResponse = new ItemImageLocationResponse();
                objResponse = _chatbotBellProcessor.GetItemImageLocation(itemImageLocationRequest.ProgramCode.Trim());
                return Ok(objResponse);
            }
            catch (Exception Ex)
            {
                _logger.LogError("Error in GetItemImageUrl -- " + Ex.Message);
                return BadRequest(new { StatusCode = 1001, Message = ItemImageLocationErrMap.ErrorItemImageLocationDic["1001"] });
            }
        }
        
        //[HttpPost]
        //public ActionResult GetScheduleVisit(AppointmentMaster appointmentMaster)
        //{
        //    try
        //    {
        //        var result = _chatbotBellProcessor.ScheduleVisit( appointmentMaster);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex);
        //    }
        //}
        
        [HttpPost]
        public ActionResult GetTimeSlotDetails(string StoreCode, int tenantID)
        {
            try
            {
                var result = _chatbotBellProcessor.GetTimeSlot(StoreCode,tenantID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        
    }
}
