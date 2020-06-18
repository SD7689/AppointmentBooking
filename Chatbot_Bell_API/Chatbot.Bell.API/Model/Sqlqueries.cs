using Microsoft.Extensions.Configuration;

namespace Chatbot.Bell.API.Model
{
    public class Sqlqueries
    {
        static IConfiguration queriesConfig = new ConfigurationBuilder()
         .AddXmlFile("SqlQueries.xml", true, true)
         .Build();
        public static string GetMasterConnection { get { return queriesConfig["GetMasterConnection"]; } }
        public static string GetLastTransaction { get { return queriesConfig["GetLastTransaction"]; } }
        public static string GetUserATVDetails { get { return queriesConfig["GetUserATVDetails"]; } }
        public static string GetItemsByArticlesSKUID { get { return queriesConfig["GetItemsByArticlesSKUID"]; } }
        public static string GetNearbyStoreDetails { get { return queriesConfig["GetNearbyStoreDetails"]; } }
        public static string GetNearbyStoreDetailsByPincode { get { return queriesConfig["GetNearbyStoreDetailsByPincode"]; } }
        public static string GetUserRecommendation { get { return queriesConfig["GetUserRecommendation"]; } }
        public static string GetUserKeyInsight { get { return queriesConfig["GetUserKeyInsight"]; } }
        public static string GetStoreDetailsByStoreCode { get { return queriesConfig["GetStoreDetailsByStoreCode"]; } }
        public static string GetHSNCodeBySKUID { get { return queriesConfig["GetHSNCodeBySKUID"]; } }
        public static string GetProgConfigDetails { get { return queriesConfig["GetProgConfigDetails"]; } }
        public static string GetProgList { get { return queriesConfig["GetProgList"]; } }
        public static string GetStoreCodeByCampaignDetail { get { return queriesConfig["GetStoreCodeByCampaignDetail"]; } }
        public static string GetStoreCodeByLastTransaction { get { return queriesConfig["GetStoreCodeByLastTransaction"]; } }
        public static string GetStoreCodeByMemberReport { get { return queriesConfig["GetStoreCodeByMemberReport"]; } }
        public static string GetStoreDetailsMaster { get { return queriesConfig["GetStoreDetailsMaster"]; } }
        public static string GetImageLocation { get { return queriesConfig["GetImageLocation"]; } }

    }
}
