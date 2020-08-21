using System;
using System.Threading.Tasks;
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
using DrivingAssistant.AndroidApp.Fragments.Home;
using DrivingAssistant.AndroidApp.Fragments.Server;
using DrivingAssistant.AndroidApp.Fragments.Session;
using DrivingAssistant.AndroidApp.Fragments.Settings;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Activities.Main
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivityView : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private Android.Support.V7.Widget.Toolbar _toolbar;
        private DrawerLayout _drawer;
        private NavigationView _navigationView;
        private TextView _textViewUser;
        private TextView _textViewUserEmail;

        private MainActivityViewPresenter _viewPresenter;
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

            _viewPresenter = new MainActivityViewPresenter(this);
            _viewPresenter.OnNotificationReceived += ViewPresenterOnNotificationReceived;

            ChangeFragment(new HomeFragmentView(this, _user), "Home");
        }

        //============================================================
        private async void ViewPresenterOnNotificationReceived(object sender, NotificationEventArgs e)
        {
            if (e.Data is Exception ex)
            {
                Utils.ShowToast(this, ex.Message, true);
                return;
            }

            switch (e.Command)
            {
                case NotificationCommand.MainActivity_Navigation:
                {
                    switch (e.Data)
                    {
                        case HomeFragmentView homeFragment:
                        {
                            ChangeFragment(homeFragment, "Home");
                            break;
                        }
                        case SessionFragmentView sessionFragment:
                        {
                            ChangeFragment(sessionFragment, "Sessions");
                            break;
                        }
                        case ServerFragmentView serverFragment:
                        {
                            ChangeFragment(serverFragment, "Servers");
                            break;
                        }
                        case SettingsFragmentView settingsFragment:
                        {
                            ChangeFragment(settingsFragment, "Settings");
                            break;
                        }
                        case null:
                        {
                            Toast.MakeText(Application.Context, "Logging out...", ToastLength.Short)?.Show();
                            await Task.Delay(2000);
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
            _viewPresenter.NavigationItemSelect(item.ItemId, _user);
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