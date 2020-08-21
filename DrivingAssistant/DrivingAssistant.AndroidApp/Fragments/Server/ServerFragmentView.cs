using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments.Server
{
    public sealed class ServerFragmentView : Fragment
    {
        private ListView _listView;
        private Button _addButton;
        private Button _deleteButton;
        private Button _modifyButon;

        private readonly Context _activityContext;
        private readonly ServerFragmentViewPresenter _viewPresenter;

        //============================================================
        public ServerFragmentView(Context activityContext, User user)
        {
            _activityContext = activityContext;
            _viewPresenter = new ServerFragmentViewPresenter(activityContext);
            _viewPresenter.OnNotificationReceived += ViewPresenterOnNotificationReceived;
        }

        //============================================================
        private void ViewPresenterOnNotificationReceived(object sender, NotificationEventArgs e)
        {
            if (e.Data is Exception ex)
            {
                Utils.ShowToast(_activityContext, ex.Message, true);
                return;
            }

            switch (e.Command)
            {
                case NotificationCommand.ServerFragment_ItemClick:
                {
                    var view = e.Data as View;
                    view?.SetBackgroundResource(Resource.Drawable.list_element_border);
                    break;
                }
                case NotificationCommand.ServerFragment_Refresh:
                {
                    _listView.Adapter?.Dispose();
                    _listView.Adapter = new ServerViewModelAdapter(Activity, e.Data);
                    break;
                }
                case NotificationCommand.ServerFragment_Add:
                {
                    if (Convert.ToBoolean(e.Data))
                    {
                        Utils.ShowToast(_activityContext, "Server successfully added!");
                    }
                    break;
                }
                case NotificationCommand.ServerFragment_Delete:
                {
                    if (Convert.ToBoolean(e.Data))
                    {
                        Utils.ShowToast(_activityContext, "Server successfully deleted!");
                    }
                    break;
                }
                case NotificationCommand.ServerFragment_Modify:
                {
                    if (Convert.ToBoolean(e.Data))
                    {
                        Utils.ShowToast(_activityContext, "Server successfully modified!");
                    }
                    break;
                }
            }
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_servers, container, false);
            SetupFragmentFields(view);
            SetupListAdapter();
            _viewPresenter.RefreshDataSource();
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
            _listView.ChoiceMode = ChoiceMode.Single;
            _listView.ItemClick += OnItemClick;
        }

        //============================================================
        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _viewPresenter.ItemClick(e.Position, e.View);
        }

        //============================================================
        private void OnAddButtonClick(object sender, EventArgs e)
        {
            _viewPresenter.AddButtonClick();
        }

        //============================================================
        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            _viewPresenter.DeleteButtonClick();
        }

        //============================================================
        private void OnModifyButtonClick(object sender, EventArgs e)
        {
            _viewPresenter.ModifyButtonClick();
        }
    }
}