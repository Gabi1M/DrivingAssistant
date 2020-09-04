using System.Threading.Tasks;
using Android.Content;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;

namespace DrivingAssistant.AndroidApp.Activities.Report
{
    public class ReportActivityViewPresenter : ViewPresenter
    {
        private readonly VideoRecording _video;
        private readonly ReportService _reportService = new ReportService();
        private readonly VideoService _videoService = new VideoService();

        //============================================================
        public ReportActivityViewPresenter(Context context, VideoRecording video)
        {
            _context = context;
            _video = video;
        }

        //============================================================
        public async Task Refresh()
        {
            var videoId = _video.IsProcessed()
                ? _video.Id
                : (await _videoService.GetVideoByProcessedIdAsync(_video.Id)).Id;
            var report = await _reportService.GetByVideoAsync(videoId);
            Notify(new NotificationEventArgs(NotificationCommand.ReportActivity_Refresh, report));
        }

        //============================================================
        public async Task Download()
        {
            string filename;
            if (_video.IsProcessed())
            {
                filename = await _reportService.DownloadReport(_video.Id);
            }
            else
            {
                filename = await _reportService.DownloadReport((await _videoService.GetVideoByProcessedIdAsync(_video.Id)).Id);
            }
            Notify(new NotificationEventArgs(NotificationCommand.ReportActivity_Download, filename));
        }
    }
}