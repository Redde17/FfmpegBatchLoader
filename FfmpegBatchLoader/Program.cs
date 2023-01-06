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

        BatchLoader BL = new BatchLoader(VideoAmount);
        BL.Start();
    }
}


//get list of videos in folder

//start x ffmpeg process with first elements of the list

//loop until list is empty

// exit



