using Chatbot.Bell.API.Model;
using Chatbot.Bell.API.Model.DigitalReceipt;
using Chatbot.Bell.API.Processor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Chatbot.Bell.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DigitalReceiptController : ControllerBase
    {
        #region Variable_Declaration
        private readonly ChatbotBellProcessor _chatbotBellProcessor;
        private readonly ILogger<DigitalReceiptController> _logger;
        private readonly ChatBotBusinessLogic _chatBotBusinessLogic;

        public DigitalReceiptController(ChatbotBellProcessor chatbotBellProcessor, ILogger<DigitalReceiptController> logger, ChatBotBusinessLogic chatBotBusinessLogic)
        {
            _chatbotBellProcessor = chatbotBellProcessor;
            _logger = logger;
            _chatBotBusinessLogic = chatBotBusinessLogic;
        }
        #endregion

        [Authorize]
        [HttpPost]
        public async Task<ActionResult>  AddBillForDigitalReceipt([FromBody]BillInfoMaster billInfoMaster)
        {
            try
            {
                var currentUser = HttpContext.User;
                var identity = User.Identity as ClaimsIdentity;
                string Key_ProgCode = string.Empty;
                IEnumerable<Claim> claims = new List<Claim>();
                if (identity != null)
                {
                    claims = identity.Claims;
                    Key_ProgCode = claims.Where(p => p.Type == "Program_Code").FirstOrDefault()?.Value;
                }
                bool isError = false;

                BillInfoMasterWithKey _billInfo = new BillInfoMasterWithKey();
                _billInfo.BillInfo = billInfoMaster;
                _billInfo.claims = claims;
                string ObjDRInfo = JsonConvert.SerializeObject(billInfoMaster);

                isError =  _chatBotBusinessLogic.QueueDigitalReceiptItems(ObjDRInfo, billInfoMaster.BillInfo.Program_Code);
                await Task.Delay(100);
                if (isError)
                    return Ok(new { StatusCode = "525", Message = "Technical error occured. Please try after sometime." });
                else
                    return Ok(new { StatusCode = "200", Message = "Record saved successfully." });

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex.Message);
                return BadRequest(new { StatusCode = "1001", Message = "Technical error occured. Please try after sometime." });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaveDigitalReceiptCommunicationInfo(DRCommunicationReq communicationInfo)
        {
            try
            {
                var currentUser = HttpContext.User;
                var identity = User.Identity as ClaimsIdentity;
                string Key_ProgCode = string.Empty;
                IEnumerable<Claim> claims = new List<Claim>();
                if (identity != null)
                {
                    claims = identity.Claims;
                    Key_ProgCode = claims.Where(p => p.Type == "Program_Code").FirstOrDefault()?.Value;
                }
                bool isError = false;

                DRCommunicationInfo _commInfo = new DRCommunicationInfo();
                _commInfo.CommunicationDetail = communicationInfo;
                _commInfo.claims = claims;
                //_billInfo.Key_ProgCode = Convert.ToString(Key_ProgCode);
                string ObjDR_CommInfo = JsonConvert.SerializeObject(_commInfo);
                isError = _chatBotBusinessLogic.QueueDigitalReceiptCommunicationInfo(ObjDR_CommInfo, Key_ProgCode);
                if (isError)
                    return Ok(new { StatusCode = "525", Message = "Technical error occured. Please try after sometime." });
                else
                    return Ok(new { StatusCode = "200", Message = "Record saved successfully." });

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex.Message);
                return BadRequest(new { StatusCode = "1001", Message = "Technical error occured. Please try after sometime." });
            }
        }

        [HttpPost]
        public ActionResult GetLastShoppingReceipt(LastShoppingInvoiceRequest ObjLastShoppingInvoiceRequest)
        {
            try
            {
                // //  call the method to get last Shopping Invoice receipt.
                var shoppingInvoice = _chatbotBellProcessor.GetLastShoppingReceipt(ObjLastShoppingInvoiceRequest);
                return Ok(shoppingInvoice);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex.Message);
                return BadRequest(new { StatusCode = "1001", Message = "Technical error occured. Please try after sometime." });
            }
        }
    }
}
