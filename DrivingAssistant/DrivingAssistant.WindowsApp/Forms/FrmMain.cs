using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using Emgu.CV;

namespace DrivingAssistant.WindowsApp.Forms
{
    public partial class FrmMain : Form
    {
        private readonly ICollection<Bitmap> _images = new List<Bitmap>();
        private readonly ICollection<string> _imagesBase64 = new List<string>();
        private readonly ICollection<byte[]> _imageBytes = new List<byte[]>();

        //============================================================
        public FrmMain()
        {
            InitializeComponent();
        }

        //============================================================
        private void OnOpenVideoToolStripMenuItemClick(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var capture = new VideoCapture(openFileDialog.FileName);
                var count = 0;
                while (true)
                {
                    var cvFrame = capture.QueryFrame();
                    if (cvFrame == null || cvFrame.DataPointer == IntPtr.Zero)
                    {
                        break;
                    }
                    if (++count == 30)
                    {
                        using var memoryStream = new MemoryStream();
                        cvFrame.ToBitmap().Save(memoryStream, ImageFormat.Jpeg);
                        _imagesBase64.Add(Convert.ToBase64String(memoryStream.ToArray()));
                        count = 0;
                    }
                    cvFrame.Dispose();
                }
                capture.Dispose();
            }
        }

        //============================================================
        private async void OnWorkToolStripMenuItemClick(object sender, EventArgs e)
        {
            using var client = new HttpClient();
            var fullString = string.Join(" ", _imagesBase64);
            _imagesBase64.Clear();
            using var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:3287/images2");
            request.Content = new StringContent(fullString);
            fullString = string.Empty;
            await client.SendAsync(request);
        }

        //============================================================
        private async void openVideo2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var file = File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:3287/videos");
                request.Content = new StreamContent(file);
                await client.SendAsync(request);
            }
        }
    }
}
