﻿using System;
using System.Diagnostics;
using System.IO;

namespace Alturos.Yolo.TestConsole
{
    public class PerformanceAnalyze
    {
        /// <summary>
        /// Compare yolo speed with optimal images in the correct size
        /// Resize before and resize in yolo
        /// </summary>
        public void Start()
        {
            Console.WriteLine("CPU");
            Console.WriteLine("---------------------------------------------------------------");
            Check(null);
            Console.WriteLine("GPU");
            Console.WriteLine("---------------------------------------------------------------");
            Check(new GpuConfig());
        }

        private void Check(GpuConfig gpuConfig)
        {
            var yoloWrapper = new YoloWrapper("yolov2-tiny-voc.cfg", "yolov2-tiny-voc.weights", "voc.names", gpuConfig);
            var files = Directory.GetFiles(@".\Images");

            var retries = 10;

            var sw = new Stopwatch();
            foreach (var file in files)
            {
                var elapsed = 0.0;
                var fileInfo = new FileInfo(file);
                var imageData = File.ReadAllBytes(file);

                for (var i = 0; i < retries; i++)
                {
                    sw.Restart();
                    yoloWrapper.Detect(imageData);
                    sw.Stop();

                    elapsed += sw.Elapsed.TotalMilliseconds;
                }

                var average = elapsed / retries;
                Console.WriteLine($"{fileInfo.Name} {average}ms");
            }

            yoloWrapper.Dispose();
        }
    }
}
