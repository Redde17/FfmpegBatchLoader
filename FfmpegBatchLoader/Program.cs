using System;
using FfmpegBatchLoader;
class Program
{
    static void Main(string[] args)
    {
        int VideoAmount = Int32.Parse(File.ReadAllText("VideoAmount.txt"));
        if(VideoAmount <= 0)
        {
            Console.WriteLine("Video amount minore o uguale a 0");
            return;
        }

        System.IO.Directory.CreateDirectory("InputVideos");
        System.IO.Directory.CreateDirectory("OutputVideos");


        BatchLoader BL = new BatchLoader(VideoAmount);
        BL.Start();
    }
}

//TO DO: chose input and output folder location
//       param customization
//       clean the code



