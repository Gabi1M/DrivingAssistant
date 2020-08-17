using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Fragments;
using DrivingAssistant.AndroidApp.Fragments.Home;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Activities.Main
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private Android.Support.V7.Widget.Toolbar _toolbar;
        private DrawerLayout _drawer;
        private NavigationView _navigationView;
        private TextView _textViewUser;
        private TextView _textViewUserEmail;
        private TextView _textViewUserRole;

        private MainActivityPresenter _presenter;
        private User _user;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            SetupActivityFields();
            SetupToolbar();
            ExtractIntent();
            SetupDrawer();

            _presenter = new MainActivityPresenter(this);
            _presenter.OnPropertyChanged += PresenterOnPropertyChanged;

            ChangeFragment(new HomeFragment(_user), "Home");
        }

        //============================================================
        private void PresenterOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.Data is Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long)?.Show();
                return;
            }

            switch (e.Command)
            {
                case NotifyCommand.MainActivity_Navigation:
                {
                    switch (e.Data)
                    {
                        case HomeFragment homeFragment:
                        {
                            ChangeFragment(homeFragment, "Home");
                            break;
                        }
                        case SessionFragment sessionFragment:
                        {
                            ChangeFragment(sessionFragment, "Sessions");
                            break;
                        }
                        case ServerFragment serverFragment:
                        {
                            ChangeFragment(serverFragment, "Servers");
                            break;
                        }
                        case SettingsFragment settingsFragment:
                        {
                            ChangeFragment(settingsFragment, "Settings");
                            break;
                        }
                        case null:
                        {
                            Toast.MakeText(Application.Context, "Logging out...", ToastLength.Short)?.Show();
                            Finish();
                            break;
                        }
                    }
                    FindViewById<DrawerLayout>(Resource.Id.drawer_layout)?.CloseDrawer(GravityCompat.Start);
                    break;
                }
            }
        }

        //============================================================
        private void SetupActivityFields()
        {
            _toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            _drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            _textViewUser = _navigationView?.GetHeaderView(0).FindViewById<TextView>(Resource.Id.headerTextUser);
            _textViewUserEmail = _navigationView?.GetHeaderView(0).FindViewById<TextView>(Resource.Id.headerTextUserEmail);
            _textViewUserRole = _navigationView?.GetHeaderView(0).FindViewById<TextView>(Resource.Id.headerTextUserRole);
        }

        //============================================================
        private void SetupToolbar()
        {
            _toolbar.Title = "Home";
            SetSupportActionBar(_toolbar);
        }

        //============================================================
        private void SetupDrawer()
        {
            var toggle = new ActionBarDrawerToggle(this, _drawer, _toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            _drawer.AddDrawerListener(toggle);
            toggle.SyncState();
            _navigationView.SetNavigationItemSelectedListener(this);
        }

        //============================================================
        private void ExtractIntent()
        {
            _user = JsonConvert.DeserializeObject<User>(Intent?.GetStringExtra("user")!);
            _textViewUser.Text = _user.FirstName + " " + _user.LastName;
            _textViewUserEmail.Text = _user.Email;
            _textViewUserRole.Text = _user.Role.ToString();
        }

        //============================================================
        private void ChangeFragment(Fragment fragment, string title)
        {
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frameLayout1, fragment).Commit();
            _toolbar.Title = title;
        }

        //============================================================
        public override void OnBackPressed()
        {
            var drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
        }

        //============================================================
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            _presenter.NavigationItemSelect(item.ItemId, _user);
            return true;
        }

        //============================================================
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}