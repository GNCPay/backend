using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace eWallet.Backend
{
    /// <summary>
    /// Lớp xử lý các vấn đề chung, xài chung
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Object kết nối và xử lý với database
        /// </summary>
        public static eWallet.Data.MongoHelper DataHelper;

        public static void Init()
        {
            DataHelper = new Data.MongoHelper(
                ConfigurationSettings.AppSettings["CoreDB_Server"],
                ConfigurationSettings.AppSettings["CoreDB_Database"]
                );
        }
    }
}