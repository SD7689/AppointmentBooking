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
    public class InsertApplicationLogController : ControllerBase
    {
        private readonly ILogger<ElasticSearchController> _logger;
        private readonly ElasticsearchApiProcessor _ElasticsearchApiProcessor;

        public InsertApplicationLogController(ILogger<ElasticSearchController> logger, ElasticsearchApiProcessor elasticsearchApiProcessor)
        {
            _ElasticsearchApiProcessor = elasticsearchApiProcessor;
            _logger = logger;
        }

        [HttpPost]
        public string PublishApplicationLog(QueueModel apiLog)
        {
            string Response = string.Empty;
            try
            {
                if (Convert.ToBoolean(AppConfig.IsRabbitMQ))
                {
                    bool IsSuccess = _ElasticsearchApiProcessor.PushLogIntoRabiitMQ(apiLog);

                    if (IsSuccess)
                    {
                        Response = string.Format("Success: [{0}]", JsonConvert.SerializeObject(apiLog));
                    }
                    else
                    {
                        Response = string.Format("Fail: [{0}]", JsonConvert.SerializeObject(apiLog));
                    }
                }
                else
                {
                    Response = _ElasticsearchApiProcessor.PushInfoLog(apiLog);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(string.Format("PublishInfoLog Call Error Occured: {0}", ex.ToString()));
                _logger.LogError(string.Format("PublishInfoLog Call Error Occured: {0}", ex.ToString()));
            }
            return Response;
        }
    }
}
