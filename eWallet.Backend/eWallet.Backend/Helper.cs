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
        private static CoreAPI.ChannelAPIClient client = new CoreAPI.ChannelAPIClient();
        public static void Init()
        {
            DataHelper = new Data.MongoHelper(
                ConfigurationSettings.AppSettings["CoreDB_Server"],
                ConfigurationSettings.AppSettings["CoreDB_Database"]
                );
        }
        public static string RequestToServer(string request)
        {
            string response = @"{error_code:'96',error_message:'Có lỗi trong quá trình xử lý. Vui lòng thử lại sau'}";
            if (client.State != System.ServiceModel.CommunicationState.Opened)
            {
                try
                {
                    client.Abort();
                    client = new CoreAPI.ChannelAPIClient();
                    client.Open();
                }
                catch
                {

                }
            }
            try
            {
                response = client.Process(request);
            }
            catch
            {
            }
            return response;
        }
    }
}