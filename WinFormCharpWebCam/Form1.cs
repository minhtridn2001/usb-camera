using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using AForge.Video;
using AForge.Video.DirectShow;


namespace WinFormCharpWebCam
{

    public partial class mainWinForm : Form
    {
        private bool DeviceExist = false;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource = null;

        public mainWinForm()
        {
            InitializeComponent();
        }

        private void getCamList()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                cboDevices.Items.Clear();
                if (videoDevices.Count == 0)
                    throw new ApplicationException();

                DeviceExist = true;
                foreach (FilterInfo device in videoDevices)
                {
                    cboDevices.Items.Add(device.Name);
                }
                cboDevices.SelectedIndex = 0; //make dafault to first cam
            }
            catch (ApplicationException)
            {
                DeviceExist = false;
                cboDevices.Items.Add("No capture device on your system");
            }
        }


        private void btnRefresh_Click(object sender, EventArgs e)
        {
            getCamList();
        }

        //toggle start and stop button
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text.Equals("Start"))
            {
                if (DeviceExist)
                {
                    videoSource = new VideoCaptureDevice(videoDevices[cboDevices.SelectedIndex].MonikerString);
                    videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                    CloseVideoSource();
                    videoSource.DesiredFrameSize = new Size(160, 120);
                    //videoSource.DesiredFrameRate = 10;
                    videoSource.Start();

                   // webcam.Start();
                    btnStart.Text = "Stop";
                  
                }
                else
                {
                    label2.Text = "Error: No Device selected.";
                }
            }
            else
            {
                if (videoSource.IsRunning)
                {
                   //// timer1.Enabled = false;
                    CloseVideoSource();
                   // webcam.Stop();
                    label2.Text = "Device stopped.";
                    btnStart.Text = "Start";
                }
            }
        }

        //eventhandler if new frame is ready
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap img = (Bitmap)eventArgs.Frame.Clone();
            //do processing here
            imgVideo.Image = img;
        }

        //close the device safely
        private void CloseVideoSource()
        {
           // webcam.Stop();

            if (!(videoSource == null))
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource = null;
                }
        }


        //prevent sudden close while device is running
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseVideoSource();
        }

        WebCam webcam;
        private void mainWinForm_Load(object sender, EventArgs e)
        {
            webcam = new WebCam();
            webcam.InitializeWebCam(ref imgVideo);
            //webcam.Start();
            getCamList();
        }

        private void bntCapture_Click(object sender, EventArgs e)
        {
            Helper.SaveImageCapture(imgVideo.Image);
        }

       



    }
}
