using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.Core.Models;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments
{
    public class SettingsFragment : Fragment
    {
        private readonly User _user;

        /*private EditText _textCannyThreshold;
        private EditText _textCannyThresholdLinking;
        private EditText _textHoughLinesRhoResolution;
        private EditText _textHoughLinesThetaResolution;
        private EditText _textHoughLinesMinimumLineWidth;
        private EditText _textHoughLinesGapBetweenLines;
        private EditText _textHoughLinesThreshold;
        private EditText _textDilateIterations;
        private Button _buttonSave;
        private Button _buttonDefault;*/

        //============================================================
        public SettingsFragment(User user)
        {
            _user = user;
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_settings, container, false);
            return view;
        }

        //============================================================
        private void OnButtonSaveClick(object sender, EventArgs e)
        {
            
        }

        //============================================================
        private void OnButtonDefaultClick(object sender, EventArgs e)
        {
            
        }
    }
}