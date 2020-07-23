using System;
using System.Globalization;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Models.ImageProcessing;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments
{
    public class SettingsFragment : Fragment
    {
        private readonly User _user;
        private readonly UserSettingsService _userSettingsService;
        private UserSettings _userSettings;

        private EditText _textCannyThreshold;
        private EditText _textCannyThresholdLinking;
        private EditText _textHoughLinesRhoResolution;
        private EditText _textHoughLinesThetaResolution;
        private EditText _textHoughLinesMinimumLineWidth;
        private EditText _textHoughLinesGapBetweenLines;
        private EditText _textHoughLinesThreshold;
        private EditText _textDilateIterations;
        private Button _buttonSave;
        private Button _buttonDefault;

        //============================================================
        public SettingsFragment(User user)
        {
            _user = user;
            _userSettingsService = new UserSettingsService(Constants.ServerUri);
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_settings, container, false);
            SetupFragmentFields(view);
            RefreshData();
            return view;
        }

        //============================================================
        private void SetupFragmentFields(View view)
        {
            _textCannyThreshold = view.FindViewById<EditText>(Resource.Id.settingsInputCannyThreshold);
            _textCannyThresholdLinking = view.FindViewById<EditText>(Resource.Id.settingsInputCannyThresholdLinking);
            _textHoughLinesRhoResolution = view.FindViewById<EditText>(Resource.Id.settingsInputHoughLinesRhoResolution);
            _textHoughLinesThetaResolution = view.FindViewById<EditText>(Resource.Id.settingsInputHoughLinesThetaResolution);
            _textHoughLinesMinimumLineWidth = view.FindViewById<EditText>(Resource.Id.settingsInputHoughLinesMinimumLineWidth);
            _textHoughLinesGapBetweenLines = view.FindViewById<EditText>(Resource.Id.settingsInputHoughLinesGapBetweenLines);
            _textHoughLinesThreshold = view.FindViewById<EditText>(Resource.Id.settingsInputHoughLinesThreshold);
            _textDilateIterations = view.FindViewById<EditText>(Resource.Id.settingsInputDilateIterations);
            _buttonSave = view.FindViewById<Button>(Resource.Id.settingsButtonSave);
            _buttonDefault = view.FindViewById<Button>(Resource.Id.settingsButtonDefault);

            _buttonSave.Click += OnButtonSaveClick;
            _buttonDefault.Click += OnButtonDefaultClick;
        }

        //============================================================
        private async void RefreshData()
        {
            _userSettings = (await _userSettingsService.GetAsync()).First(x => x.UserId == _user.Id);
            _textCannyThreshold.Text = _userSettings.Parameters.CannyThreshold.ToString(CultureInfo.InvariantCulture);
            _textCannyThresholdLinking.Text = _userSettings.Parameters.CannyThresholdLinking.ToString(CultureInfo.InvariantCulture);
            _textHoughLinesRhoResolution.Text = _userSettings.Parameters.HoughLinesRhoResolution.ToString(CultureInfo.InvariantCulture);
            _textHoughLinesThetaResolution.Text = _userSettings.Parameters.HoughLinesThetaResolution.ToString(CultureInfo.InvariantCulture);
            _textHoughLinesMinimumLineWidth.Text = _userSettings.Parameters.HoughLinesMinimumLineWidth.ToString(CultureInfo.InvariantCulture);
            _textHoughLinesGapBetweenLines.Text = _userSettings.Parameters.HoughLinesGapBetweenLines.ToString(CultureInfo.InvariantCulture);
            _textHoughLinesThreshold.Text = _userSettings.Parameters.HoughLinesThreshold.ToString();
            _textDilateIterations.Text = _userSettings.Parameters.DilateIterations.ToString();
        }

        //============================================================
        private async void OnButtonSaveClick(object sender, EventArgs e)
        {
            var imageProcessorParameters = new Parameters
            {
                CannyThreshold = Convert.ToDouble(_textCannyThreshold.Text.Trim()),
                CannyThresholdLinking = Convert.ToDouble(_textCannyThresholdLinking.Text.Trim()),
                HoughLinesRhoResolution = Convert.ToDouble(_textHoughLinesRhoResolution.Text.Trim()),
                HoughLinesThetaResolution = Convert.ToDouble(_textHoughLinesThetaResolution.Text.Trim()),
                HoughLinesGapBetweenLines = Convert.ToDouble(_textHoughLinesGapBetweenLines.Text.Trim()),
                HoughLinesThreshold = Convert.ToInt32(_textHoughLinesThreshold.Text.Trim()),
                DilateIterations = Convert.ToInt32(_textDilateIterations.Text.Trim())
            };
            _userSettings.Parameters = imageProcessorParameters;
            await _userSettingsService.UpdateAsync(_userSettings);
            Toast.MakeText(Context, "Settings successfully saved!", ToastLength.Short).Show();
            RefreshData();
        }

        //============================================================
        private void OnButtonDefaultClick(object sender, EventArgs e)
        {
            var alert = new AlertDialog.Builder(Context);
            alert.SetTitle("Restore Default Settings");
            alert.SetMessage("Are you sure?");
            alert.SetPositiveButton("Yes", async (o, args) =>
            {
                _userSettings.Parameters = Parameters.Default();
                await _userSettingsService.UpdateAsync(_userSettings);
                Toast.MakeText(Context, "Settings successfully saved!", ToastLength.Short).Show();
                RefreshData();
            });
            alert.SetNegativeButton("No", (o, args) => { });
            var dialog = alert.Create();
            dialog.Show();
        }
    }
}