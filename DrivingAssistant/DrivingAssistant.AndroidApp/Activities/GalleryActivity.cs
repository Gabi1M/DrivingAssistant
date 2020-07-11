using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Activities
{
    [Activity(Label = "GalleryActivity")]
    public class GalleryActivity : AppCompatActivity
    {
        private ImageView _imageView;
        private Media _image;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_gallery);
            _imageView = FindViewById<ImageView>(Resource.Id.galleryView);
            _image = JsonConvert.DeserializeObject<Media>(Intent.GetStringExtra("image"));
            LoadImage();
        }

        //============================================================
        private async void LoadImage()
        {
            var mediaService = new MediaService("http://192.168.100.234:3287");
            _imageView.SetImageBitmap(await mediaService.DownloadImageAsync(_image.Id));
        }
    }
}