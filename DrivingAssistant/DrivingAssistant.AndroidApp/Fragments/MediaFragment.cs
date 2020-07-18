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
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;
using Plugin.FilePicker;
using Fragment = Android.Support.V4.App.Fragment;
using Path = System.IO.Path;

namespace DrivingAssistant.AndroidApp.Fragments
{
    public class MediaFragment : Fragment
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
        private ICollection<Media> _currentMedias;

        //============================================================
        public MediaFragment(User user)
        {
            _user = user;
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_medias, container, false);
            _mediaService = new MediaService(Constants.ServerUri);
            SetupFragmentFields(view);
            SetupListAdapter();
            return view;
        }

        //============================================================
        private void SetupFragmentFields(View view)
        {
            _listView = view.FindViewById<ListView>(Resource.Id.mediasListView);
            _addButton = view.FindViewById<Button>(Resource.Id.mediasButtonUpload);
            _modifyButton = view.FindViewById<Button>(Resource.Id.mediasButtonModify);
            _deleteButton = view.FindViewById<Button>(Resource.Id.mediasButtonDelete);
            _viewButton = view.FindViewById<Button>(Resource.Id.mediasButtonView);

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
            _currentMedias = await _mediaService.GetMediaAsync(_user.Id);
            _listView.Adapter?.Dispose();
            _listView.Adapter = new MediaViewModelAdapter(Activity, _currentMedias);
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
            var filedata = await CrossFilePicker.Current.PickFile();
            if (filedata == null)
            {
                return;
            }

            if (Path.GetExtension(filedata.FilePath) != ".jpg" && Path.GetExtension(filedata.FilePath) != ".mp4")
            {
                Toast.MakeText(Context, "Selected file is not a Jpeg image file or MP4 video file!", ToastLength.Short).Show();
                return;
            }

            var mediaType = Path.GetExtension(filedata.FilePath) == ".jpg" ? MediaType.Image : MediaType.Video;

            var alert = new AlertDialog.Builder(Context);
            alert.SetTitle("Choose a unique description for this media");
            var textEdit = new EditText(Context);
            var layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            textEdit.LayoutParameters = layoutParams;
            textEdit.Gravity = GravityFlags.Center;
            alert.SetView(textEdit);
            alert.SetPositiveButton("Ok", async  (o, args) =>
            {
                var progressDialog = ProgressDialog.Show(Context, mediaType == MediaType.Image ? "Image Upload" : "Video Upload", "Uploading...");
                await using var stream = filedata.GetStream();
                await _mediaService.SetMediaStreamAsync(stream, mediaType, _user.Id, textEdit.Text);
                progressDialog.Dismiss();
                await RefreshDataSource();
            });

            var dialog = alert.Create();
            dialog.Show();
        }

        //============================================================
        private void OnModifyButtonClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        //============================================================
        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            if (_selectedPosition == -1)
            {
                Toast.MakeText(Context, "No media selected!", ToastLength.Short).Show();
                return;
            }

            var alert = new AlertDialog.Builder(Context);
            alert.SetTitle("Confirm Delete");
            alert.SetMessage("Action cannot be undone");
            alert.SetPositiveButton("Delete", async (o, args) =>
            {
                var media = _currentMedias.ElementAt(_selectedPosition);
                await _mediaService.DeleteMediaAsync(media.Id);
                Toast.MakeText(Context, "Media deleted!", ToastLength.Short).Show();
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
                Toast.MakeText(Context, "No media selected!", ToastLength.Short).Show();
                return;
            }

            var media = _currentMedias.ElementAt(_selectedPosition);
            if (media.Type == MediaType.Image)
            {
                var intent = new Intent(Context, typeof(GalleryActivity));
                intent.PutExtra("image", JsonConvert.SerializeObject(media));
                StartActivity(intent);
            }
            else if (media.Type == MediaType.Video)
            {
                var intent = new Intent(Context, typeof(VideoActivity));
                intent.PutExtra("video", JsonConvert.SerializeObject(media));
                StartActivity(intent);
            }
        }
    }
}