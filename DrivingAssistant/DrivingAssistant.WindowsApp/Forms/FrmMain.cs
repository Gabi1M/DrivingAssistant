using System.IO;
using System.Net.Http;
using System.Windows.Forms;

namespace DrivingAssistant.WindowsApp.Forms
{
    public partial class FrmMain : Form
    {
        private readonly HttpClient _client = new HttpClient();

        //============================================================
        public FrmMain()
        {
            InitializeComponent();
        }

        //============================================================
        private async void OnUploadImageToolStripMenuItemClick(object sender, System.EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = @"Jpeg Image File(*.jpg)|*.jpg"
            };
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var filename = openFileDialog.FileName;
                var request = new HttpRequestMessage(HttpMethod.Post, "http://192.168.100.234:3287/image_stream")
                {
                    Content = new StreamContent(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                };
                await _client.SendAsync(request);
                MessageBox.Show(@"Finished uploading image!", @"Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //============================================================
        private async void OnUploadVideoToolStripMenuItemClick(object sender, System.EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = @"MP4 Video File(*.mp4)|*.mp4"
            };
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var filename = openFileDialog.FileName;
                var request = new HttpRequestMessage(HttpMethod.Post, "http://192.168.100.234:3287/video_stream")
                {
                    Content = new StreamContent(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                };
                await _client.SendAsync(request);
                MessageBox.Show(@"Finished uploading video!", @"Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
