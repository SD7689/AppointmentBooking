using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chatbot.Bell.API.Model;
using Chatbot.Bell.API.Processor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Chatbot.Bell.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElasticSearchController : ControllerBase
    {
        private readonly ILogger<ElasticSearchController> _logger;
        private readonly ElasticsearchApiProcessor _ElasticsearchApiProcessor;

        public ElasticSearchController(ILogger<ElasticSearchController> logger, ElasticsearchApiProcessor elasticsearchApiProcessor)
        {
            _ElasticsearchApiProcessor = elasticsearchApiProcessor;
            _logger = logger;
        }

        [HttpPost]
        public string GetItemDetails(item_master_inventory searchInfoLog)
        {
            List<Source> Response = new List<Source>();
            try
            {
                Response = _ElasticsearchApiProcessor.SearchItemDetails(searchInfoLog);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(string.Format("GetItemDetails Call Error Occured: {0}", ex.ToString()));
                _logger.LogError(string.Format("GetItemDetails Call Error Occured: {0}", ex.ToString()));
            }
            var json = JsonConvert.SerializeObject(Response);
            return json;
        }
    }
}
