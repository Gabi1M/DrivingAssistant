using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Activities;
using DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;
using Plugin.FilePicker;
using Fragment = Android.Support.V4.App.Fragment;
using Path = System.IO.Path;

namespace DrivingAssistant.AndroidApp.Fragments
{
    public class ImageFragment : Fragment
    {
        private readonly User _user;

        private ListView _listView;
        private Button _addButton;
        private Button _modifyButton;
        private Button _deleteButton;
        private Button _viewButton;

        private int _selectedPosition = -1;
        private View _selectedView;

        private MediaService _mediaService;
        private ICollection<Media> _currentImages;

        //============================================================
        public ImageFragment(User user)
        {
            _user = user;
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_images, container, false);
            _mediaService = new MediaService("http://192.168.100.234:3287");
            SetupFragmentFields(view);
            SetupListAdapter();
            return view;
        }

        //============================================================
        private void SetupFragmentFields(View view)
        {
            _listView = view.FindViewById<ListView>(Resource.Id.imagesListView);
            _addButton = view.FindViewById<Button>(Resource.Id.imagesButtonUpload);
            _modifyButton = view.FindViewById<Button>(Resource.Id.imagesButtonModify);
            _deleteButton = view.FindViewById<Button>(Resource.Id.imagesButtonDelete);
            _viewButton = view.FindViewById<Button>(Resource.Id.imagesButtonView);

            _addButton.Click += OnAddButtonClick;
            _modifyButton.Click += OnModifyButtonClick;
            _deleteButton.Click += OnDeleteButtonClick;
            _viewButton.Click += OnViewButtonClick;
        }

        //============================================================
        private async void SetupListAdapter()
        {
            await RefreshDataSource();
            _listView.ChoiceMode = ChoiceMode.Single;
            _listView.ItemClick += OnItemClick;
        }

        //============================================================
        private async Task RefreshDataSource()
        {
            _currentImages = await _mediaService.GetImagesAsync(_user.Id);
            _listView.Adapter?.Dispose();
            _listView.Adapter = new ImageViewModelAdapter(Activity, _currentImages);
        }

        //============================================================
        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedView?.SetBackgroundColor(Color.White);
            e.View.SetBackgroundColor(Color.Aqua);
            _selectedPosition = e.Position;
            _selectedView = e.View;
        }

        //============================================================
        private async void OnAddButtonClick(object sender, EventArgs e)
        {
            var filedata = await CrossFilePicker.Current.PickFile(new []{"*.jpg"});
            if (filedata == null)
            {
                return;
            }

            if (Path.GetExtension(filedata.FilePath) != ".jpg")
            {
                Toast.MakeText(Context, "Selected file is not a Jpeg image file!", ToastLength.Short).Show();
                return;
            }

            var progressDialog = ProgressDialog.Show(Context, "Image Upload", "Uploading...");
            await using var stream = filedata.GetStream();
            var mediaService = new MediaService("http://192.168.100.234:3287");
            Toast.MakeText(Context, "Uploading image...", ToastLength.Short).Show();
            await mediaService.SetMediaStreamAsync(stream, MediaType.Image, _user.Id);
            progressDialog.Dismiss();
            Toast.MakeText(Context, "Image uploaded!", ToastLength.Short).Show();
            await RefreshDataSource();
        }

        //============================================================
        private void OnModifyButtonClick(object sender, EventArgs e)
        {
            //TODO
        }

        //============================================================
        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            if (_selectedPosition == -1)
            {
                Toast.MakeText(Context, "No image selected!", ToastLength.Short).Show();
                return;
            }

            var alert = new AlertDialog.Builder(Context);
            alert.SetTitle("Confirm Delete");
            alert.SetMessage("Action cannot be undone");
            alert.SetPositiveButton("Delete", async (o, args) =>
            {
                var image = _currentImages.ElementAt(_selectedPosition);
                var mediaService = new MediaService("http://192.168.100.234:3287");
                await mediaService.DeleteImageAsync(image.Id);
                Toast.MakeText(Context, "Image deleted!", ToastLength.Short).Show();
                await RefreshDataSource();
            });
            alert.SetNegativeButton("Cancel", (o, args) =>
            {
                //NOTHING
            });

            var dialog = alert.Create();
            dialog.Show();
        }

        //============================================================
        private void OnViewButtonClick(object sender, EventArgs e)
        {
            if (_selectedPosition == -1)
            {
                Toast.MakeText(Context, "No image selected!", ToastLength.Short).Show();
                return;
            }

            var image = _currentImages.ElementAt(_selectedPosition);
            var intent = new Intent(Context, typeof(GalleryActivity));
            intent.PutExtra("image", JsonConvert.SerializeObject(image));
            StartActivity(intent);
        }
    }
}