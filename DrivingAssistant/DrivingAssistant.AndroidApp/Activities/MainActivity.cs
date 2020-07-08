using System;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Fragments;
using DrivingAssistant.AndroidApp.Services;

namespace DrivingAssistant.AndroidApp.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            var drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            var toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);
        }

        //============================================================
        public override void OnBackPressed()
        {
            var drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                //base.OnBackPressed();
            }
        }

        //============================================================
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        //============================================================
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;
            return id == Resource.Id.action_settings || base.OnOptionsItemSelected(item);
        }

        //============================================================
        private static void FabOnClick(object sender, EventArgs eventArgs)
        {
            var view = sender as View;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong).SetAction("Action", (View.IOnClickListener)null).Show();
        }

        //============================================================
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            ProcessNavigationItemSelected(item.ItemId);
            return true;
        }

        //============================================================
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        //============================================================
        private async void ProcessNavigationItemSelected(int id)
        {
            switch (id)
            {
                case Resource.Id.nav_images:
                {
                    using var imageService = new ImageService("http://192.168.100.234:3287");
                    var images = await imageService.GetAsync();
                    var fragment = new ImageFragment(images);
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frameLayout1, fragment).Commit();
                    break;
                }
                case Resource.Id.nav_videos:
                {
                    using var videoService = new VideoService("http://192.168.100.234:3287");
                    var videos = await videoService.GetAsync();
                    var fragment = new VideoFragment(videos);
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frameLayout1, fragment).Commit();
                    break;
                }
                case Resource.Id.nav_logout:
                {
                    Toast.MakeText(Application.Context, "Logging out...", ToastLength.Short).Show();
                    await Task.Delay(1000);
                    Finish();
                    break;
                }
            }

            FindViewById<DrawerLayout>(Resource.Id.drawer_layout).CloseDrawer(GravityCompat.Start);
        }
    }
}

