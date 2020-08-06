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
using DrivingAssistant.AndroidApp.Fragments;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Activities
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

        private User _user;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            SetupActivityFields();
            _toolbar.Title = "Home";
            SetSupportActionBar(_toolbar);

            _user = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("user"));
            var toggle = new ActionBarDrawerToggle(this, _drawer, _toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            _drawer.AddDrawerListener(toggle);
            toggle.SyncState();
            _navigationView.SetNavigationItemSelectedListener(this);
            _textViewUser.Text = _user.FirstName + " " + _user.LastName;
            _textViewUserEmail.Text = _user.Email;
            _textViewUserRole.Text = _user.Role.ToString();

            var fragment = new HomeFragment(_user);
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frameLayout1, fragment).Commit();
        }

        //============================================================
        private void SetupActivityFields()
        {
            _toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            _drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            _textViewUser = _navigationView.GetHeaderView(0).FindViewById<TextView>(Resource.Id.headerTextUser);
            _textViewUserEmail = _navigationView.GetHeaderView(0).FindViewById<TextView>(Resource.Id.headerTextUserEmail);
            _textViewUserRole = _navigationView.GetHeaderView(0).FindViewById<TextView>(Resource.Id.headerTextUserRole);
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
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_settings:
                {
                    break;
                }
            }
            return item.ItemId == Resource.Id.action_settings || base.OnOptionsItemSelected(item);
        }

        //============================================================
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            ProcessNavigationItemSelected(item.ItemId);
            return true;
        }

        //============================================================
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        //============================================================
        private async void ProcessNavigationItemSelected(int id)
        {
            switch (id)
            {
                case Resource.Id.nav_home:
                {
                    var fragment = new HomeFragment(_user);
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frameLayout1, fragment).Commit();
                    _toolbar.Title = "Home";
                    break;
                }
                case Resource.Id.nav_sessions:
                {
                    var fragment = new SessionFragment(_user);
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frameLayout1, fragment).Commit();
                    _toolbar.Title = "Driving Sessions";
                    break;
                }
                case Resource.Id.nav_servers:
                {
                    var fragment = new ServerFragment(_user);
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frameLayout1, fragment).Commit();
                    _toolbar.Title = "Servers";
                    break;
                }
                case Resource.Id.nav_settings:
                {
                    var fragment = new SettingsFragment(_user);
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frameLayout1, fragment).Commit();
                    _toolbar.Title = "Settings";
                    break;
                }
                case Resource.Id.nav_logout:
                {
                    Toast.MakeText(Application.Context, "Logging out...", ToastLength.Short).Show();
                    await Task.Delay(500);
                    Finish();
                    break;
                }
            }

            FindViewById<DrawerLayout>(Resource.Id.drawer_layout).CloseDrawer(GravityCompat.Start);
        }
    }
}

