using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using DrivingAssistant.Core.Models;
using Object = Java.Lang.Object;

namespace DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters
{
    public class ServerViewModelAdapter : BaseAdapter
    {
        private readonly Activity _activity;
        private readonly IEnumerable<HostServer> _servers;

        //============================================================
        public ServerViewModelAdapter(Activity activity, IEnumerable<HostServer> servers)
        {
            _activity = activity;
            _servers = servers;
        }

        //============================================================
        public override int Count => _servers.Count();

        //============================================================
        public override Object GetItem(int position)
        {
            return null;
        }

        //============================================================
        public override long GetItemId(int position)
        {
            return default;
        }

        //============================================================
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.view_model_server_list, parent, false);
            var textName = view.FindViewById<TextView>(Resource.Id.serverTextName);
            var textAddress = view.FindViewById<TextView>(Resource.Id.serverTextAddress);

            var currentServer = _servers.ElementAt(position);

            textName.Text = currentServer.Name;
            textAddress.Text = currentServer.Address;

            return view;
        }
    }
}