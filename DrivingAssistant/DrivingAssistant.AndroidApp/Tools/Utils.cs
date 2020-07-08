using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DrivingAssistant.AndroidApp.Tools
{
    public static class Utils
    {
        //============================================================
        public static async Task<bool> CheckConnectionAsync(string serverUri)
        {
            try
            {
                var request = new HttpWebRequest(new Uri(serverUri + "/check_connection"))
                {
                    Method = "GET",
                    Timeout = 5000
                };

                var response = await request.GetResponseAsync() as HttpWebResponse;
                using var streamReader = new StreamReader(response.GetResponseStream());
                return await streamReader.ReadToEndAsync() == "Success";
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}