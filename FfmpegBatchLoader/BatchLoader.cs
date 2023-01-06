﻿using System.Diagnostics;
using System.Threading;

namespace FfmpegBatchLoader
{
    public class Video
    {
        
        public string Path { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

        public Video(string Path, string Name)
        {
            this.Path = Path;
            this.Name = Name;
            Status = "not ready";
        }
    }

    public class BatchLoader
    {
        Process[] waitingList;
        Stack<Video> videoStack;

        public BatchLoader(int videoAmount)
        {
            waitingList = new Process[videoAmount];
            videoStack= new Stack<Video>();
        }
        public void Start()
        {
            InitiateVideoList();
            PrintVideoList();

            Console.WriteLine();


            Video vd;
            for (int i = 0; i < waitingList.Length; i++)
            {
                vd = videoStack.Pop();
                Console.WriteLine($"Processing: {vd.Path}");
                waitingList[i] = LaunchFfmpegInstance(vd.Path, vd.Name);
            }

            while (videoStack.Count != 0)
            {
                Thread.Sleep(500);
                for (int i = 0; i < waitingList.Length; i++)
                {
                    if (waitingList[i].HasExited)
                    {
                        if (videoStack.Count == 0)
                            break;

                        vd = videoStack.Pop();
                        Console.WriteLine($"Processing: {vd.Path}");
                        waitingList[i] = LaunchFfmpegInstance(vd.Path, vd.Name);
                    }
                }
            }

            Console.WriteLine("Done");
        }

        public void PrintVideoList()
        {
            Console.WriteLine("Video in processo:");
            foreach (Video video in videoStack)
                Console.WriteLine("{0,-10}", video.Path); //da sistemare 
        }

        //==========PRIVATE==========//

        private void InitiateVideoList()
        {
            foreach (string file in Directory.GetFiles(@"InputVideos", "*.mkv"))
                videoStack.Push(new Video(file, Path.GetFileNameWithoutExtension(file)));
        }

        private Process LaunchFfmpegInstance(string path, string name)
        {
            string Args = $"-i \"{path}\" -map 0 -c:v copy \"OutputVideos\\{name}.mp4\" -y";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true; 
            startInfo.UseShellExecute = false;
            startInfo.FileName = "ffmpeg.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = Args;

            return Process.Start(startInfo);
        }

    }
}