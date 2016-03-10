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

//=================================================== SVM Classifier Data Training Kursi ===========================================
        private static float[] GetDataObjects(string objectData)
        {
            List<float> data = new List<float>();
            using (System.IO.StreamReader reader = new System.IO.StreamReader(objectData))
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

        static Size blockSize = new Size(16, 16);
        static Size blockStride = new Size(8, 8);
        static Size winStride = new Size(8, 8);
        static Size cellSize = new Size(8, 8);
        static int nbins = 9;

//=================================================== Feature Descriptor (HOG) Data Training Kursi ===========================================
        public static Rectangle[] FindObject(Image<Bgr, Byte> image, out long processingTime, Size winSizeObject, string dataFile)
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
                using (HOGDescriptor des = new HOGDescriptor(winSizeObject, blockSize, blockStride, cellSize, nbins, 1, -1, 0.2, true))
                {
                    des.SetSVMDetector(GetDataObjects(dataFile));
                    //des.SetSVMDetector(GetData2());

                    watch = Stopwatch.StartNew();
                    regions = des.DetectMultiScale(image);

                }
            }
            watch.Stop();

            processingTime = watch.ElapsedMilliseconds;

            return regions;
        }

    }
}
