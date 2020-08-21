using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;

namespace DrivingAssistant.AndroidApp.Fragments.Server
{
    public class ServerFragmentViewPresenter : ViewPresenter
    {
        private readonly ServerService _serverService = new ServerService();

        private IEnumerable<HostServer> _servers;
        private int _selectedPosition;
        private View _selectedView;

        //============================================================
        public ServerFragmentViewPresenter(Context context)
        {
            _context = context;
        }

        //============================================================
        public void RefreshDataSource()
        {
            _servers = _serverService.GetAll().ToList();
            Notify(new NotificationEventArgs(NotificationCommand.ServerFragment_Refresh, _servers));
        }

        //============================================================
        public void AddButtonClick()
        {
            var alert = new AlertDialog.Builder(_context);
            alert.SetTitle("Input the server name");
            var textEditName = new EditText(_context)
            {
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent),
                Gravity = GravityFlags.Center
            };
            var textEditAddress = new EditText(_context)
            {
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent),
                Gravity = GravityFlags.Center
            };
            alert.SetView(textEditName);
            alert.SetPositiveButton("Ok", (o, args) =>
            {
                var alert2 = new AlertDialog.Builder(_context);
                alert2.SetTitle("Input the server address");
                alert2.SetView(textEditAddress);
                alert2.SetPositiveButton("Ok", (sender1, eventArgs) =>
                {
                    var server = new HostServer
                    {
                        Name = textEditName.Text?.Trim(),
                        Address = textEditAddress.Text?.Trim()
                    };
                    _serverService.Set(server);
                    RefreshDataSource();
                    Notify(new NotificationEventArgs(NotificationCommand.ServerFragment_Add, true));
                });
                alert2.SetNegativeButton("Cancel", (sender1, eventArgs) =>
                {
                });

                alert2.Create()?.Show();
            });
            alert.SetNegativeButton("Cancel", (o, args) =>
            {
            });

            alert.Create()?.Show();
        }

        //============================================================
        public void DeleteButtonClick()
        {
            if (_selectedPosition == -1)
            {
                Notify(new NotificationEventArgs(NotificationCommand.ServerFragment_Delete, new Exception("No server selected!")));
                return;
            }

            if (_servers.ElementAt(_selectedPosition).Name == HostServer.Default.Name)
            {
                Notify(new NotificationEventArgs(NotificationCommand.ServerFragment_Delete, new Exception("Cannot delete default server!")));
                return;
            }

            var alert = new AlertDialog.Builder(_context);
            alert.SetTitle("Confirm Delete");
            alert.SetMessage("Action cannot be undone");
            alert.SetPositiveButton("Delete", (o, args) =>
            {
                var server = _servers.ElementAt(_selectedPosition);
                _serverService.Delete(server.Name);
                RefreshDataSource();
                Notify(new NotificationEventArgs(NotificationCommand.ServerFragment_Delete, true));
            });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            alert.Create()?.Show();
        }

        //============================================================
        public void ModifyButtonClick()
        {
            if (_selectedPosition == -1)
            {
                Notify(new NotificationEventArgs(NotificationCommand.ServerFragment_Modify, new Exception("No server selected!")));
                return;
            }

            if (_servers.ElementAt(_selectedPosition).Name == HostServer.Default.Name)
            {
                Notify(new NotificationEventArgs(NotificationCommand.ServerFragment_Modify, new Exception("Cannot modify default server!")));
                return;
            }

            var alert = new AlertDialog.Builder(_context);
            alert.SetTitle("Input the server name");
            var textEditName = new EditText(_context)
            {
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent),
                Gravity = GravityFlags.Center
            };
            var textEditAddress = new EditText(_context)
            {
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent),
                Gravity = GravityFlags.Center
            };
            alert.SetView(textEditName);
            alert.SetPositiveButton("Ok", (o, args) =>
            {
                var alert2 = new AlertDialog.Builder(_context);
                alert2.SetTitle("Input the server address");
                alert2.SetView(textEditAddress);
                alert2.SetPositiveButton("Ok", (sender1, eventArgs) =>
                {
                    var server = _servers.ElementAt(_selectedPosition);
                    _serverService.Delete(server.Name);
                    server = new HostServer
                    {
                        Name = textEditName.Text?.Trim(),
                        Address = textEditAddress.Text?.Trim()
                    };
                    _serverService.Set(server);
                    RefreshDataSource();
                    Notify(new NotificationEventArgs(NotificationCommand.ServerFragment_Modify, true));
                });
                alert2.SetNegativeButton("Cancel", (sender1, eventArgs) =>
                {
                });

                alert2.Create()?.Show();
            });
            alert.SetNegativeButton("Cancel", (o, args) =>
            {
            });

            alert.Create()?.Show();
        }

        //============================================================
        public void ItemClick(int position, View view)
        {
            _selectedView?.SetBackgroundResource(0);
            _selectedPosition = position;
            _selectedView = view;
            Notify(new NotificationEventArgs(NotificationCommand.ServerFragment_ItemClick, view));
        }
    }
}