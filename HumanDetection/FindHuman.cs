//----------------------------------------------------------------------------
//  Copyright (C) 2004-2013 by EMGU. All rights reserved.       
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Diagnostics;
#if !IOS
using Emgu.CV.GPU;
#endif

namespace HumanDetection
{
    public static class FindHuman
    {
        /// <summary>
        /// Find the pedestrian in the image
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="processingTime">The pedestrian detection time in milliseconds</param>
        /// <returns>The region where pedestrians are detected</returns>

        /*private static float[] GetData()
        {
            List<float> data = new List<float>();
            using (System.IO.StreamReader reader = new System.IO.StreamReader("D:/18.txt"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var arr = new float[line.Length];
                    for (var i = 0; i < line.Length; i++)
                    {
                        arr[i] = (float)Convert.ToDouble(line[i]);
                        data.Add(arr[i]);
                    }
                }
            }

            var array = data.ToArray();
            return array;
        }*/
//=================================================== SVM Classifier Data Training Kursi ===========================================
        /*private static float[] GetDataKursi()
        {
            List<float> data = new List<float>();
            using (System.IO.StreamReader reader = new System.IO.StreamReader("D:/13.txt"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var arr = new float[line.Length];
                    for (var i = 0; i < line.Length; i++)
                    {
                        arr[i] = (float)Convert.ToDouble(line[i]);
                        data.Add(arr[i]);
                    }
                }
            }

            var array = data.ToArray();
            return array;
        }*/
//====================================================================================================================================

//=================================================== SVM Classifier Data Training Monitor ===========================================
        private static float[] GetDataMonitor()
        {
            List<float> data = new List<float>(); //20.txt = Data Training Sampah
            using (System.IO.StreamReader reader = new System.IO.StreamReader("D:/30.txt")) // 17.txt = Data Training Monitor
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var arr = new float[line.Length];
                    for (var i = 0; i < line.Length; i++)
                    {
                        arr[i] = (float)Convert.ToDouble(line[i]);
                        data.Add(arr[i]);
                    }
                }
            }

            var array = data.ToArray();
            return array;
        }


//====================================================================================================================================
        //=================================================== SVM Classifier Data Training Meja ===========================================
        /*private static float[] GetDataMeja()
        {
            List<float> data = new List<float>();
            using (System.IO.StreamReader reader = new System.IO.StreamReader("D:/15.txt"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var arr = new float[line.Length];
                    for (var i = 0; i < line.Length; i++)
                    {
                        arr[i] = (float)Convert.ToDouble(line[i]);
                        data.Add(arr[i]);
                    }
                }
            }

            var array = data.ToArray();
            return array;
        }*/
        //==================================================================================================================================
        // DATA TRAINING OBJECT :
        // 13.txt = Kursi (64X128)
        // 16.txt = Galon (64X128)

        // 15.txt = Meja (64X64)
        // 17.txt = Monitor (64X64)

        /*private static float[] GetData2()
        {
            List<float> data = new List<float>();
            using (System.IO.StreamReader reader = new System.IO.StreamReader("D:/8.txt"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var arr = new float[line.Length];
                    for (var i = 0; i < line.Length; i++)
                    {
                        arr[i] = (float)Convert.ToDouble(line[i]);
                        data.Add(arr[i]);
                    }
                }
            }

            var array = data.ToArray();
            return array;
        }*/

        static Size winSizeMeja = new Size(64, 64);
        static Size winSizeMonitor = new Size(64, 64);
        static Size winSizeKursi = new Size(64, 128);
        static Size winSampah = new Size(64, 64);
        //static Size winSizeGalon = new Size(64, 128);
        //static Size winSize = new Size(64, 128);
        static Size blockSize = new Size(16, 16);
        static Size blockStride = new Size(8, 8);
        static Size winStride = new Size(8, 8);
        static Size cellSize = new Size(8, 8);
        static int nbins = 9;
     
        /*public static Rectangle[] Find(Image<Bgr, Byte> image, out long processingTime)
        {
            Stopwatch watch;
            Rectangle[] regions;
            //check if there is a compatible GPU to run pedestrian detection
            if (GpuInvoke.HasCuda)
            {  //this is the GPU version
                using (GpuHOGDescriptor des = new GpuHOGDescriptor())
                {
                    des.SetSVMDetector(GpuHOGDescriptor.GetDefaultPeopleDetector());

                    watch = Stopwatch.StartNew();
                    using (GpuImage<Bgr, Byte> gpuImg = new GpuImage<Bgr, byte>(image))
                    using (GpuImage<Bgra, Byte> gpuBgra = gpuImg.Convert<Bgra, Byte>())
                    {
                        regions = des.DetectMultiScale(gpuBgra);
                    }
                }
            }
            else
            {  //this is the CPU version
                using (HOGDescriptor des = new HOGDescriptor(winSize, blockSize, blockStride, cellSize, nbins, 1, -1, 0.2, true))
                {
                    des.SetSVMDetector(GetData());
                    //des.SetSVMDetector(GetData2());

                    watch = Stopwatch.StartNew();
                    regions = des.DetectMultiScale(image);
                    
                }
            }
            watch.Stop();

            processingTime = watch.ElapsedMilliseconds;

            return regions;
        }*/
