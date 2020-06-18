using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Chatbot.Bell.API.Model
{
    public class DBConfig
    {
        static IConfiguration dbConfig = new ConfigurationBuilder()
        .AddJsonFile("dbSettings.json", true, true)
        .Build();
        public static string ConnectionString { get { return dbConfig["ConnectionStrings:DefaultConnection"]; } }
        public static string GetATVDataByMobileNumber { get { return dbConfig["USP_Bell_GetUserDetails"]; } }
        public static string S8 { get { return dbConfig["ConnectionStrings:S8"]; } }
        public static string TestConnection { get { return dbConfig["ConnectionStrings:TestConnection"]; } }
        public static string GetStoreLocator { get { return dbConfig["GetStoreLocator"]; } }
    }
}
