using System.Diagnostics;
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
        Settings settings;

        public BatchLoader(Settings settings)
        {
            waitingList = new Process[settings.VideoAmount];
            videoStack= new Stack<Video>();
            this.settings = settings;
        }

        public void Start()
        {
            InitiateVideoList();
            PrintVideoList();

            //create sub folder for the batch process
            string batchFolder = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff");
            System.IO.Directory.CreateDirectory($@"{settings.OutputFolder}\{batchFolder}");

            //manage the ffmpeg instances for a set maximum at a time
            StartProcessing(batchFolder);

            //wait for all process to end
            WaitAll();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nDone");
            Console.ResetColor();
        }

        public void PrintVideoList()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Videos to process:");
            Console.ResetColor();
            foreach (Video video in videoStack)
                Console.WriteLine("\t{0,-10}", video.Path); //da sistemare 
        }

        //==========PRIVATE==========//
        private void WaitAll()
        {
            foreach (var process in waitingList)
                if (process != null)
                    process.WaitForExit();
        }

        private void StartProcessing(string batchFolder)
        {
            
            Video vd;
            while (videoStack.Count > 0)
            {
                for (int i = 0; i < waitingList.Length; i++)
                {
                    if (waitingList[i] == null || waitingList[i].HasExited)
                    {
                        vd = videoStack.Pop();          
                        waitingList[i] = LaunchFfmpegInstance(vd.Path, vd.Name, batchFolder);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"\nProcessing: ");
                        Console.ResetColor();
                        Console.Write($"{vd.Path} - {waitingList[i].StartTime}\n\n");
                    }

                    if (videoStack.Count == 0)
                        break;
                }
                Thread.Sleep(500);
            }
        }

        private void InitiateVideoList()
        {
            foreach (string file in Directory.GetFiles($@"{settings.InputFolder}", "*.mkv"))
                videoStack.Push(new Video(file, Path.GetFileNameWithoutExtension(file)));
        }

        private Process LaunchFfmpegInstance(string path, string name, string folder)
        {
            string Args = $"-i \"{path}\" -map 0 -c:v copy \"{settings.OutputFolder}\\{folder}\\{name}.mp4\" -y";
            if (!settings.ShowFfmpegInAnotherWindow)
                Args = Args + " -loglevel error -stats";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false; 
            startInfo.UseShellExecute = settings.ShowFfmpegInAnotherWindow;
            startInfo.FileName = "ffmpeg.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = Args;

            return Process.Start(startInfo);
        }

    }
}