//=================================================== Feature Descriptor (HOG) Data Training Kursi ===========================================
        /*public static Rectangle[] FindKursi(Image<Bgr, Byte> image, out long processingTime)
        {
            Stopwatch watch;
            Rectangle[] regions;
            //check if there is a compatible GPU to run pedestrian detection
            if (GpuInvoke.HasCuda)
            {  //this is the GPU version
                using (GpuHOGDescriptor des = new GpuHOGDescriptor())
                {
                    des.SetSVMDetector(GpuHOGDescriptor.GetDefaultPeopleDetector());

                    watch = Stopwatch.StartNew();
                    using (GpuImage<Bgr, Byte> gpuImg = new GpuImage<Bgr, byte>(image))
                    using (GpuImage<Bgra, Byte> gpuBgra = gpuImg.Convert<Bgra, Byte>())
                    {
                        regions = des.DetectMultiScale(gpuBgra);
                    }
                }
            }
            else
            {  //this is the CPU version
                using (HOGDescriptor des = new HOGDescriptor(winSizeKursi, blockSize, blockStride, cellSize, nbins, 1, -1, 0.2, true))
                {
                    des.SetSVMDetector(GetDataKursi());
                    //des.SetSVMDetector(GetData2());

                    watch = Stopwatch.StartNew();
                    regions = des.DetectMultiScale(image);

                }
            }
            watch.Stop();

            processingTime = watch.ElapsedMilliseconds;

            return regions;
        }*/

//=================================================== Feature Descriptor (HOG) Data Training Monitor ===========================================
        public static Rectangle[] FindMonitor(Image<Bgr, Byte> image, out long processingTime)
        {
            Stopwatch watch;
            Rectangle[] regions;
            //check if there is a compatible GPU to run pedestrian detection
            if (GpuInvoke.HasCuda)
            {  //this is the GPU version
                using (GpuHOGDescriptor des = new GpuHOGDescriptor())
                {
                    des.SetSVMDetector(GpuHOGDescriptor.GetDefaultPeopleDetector());

                    watch = Stopwatch.StartNew();
                    using (GpuImage<Bgr, Byte> gpuImg = new GpuImage<Bgr, byte>(image))
                    using (GpuImage<Bgra, Byte> gpuBgra = gpuImg.Convert<Bgra, Byte>())
                    {
                        regions = des.DetectMultiScale(gpuBgra);
                    }
                }
            }
            else
            {  //this is the CPU version
                using (HOGDescriptor des = new HOGDescriptor(winSampah, blockSize, blockStride, cellSize, nbins, 1, -1, 0.2, true))
                {
                    des.SetSVMDetector(GetDataMonitor());
                    //des.SetSVMDetector(GetData2());

                    watch = Stopwatch.StartNew();
                    regions = des.DetectMultiScale(image);

                }
            }
            watch.Stop();

            processingTime = watch.ElapsedMilliseconds;

            return regions;
        }

//==================================================================================================================================

//==================================================================================================================================

//=================================================== Feature Descriptor (HOG) Data Training Meja ===========================================
        /*public static Rectangle[] FindMeja(Image<Bgr, Byte> image, out long processingTime)
        {
            Stopwatch watch;
            Rectangle[] regions;
            //check if there is a compatible GPU to run pedestrian detection
            if (GpuInvoke.HasCuda)
            {  //this is the GPU version
                using (GpuHOGDescriptor des = new GpuHOGDescriptor())
                {
                    des.SetSVMDetector(GpuHOGDescriptor.GetDefaultPeopleDetector());

                    watch = Stopwatch.StartNew();
                    using (GpuImage<Bgr, Byte> gpuImg = new GpuImage<Bgr, byte>(image))
                    using (GpuImage<Bgra, Byte> gpuBgra = gpuImg.Convert<Bgra, Byte>())
                    {
                        regions = des.DetectMultiScale(gpuBgra);
                    }
                }
            }
            else
            {  //this is the CPU version
                using (HOGDescriptor des = new HOGDescriptor(winSizeMeja, blockSize, blockStride, cellSize, nbins, 1, -1, 0.2, true))
                {
                    des.SetSVMDetector(GetDataMeja());
                    //des.SetSVMDetector(GetData2());

                    watch = Stopwatch.StartNew();
                    regions = des.DetectMultiScale(image);

                }
            }
            watch.Stop();

            processingTime = watch.ElapsedMilliseconds;

            return regions;
        }*/
//==================================================================================================================================

        /*public static Rectangle[] Find2(Image<Bgr, Byte> image, out long processingTime)
        {
            Stopwatch watch;
            Rectangle[] regions;
            //check if there is a compatible GPU to run pedestrian detection
            if (GpuInvoke.HasCuda)
            {  //this is the GPU version
                using (GpuHOGDescriptor des = new GpuHOGDescriptor())
                {
                    des.SetSVMDetector(GpuHOGDescriptor.GetDefaultPeopleDetector());

                    watch = Stopwatch.StartNew();
                    using (GpuImage<Bgr, Byte> gpuImg = new GpuImage<Bgr, byte>(image))
                    using (GpuImage<Bgra, Byte> gpuBgra = gpuImg.Convert<Bgra, Byte>())
                    {
                        regions = des.DetectMultiScale(gpuBgra);
                    }
                }
            }
            else
            {  //this is the CPU version
                using (HOGDescriptor des = new HOGDescriptor(winSize, blockSize, blockStride, cellSize, nbins, 1, -1, 0.2, true))
                {
                    des.SetSVMDetector(GetData2());
                    //des.SetSVMDetector(GetData2());

                    watch = Stopwatch.StartNew();
                    regions = des.DetectMultiScale(image);

                }
            }
            watch.Stop();

            processingTime = watch.ElapsedMilliseconds;

            return regions;
        }*/
    }
}
