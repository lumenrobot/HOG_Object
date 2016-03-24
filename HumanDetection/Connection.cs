using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Util;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Threading;
using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using System.IO;
using System.Media;
using System.Drawing;
using Aldebaran.Proxies;
using NAudio;
using NAudio.Wave;
using NAudio.WindowsMediaFormat;
namespace HumanDetection
{
    class Connection
    {
        private static BasicDeliverEventArgs global = new BasicDeliverEventArgs();
        private static BasicDeliverEventArgs globalBawah = new BasicDeliverEventArgs();
       
        public static QueueingBasicConsumer consumer, consumerBawah;
        static Stopwatch s = new Stopwatch();
        static string ip = "167.205.66.80";
        //static string ip = "127.0.0.1";
        static int port = 9559;
        public bool isConnected = false;
        public static IModel channelSend, channelSendBawah, channelData, channelDataBawah;
        private IModel channelVisual1, channelVisual2, channelVisual3, channelAudio1, channelAudio2, channelAudio3, channelAvatar1, channelAvatar3;
        public EventingBasicConsumer consumerVisual1, consumerVisual2, consumerVisual3, consumerAudio1, consumerAudio2, consumerAudio3, consumerAvatar1, consumerAvatar2;
        public static QueueingBasicConsumer consumerData;
        public static IConnection connection;
        public static IConnection connectionBawah;
        Connection connection2;
        public QueueingBasicConsumer  consumerFaceLocation, consumerDataJoint; // buat ack command
        public bool isAck = false;
        public string ackRoutingKey;
        public string corrId;
        TextName2 textName2;
        public Rectangle boundAtas, boundBawah;
        public static ConnectionFactory factory;
        public static ConnectionFactory factoryBawah;

        public bool isCollecting = false;

        public Connection()
        {
            topCamera();
            bottomCamera();
        }

        public static void topCamera()
        {
            try
            {
                factory = new ConnectionFactory();
                factory.Uri = "amqp://lumen:lumen@167.205.66.35/%2F";
                connection = factory.CreateConnection();
                IModel channel = connection.CreateModel();
                channelSend = connection.CreateModel();
                channelData = connection.CreateModel();
                string routingKey = "avatar.nao1.camera.bottom";
                var arg = new Dictionary<string, object>
                {
                    {"x-message-ttl",50}
                };
                QueueDeclareOk queue = channel.QueueDeclare("", false, true, true, arg); // Durable untuk mengehentikan pengiriman data ke server saat aplikasi kita di matikan
                channel.QueueBind(queue.QueueName, "amq.topic", routingKey);
                consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queue.QueueName, true, consumer);
                Thread Query = new Thread(QueryImage);
                Query.Start();

                //QueueDeclareOk queueData = channelData.QueueDeclare("", true, false, true, null);
                //channelData.QueueBind(queueData.QueueName, "amq.topic", "lumen.visual.get.text");
                //consumerData = new QueueingBasicConsumer(channelData);
                //channelData.BasicConsume(queueData.QueueName, true, consumerData);
                //Thread data = new Thread(QueryData);
                //data.Start();
                //Console.WriteLine("Setting Selesai");
            }
            catch
            {
                throw new InvalidOperationException();
                //Console.Write
            }
        }

        public static void bottomCamera()
        {
            try
            {
                factoryBawah = new ConnectionFactory();
                factoryBawah.Uri = "amqp://lumen:lumen@167.205.66.35/%2F";
                connectionBawah = factoryBawah.CreateConnection();
                IModel channelBawah = connectionBawah.CreateModel();
                channelSendBawah = connectionBawah.CreateModel();
                channelDataBawah = connectionBawah.CreateModel();
                string routingKey = "avatar.nao1.camera.bottom";
                var arg = new Dictionary<string, object>
                {
                    {"x-message-ttl",50}
                };
                QueueDeclareOk queueBawah = channelBawah.QueueDeclare("", false, true, true, arg);
                channelBawah.QueueBind(queueBawah.QueueName, "amq.topic", routingKey);
                consumerBawah = new QueueingBasicConsumer(channelBawah);
                channelBawah.BasicConsume(queueBawah.QueueName, true, consumerBawah);
                Thread Query = new Thread(QueryImageBawah);
                Query.Start();

                //QueueDeclareOk queueData = channelData.QueueDeclare("", false, false, true, null);
                //channelData.QueueBind(queueData.QueueName, "amq.topic", "lumen.visual.get.text");
                //consumerData = new QueueingBasicConsumer(channelData);
                //channelData.BasicConsume(queueData.QueueName, true, consumerData);
                //Thread data = new Thread(QueryData);
                //data.Start();
                //Console.WriteLine("Setting Selesai");
            }
            catch
            {
                throw new InvalidOperationException();
                //Console.Write
            }
        }

