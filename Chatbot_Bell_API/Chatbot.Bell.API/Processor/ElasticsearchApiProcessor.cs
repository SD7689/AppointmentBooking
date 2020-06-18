using Chatbot.Bell.API.Model;
using Chatbot.Bell.API.RabbitMQManager;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chatbot.Bell.API.Processor
{
    public class ElasticsearchApiProcessor
    {
        private readonly ILogger<ElasticsearchApiProcessor> _logger;
        private readonly IRabbitManager _rabbitManager;
        public ElasticsearchApiProcessor(ILogger<ElasticsearchApiProcessor> logger, IRabbitManager rabbitManager)
        {
            _rabbitManager = rabbitManager;

            _logger = logger;
        }
        public List<Source> SearchItemDetails(item_master_inventory search)
        {
            List<Source> Response = new List<Source>();
            try
            {
                if (search != null)
                {
                    //string Request = PrepareJsonRequestForsearch2(search);
                    if (!string.IsNullOrEmpty(search.BrandName) && !string.IsNullOrEmpty(search.ProgramCode))
                    {
                        string APIURL = string.Format(AppConfig.InfoAPISearchUrl, search.ProgramCode.ToLower().Trim(), search.BrandName.ToLower().Trim());
                        _logger.LogInformation(string.Format("APIURL: {0}", APIURL));
                        Response = GetAPIResponse(APIURL);
                    }
                }
                else
                {
                    _logger.LogError(string.Format("No search result found"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(string.Format("PushErrorLog Call Error Occured: {0}", ex.ToString()));
                _logger.LogError(string.Format("PushErrorLog Call Error Occured: {0}", ex.ToString()));
            }
            return Response;
        }

        #region Get Item Details Elasticsearch Json Prepare
        private string PrepareJsonRequestForsearch2(item_master_inventory objsearch)
        {
            string Request = string.Empty;
           // bool bFlag = false;
            try
            {
                if (objsearch != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{");
                    sb.Append("\"query\": {");
                    sb.Append("\"bool\": {");
                    sb.Append("\"must\": [");

                    if (!string.IsNullOrEmpty(objsearch.BrandName))
                    {
                        sb.Append("{");
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"brandname\": \"{0}\"", objsearch.BrandName);
                        sb.Append("}");
                        sb.Append("}");
                      //  bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }

                    sb.Append("]");
                    sb.Append("}");
                    sb.Append("}");
                    sb.Append("}");

                    Request = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(string.Format("PrepareJsonRequest Call Error Occured: {0}", ex.ToString()));
                _logger.LogError(string.Format("PrepareJsonRequest Call Error Occured: {0}", ex.ToString()));
            }
            return Request;
        }
        #endregion

        #region Commented Code for Search Itam Details Json Request Prepare
        /*
        private string PrepareJsonRequestForsearch2(item_master_inventory objsearch)
        {
            string Request = string.Empty;
            bool bFlag = false;
            try
            {
                if (objsearch != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{");
                    sb.Append("\"query\": {");
                    sb.Append("\"bool\": {");
                    sb.Append("\"must\": [");

                    if (objsearch.sno != 0)
                    {
                        sb.Append("{");
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"sno\": \"{0}\"", objsearch.sno);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }

                    if (!string.IsNullOrEmpty(objsearch.Gender))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"Gender\": \"{0}\"", objsearch.Gender);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }

                    if (!string.IsNullOrEmpty(objsearch.Category))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"Category\": \"{0}\"", objsearch.Category);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }

                    if (!string.IsNullOrEmpty(objsearch.SubCategoryName))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"SubCategoryName\": \"{0}\"", objsearch.SubCategoryName);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.BrandName))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"BrandName\": \"{0}\"", objsearch.BrandName);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.Colour))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"Colour\": \"{0}\"", objsearch.Colour);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ColourCode))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ColourCode\": \"{0}\"", objsearch.ColourCode);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.Gender))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"response\": \"{0}\"", objsearch.Gender);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.Price))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"Price\": \"{0}\"", objsearch.Price);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.Discount))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"Discount\": \"{0}\"", objsearch.Discount);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.Size))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"Size\": \"{0}\"", objsearch.Size);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemProperties1))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemProperties1\": \"{0}\"", objsearch.ItemProperties1);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemProperties2))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemProperties2\": \"{0}\"", objsearch.ItemProperties2);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemProperties3))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemProperties3\": \"{0}\"", objsearch.ItemProperties3);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemProperties4))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemProperties4\": \"{0}\"", objsearch.ItemProperties4);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemProperties5))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemProperties5\": \"{0}\"", objsearch.ItemProperties5);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemProperties6))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemProperties6\": \"{0}\"", objsearch.ItemProperties6);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemProperties7))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemProperties7\": \"{0}\"", objsearch.ItemProperties7);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemProperties8))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemProperties8\": \"{0}\"", objsearch.ItemProperties8);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemProperties9))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemProperties9\": \"{0}\"", objsearch.ItemProperties9);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemProperties10))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemProperties10\": \"{0}\"", objsearch.ItemProperties10);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemProperties11))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemProperties11\": \"{0}\"", objsearch.ItemProperties11);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemProperties12))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemProperties12\": \"{0}\"", objsearch.ItemProperties12);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemProperties13))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemProperties13\": \"{0}\"", objsearch.ItemProperties13);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemProperties14))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemProperties14\": \"{0}\"", objsearch.ItemProperties14);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemProperties15))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemProperties15\": \"{0}\"", objsearch.ItemProperties15);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties1))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemSubProperties1\": \"{0}\"", objsearch.ItemSubProperties1);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties2))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemSubProperties2\": \"{0}\"", objsearch.ItemSubProperties2);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties3))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemSubProperties3\": \"{0}\"", objsearch.ItemSubProperties3);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties4))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemSubProperties3\": \"{0}\"", objsearch.ItemSubProperties3);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties4))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemSubProperties4\": \"{0}\"", objsearch.ItemSubProperties4);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties5))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemSubProperties5\": \"{0}\"", objsearch.ItemSubProperties5);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties6))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"response\": \"{0}\"", objsearch.ItemSubProperties6);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties7))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemSubProperties7\": \"{0}\"", objsearch.ItemSubProperties7);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties8))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemSubProperties8\": \"{0}\"", objsearch.ItemSubProperties8);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties9))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemSubProperties9\": \"{0}\"", objsearch.ItemSubProperties9);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties10))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemSubProperties10\": \"{0}\"", objsearch.ItemSubProperties10);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties11))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemSubProperties11\": \"{0}\"", objsearch.ItemSubProperties11);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties12))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemSubProperties12\": \"{0}\"", objsearch.ItemSubProperties12);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties13))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemSubProperties13\": \"{0}\"", objsearch.ItemSubProperties13);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties14))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemSubProperties14\": \"{0}\"", objsearch.ItemSubProperties14);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemSubProperties15))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemSubProperties15\": \"{0}\"", objsearch.ItemSubProperties15);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemCode))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemCode\": \"{0}\"", objsearch.ItemCode);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ItemName))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ItemName\": \"{0}\"", objsearch.ItemName);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ProductURL))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ProductURL\": \"{0}\"", objsearch.ProductURL);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.ImageURL))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"ImageURL\": \"{0}\"", objsearch.ImageURL);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.Store))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"Store\": \"{0}\"", objsearch.Store);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }
                    if (!string.IsNullOrEmpty(objsearch.inventoryCount))
                    {
                        if (bFlag)
                        {
                            sb.Append(",{");
                        }
                        else
                        {
                            sb.Append("{");
                        }
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"inventoryCount\": \"{0}\"", objsearch.inventoryCount);
                        sb.Append("}");
                        sb.Append("}");
                        bFlag = true;
                    }
                    else
                    {
                        //bFlag = false;
                    }

                    sb.Append("}");
                    sb.Append("}");
                    sb.Append("]");
                    sb.Append("}");




                    Request = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(string.Format("PrepareJsonRequest Call Error Occured: {0}", ex.ToString()));
                _logger.LogError(string.Format("PrepareJsonRequest Call Error Occured: {0}", ex.ToString()));
            }
            return Request;
        }
        */
        #endregion

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
                webReq.Method = "GET";
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
                    _logger.LogInformation(string.Format("Response Status: {0}", response.StatusCode));
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(string.Format("GetAPIResponse Call Error Occured: {0}", ex.ToString()));
                _logger.LogError(string.Format("GetAPIResponse Call Error Occured: {0}", ex.ToString()));
            }
            return responseStr;
        }
        private List<Source> GetAPIResponse(string WebApiUrl)
        {
            string responseStr = string.Empty;
            List<Source> responseList = new List<Source>();

            try
            {
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(WebApiUrl.ToLower());
                //webReq.ContentType = "application/json; encoding='utf-8'";
                // webReq.Method = "GET";
                webReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseStr = reader.ReadToEnd();
                    var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseStr);
                    
                    foreach (var items in jsonResult.hits.hits)
                    {
                        Source objResponse = new Source();
                        objResponse.imageurl = items["_source"]["imageurl"];
                        objResponse.brandname = items["_source"]["brandname"];
                        objResponse.@version = items["_source"]["@version"];
                        objResponse.type = items["_source"]["type"];
                        objResponse.@timestamp = (DateTime)(items["_source"]["@timestamp"]);
                        /* DateTime.ParseExact(items["_source"]["@timestamp"], "MM/dd/yyyy", CultureInfo.InvariantCulture);*/
                        objResponse.subcategoryname = items["_source"]["subcategoryname"];
                        objResponse.store = items["_source"]["store"];
                        objResponse.sno = items["_source"]["sno"];
                        objResponse.size = items["_source"]["size"];
                        objResponse.producturl = items["_source"]["producturl"];
                        objResponse.price = items["_source"]["price"];
                        objResponse.modificationdatetime = (DateTime)(items["_source"]["modificationdatetime"]);
                        objResponse.itemsubproperties9 = items["_source"]["itemsubproperties9"];
                        objResponse.itemsubproperties8 = items["_source"]["itemsubproperties8"];
                        objResponse.itemsubproperties7 = items["_source"]["itemsubproperties7"];
                        objResponse.itemsubproperties6 = items["_source"]["itemsubproperties6"];
                        objResponse.itemsubproperties5 = items["_source"]["itemsubproperties5"];
                        objResponse.itemsubproperties4 = items["_source"]["itemsubproperties4"];
                        objResponse.itemsubproperties3 = items["_source"]["itemsubproperties3"];
                        objResponse.itemsubproperties2 = items["_source"]["itemsubproperties2"];
                        objResponse.itemsubproperties15 = items["_source"]["itemsubproperties15"];
                        objResponse.itemsubproperties14 = items["_source"]["itemsubproperties14"];
                        objResponse.itemsubproperties13 = items["_source"]["itemsubproperties13"];
                        objResponse.itemsubproperties12 = items["_source"]["itemsubproperties12"];
                        objResponse.itemsubproperties11 = items["_source"]["itemsubproperties11"];
                        objResponse.itemsubproperties10 = items["_source"]["itemsubproperties10"];
                        objResponse.itemsubproperties1 = items["_source"]["itemsubproperties1"];
                        objResponse.itemproperties9 = items["_source"]["itemproperties9"];
                        objResponse.itemproperties8 = items["_source"]["itemproperties8"];
                        objResponse.itemproperties7 = items["_source"]["itemproperties7"];
                        objResponse.itemproperties6 = items["_source"]["itemproperties6"];
                        objResponse.itemproperties5 = items["_source"]["itemproperties5"];
                        objResponse.itemproperties4 = items["_source"]["itemproperties4"];
                        objResponse.itemproperties3 = items["_source"]["itemproperties3"];
                        objResponse.itemproperties2 = items["_source"]["itemproperties2"];
                        objResponse.itemproperties15 = items["_source"]["itemproperties15"];
                        objResponse.itemproperties14 = items["_source"]["itemproperties14"];
                        objResponse.itemproperties13 = items["_source"]["itemproperties13"];
                        objResponse.itemproperties12 = items["_source"]["itemproperties12"];
                        objResponse.itemproperties11 = items["_source"]["itemproperties11"];
                        objResponse.itemproperties10 = items["_source"]["itemproperties10"];
                        objResponse.itemproperties1 = items["_source"]["itemproperties1"];
                        objResponse.itemname = items["_source"]["itemname"];
                        objResponse.itemcode = items["_source"]["itemcode"];
                        objResponse.inventorycount = items["_source"]["inventorycount"];
                        objResponse.gender = items["_source"]["gender"];
                        objResponse.discount = items["_source"]["discount"];
                        objResponse.creationdatetime = (DateTime)(items["_source"]["creationdatetime"]);
                        objResponse.colourcode = items["_source"]["colourcode"];
                        objResponse.colour = items["_source"]["colour"];
                        responseList.Add(objResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(string.Format("GetAPIResponse Call Error Occured: {0}", ex.ToString()));
                _logger.LogError(string.Format("GetAPIResponse Call Error Occured: {0}", ex.ToString()));
            }
            return responseList;
        }
        public string PushInfoLog(QueueModel apiLog)
        {
            string Response = string.Empty;
            try
            {
                if (apiLog != null)
                {
                    string Request = PrepareJsonRequestForInfoLog(apiLog);
                    if (!string.IsNullOrEmpty(Request))
                    {
                        string APIURL = string.Format(AppConfig.InfoAPILogUrl, apiLog.ApplicationId.ToLower());
                        Response = GetAPIResponseRabbit(Request, APIURL);
                    }
                }
                else
                {
                    Response = "The Info Log Request is Empty.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(string.Format("PushInfoLog Call Error Occured: {0}", ex.ToString()));
                _logger.LogError(string.Format("PushInfoLog Call Error Occured: {0}", ex.ToString()));
            }
            return Response;
        }
        private string GetAPIResponseRabbit(string request, string WebApiUrl)
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
                    _logger.LogInformation(string.Format("Response Status: {0}", response.StatusCode));
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(string.Format("GetAPIResponse Call Error Occured: {0}", ex.ToString()));
                _logger.LogInformation(string.Format("GetAPIResponse Call Error Occured: {0}", ex.ToString()));
            }
            return responseStr;
        }

        private string PrepareJsonRequestForInfoLog(QueueModel objApiLog)
        {
            string Request = string.Empty;
            try
            {
                if (objApiLog != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{");
                    sb.AppendFormat("\"ApplicationId\": \"{0}\",", objApiLog.ApplicationId.Replace(@"""", @"\"""));
                    sb.AppendFormat("\"ActionName\": \"{0}\",", objApiLog.ActionName.Replace(@"""", @"\"""));
                    sb.AppendFormat("\"IPAddress\": \"{0}\",", objApiLog.IPAddress);
                    sb.AppendFormat("\"TenantID\": \"{0}\",", objApiLog.TenantID);
                    sb.AppendFormat("\"ControllerName\": \"{0}\",", objApiLog.ControllerName);
                    sb.AppendFormat("\"MessageException\": \"{0}\",", objApiLog.MessageException);
                    sb.AppendFormat("\"UserID\": \"{0}\",", objApiLog.UserID);
                    sb.AppendFormat("\"Exceptions\": \"{0}\",", objApiLog.Exceptions);
                    sb.Append("}");
                    Request = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(string.Format("PrepareJsonRequest Call Error Occured: {0}", ex.ToString()));
                _logger.LogError(string.Format("PrepareJsonRequest Call Error Occured: {0}", ex.ToString()));
            }
            return Request;
        }
        private bool RabbitMQUplad(object LogData, string RabbitMQName)
        {
            _logger.LogInformation(string.Format("RabbitMQUplad Processing for Rabbit MQ [{0}]", RabbitMQName));
            try
            {
                _rabbitManager.Publish(LogData, RabbitMQName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _logger.LogInformation("RabbitMQUplad end...");
            }
        }
        private bool UploadInRabbitMQ(QueueModel objApiLog)
        {
            bool IsSuccess = true;
            try
            {
                _logger.LogInformation("UploadInRabbitMQ Started...");
                _logger.LogInformation(string.Format("UploadInRabbitMQ Process start for Data: [{0}]", JsonConvert.SerializeObject(objApiLog)));

                _logger.LogInformation(string.Format("ApplicationId [{0}].", objApiLog.ApplicationId));
                _logger.LogInformation(string.Format("ActionName [{0}].", objApiLog.ActionName));
                _logger.LogInformation(string.Format("IPAddress [{0}].", objApiLog.IPAddress));
                _logger.LogInformation(string.Format("ControllerName [{0}].", objApiLog.ControllerName));
                _logger.LogInformation(string.Format("Exceptions [{0}].", objApiLog.Exceptions));
                _logger.LogInformation(string.Format("TenantID [{0}].", objApiLog.TenantID));
                _logger.LogInformation(string.Format("UserID [{0}].", objApiLog.UserID));
                _logger.LogInformation(string.Format("MessageException [{0}].", objApiLog.MessageException));


                string RabbitMQName = string.Empty;
                if (Convert.ToBoolean(AppConfig.IsMultiQueue))
                {
                    string[] RabbitMQNames = AppConfig.RabbitMQName.Split('|');
                    Random rnd = new Random();
                    RabbitMQName = RabbitMQNames[rnd.Next(0, RabbitMQNames.Length)];
                }
                else
                {
                    RabbitMQName = AppConfig.RabbitMQName;
                }

                if (!string.IsNullOrEmpty(RabbitMQName))
                {
                    _logger.LogInformation(string.Format("The Rabbit MQ Name [{0}] Found", RabbitMQName));
                    _logger.LogInformation(string.Format("RabbitMQUplad Process start into the Rabbit MQ [{0}]", RabbitMQName));
                    IsSuccess = RabbitMQUplad(objApiLog, RabbitMQName);
                }
                else
                {
                    _logger.LogInformation(string.Format("No Rabbit MQ Name Found"));
                }
                _logger.LogInformation("RabbitMQUplad Process end for Data");
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                _logger.LogError("UploadInRabbitMQ error for Data: " + JsonConvert.SerializeObject(objApiLog) + "  Error Message : " + ex.Message + "");
            }
            return IsSuccess;
        }
        private bool PushLogIntoRabiitMQueue(QueueModel objApiLog)
        {
            _logger.LogInformation("UploadInRabbitMQ started...");
            string jsonRabitMQLog = JsonConvert.SerializeObject(objApiLog);
            _logger.LogInformation("UploadInRabbitMQ Data " + jsonRabitMQLog + "");
            bool IsSuccess;
            try
            {
                _logger.LogInformation("Uploading in RabbitMQ start for Data");
                IsSuccess = UploadInRabbitMQ(objApiLog);
                if (IsSuccess)
                {
                    _logger.LogInformation("Uploading in RabbitMQ Success");
                }
                else
                {
                    _logger.LogInformation("Uploading in RabbitMQ Failed");
                }
                _logger.LogInformation("Uploading in RabbitMQ end for Data");
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                _logger.LogError("UploadInRabbitMQ error for Data: " + jsonRabitMQLog + "  Error Message: " + ex.Message + "");
            }

            return IsSuccess;
        }
        public bool PushLogIntoRabiitMQ(QueueModel objApiLog)
        {
            _logger.LogInformation("PushLogIntoRabiitMQ started...");
            string jsonRabitMQLog = JsonConvert.SerializeObject(objApiLog);
            _logger.LogInformation("PushLogIntoTabitMQ Data " + jsonRabitMQLog + "");
            bool IsSuccess;
            try
            {
                _logger.LogInformation("Uploading in RabbitMQ start for Data");
                IsSuccess = PushLogIntoRabiitMQueue(objApiLog);
                if (IsSuccess)
                {
                    _logger.LogInformation("Uploading in RabbitMQ Success");
                }
                else
                {
                    _logger.LogInformation("Uploading in RabbitMQ Failed");
                }
                _logger.LogInformation("Uploading in RabbitMQ end for Data ");
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                _logger.LogError("PushLogIntoTabitMQ error for Data: " + jsonRabitMQLog + "  Error Message: " + ex.Message + "");
            }

            return IsSuccess;
        }

    }

}

