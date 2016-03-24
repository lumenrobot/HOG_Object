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
        private Capture _captureBottom = null;
        float X, Y;
        private bool _captureInProgress;
        Connection con;

        public HumanDet()
        {
            InitializeComponent();
            con = new Connection();
            //try
            //{
                //timer1.Enabled = true;
                //_capture = new Capture();
                //_capture.ImageGrabbed += ProcessFrame;

                //_captureBottom = new Capture();
                //_captureBottom.ImageGrabbed += prosesFrameBawah;
            //}
            //catch (NullReferenceException excpt)
            //{
                //MessageBox.Show(excpt.Message);
            //}
        }

        public void ProcessFrame(object sender, EventArgs arg)
        {
            Console.WriteLine("ProcessFrame(sender,arg)");
            if (_capture != null)
            {
            Image<Bgr, Byte> frame = /*_capture.RetrieveBgrFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Copy();*/ con.getImageAtas().Resize(640, 480, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);   
                int detected_human = 0;
                long processingTime;
                //Rectangle[] results = FindHuman.Find(frame, out processingTime);
                MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX_SMALL, 0.6, 0.7);
                

                List<Rectangle> dataBound = new List<Rectangle>();
                List<string> dataTrain = new List<string>();

                //============================================ DATA1 Cabinet ============================================================
                string fileCabinet = "E:\\cabinet.txt";//
                Size winSizeCabinet = new Size(64, 128);
                Rectangle[] resultsCabinet = FindHuman.FindObject(frame, out processingTime, winSizeCabinet, fileCabinet);
                foreach (Rectangle rect in resultsCabinet)
                {
                    ++detected_human;
                    frame.Draw(rect, new Bgr(Color.Red), 2);
                    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                    X = hogLocation.X;
                    Y = hogLocation.Y;
                    
                    frame.Draw("[" + detected_human + "]" + "Cabinet", ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                    //con.koorHOG(X, Y);
                    Rectangle baru = rect;
                    dataBound.Add(baru);
                    dataTrain.Add("Cabinet");
                }

                //============================================ DATA2 Chair =============================================================
                string fileChair = "E:\\chair.txt";
                Size winSizeChair = new Size(64, 64);
                Rectangle[] resultsChair = FindHuman.FindObject(frame, out processingTime, winSizeChair, fileChair);
                foreach (Rectangle rect in resultsChair)
                {
                    ++detected_human;
                    frame.Draw(rect, new Bgr(Color.Red), 2);
                    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                    X = hogLocation.X;
                    Y = hogLocation.Y;

                    frame.Draw("[" + detected_human + "]" + "Chair", ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                    //con.koorHOG(X, Y);
                    Rectangle baru = rect;
                    dataBound.Add(baru);
                    dataTrain.Add("Chair");
                }

                ////============================================ DATA3 Table =============================================================
                //string fileTable = "E:\\table.txt";
                //Size winSizeTable = new Size(64, 64);
                //Rectangle[] resultsTable = FindHuman.FindObject(frame, out processingTime, winSizeTable, fileTable);
                //foreach (Rectangle rect in resultsTable)
                //{
                //    ++detected_human;
                //    frame.Draw(rect, new Bgr(Color.Red), 2);
                //    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                //    X = hogLocation.X;
                //    Y = hogLocation.Y;

                //    frame.Draw("[" + detected_human + "]" + "Table", ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                //    //con.koorHOG(X, Y);
                //    Rectangle baru = rect;
                //    dataBound.Add(baru);
                //    dataTrain.Add("Table");
                //}

                
                ////============================================ DATA4 Arm chair =============================================================
                string fileArmchair = "E:\\arm chair.txt";
                Size winSizeArmchair = new Size(64, 64);
                Rectangle[] resultsArmchair = FindHuman.FindObject(frame, out processingTime, winSizeArmchair, fileArmchair);
                foreach (Rectangle rect in resultsArmchair)
                {
                    ++detected_human;
                    frame.Draw(rect, new Bgr(Color.Red), 2);
                    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                    X = hogLocation.X;
                    Y = hogLocation.Y;

                    frame.Draw("[" + detected_human + "]" + "Arm Chair", ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                //    //con.koorHOG(X, Y);
                    Rectangle baru = rect;
                    dataBound.Add(baru);
                    dataTrain.Add("Arm chair");
                }

                ////============================================ DATA5 Bed =============================================================
                string fileBed = "E:\\bed.txt";
                Size winSizeBed = new Size(128, 64);
                Rectangle[] resultsBed = FindHuman.FindObject(frame, out processingTime, winSizeBed, fileBed);
                foreach (Rectangle rect in resultsBed)
                {
                    ++detected_human;
                    frame.Draw(rect, new Bgr(Color.Red), 2);
                    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                    X = hogLocation.X;
                    Y = hogLocation.Y;

                    frame.Draw("[" + detected_human + "]" + "Bed", ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                //    //con.koorHOG(X, Y);
                    Rectangle baru = rect;
                    dataBound.Add(baru);
                    dataTrain.Add("Bed");
                }

                ////============================================ DATA6 Books =============================================================
                string fileSink = "E:\\sink.txt";
                Size winSizeSink = new Size(64, 128);
                Rectangle[] resultsSink = FindHuman.FindObject(frame, out processingTime, winSizeSink, fileSink);
                foreach (Rectangle rect in resultsSink)
                {
                    ++detected_human;
                    frame.Draw(rect, new Bgr(Color.Red), 2);
                    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                    X = hogLocation.X;
                    Y = hogLocation.Y;

                    frame.Draw("[" + detected_human + "]" + "Sink", ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                    //con.koorHOG(X, Y);
                    Rectangle baru = rect;
                    dataBound.Add(baru);
                    dataTrain.Add("Sink");
                }

                ////============================================ DATA7 Books =============================================================
                //string fileWestafel = "D:\\percobaan3.txt";
                //Size winSizeWestafel = new Size(64, 64);
                //Rectangle[] resultsWestafel = FindHuman.FindObject(frame, out processingTime, winSizeWestafel, fileWestafel);
                //foreach (Rectangle rect in resultsWestafel)
                //{
                //    ++detected_human;
                //    frame.Draw(rect, new Bgr(Color.Red), 2);
                //    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                //    X = hogLocation.X;
                //    Y = hogLocation.Y;

                //    frame.Draw("[" + detected_human + "]" + dataTrain, ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                //    //con.koorHOG(X, Y);
                //    Rectangle baru = rect;
                //    dataBound.Add(baru);
                //    dataTrain.Add("Westafel");
                //}

                ////============================================ DATA8 Bathup =============================================================
                //string fileBathup = "D:\\percobaan3.txt";
                //Size winSizeBathup = new Size(64, 64);
                //Rectangle[] resultsBathup = FindHuman.FindObject(frame, out processingTime, winSizeBathup, fileBathup);
                //foreach (Rectangle rect in resultsBathup)
                //{
                //    ++detected_human;
                //    frame.Draw(rect, new Bgr(Color.Red), 2);
                //    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                //    X = hogLocation.X;
                //    Y = hogLocation.Y;

                //    frame.Draw("[" + detected_human + "]" + dataTrain, ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                //    //con.koorHOG(X, Y);
                //    Rectangle baru = rect;
                //    dataBound.Add(baru);
                //    dataTrain.Add("Bathup");
                //}


                ////============================================ DATA9 Stove =============================================================
                //string fileStove = "D:\\percobaan3.txt";
                //Size winSizeStove = new Size(64, 64);
                //Rectangle[] resultsStove = FindHuman.FindObject(frame, out processingTime, winSizeStove, fileStove);
                //foreach (Rectangle rect in resultsStove)
                //{
                //    ++detected_human;
                //    frame.Draw(rect, new Bgr(Color.Red), 2);
                //    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                //    X = hogLocation.X;
                //    Y = hogLocation.Y;

                //    frame.Draw("[" + detected_human + "]" + dataTrain, ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                //    //con.koorHOG(X, Y);
                //    Rectangle baru = rect;
                //    dataBound.Add(baru);
                //    dataTrain.Add("Stove");
                //}

                ////============================================ DATA10 Refrigerator =============================================================
                //string fileRefrigerator = "D:\\percobaan3.txt";
                //Size winSizeRefrigerator = new Size(64, 64);
                //Rectangle[] resultsRefrigerator = FindHuman.FindObject(frame, out processingTime, winSizeRefrigerator, fileRefrigerator);
                //foreach (Rectangle rect in resultsRefrigerator)
                //{
                //    ++detected_human;
                //    frame.Draw(rect, new Bgr(Color.Red), 2);
                //    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                //    X = hogLocation.X;
                //    Y = hogLocation.Y;

                //    frame.Draw("[" + detected_human + "]" + dataTrain, ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                //    //con.koorHOG(X, Y);
                //    Rectangle baru = rect;
                //    dataBound.Add(baru);
                //    dataTrain.Add("Refrigerator");
                //}
                
                //con.labelHOGAtas(dataTrain, dataBound);
                captureImageBox.Image = frame;
                try
                {
                    lblTime.Invoke((Action)(() => lblX.Text = X.ToString()));
                    lblTime.Invoke((Action)(() => lblY.Text = Y.ToString()));
                }
                catch
                {

                }
                
            }
        }

        /*private void prosesFrameBawah(object sender, EventArgs args)
        {
            if (con.getImageBottom() != null)
            {
                Image<Bgr, Byte> frame = con.getImageBottom().Resize(640, 480, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR); // _capture.RetrieveBgrFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR).Copy(); ;
                int detected_human = 0;
                long processingTime;
                //Rectangle[] results = FindHuman.Find(frame, out processingTime);
                MCvFont f = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX_SMALL, 0.6, 0.7);

                List<Rectangle> dataBound = new List<Rectangle>();
                List<string> dataTrain = new List<string>();
                //string dataTrain = "Trash";
                //============================================ DATA1 Cabinet ============================================================
                string fileCabinet = "D:\\cabinet.txt";
                Size winSizeCabinet = new Size(64, 128);
                Rectangle[] resultsCabinet = FindHuman.FindObject(frame, out processingTime, winSizeCabinet, fileCabinet);
                foreach (Rectangle rect in resultsCabinet)
                {
                    ++detected_human;
                    frame.Draw(rect, new Bgr(Color.Red), 2);
                    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                    X = hogLocation.X;
                    Y = hogLocation.Y;

                    frame.Draw("[" + detected_human + "]" + "Cabinet", ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                    //con.koorHOG(X, Y);
                    Rectangle baru = rect;
                    dataBound.Add(baru);
                    dataTrain.Add("Cabinet");
                }

                //============================================ DATA2 Chair =============================================================
                string fileChair = "D:\\chair.txt";
                Size winSizeChair = new Size(64, 64);
                Rectangle[] resultsChair = FindHuman.FindObject(frame, out processingTime, winSizeChair, fileChair);
                foreach (Rectangle rect in resultsChair)
                {
                    ++detected_human;
                    frame.Draw(rect, new Bgr(Color.Red), 2);
                    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                    X = hogLocation.X;
                    Y = hogLocation.Y;

                    frame.Draw("[" + detected_human + "]" + "Chair", ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                    //con.koorHOG(X, Y);
                    Rectangle baru = rect;
                    dataBound.Add(baru);
                    dataTrain.Add("Chair");
                }

                //============================================ DATA3 Table =============================================================
                string fileTable = "D:\\table.txt";
                Size winSizeTable = new Size(64, 64);
                Rectangle[] resultsTable = FindHuman.FindObject(frame, out processingTime, winSizeTable, fileTable);
                foreach (Rectangle rect in resultsTable)
                {
                    ++detected_human;
                    frame.Draw(rect, new Bgr(Color.Red), 2);
                    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                    X = hogLocation.X;
                    Y = hogLocation.Y;

                    frame.Draw("[" + detected_human + "]" + "Table", ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                    //con.koorHOG(X, Y);
                    Rectangle baru = rect;
                    dataBound.Add(baru);
                    dataTrain.Add("Table");
                }


                ////============================================ DATA4 Arm chair =============================================================
                string fileArmchair = "D:\\arm chair.txt";
                Size winSizeArmchair = new Size(64, 64);
                Rectangle[] resultsArmchair = FindHuman.FindObject(frame, out processingTime, winSizeArmchair, fileArmchair);
                foreach (Rectangle rect in resultsArmchair)
                {
                    ++detected_human;
                    frame.Draw(rect, new Bgr(Color.Red), 2);
                    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                    X = hogLocation.X;
                    Y = hogLocation.Y;

                    frame.Draw("[" + detected_human + "]" + "Arm Chair", ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                    //    //con.koorHOG(X, Y);
                    Rectangle baru = rect;
                    dataBound.Add(baru);
                    dataTrain.Add("Arm chair");
                }

                ////============================================ DATA5 Bed =============================================================
                string fileBed = "D:\\bed.txt";
                Size winSizeBed = new Size(128, 64);
                Rectangle[] resultsBed = FindHuman.FindObject(frame, out processingTime, winSizeBed, fileBed);
                foreach (Rectangle rect in resultsBed)
                {
                    ++detected_human;
                    frame.Draw(rect, new Bgr(Color.Red), 2);
                    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                    X = hogLocation.X;
                    Y = hogLocation.Y;

                    frame.Draw("[" + detected_human + "]" + "Bed", ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                    //    //con.koorHOG(X, Y);
                    Rectangle baru = rect;
                    dataBound.Add(baru);
                    dataTrain.Add("Bed");
                }

                ////============================================ DATA6 Books =============================================================
                string fileSink = "D:\\sink.txt";
                Size winSizeSink = new Size(64, 128);
                Rectangle[] resultsSink = FindHuman.FindObject(frame, out processingTime, winSizeSink, fileSink);
                foreach (Rectangle rect in resultsSink)
                {
                    ++detected_human;
                    frame.Draw(rect, new Bgr(Color.Red), 2);
                    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                    X = hogLocation.X;
                    Y = hogLocation.Y;

                    frame.Draw("[" + detected_human + "]" + "Sink", ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                    //con.koorHOG(X, Y);
                    Rectangle baru = rect;
                    dataBound.Add(baru);
                    dataTrain.Add("Sink");
                }

                ////============================================ DATA7 Wastafel =============================================================
                //string fileWestafel = "D:\\percobaan3.txt";
                //Size winSizeWestafel = new Size(64, 64);
                //Rectangle[] resultsWestafel = FindHuman.FindObject(frame, out processingTime, winSizeWestafel, fileWestafel);
                //foreach (Rectangle rect in resultsWestafel)
                //{
                //    ++detected_human;
                //    frame.Draw(rect, new Bgr(Color.Red), 2);
                //    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                //    X = hogLocation.X;
                //    Y = hogLocation.Y;

                //    frame.Draw("[" + detected_human + "]" + dataTrain, ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                //    //con.koorHOG(X, Y);
                //    Rectangle baru = rect;
                //    dataBound.Add(baru);
                //    dataTrain.Add("Westafel");
                //}

                ////============================================ DATA8 Bathup =============================================================
                //string fileBathup = "D:\\percobaan3.txt";
                //Size winSizeBathup = new Size(64, 64);
                //Rectangle[] resultsBathup = FindHuman.FindObject(frame, out processingTime, winSizeBathup, fileBathup);
                //foreach (Rectangle rect in resultsBathup)
                //{
                //    ++detected_human;
                //    frame.Draw(rect, new Bgr(Color.Red), 2);
                //    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                //    X = hogLocation.X;
                //    Y = hogLocation.Y;

                //    frame.Draw("[" + detected_human + "]" + dataTrain, ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                //    //con.koorHOG(X, Y);
                //    Rectangle baru = rect;
                //    dataBound.Add(baru);
                //    dataTrain.Add("Bathup");
                //}


                ////============================================ DATA9 Stove =============================================================
                //string fileStove = "D:\\percobaan3.txt";
                //Size winSizeStove = new Size(64, 64);
                //Rectangle[] resultsStove = FindHuman.FindObject(frame, out processingTime, winSizeStove, fileStove);
                //foreach (Rectangle rect in resultsStove)
                //{
                //    ++detected_human;
                //    frame.Draw(rect, new Bgr(Color.Red), 2);
                //    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                //    X = hogLocation.X;
                //    Y = hogLocation.Y;

                //    frame.Draw("[" + detected_human + "]" + dataTrain, ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                //    //con.koorHOG(X, Y);
                //    Rectangle baru = rect;
                //    dataBound.Add(baru);
                //    dataTrain.Add("Stove");
                //}

                ////============================================ DATA10 Refrigerator =============================================================
                //string fileRefrigerator = "D:\\percobaan3.txt";
                //Size winSizeRefrigerator = new Size(64, 64);
                //Rectangle[] resultsRefrigerator = FindHuman.FindObject(frame, out processingTime, winSizeRefrigerator, fileRefrigerator);
                //foreach (Rectangle rect in resultsRefrigerator)
                //{
                //    ++detected_human;
                //    frame.Draw(rect, new Bgr(Color.Red), 2);
                //    Point hogLocation = rect.Location; // Menampilkan lokasi objek
                //    X = hogLocation.X;
                //    Y = hogLocation.Y;

                //    frame.Draw("[" + detected_human + "]" + dataTrain, ref f, new Point(rect.Left, rect.Top - 5), new Bgr(Color.Yellow));
                //    //con.koorHOG(X, Y);
                //    Rectangle baru = rect;
                //    dataBound.Add(baru);
                //    dataTrain.Add("Refrigerator");
                //}

                con.labelHOGBawah(dataTrain, dataBound);
                imageBox1.Image = frame;
                try
                {
                    lblTime.Invoke((Action)(() => lblX.Text = X.ToString()));
                    lblTime.Invoke((Action)(() => lblY.Text = Y.ToString()));
                }
                catch
                {

                }

            }
        }*/

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
                    captureStart.Text = "Start";
                    _capture.Pause();
                }
                else
                {
                    //start the capture
                    captureStart.Text = "Stop";
                    _capture.Start();
                }

                _captureInProgress = !_captureInProgress;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("timer1_Tick");
                ProcessFrame(this, null);
                //prosesFrameBawah(this, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            captureImageBox.Image.Save("atas.jpg");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            imageBox1.Image.Save("bawah.jpg");
        }

        private void captureStart_Click(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                if (_captureInProgress)
                {  //stop the capture
                    captureStart.Text = "Start";
                    _capture.Pause();
                }
                else
                {
                    //start the capture
                    captureStart.Text = "Stop";
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