        static void bicara()
        {
            TextToSpeechProxy tts = new TextToSpeechProxy("167.205.66.80", 9559);
            tts.say("Hallo buddy");
        }
        public void dataCollect_textRecognitionReceived(object sender, TextName2 name)
        {

        }

        private static void QueryImage()
        {
            BasicDeliverEventArgs ev = null;
            while (true)
            {
                ev = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                var bodyStr = System.Text.Encoding.UTF8.GetString(ev.Body);
                Console.WriteLine("QueryImage received {0} {1}", ev.RoutingKey, bodyStr.Substring(0, 200) + "...");
                lock (global)
                {
                    global = ev;
                }
            }
        }

        public static void QueryImageBawah()
        {
            BasicDeliverEventArgs evBawah = null;
            while (true)
            {
                evBawah = (BasicDeliverEventArgs)consumerBawah.Queue.Dequeue();
                var bodyStr = System.Text.Encoding.UTF8.GetString(evBawah.Body);
                Console.WriteLine("QueryImageBawah received {0} {1}", evBawah.RoutingKey, bodyStr.Substring(0, 200) + "...");
                lock (globalBawah)
                {
                    globalBawah = evBawah;
                }
            }
        }

        public Image<Bgr, byte> getImageAtas()
        {
            Console.WriteLine("getImageAtas ...");
            Image<Bgr, byte> ImageFrame;
            BasicDeliverEventArgs ev;
            if (global != null)
            {
                lock (global)
                {
                    ev = global;
                }
                if (ev.Body != null)
                {
                    string body = Encoding.UTF8.GetString(ev.Body);
                    JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                    ImageObject image = JsonConvert.DeserializeObject<ImageObject>(body, setting);
                    string base64 = image.ContentUrl.Replace("data:image/jpeg;base64,", "");
                    if (base64 != null)
                    {
                        byte[] imageByte = Convert.FromBase64String(base64);
                        MemoryStream ms = new MemoryStream(imageByte);
                        Bitmap bmp = (Bitmap)Image.FromStream(ms);
                        ImageFrame = new Image<Bgr, byte>(bmp);
                        return ImageFrame;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }

        public Image<Bgr, byte> getImageBottom()
        {
            Image<Bgr, byte> imageFrameBottom;
            BasicDeliverEventArgs ev;
            if(globalBawah != null)
            {
                lock (globalBawah)
                {
                    ev = globalBawah;
                }
                
                if(ev.Body != null)
                {
                    string body = Encoding.UTF8.GetString(ev.Body);
                    JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                    ImageObject image = JsonConvert.DeserializeObject<ImageObject>(body, setting);
                    string base64 = image.ContentUrl.Replace("data:image/jpeg;base64,", "");
                    if(base64 != null)
                    {
                        byte[] imageByte = Convert.FromBase64String(base64);
                        MemoryStream ms = new MemoryStream(imageByte);
                        Bitmap bmp = (Bitmap)Image.FromStream(ms);
                        imageFrameBottom = new Image<Bgr, byte>(bmp);
                        return imageFrameBottom;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void connect()
        {
            if (!isConnected)
            {
                try
                {
                    string routingKey;
                    ConnectionFactory factory = new ConnectionFactory();
                    factory.Uri = "amqp://lumen:lumen@167.205.66.76/%2F";
                    connection = factory.CreateConnection();
                    channelSend = connection.CreateModel(); // untuk mengirim
                    channelData = connection.CreateModel();
                    channelAvatar3 = connection.CreateModel();

                    QueueDeclareOk queueData = channelData.QueueDeclare("", false, true, true, null);
                    QueueDeclareOk queueFaceLocation = channelData.QueueDeclare("", false, true, true, null);

                    channelData.QueueBind(queueData.QueueName, "amq.topic", "lumen.visual.get.text");

                    consumerFaceLocation = new QueueingBasicConsumer(channelData);
                    consumerDataJoint = new QueueingBasicConsumer(channelAvatar3);

                    channelData.BasicConsume(queueData.QueueName, true, consumerData);
                    channelData.BasicConsume(queueFaceLocation.QueueName, true, consumerFaceLocation);

                    isConnected = true;
                    Console.WriteLine("program is connected to server");
                    //Program.panel.btn_connect.Text = "Disconnect";
                }
                catch
                {
                    //MessageBox.Show("unable to connect to server", "connection");
                }
            }
            else
            {
                //MessageBox.Show("already connected to server!", "connection");
            }
        }

        public void disconnect()
        {
            if (isConnected)
            {
                if (!this.isProcessRunning())
                {
                    isConnected = false;
                    //Program.panel.btn_connect.Text = "Connect";
                    connection.Close();
                    connectionBawah.Close();
                }
                else
                {
                    //MessageBox.Show("Stop Process Before Disconnecting", "Connection");
                }
            }
        }

        public bool isProcessRunning()
        {
            return true;
            //if ((Program.panel.dataCollect.isCollecting) || (Program.panel.command.isHandling))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        

        public delegate void TextRecognition_callback(object sender, TextName2 name);
        public event TextRecognition_callback textRecognitionReceived;
        public static void QueryData()
        {
            BasicDeliverEventArgs ev = null;
            Console.WriteLine("start data thread");
            while (true)
            {
                Console.WriteLine("waiting..");
                ev = (BasicDeliverEventArgs)consumerData.Queue.Dequeue();
                Console.WriteLine("data Received");
                string body = Encoding.UTF8.GetString(ev.Body);
                JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                //textName2 = JsonConvert.DeserializeObject<TextName2>(body, setting);
                //if (textRecognitionReceived != null)
                //{
                //    textRecognitionReceived(this, textName2);

                //    //Console.WriteLine("Data Received {0}", textName2.text2);

                //}
            }
        }
        public void consumerData_Received(object sender, BasicDeliverEventArgs ev)
        {
            string body = Encoding.UTF8.GetString(ev.Body);
            JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            textName2 = JsonConvert.DeserializeObject<TextName2>(body, setting);
            if (textRecognitionReceived != null)
            {
                textRecognitionReceived(this, textName2);
                Console.WriteLine("Data Received {0}",textName2.text2);

            }
            Console.WriteLine("Data Received {0}", textName2.text2);
            
        }

        public void sendGestureName(string name)
        {
            string routingKey = "lumen.visual.gesture.recognition";

            String gestName = name;
            HandGesture ha = new HandGesture { gesture = name };

            string hand = JsonConvert.SerializeObject(ha);
            byte[] buffer = Encoding.UTF8.GetBytes(hand);
            channelSend.BasicPublish("amq.topic", routingKey, null, buffer);
        }
        public void hogObjectNama(string hogObj)
        {
            string routingKey = "lumen.visual.hogobj.recognition";

            String hoggName = hogObj;
            hogNama haa = new hogNama { hog = hogObj };

            string benda = JsonConvert.SerializeObject(haa);
            byte[] buffer = Encoding.UTF8.GetBytes(benda);
            channelSend.BasicPublish("amq.topic", routingKey, null, buffer);
        }
        public void sendHOGName(string hogName)
        {
            string routingKey = "lumen.visual.hog.recognition";

            String hogNama = hogName;
            HandGesture ha = new HandGesture { gesture = hogName };

            string obj = JsonConvert.SerializeObject(ha);
            byte[] buffer = Encoding.UTF8.GetBytes(obj);
            channelSend.BasicPublish("amq.topic", routingKey, null, buffer);
        }
        public void sendObjectSurf(string name)
        {
            string routingKey = "lumen.visual.surf.recognition";

            String gestName = name;
            Object ob = new Object { objectsurf = name };

            string obj = JsonConvert.SerializeObject(ob);
            byte[] buffer = Encoding.UTF8.GetBytes(obj);
            channelSend.BasicPublish("amq.topic", routingKey, null, buffer);    
        }
        public void speak(String kata)
        {
            Parameter par = new Parameter { text = kata };
            Command cmd = new Command { type = "texttospeech", method = "say", parameter = par };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void berdiri(String command)
        {
            Parameter par = new Parameter { postureName = command };
            Command cmd = new Command { type = "posture", method = "gotoposture", parameter = new Parameter { postureName = "Stand", speed = 0.5f } };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void chrouch(String comChrouch)
        {
            Parameter par = new Parameter { postureName = comChrouch };
            Command cmd = new Command { type = "posture", method = "gotoposture", parameter = new Parameter { postureName = "Crouch", speed = 0.5f } };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void sit(String comSit)
        {
            Parameter par = new Parameter { postureName = comSit };
            Command cmd = new Command { type = "posture", method = "gotoposture", parameter = new Parameter { postureName = "Sit", speed = 0.5f } };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void lyingBelly(String lyingbelly)
        {
            Parameter par = new Parameter { postureName = lyingbelly };
            Command cmd = new Command { type = "posture", method = "gotoposture", parameter = new Parameter { postureName = "LyingBelly", speed = 0.5f } };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void jalan(float comJalan)
        {
            Parameter par = new Parameter {x = comJalan};
            Command cmd = new Command { type = "motion", method = "moveto", parameter = new Parameter { x = 0.1f, y = 0.0f, tetha = 0.0f } };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void koorHOG(float kX, float kY)
        {
            string routingKey = "lumen.misc.save.birdcalibration";
            var recognizedObjects =  new RecognizedObjects();
            var obj = new RecognizedObject { topPosition = new Vector2 { x = kX, y = kY } };
            recognizedObjects.trashes.Add(obj);
            //HOGobject hgb = new HOGobject { x = kX, y = kY };
            string hand = JsonConvert.SerializeObject(recognizedObjects);
            byte[] buffer = Encoding.UTF8.GetBytes(hand);
            channelSend.BasicPublish("amq.topic", routingKey, null, buffer);
        }
        public void labelHOG(string objNama, List<Rectangle> rec)
        {
            string routingKey = "lumen.visual.hogobj.recognition";
            var recognizedObjects = new RecognizedObjects();
            for (int a = 0; a < rec.Count; a++)
            {
                Rectangle bound;
                bound = rec[a];
                var recognizedObject = new RecognizedObject();
                var lblObj = new RecognizedObject { name = objNama, topPosition = new Vector2 { x = bound.X, y = bound.Y } };
                recognizedObjects.trashes.Add(lblObj);
            }
            string nama = JsonConvert.SerializeObject(recognizedObjects);
            byte[] buffer = Encoding.UTF8.GetBytes(nama);
            channelSend.BasicPublish("amq.topic", routingKey, null, buffer);
        }






        public void labelHOGAtas(List<string> namaBendaAtas, List<Rectangle> recAtas)
        {
            string routingKey = "lumen.visual.hogobj.recognition";
            var recognizedObjects = new RecognizedObjects();
            recognizedObjects.hasPosition = true;
            recognizedObjects.hasDistance = false;
            recognizedObjects.hasYaw = false;

            string ba = "";
            for (int a = 0; a < recAtas.Count; a++)
            {
                boundAtas = recAtas[a];
                ba = namaBendaAtas[a];
                var recognizedObject = new RecognizedObject();
                var lblObj = new RecognizedObject { name = ba, topPosition = new Vector2 { x = boundAtas.Location.X + (boundAtas.Width / 2), y = boundAtas.Location.Y + (boundAtas.Height / 2) } };
                recognizedObjects.trashes.Add(lblObj);
            }
        
            string nama = JsonConvert.SerializeObject(recognizedObjects);
            byte[] buffer = Encoding.UTF8.GetBytes(nama);
            channelSend.BasicPublish("amq.topic", routingKey, null, buffer);
        }

        public void labelHOGBawah(List<string> namaBendaBawah, List<Rectangle> recBawah)
        {
            string routingKey = "lumen.visual.hogobj.recognition";
            var recognizedObjects = new RecognizedObjects();
            recognizedObjects.hasPosition = true;
            recognizedObjects.hasDistance = false;
            recognizedObjects.hasYaw = false;

            string ba = "";
            for (int a = 0; a < recBawah.Count; a++)
            {
                boundBawah = recBawah[a];
                ba = namaBendaBawah[a];
                var recognizedObject = new RecognizedObject();
                var lblObj = new RecognizedObject { name = ba, bottomPosition = new Vector2 { x = boundBawah.Location.X + (boundBawah.Width / 2), y = boundBawah.Location.Y + (boundBawah.Height / 2) } };
                recognizedObjects.trashes.Add(lblObj);
            }

            string nama = JsonConvert.SerializeObject(recognizedObjects);
            byte[] buffer = Encoding.UTF8.GetBytes(nama);
            channelSend.BasicPublish("amq.topic", routingKey, null, buffer);
        }





        public void stop(String comStop)
        {
            Parameter par = new Parameter();
            Command cmd = new Command { type = "motion", method = "rest", parameter = par };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void offNAO(String comOff)
        {
            Parameter par = new Parameter();
            Command cmd = new Command { type = "posture", method = "stopmove", parameter = par };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void putarBalik(float comPutar)
        {
            Parameter par = new Parameter { x = comPutar };
            Command cmd = new Command { type = "motion", method = "moveto", parameter = new Parameter { x = 0.1f, y = 0.0f, tetha = 3.0f } };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
        }
        //public void testSonar(String A)
        //{
        //    SonarProxy s = new SonarProxy(ip, port);
        //    MemoryProxy m = new MemoryProxy(ip, port);
        //    s.subscribe(A);
        //    while (true)
        //    {
        //        Console.WriteLine(m.getData("Device/SubDeviceList/US/Right/Sensor/Value"));
        //        delay(100);
        //    }
        //}
        public void delay(int time)
        {
            Stopwatch s = new Stopwatch();
            s.Reset();
            s.Start();
            while (s.ElapsedMilliseconds < time)
            {
            }
            s.Stop();
        }
    }
}
