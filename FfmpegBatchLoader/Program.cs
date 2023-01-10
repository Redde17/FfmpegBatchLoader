using FfmpegBatchLoader;
using Newtonsoft.Json;

public class Settings
{
    public int VideoAmount { get; set; }
    public string InputFolder { get; set; }
    public string OutputFolder { get; set; }
    public bool CreateFolder { get; set; }
    public bool ShowFfmpegInAnotherWindow { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("appsettings.json"));

        Console.WriteLine($"Video amount:{settings.VideoAmount}\nInput folder:{settings.InputFolder}\nOutput folder:{settings.OutputFolder}\n");

        if (!ValidateSettings(settings))
        {
            Console.ResetColor();
            return;
        }

        if (!CreateFolderCheck(settings))
        {
            Console.ResetColor();
            return;
        }

        BatchLoader BL = new(settings);
        BL.Start();
    }

    static bool CreateFolderCheck(Settings settings)
    {
        if (settings.CreateFolder)
        {
            System.IO.Directory.CreateDirectory(settings.InputFolder);
            System.IO.Directory.CreateDirectory(settings.OutputFolder);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (!System.IO.Directory.Exists(settings.InputFolder))
            {
                Console.WriteLine("Input folder not found");
                return false;
            }

            if (!System.IO.Directory.Exists(settings.OutputFolder))
            {
                Console.WriteLine("Output folder not found");
                return false;
            }
        }

        return true;
    }

    static bool ValidateSettings(Settings settings)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        if (settings.VideoAmount <= 0) 
        {
            Console.WriteLine("Invalid video amount");
            return false;
        }

        if(settings.InputFolder == "") 
        {
            Console.WriteLine("Invalid input folder");
            return false;
        }

        if (settings.OutputFolder == "") 
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Invalid output folder, setting default Input folder");
            settings.OutputFolder = "OutputVideos";
        }

        Console.ResetColor();
        return true;
    }
}

//TO DO:
//       chose input and output folder location DONE
//       param customization
//       clean the code in Program.cs DONE
//       clean the code in BatchLoader.cs
//       folder for batch process DONE



