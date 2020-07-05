using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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
                    if (cvFrame == null)
                    {
                        break;
                    }
                    if (++count == 5)
                    {
                        using var memoryStream = new MemoryStream();
                        cvFrame.ToBitmap().Save(memoryStream, ImageFormat.Jpeg);
                        _imagesBase64.Add(Convert.ToBase64String(memoryStream.ToArray()));
                        count = 0;
                    }
                    cvFrame.Dispose();
                }
            }
        }

        //============================================================
        private async void OnWorkToolStripMenuItemClick(object sender, EventArgs e)
        {
            var client = new HttpClient();
            foreach (var base64 in _imagesBase64)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://192.168.100.246:3287/images");
                request.Content = new StringContent(base64);
                await client.SendAsync(request);
            }
        }
    }
}
