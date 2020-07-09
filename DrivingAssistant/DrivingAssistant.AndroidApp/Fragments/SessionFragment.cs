using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Fragments
{
    public class SessionFragment : Fragment
    {
        //============================================================
        public SessionFragment(ICollection<Session> sessions)
        {
            Arguments = new Bundle();
            Arguments.PutString("sessions", JsonConvert.SerializeObject(sessions));
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_sessions, container, false);
            var listView = view.FindViewById<ListView>(Resource.Id.sessionsListView);

            var sessions = JsonConvert.DeserializeObject<ICollection<Session>>(Arguments.GetString("sessions"));
            listView.Adapter = new SessionViewModelAdapter(Activity, sessions);
            listView.ItemClick += OnItemClick;
            return view;
        }

        //============================================================
        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //TODO
        }
    }
}