using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AltoHttp;

namespace CSFileDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        HttpDownloader httpDownloader;
        
        private void btnStart_Click(object sender, EventArgs e)
        {
            httpDownloader = new HttpDownloader(txtUrl.Text, $"{Application.StartupPath}\\{Path.GetFileName(txtUrl.Text)}");
            httpDownloader.DownloadCompleted += HttpDownloader_DownloadCompleted;
            httpDownloader.ProgressChanged += HttpDownloader_ProgressChanged;
            httpDownloader.Start();
        }

        private void HttpDownloader_ProgressChanged(object sender, AltoHttp.ProgressChangedEventArgs e)
        {
            progressBar.Value = (int)e.Progress;
            lblPercent.Text = $"{e.Progress.ToString("0.00")}%";
            lblSpeed.Text = string.Format("{0} MB/s", (e.SpeedInBytes / 1024d / 1024d).ToString("0.00"));
            lblDownloaded.Text = string.Format("{0} MB/s", (httpDownloader.TotalBytesReceived / 1024d / 1024d).ToString("0.00"));
            lblStatus.Text = "Downloading...";
        }

        private void HttpDownloader_DownloadCompleted(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                lblStatus.Text = "Finished!";
                lblPercent.Text = "100%";
            });
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (httpDownloader != null)
                httpDownloader.Pause();
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            if (httpDownloader != null)
                httpDownloader.Resume();
        }
    }
}
