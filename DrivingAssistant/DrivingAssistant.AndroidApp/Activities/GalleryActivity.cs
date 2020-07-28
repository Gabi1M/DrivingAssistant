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

        private readonly MediaService _mediaService = new MediaService();
        private Media _image;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_gallery);
            SetupActivityFields();
            LoadImage();
        }

        //============================================================
        private void SetupActivityFields()
        {
            _imageView = FindViewById<ImageView>(Resource.Id.galleryView);
            _image = JsonConvert.DeserializeObject<Media>(Intent.GetStringExtra("image"));
        }

        //============================================================
        private async void LoadImage()
        {
            _imageView.SetImageBitmap(await _mediaService.DownloadImageAsync(_image.Id));
        }
    }
}