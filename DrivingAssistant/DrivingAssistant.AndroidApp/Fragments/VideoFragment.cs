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
    public class VideoFragment : Fragment
    {
        private ListView _listView;
        private Button _addButton;
        private Button _modifyButton;
        private Button _deleteButton;
        private Button _viewButton;

        private int _selectedPosition = -1;
        private View _selectedView;

        private MediaService _mediaService;
        private ICollection<Media> _currentVideos;

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_videos, container, false);
            _mediaService = new MediaService("http://192.168.100.234:3287");
            SetupFragmentFields(view);
            SetupListAdapter();
            return view;
        }

        //============================================================
        private void SetupFragmentFields(View view)
        {
            _listView = view.FindViewById<ListView>(Resource.Id.videosListView);
            _addButton = view.FindViewById<Button>(Resource.Id.videosButtonUpload);
            _modifyButton = view.FindViewById<Button>(Resource.Id.videosButtonModify);
            _deleteButton = view.FindViewById<Button>(Resource.Id.videosButtonDelete);
            _viewButton = view.FindViewById<Button>(Resource.Id.videosButtonView);

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
            _currentVideos = await _mediaService.GetVideosAsync();
            _listView.Adapter?.Dispose();
            _listView.Adapter = new VideoViewModelAdapter(Activity, _currentVideos);
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
            var filedata = await CrossFilePicker.Current.PickFile(new[] { ".mp4" });
            if (filedata == null)
            {
                return;
            }

            if (Path.GetExtension(filedata.FilePath) != ".mp4")
            {
                Toast.MakeText(Context, "Selected file is not a Mp4 video file!", ToastLength.Short).Show();
                return;
            }

            var progressDialog = ProgressDialog.Show(Context, "Video Upload", "Uploading...");
            await using var stream = filedata.GetStream();
            var mediaService = new MediaService("http://192.168.100.234:3287");
            Toast.MakeText(Context, "Uploading video...", ToastLength.Short).Show();
            await mediaService.SetMediaStreamAsync(stream, MediaType.Video);
            progressDialog.Dismiss();
            Toast.MakeText(Context, "Video uploaded!", ToastLength.Short).Show();
            await RefreshDataSource();
        }

        //============================================================
        private void OnModifyButtonClick(object sender, EventArgs e)
        {
            
        }

        //============================================================
        private async void OnDeleteButtonClick(object sender, EventArgs e)
        {
            if (_selectedPosition == -1)
            {
                Toast.MakeText(Context, "No videos selected!", ToastLength.Short).Show();
                return;
            }

            var video = _currentVideos.ElementAt(_selectedPosition);
            var mediaService = new MediaService("http://192.168.100.234:3287");
            await mediaService.DeleteVideoAsync(video.Id);
            Toast.MakeText(Context, "Video deleted!", ToastLength.Short).Show();
            await RefreshDataSource();
        }

        //============================================================
        private void OnViewButtonClick(object sender, EventArgs e)
        {
            if (_selectedPosition == -1)
            {
                Toast.MakeText(Context, "No video selected!", ToastLength.Short).Show();
                return;
            }

            var video = _currentVideos.ElementAt(_selectedPosition);
            var intent = new Intent(Context, typeof(VideoActivity));
            intent.PutExtra("video", JsonConvert.SerializeObject(video));
            StartActivity(intent);
        }
    }
}