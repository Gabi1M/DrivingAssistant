using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Widget;
using DrivingAssistant.Core.Tools;

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
                    Timeout = 10000
                };

                var response = await request.GetResponseAsync() as HttpWebResponse;
                using var streamReader = new StreamReader(response?.GetResponseStream()!);
                return await streamReader.ReadToEndAsync() == "Success";
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return false;
            }
        }

        //============================================================
        public static void ShowToast(Context context, string message, bool _long = false)
        {
            Toast.MakeText(context, message, _long ? ToastLength.Long : ToastLength.Short);
        }

        //============================================================
        public static ProgressDialog ShowProgressDialog(Context context, string title, string message)
        {
            var progressDialog = new ProgressDialog(context);
            if (!string.IsNullOrEmpty(title))
            {
                progressDialog.SetTitle(title);
            }

            if (!string.IsNullOrEmpty(message))
            {
                progressDialog.SetMessage(message);
            }

            progressDialog.SetCancelable(false);
            progressDialog.Show();
            return progressDialog;
        }
    }
}