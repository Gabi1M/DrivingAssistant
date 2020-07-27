using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.Core.Models;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments
{
    public class ServerFragment : Fragment
    {
        private readonly User _user;

        private ListView _listView;
        private Button _addButton;
        private Button _deleteButton;
        private Button _modifyButon;

        private int _selectedPosition = -1;
        private View _selectedView;

        private readonly ServerService _serverService = new ServerService();
        private ICollection<HostServer> _currentServers;

        //============================================================
        public ServerFragment(User user)
        {
            _user = user;
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_servers, container, false);
            SetupFragmentFields(view);
            SetupListAdapter();
            return view;
        }

        //============================================================
        private void SetupFragmentFields(View view)
        {
            _listView = view.FindViewById<ListView>(Resource.Id.serversListView);
            _addButton = view.FindViewById<Button>(Resource.Id.serversButtonAdd);
            _deleteButton = view.FindViewById<Button>(Resource.Id.serversButtonDelete);
            _modifyButon = view.FindViewById<Button>(Resource.Id.serversButtonModify);

            _addButton.Click += OnAddButtonClick;
            _deleteButton.Click += OnDeleteButtonClick;
            _modifyButon.Click += OnModifyButtonClick;
        }

        //============================================================
        private void SetupListAdapter()
        {
            RefreshDataSource();
            _listView.ChoiceMode = ChoiceMode.Single;
            _listView.ItemClick += OnItemClick;
        }

        //============================================================
        private void RefreshDataSource()
        {
            _currentServers = _serverService.GetAll().ToList();
            _listView.Adapter?.Dispose();
            _listView.Adapter = new ServerViewModelAdapter(Activity, _currentServers);
        }

        //============================================================
        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedView?.SetBackgroundResource(0);
            e.View.SetBackgroundResource(Resource.Drawable.list_element_border);
            _selectedPosition = e.Position;
            _selectedView = e.View;
        }

        //============================================================
        private void OnAddButtonClick(object sender, EventArgs e)
        {
            var alert = new AlertDialog.Builder(Context);
            alert.SetTitle("Input the server name");
            var textEditName = new EditText(Context);
            var textEditAddress = new EditText(Context);
            var layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            textEditName.LayoutParameters = layoutParams;
            textEditAddress.LayoutParameters = layoutParams;
            textEditName.Gravity = GravityFlags.Center;
            textEditAddress.Gravity = GravityFlags.Center;
            alert.SetView(textEditName);
            alert.SetPositiveButton("Ok", (o, args) =>
            {
                var alert2 = new AlertDialog.Builder(Context);
                alert2.SetTitle("Input the server address");
                alert2.SetView(textEditAddress);
                alert2.SetPositiveButton("Ok", (sender1, eventArgs) =>
                {
                    var server = new HostServer
                    {
                        Name = textEditName.Text.Trim(),
                        Address = textEditAddress.Text.Trim()
                    };
                    _serverService.Set(server);
                    RefreshDataSource();
                });
                alert2.SetNegativeButton("Cancel", (sender1, eventArgs) =>
                {
                });

                var dialog2 = alert2.Create();
                dialog2.Show();
            });
            alert.SetNegativeButton("Cancel", (o, args) =>
            {
            });

            var dialog = alert.Create();
            dialog.Show();
        }

        //============================================================
        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            if (_selectedPosition == -1)
            {
                Toast.MakeText(Context, "No Server selected!", ToastLength.Short).Show();
                return;
            }

            if (_currentServers.ElementAt(_selectedPosition).Name == HostServer.Default.Name)
            {
                Toast.MakeText(Context, "Cannot delete default server!", ToastLength.Short).Show();
                return;
            }

            var alert = new AlertDialog.Builder(Context);
            alert.SetTitle("Confirm Delete");
            alert.SetMessage("Action cannot be undone");
            alert.SetPositiveButton("Delete", (o, args) =>
            {
                var server = _currentServers.ElementAt(_selectedPosition);
                _serverService.Delete(server.Name);
                Toast.MakeText(Context, "Server deleted!", ToastLength.Short).Show();
                RefreshDataSource();
            });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            var dialog = alert.Create();
            dialog.Show();
        }

        //============================================================
        private void OnModifyButtonClick(object sender, EventArgs e)
        {
            if (_selectedPosition == -1)
            {
                Toast.MakeText(Context, "No server selected!", ToastLength.Short).Show();
                return;
            }

            if (_currentServers.ElementAt(_selectedPosition).Name == HostServer.Default.Name)
            {
                Toast.MakeText(Context, "Cannot modify default server!", ToastLength.Short).Show();
                return;
            }

            var alert = new AlertDialog.Builder(Context);
            alert.SetTitle("Input the server name");
            var textEditName = new EditText(Context);
            var textEditAddress = new EditText(Context);
            var layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            textEditName.LayoutParameters = layoutParams;
            textEditAddress.LayoutParameters = layoutParams;
            textEditName.Gravity = GravityFlags.Center;
            textEditAddress.Gravity = GravityFlags.Center;
            alert.SetView(textEditName);
            alert.SetPositiveButton("Ok", (o, args) =>
            {
                var alert2 = new AlertDialog.Builder(Context);
                alert2.SetTitle("Input the server address");
                alert2.SetView(textEditAddress);
                alert2.SetPositiveButton("Ok", (sender1, eventArgs) =>
                {
                    var server = _currentServers.ElementAt(_selectedPosition);
                    _serverService.Delete(server.Name);

                    server = new HostServer
                    {
                        Name = textEditName.Text.Trim(),
                        Address = textEditAddress.Text.Trim()
                    };
                    _serverService.Set(server);
                    RefreshDataSource();
                });
                alert2.SetNegativeButton("Cancel", (sender1, eventArgs) =>
                {
                });

                var dialog2 = alert2.Create();
                dialog2.Show();
            });
            alert.SetNegativeButton("Cancel", (o, args) =>
            {
            });

            var dialog = alert.Create();
            dialog.Show();
        }
    }
}