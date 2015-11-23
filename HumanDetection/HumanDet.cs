using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.GPU;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using Aldebaran.Proxies;


namespace HumanDetection
{
    public partial class HumanDet : Form
    {
        private Capture _capture = null;
        float X, Y;
        private bool _captureInProgress;
        Connection con;

        public HumanDet()
        {
            InitializeComponent();
            con = new Connection();
            try
            {
                _capture = new Capture();
                _capture.ImageGrabbed += ProcessFrame;
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            //if (con.getImage() != null)
            //{
            Image<Bgr, Byte> frame = con.getImage().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR); //_capture.RetrieveBgrFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Copy(); ;
            int detected_human = 0;
                long processingTime;
                //Rectangle[] results = FindHuman.Find(frame, out processingTime);
                MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX_SMALL, 0.6, 0.7);
                //foreach (Rectangle rect in results)
                //{
                //++detected_human;
                //frame.Draw(rect, new Bgr(Color.Red), 2);
                //frame.Draw("[" + detected_human + "]" + rect.Left.ToString() + "," + rect.Top.ToString(), ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Red));
                //frame.Draw("[" + detected_human + "]" + "Galon", ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                //}
                /*Rectangle[] resultsMeja = FindHuman.FindMeja(frame, out processingTime);
                foreach (Rectangle rect in resultsMeja)
                {
                    ++detected_human;
                    frame.Draw(rect, new Bgr(Color.Red), 2);
                    //frame.Draw("[" + detected_human + "]" + rect.Left.ToString() + "," + rect.Top.ToString(), ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Red));
                    frame.Draw("[" + detected_human + "]" + "Meja", ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                }*/

                /*Rectangle[] resultsKursi = FindHuman.FindKursi(frame, out processingTime);
                foreach (Rectangle rect in resultsKursi)
                {
                    ++detected_human;
                    frame.Draw(rect, new Bgr(Color.Blue), 2);
                    //frame.Draw("[" + detected_human + "]" + rect.Left.ToString() + "," + rect.Top.ToString(), ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Red));
                    frame.Draw("[" + detected_human + "]" + "Kursi", ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                 * frame.Draw("[" + detected_human + "]" + rect.Left.ToString() + "," + rect.Top.ToString(), ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Red));
                }*/

                Rectangle[] resultsMonitor = FindHuman.FindMonitor(frame, out processingTime);
            List<Rectangle> dataBound = new List<Rectangle>();
            string dataTrain = "Trash";
            foreach (Rectangle rect in resultsMonitor)
                {
                    ++detected_human;
                    frame.Draw(rect, new Bgr(Color.Red), 2);
                    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                    X = hogLocation.X + (rect.Height / 2);
                    Y = hogLocation.Y + (rect.Width / 2);
                
                    //frame.Draw("[" + detected_human + "]" + rect.Left.ToString() + "," + rect.Top.ToString(), ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Red));
                    frame.Draw("[" + detected_human + "]" + dataTrain, ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                //con.koorHOG(X, Y);
                dataBound.Add(rect);
                }
                con.labelHOG(dataTrain, dataBound);

            captureImageBox.Image = frame;
                try
                {
                    lblTime.Invoke((Action)(() => lblX.Text = X.ToString()));
                    lblTime.Invoke((Action)(() => lblY.Text = Y.ToString()));
                }
                catch
                {
                    
                }
                
            //}
        }

        private void ReleaseData()
        {
            if (_capture != null)
                _capture.Dispose();
        }

        private void captureButton_Click(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                if (_captureInProgress)
                {  //stop the capture
                    captureButton.Text = "Start";
                    _capture.Pause();
                }
                else
                {
                    //start the capture
                    captureButton.Text = "Stop";
                    _capture.Start();
                }

                _captureInProgress = !_captureInProgress;
            }
        }
        
        private void HumanDet_Load(object sender, EventArgs e)
        {

        }
    }
}
